using AuthCommon.Models;
using KurosukeHueClient.Models;
using KurosukeHueClient.Models.HueObjects;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KurosukeHueClient.Utils
{
    public class HueClient : IDisposable
    {
        private StreamingHueClient streamingClient;
        private LocalHueClient client;
        private Bridge bridge;
        private HueUser user;
        private CancellationTokenSource cancellationTokenSource;
        private EntertainmentLayer baseLayer;
        private EntertainmentLayer effectLayer;
        public HueClient(HueUser user)
        {
            this.user = user;
            bridge = user.Bridge;
            streamingClient = new StreamingHueClient(bridge.Config.IpAddress, user.Token.AccessToken, user.Token.EntertainmentKey);
            client = (LocalHueClient)streamingClient.LocalHueClient;
        }

        public void Dispose()
        {
            //cancel all ongoing operations
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            //dispose all
            client = null;
            streamingClient.Dispose();
            streamingClient = null;
        }

        #region Entertainment APIs
        public async Task<List<Group>> GetEntertainmentGroupsAsync()
        {
            var hueDeviceGroups = new List<Group>();
            var allLights = new List<Models.HueObjects.Light>();
            var allHueGroups = await client.GetEntertainmentGroups();
            var allHueLights = await client.GetLightsAsync();

            foreach (var hueGroup in allHueGroups)
            {
                var streamingGroup = new StreamingGroup(hueGroup.Locations);
                var baseLayer = streamingGroup.GetNewLayer(isBaseLayer: true);

                var lights = new List<Models.HueObjects.Light>();
                foreach (var entLight in baseLayer)
                {
                    var light = (from hueLight in allHueLights
                                 where hueLight.Id == entLight.Id.ToString()
                                 select hueLight).First();
                    lights.Add(new Models.HueObjects.Light(light, entLight));
                }
                hueDeviceGroups.Add(new Group(hueGroup, lights.ToList(), null));
            }

            return hueDeviceGroups;
        }

        public bool IsConnected { get; set; } = false;
        public async Task ConnectEntertainmentGroup(Models.HueObjects.Group entertainmentGroup)
        {
            await streamingClient.Connect(entertainmentGroup.HueGroup.Id);
            var fantasmicStream = new StreamingGroup(entertainmentGroup.HueGroup.Locations);
            cancellationTokenSource = new CancellationTokenSource();
            _ = streamingClient.AutoUpdate(fantasmicStream, cancellationTokenSource.Token, 50, onlySendDirtyStates: false);
            baseLayer = fantasmicStream.GetNewLayer(isBaseLayer: true);
            effectLayer = fantasmicStream.GetNewLayer();
            baseLayer.AutoCalculateEffectUpdate(cancellationTokenSource.Token);
            effectLayer.AutoCalculateEffectUpdate(cancellationTokenSource.Token);
            IsConnected = true;
        }

        /// <summary>
        /// Call Hue Action
        /// </summary>
        /// <param name="action">The acction to play</param>
        public void SendEntertainmentAction(HueAction action)
        {
            if (baseLayer == null)
            {
                throw new InvalidOperationException("Hue Client is not connected to Entertainment API.");
            }
            //pick lights selected in EntertainmentAction
            var ids = from target in action.TargetLights
                      select target.Id;
            var lights = from light in baseLayer
                         where ids.Contains(light.Id)
                         select light;
            //set light state
            CommonUtils.DebugHelper.WriteDebugLog($"HueClient: Sending action with Color '{action.Color.ToHex()}' Brightness '{action.Brightness}' Duration '{action.TransitionDuration.TotalSeconds} sec' Margin '{action.Margin.TotalSeconds} sec'");
            lights.SetState(cancellationTokenSource.Token, action.Color, action.Brightness, action.TransitionDuration);
        }

        /// <summary>
        /// Call Hue Effect
        /// </summary>
        /// <param name="effect">The effect to play</param>
        /// <param name="duration">Duration of the effect. The Effect stopps after this timespan.</param>
        public async Task SendEntertainmentEffect(HueEffect effect, TimeSpan duration)
        {
            if (baseLayer == null)
            {
                throw new InvalidOperationException("Hue Client is not connected to Entertainment API.");
            }

            if (effect.EffectMode == EffectModes.IteratorEffect)
            {
                await sendIteratorEffect(effect, duration, cancellationTokenSource.Token);
            }
            else if (effect.EffectMode == EffectModes.Actions)
            {
                await sendActionsEffect(effect, duration, cancellationTokenSource.Token);
            }
        }

        private async Task sendActionsEffect(HueEffect effect, TimeSpan duration, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();
            timer.Start();
            while (timer.ElapsedTicks < duration.Ticks)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                foreach (var action in effect.Actions)
                {
                    SendEntertainmentAction(action);
                    await Task.Delay(action.Margin);
                }
                await Task.Delay(effect.EffectMargin);
            }
        }

        private async Task sendIteratorEffect(HueEffect effect, TimeSpan duration, CancellationToken cancellationToken)
        {
            //pick lights selected in EntertainmentAction
            var ids = from target in effect.TargetLights
                      select target.Id;
            var lights = from light in baseLayer
                         where ids.Contains(light.Id)
                         select light;
            //call effect
            int iteration = 0;
            int lightCount = lights.Count();
            await lights.IteratorEffect(
                cancellationToken: cancellationToken,
                effectFunction: async (currentLight, cancel, timespan) =>
                {
                    foreach (var action in effect.Actions)
                    {
                        if (cancel.IsCancellationRequested)
                        {
                            cancel.ThrowIfCancellationRequested();
                        }
                        currentLight.SetState(cancel, action.Color, action.Brightness, action.TransitionDuration);
                        await Task.Delay(action.Margin);
                    }
                },
            mode: effect.IteratorEffectMode,
            waitTime: () =>
            {
                // twice as the actual count because waitTime is called before and after the execution
                if (Interlocked.Increment(ref iteration) >= lightCount * 2) 
                {
                    // reset when it reaches to the last light
                    Interlocked.Exchange(ref iteration, 0); 
                    return effect.EffectMargin;
                }
                else
                {
                    return effect.IteratorMargin;
                }
            },
                duration: duration
            );
        }

        #endregion

        #region Non-Entertainment APIs
        public async Task<List<Models.HueObjects.Group>> GetDeviceGroupsAsync()
        {
            var hueDeviceGroups = new List<Models.HueObjects.Group>();
            var allHueGroups = await client.GetGroupsAsync();
            var allHueScenes = await client.GetScenesAsync();
            var allHueLights = await client.GetLightsAsync();
            var allLights = new List<Models.HueObjects.Light>();
            foreach (var light in allHueLights)
            {
                allLights.Add(new Models.HueObjects.Light(light, null));
            }

            foreach (var hueGroup in allHueGroups)
            {
                var lights = from light in allLights
                             where hueGroup.Lights.Contains(light.HueLight.Id)
                             select light;


                List<Scene> scenes = new List<Scene>();
                if (allHueScenes != null)
                {
                    scenes.AddRange(from scene in allHueScenes
                                    where scene.Type != null
                                       && scene.Type == SceneType.GroupScene
                                       && scene.Group == hueGroup.Id
                                    select scene);
                }

                hueDeviceGroups.Add(new Models.HueObjects.Group(hueGroup, lights.ToList(), scenes.ToList()));
            }

            return hueDeviceGroups;
        }

        public async Task<Q42.HueApi.Models.Groups.Group> GetHueGroupByIdAsync(string id)
        {
            return await client.GetGroupAsync(id);
        }

        public async Task<Q42.HueApi.Light> SendCommandAsync(Q42.HueApi.Light light, RGBColor? color = null)
        {
            var command = new LightCommand();


            if (color != null)
            {
                command.SetColor((RGBColor)color);
            }

            command.On = light.State.On;
            command.Brightness = light.State.Brightness;
            await client.SendCommandAsync(command, new List<string>() { light.Id });
            return await client.GetLightAsync(light.Id);
        }

        public async Task SendCommandAsync(Q42.HueApi.Models.Groups.Group group)
        {
            var command = new LightCommand();

            command.On = group.Action.On;
            command.Brightness = group.Action.Brightness;

            await client.SendGroupCommandAsync(command, group.Id);
        }

        public async Task SendCommandAsync(Q42.HueApi.Models.Scene scene)
        {
            await client.RecallSceneAsync(scene.Id, scene.Group);
        }
        #endregion
    }
}
