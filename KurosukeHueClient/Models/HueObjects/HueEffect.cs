using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KurosukeHueClient.Models.HueObjects
{
    /// <summary>
    /// HueEffect will contains the target light list and HueAction(s).
    /// This object will work as a set of the HueActions.
    /// </summary>

    public class HueEffect
    {
        public enum EffectModes { Actions, IteratorEffect, LightSourceEffect }
        public EffectModes EffectMode { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        /// <summary>
        /// Calculated Duration of one iteration.
        /// </summary>
        [JsonIgnore]
        public TimeSpan Duration
        {
            get
            {
                if (EffectMode == EffectModes.Actions)
                {
                    return TimeSpan.FromTicks(Actions.Sum(a => a.Margin.Ticks));
                }
                else
                {
                    return Margin * TargetLights.Count();
                }
            }
        }
        /// <summary>
        /// IteratorEffect: Timespan between the action to each light. The effect for the next light iterated starts after this duration.
        /// Actions: Timespan between the run of set of actions to be repeated.
        /// </summary>
        public TimeSpan Margin { get; set; }
        /// <summary>
        /// The list of actions to apply.
        /// IteratorEffect: Applied to each iterated light.
        /// Actions: The action will be executed in row.
        /// </summary>
        public List<HueAction> Actions { get; set; }
        [JsonIgnore]
        public int ActionsCount { get { return Actions.Count(); } }
        /// <summary>
        /// Target Lights for the Iterator Effect/Light Source Effect.
        /// This will be ignored with Actions mode.
        /// </summary>
        public IEnumerable<EntertainmentLight> TargetLights { get; set; }
        public IteratorEffectMode IteratorEffectMode { get; set; }
    }
}
