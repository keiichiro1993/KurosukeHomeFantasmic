using AuthCommon.Models;
using Windows.UI.Xaml;

namespace KurosukeHueClient.Models.HueObjects
{
    public class Light : IDevice
    {
        public Light(Q42.HueApi.Light light, HueUser user)
        {
            HueLight = light;
            HueUser = user;
        }

        public Q42.HueApi.Light HueLight { get; set; }
        public HueUser HueUser { get; set; }

        public string DeviceName { get { return HueLight.Name; } }

        public string DeviceType { get { return HueLight.Type; } }

        public string IconImage
        {
            get
            {
                var path = Application.Current.RequestedTheme == ApplicationTheme.Light ? "ms-appx:///Assets/Icons/IRControls_black/" : "ms-appx:///Assets/Icons/IRControls_white/";
                return path + "Light_new.svg";
            }
        }
    }
}
