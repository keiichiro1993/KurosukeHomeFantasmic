using KurosukeHomeFantasmicUWP.Models.JSON;
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
        public enum TimelineHueItemType { Scene, Entertainment }
        public TimelineHueItemType Type { get; set; }
        public Visibility IsEntertainment { get { return Type == TimelineHueItemType.Entertainment ? Visibility.Visible : Visibility.Collapsed; } }
        public string Name { get; set; }
        public bool Locked { get; set; }

        public double Left { get { return CanvasWidth * (StartTime / TotalCanvasDuration); } }

        public double Width { get { return Type == TimelineHueItemType.Scene ? 50 : CanvasWidth * (Duration / TotalCanvasDuration); } }

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

        public TimelineVideoItemEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}
