using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public class TimelineHueItem : ViewModels.ViewModelBase, ITimelineItem
    {
        private HueAction hueAction;
        private HueEffect hueEffect;
        public TimelineHueItem() { }
        public TimelineHueItem(HueAction action)
        {
            hueAction = action;
            Duration = action.Duration;
        }
        public TimelineHueItem(HueEffect effect)
        {
            hueEffect = effect;
            Duration = effect.Duration;
        }

        public enum TimelineHueItemTypes { Action, Actions, IteratorEffect }
        public TimelineHueItemTypes HueItemType
        {
            get
            {
                if (hueAction != null)
                {
                    return TimelineHueItemTypes.Action;
                }
                else
                {
                    return hueEffect.EffectMode == HueEffect.EffectModes.Actions ? TimelineHueItemTypes.Actions : TimelineHueItemTypes.IteratorEffect;
                }
            }
        }
        public Visibility IsResizable { get { return HueItemType == TimelineHueItemTypes.IteratorEffect ? Visibility.Visible : Visibility.Collapsed; } }
        public string Name { get; set; }
        public bool Locked { get; set; }

        public double Left { get { return CanvasWidth * (StartTime / TotalCanvasDuration); } }

        public double Width { get { return CanvasWidth * (Duration / TotalCanvasDuration); } }

        private TimeSpan _StartTime;
        public TimeSpan StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime != value)
                {
                    _StartTime = value >= TimeSpan.Zero ? (value <= TotalCanvasDuration - Duration ? value : TotalCanvasDuration - Duration) : TimeSpan.Zero;
                    RaisePositionPropertyChanged();
                }
            }
        }
        public TimeSpan EndTime { get { return StartTime + Duration; } }
        private TimeSpan _Duration;
        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                if (_Duration != value)
                {
                    _Duration = value;
                    RaisePositionPropertyChanged();
                }
            }
        }

        private double _CanvasWidth;
        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set
            {
                _CanvasWidth = value;
                RaisePositionPropertyChanged();
            }
        }

        private TimeSpan _TotalCanvasDuration;
        public TimeSpan TotalCanvasDuration
        {
            get { return _TotalCanvasDuration; }
            set
            {
                _TotalCanvasDuration = value;
                RaisePositionPropertyChanged();
            }
        }


        private void RaisePositionPropertyChanged()
        {
            RaisePropertyChanged("StartTime");
            RaisePropertyChanged("Duration");
            RaisePropertyChanged("EndTime");
            RaisePropertyChanged("Width");
            RaisePropertyChanged("Left");
        }

        public string ItemId
        {
            get
            {
                switch (HueItemType)
                {
                    case TimelineHueItemTypes.Action:
                        return hueAction.Id;
                    case TimelineHueItemTypes.Actions:
                        return hueEffect.Id;
                    case TimelineHueItemTypes.IteratorEffect:
                        return hueEffect.Id;
                    default:
                        return null;
                }
            }
        }

        public async Task Init(ITimelineItemEntity entity)
        {
            await Task.Run(() =>
            {
                var hueEntity = (TimelineHueItemEntity)entity;
                if (hueEntity.HueItemType == TimelineHueItemTypes.Action)
                {
                    var actions = from action in Utils.OnMemoryCache.HueActions
                                  where action.Id == hueEntity.HueItemId
                                  select action;
                    if (actions.Any())
                    {
                        hueAction = actions.First();
                        _StartTime = hueEntity.StartTime;
                        Locked = hueEntity.Locked;
                        _Duration = hueAction.Duration;
                    }
                    else
                    {
                        throw new InvalidOperationException("Hue Action with ID " + hueEntity.HueItemId + " not found.");
                    }
                }
                else
                {
                    var effects = from effect in Utils.OnMemoryCache.HueEffects
                                  where effect.Id == hueEntity.HueItemId
                                  select effect;
                    if (effects.Any())
                    {
                        hueEffect = effects.First();
                        _StartTime = hueEntity.StartTime;
                        Locked = hueEntity.Locked;
                        _Duration = hueEntity.Duration;
                    }
                }
            });
        }

        public ITimelineItemEntity ToEntity()
        {
            var entity = new TimelineHueItemEntity();
            entity.StartTime = StartTime;
            entity.Locked = Locked;
            entity.Duration = Duration;
            entity.HueItemType = HueItemType;
            entity.HueItemId = hueAction != null ? hueAction.Id : hueEffect.Id;
            return entity;
        }
    }
}
