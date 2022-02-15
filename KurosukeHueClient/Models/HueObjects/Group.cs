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
        public Group(Q42.HueApi.Models.Groups.Group group, List<Light> lights, List<Scene> scenes)
        {
            DeviceGroupName = group.Name + " - " + group.Class;
            HueGroup = group;
            HueScenes = scenes;
            Devices = new List<IDevice>(lights);
        }

        public Q42.HueApi.Models.Groups.Group HueGroup { get; set; }

        public Q42.HueApi.Models.Groups.GroupType? GroupType { get { return HueGroup.Type; } }

        public List<Scene> HueScenes { get; set; }

        public string DeviceGroupName { get; set; }

        public List<IDevice> Devices { get; set; }
    }
}
