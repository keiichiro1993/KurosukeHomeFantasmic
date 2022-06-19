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
        private Task autoUpdateTask; //probably OK to waste this with _
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
                cancellationTokenSource.Dispose();
            }
            if (autoUpdateTask != null) { autoUpdateTask.Dispose(); }

            //dispose all
            client = null;
            streamingClient.Dispose();
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

        public async Task ConnectEntertainmentGroup(Models.HueObjects.Group entertainmentGroup)
        {
            await streamingClient.Connect(entertainmentGroup.HueGroup.Id);
            var fantasmicStream = new StreamingGroup(entertainmentGroup.HueGroup.Locations);
            cancellationTokenSource = new CancellationTokenSource();
            //_ = streamingClient.AutoUpdate(fantasmicStream, cancellationTokenSource.Token, 50, onlySendDirtyStates: false);
            autoUpdateTask = streamingClient.AutoUpdate(fantasmicStream, cancellationTokenSource.Token, 50, onlySendDirtyStates: false);
            baseLayer = fantasmicStream.GetNewLayer(isBaseLayer: true);
            effectLayer = fantasmicStream.GetNewLayer();
            baseLayer.AutoCalculateEffectUpdate(cancellationTokenSource.Token);
            effectLayer.AutoCalculateEffectUpdate(cancellationTokenSource.Token);
        }

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
            lights.SetState(cancellationTokenSource.Token, action.Color, action.Brightness, action.Duration);
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
