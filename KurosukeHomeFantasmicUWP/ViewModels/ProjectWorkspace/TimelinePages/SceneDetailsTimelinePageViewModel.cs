using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.TimelinePages
{
    public class SceneDetailsTimelinePageViewModel : ViewModelBase
    {
        private TimeSpan _TotalCanvasDuration = new TimeSpan(0, 15, 0);
        public TimeSpan TotalCanvasDuration
        {
            get { return _TotalCanvasDuration; }
            set
            {
                _TotalCanvasDuration = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<ITimeline> _Timelines;
        public ObservableCollection<ITimeline> Timelines
        {
            get { return _Timelines; }
            set
            {
                _Timelines = value;
                RaisePropertyChanged();
            }
        }
    }
}
