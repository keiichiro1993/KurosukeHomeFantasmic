using AuthCommon.Models;
using KurosukeHueClient.Models;
using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models;
using Q42.HueApi.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Utils
{
    public class HueClient : IDisposable
    {
        private StreamingHueClient streamingClient;
        private LocalHueClient client;
        private Bridge bridge;
        private HueUser user;
        public HueClient(HueUser user)
        {
            this.user = user;
            bridge = user.Bridge;
            streamingClient = new StreamingHueClient(bridge.Config.IpAddress, user.Token.AccessToken, user.Token.EntertainmentKey);
            client = (LocalHueClient)streamingClient.LocalHueClient;
        }

        public void Dispose()
        {
            client = null;
            streamingClient.Dispose();
        }

        public async Task<List<Models.HueObjects.Group>> GetEntertainmentGroupsAsync()
        {
            var hueDeviceGroups = new List<Models.HueObjects.Group>();
            var allLights = new List<Models.HueObjects.Light>();
            var allHueGroups = await client.GetEntertainmentGroups();
            var allHueLights = await client.GetLightsAsync();

            foreach (var light in allHueLights)
            {
                allLights.Add(new Models.HueObjects.Light(light, user));
            }

            foreach (var hueGroup in allHueGroups)
            {
                var lights = from light in allLights
                             where hueGroup.Lights.Contains(light.HueLight.Id)
                             select light;
                hueDeviceGroups.Add(new Models.HueObjects.Group(hueGroup, lights.ToList(), null));
            }

            return hueDeviceGroups;
        }

        public async Task<List<Models.HueObjects.Group>> GetDeviceGroupsAsync()
        {
            var hueDeviceGroups = new List<Models.HueObjects.Group>();
            var allHueGroups = await client.GetGroupsAsync();
            var allHueScenes = await client.GetScenesAsync();
            var allHueLights = await client.GetLightsAsync();
            var allLights = new List<Models.HueObjects.Light>();
            foreach (var light in allHueLights)
            {
                allLights.Add(new Models.HueObjects.Light(light, user));
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

        public async Task<Light> SendCommandAsync(Light light, RGBColor? color = null)
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
    }
}
