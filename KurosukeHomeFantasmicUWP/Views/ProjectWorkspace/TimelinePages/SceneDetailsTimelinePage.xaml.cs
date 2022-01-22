using KurosukeHomeFantasmicUWP.Controls.ContentDialogs;
using KurosukeHomeFantasmicUWP.Models;
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
            var scene = e.Parameter as ShowScene;
            if (scene.Timelines == null)
            {
                scene.Timelines = new ObservableCollection<Timeline>();
            }
            ViewModel.Scene = scene;
            Utils.OnMemoryCache.GlobalViewModel.CurrentScene = scene;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Utils.OnMemoryCache.GlobalViewModel.CurrentScene = null;
        }

        private async void AddTimelineButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddTimelineDialog(ViewModel.Scene);
            await dialog.ShowAsync();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}
