using Q42.HueApi;
using System;

namespace KurosukeHueClient.Models.HueObjects
{
    public class JsonState
    {
        public JsonState() { }
        public JsonState(State state)
        {
            On = state.On;
            Brightness = state.Brightness;
            Hue = state.Hue;
            Saturation = state.Saturation;
            ColorCoordinates = state.ColorCoordinates != null ? new int[] { (int)(state.ColorCoordinates[0] * 10000), (int)(state.ColorCoordinates[1] * 10000) } : null;
            ColorTemperature = state.ColorTemperature;
            Alert = state.Alert;
            Effect = state.Effect;
            ColorMode = state.ColorMode;
            TransitionTime = state.TransitionTime;
            Mode = state.Mode;
        }
        public bool On { get; set; }
        public byte Brightness { get; set; }
        public int? Hue { get; set; }
        public int? Saturation { get; set; }
        public int[] ColorCoordinates { get; set; }
        public int? ColorTemperature { get; set; }
        public Alert? Alert { get; set; }
        public Effect? Effect { get; set; }
        public string ColorMode { get; set; }
        public TimeSpan? TransitionTime { get; set; }
        public string Mode { get; set; }
    }
}
