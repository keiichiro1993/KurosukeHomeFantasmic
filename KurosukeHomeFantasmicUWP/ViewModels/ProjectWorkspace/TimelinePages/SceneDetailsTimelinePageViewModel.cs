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

        private TimeSpan _CurrentPosition = TimeSpan.Zero;
        public TimeSpan CurrentPosition
        {
            get { return _CurrentPosition; }
            set
            {
                _CurrentPosition = value;
                RaisePropertyChanged();
            }
        }

        private ShowScene _Scene;
        public ShowScene Scene
        {
            get { return _Scene; }
            set
            {
                _Scene = value;
                RaisePropertyChanged();
            }
        }
    }
}
