﻿using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    public class EntertainmentAction
    {
        public IEnumerable<EntertainmentLight> TargetLights { get; set; }
        public TimeSpan StartTime { get; set; }
        public RGBColor Color { get; set; }
        public double Brightness { get; set; } 
        public TimeSpan Duration { get; set; }
    }
}
