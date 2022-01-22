using AuthCommon.Models;
using KurosukeHueClient.Utils;
using Q42.HueApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    public class Group : IDeviceGroup
    {
        public Group(Q42.HueApi.Models.Groups.Group group)
        {
            DeviceGroupName = group.Name + " - " + group.Class;
            HueGroup = group;
            HueScenes = new List<Scene>();
        }

        Q42.HueApi.Models.Groups.Group _HueGroup;

        public Q42.HueApi.Models.Groups.Group HueGroup
        {
            get { return _HueGroup; }
            set { _HueGroup = value; }
        }

        public List<Scene> HueScenes { get; set; }

        public string DeviceGroupName { get; set; }

        public List<IDevice> Devices { get; set; }

        public byte HueBrightness
        {
            get { return HueGroup.Action.Brightness; }
            set
            {
                if (HueGroup.Action.Brightness != value)
                {
                    HueGroup.Action.Brightness = value;
                    SelectedHueScene = null;
                    //SendGroupCommand(500);
                }
            }
        }

        public bool HueIsOn
        {
            get { return HueGroup.Action.On; }
            set
            {
                if (HueGroup.Action.On != value)
                {
                    HueGroup.Action.On = value;
                    SelectedHueScene = null;
                    //SendGroupCommand();
                }
            }
        }

        private Scene _SelectedHueScene;
        public Scene SelectedHueScene
        {
            get { return _SelectedHueScene; }
            set
            {
                if (_SelectedHueScene != value)
                {
                    _SelectedHueScene = value;
                    //SendSceneCommand();
                }
            }
        }

        /*
        private DateTime lastCommand;
        private async void SendGroupCommand(int delay = 0)
        {
            lastCommand = DateTime.Now;
            await Task.Delay(delay);
            if (delay == 0 || DateTime.Now - lastCommand >= new TimeSpan(0, 0, 0, 0, delay))
            {
                IsLoading = true;
                var appliance = Devices.FirstOrDefault() as Light;
                if (appliance != null)
                {
                    try
                    {
                        var client = new HueClient(appliance.HueUser);
                        await client.SendCommandAsync(HueGroup);
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Debugger.WriteErrorLog("Error occurred while sending group command.", ex);
                        await new MessageDialog("Error occurred while sending group command: " + ex.Message).ShowAsync();
                    }
                }
                IsLoading = false;
            }
        }

        private async void SendSceneCommand()
        {
            if (SelectedHueScene != null)
            {
                IsLoading = true;
                var appliance = Devices.FirstOrDefault() as Light;
                if (appliance != null && SelectedHueScene != null)
                {
                    try
                    {
                        var client = new HueClient(appliance.HueUser);
                        await client.SendCommandAsync(SelectedHueScene);
                        HueGroup = await client.GetHueGroupByIdAsync(HueGroup.Id);
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.Debugger.WriteErrorLog("Error occurred while sending scene command.", ex);
                        await new MessageDialog("Error occurred while sending scene command: " + ex.Message).ShowAsync();
                    }
                }
                IsLoading = false;
            }
        }
        */
    }
}
