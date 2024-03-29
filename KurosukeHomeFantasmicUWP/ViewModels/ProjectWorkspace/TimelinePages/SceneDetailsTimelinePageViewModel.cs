﻿using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using System;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.TimelinePages
{
    public class SceneDetailsTimelinePageViewModel : ViewModelBase
    {
        public ProjectWorkspaceViewModel GlobalViewModel { get { return Utils.OnMemoryCache.GlobalViewModel; } }

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
