using AuthCommon.Models;
using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models
{
    public class HueUser : IUser
    {
        public Bridge Bridge;

        public string UserName { get; set; }
        public IToken Token { get; set; }
        public UserType UserType { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string Id { get; set; }

        //TODO: これらのInitializeと変更UI等
        public bool IsActive { get; set; }
        public Models.HueObjects.Group ActiveGroup { get; set; }

        public HueUser(Bridge bridge)
        {
            UserType = UserType.Hue;
            UserName = "Hue Bridge - " + bridge.Config.Name;
            Id = bridge.Config.BridgeId;
            ProfilePictureUrl = "/Assets/Icons/phillips_hue_logo.png";
            Bridge = bridge;
        }

        public HueUser() { }
    }
}
