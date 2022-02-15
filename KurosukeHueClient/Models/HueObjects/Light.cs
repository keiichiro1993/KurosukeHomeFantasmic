using AuthCommon.Models;
using KurosukeHueClient.Utils.Helpers;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using System;
using Windows.UI;
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

        private Color _Color;
        public Color Color
        {
            get
            {
                if (_Color == null) 
                {
                    _Color = ColorConverter.HexToColor("#" + HueLight.State.ToHex());
                }
                return _Color;
            }
            set
            {
                _Color = value;
            }
        }

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
