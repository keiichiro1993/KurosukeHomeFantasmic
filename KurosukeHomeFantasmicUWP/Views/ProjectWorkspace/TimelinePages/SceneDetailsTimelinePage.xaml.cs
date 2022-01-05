﻿using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.TimelinePages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.TimelinePages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SceneDetailsTimelinePage : Page
    {
        public SceneDetailsTimelinePageViewModel ViewModel { get; set; } = new SceneDetailsTimelinePageViewModel();
        public SceneDetailsTimelinePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Timelines = new ObservableCollection<Models.Timeline.ITimeline>();
            var timeline = new VideoTimeline();
            timeline.Id = Guid.NewGuid().ToString();
            timeline.Name = "test timeline 1";
            timeline.Description = "test timeline description";
            timeline.TimelineType = ITimeline.TimelineTypeEnum.Video;
            timeline.TargetDisplayId = Guid.NewGuid().ToString();
            ViewModel.Timelines.Add(timeline);
        }
    }
}