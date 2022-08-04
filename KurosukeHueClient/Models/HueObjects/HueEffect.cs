using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    /// <summary>
    /// HueEffect will contains the target light list and HueAction(s).
    /// This object will work as a set of the HueActions.
    /// In case the HueActions objects has its' own TargetLights, it will be ignored and the TargetLights in this object will be used.
    /// </summary>
    public class HueEffect
    {
        public enum EffectModes { IteratorEffect, Actions }
        public EffectModes EffectMode { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public IEnumerable<HueAction> Actions { get; set; }
        public int ActionsCount { get { return Actions.Count(); } }
        public IEnumerable<EntertainmentLight> TargetLights { get; set; }
        public IteratorEffectMode IteratorEffectMode { get; set; }
        public TimeSpan EffectMargin { get; set; }
    }
}
