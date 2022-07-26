﻿using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace KurosukeHueClient.Models.HueObjects
{
    /// <summary>
    /// HueAction represents the light state including the Color, Brightness and change duration.
    /// </summary>
    public class HueAction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<EntertainmentLight> TargetLights { get; set; }
        public RGBColor Color { get; set; }
        public double Brightness { get; set; } 
        public TimeSpan Duration { get; set; }
    }
}
