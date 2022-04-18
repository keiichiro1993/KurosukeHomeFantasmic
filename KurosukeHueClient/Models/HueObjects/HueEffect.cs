using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    public class HueEffect
    {
        public enum EffectModes { Iterator, Actions }
        public EffectModes EffectMode { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<HueAction> Actions { get; set; }
        public int ActionsCount { get { return Actions.Count(); } }
        public IEnumerable<EntertainmentLight> TargetLights { get; set; }
        public IteratorEffectMode IteratorEffectMode { get; set; }
        public TimeSpan EffectMargin { get; set; }
    }
}
