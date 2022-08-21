using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.PreviewPages;
using System;
using System.Collections.Generic;
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

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.PreviewPages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class PreviewPage : Page
    {
        public PreviewPageViewModel ViewModel { get; set; } = new PreviewPageViewModel();
        public PreviewPage()
        {
            this.InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.GlobalViewModel.GlobalPlaybackState != Windows.Media.Playback.MediaPlaybackState.Playing)
            {
                ViewModel.GlobalViewModel.GlobalPlaybackState = Windows.Media.Playback.MediaPlaybackState.Playing;
                ViewModel.PlayButtonVisibility = Visibility.Collapsed;
            }
            else
            {
                ViewModel.GlobalViewModel.GlobalPlaybackState = Windows.Media.Playback.MediaPlaybackState.Paused;
                ViewModel.PlayButtonVisibility = Visibility.Visible;
            }
        }
    }

    internal class PreviewPlayerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate VideoTemplate { get; set; }
        public DataTemplate HueTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var timeline = item as Timeline;
            if (timeline == null)
            {
                throw new ArgumentNullException("Passed timeline item is null.");
            }
            switch (timeline.TimelineType)
            {
                case Timeline.TimelineTypes.Video:
                    return VideoTemplate;
                case Timeline.TimelineTypes.Hue:
                    return HueTemplate;
                default:
                    throw new InvalidOperationException($"Timeline type is not defined in template selector: {timeline.TimelineType}");
            }
        }
    }
}
