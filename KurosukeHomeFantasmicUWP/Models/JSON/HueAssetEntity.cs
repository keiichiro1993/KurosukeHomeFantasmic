using KurosukeHueClient.Models.HueObjects;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public class HueAssetEntity
    {
        public HueAssetEntity() { }
        public HueAssetEntity(List<HueAction> hueActions, List<HueEffect> hueEffects)
        {
            HueActions = (from action in hueActions
                         select new HueActionEntity(action)).ToList();
            HueEffects = (from effect in hueEffects
                         select new HueEffectEntity(effect)).ToList();
        }

        public List<HueActionEntity> HueActions { get; set; }
        public List<HueEffectEntity> HueEffects { get; set; }
    }

    public class HueActionEntity
    {
        public HueActionEntity() { }
        public HueActionEntity(HueAction action)
        {
            Id = action.Id;
            Name = action.Name;
            Description = action.Description;
            TargetLightIds = (from light in action.TargetLights
                              select light.Id).ToList();
            Color = action.Color;
            Brightness = action.Brightness;
            Duration = action.Duration;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<byte> TargetLightIds { get; set; }
        public RGBColor Color { get; set; }
        public double Brightness { get; set; }
        public TimeSpan Duration { get; set; }

        public HueAction ToHueAction(List<EntertainmentLight> lights)
        {
            var action = new HueAction();
            action.Id = Id;
            action.Name = Name;
            action.Description = Description;
            action.TargetLights = (from light in lights
                                  where TargetLightIds.Contains(light.Id)
                                  select light).ToList();
            action.Color = Color;
            action.Brightness = Brightness;
            action.Duration = Duration;

            return action;
        }
    }

    public class HueEffectEntity
    {
        public HueEffectEntity() { }
        public HueEffectEntity(HueEffect effect)
        {
            EffectMode = effect.EffectMode;
            Id = effect.Id;
            Name = effect.Name;
            Description = effect.Description;
            StartTime = effect.StartTime;
            Duration = effect.Duration;
            ActionIds = (from action in effect.Actions
                         select action.Id).ToList();
            TargetLightIds = (from light in effect.TargetLights
                              select light.Id).ToList();
            IteratorEffectMode = effect.IteratorEffectMode;
            EffectMargin = effect.EffectMargin;
        }
        public HueEffect.EffectModes EffectMode { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> ActionIds { get; set; }
        public List<byte> TargetLightIds { get; set; }
        public IteratorEffectMode IteratorEffectMode { get; set; }
        public TimeSpan EffectMargin { get; set; }

        public HueEffect ToHueEffect(List<EntertainmentLight> lights, List<HueAction> actions)
        {
            var effect = new HueEffect();
            effect.Id = Id;
            effect.Name = Name;
            effect.Description = Description;
            effect.StartTime = StartTime;
            effect.Duration = Duration;
            effect.Actions = (from action in actions
                             where ActionIds.Contains(action.Id)
                             select action).ToList();
            effect.TargetLights = (from light in lights
                                   where TargetLightIds.Contains(light.Id)
                                   select light).ToList();
            effect.IteratorEffectMode = IteratorEffectMode;
            effect.EffectMargin = EffectMargin;

            return effect;
        }
    }
}
