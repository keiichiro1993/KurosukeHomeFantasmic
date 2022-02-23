using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    public class EntertainmentEffect
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<EntertainmentAction> Actions { get; set; }
        public IteratorEffectMode EffectMode { get; set; }
        public TimeSpan EffectMargin { get; set; }
    }
}
