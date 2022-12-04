using CommonUtils;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Players
{
    public sealed partial class VideoTimelinePreviewControl : UserControl
    {
        VideoTimelinePreviewControlViewModel ViewModel { get; set; } = new VideoTimelinePreviewControlViewModel();

        public VideoTimelinePreviewControl()
        {
            this.InitializeComponent();
        }

        public Models.Timeline.Timeline VideoTimeline
        {
            get => (Models.Timeline.Timeline)GetValue(VideoTimelineProperty);
            set => SetValue(VideoTimelineProperty, value);
        }

        public static readonly DependencyProperty VideoTimelineProperty =
            DependencyProperty.Register(nameof(VideoTimeline), typeof(Models.Timeline.Timeline), typeof(VideoTimelinePreviewControl),
                new PropertyMetadata(null, VideoTimelineChanged));

        private static void VideoTimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var previewControl = (VideoTimelinePreviewControl)d;
            previewControl.ViewModel.Timeline = (Models.Timeline.Timeline)e.NewValue;
        }
    }

    public class VideoTimelinePreviewControlViewModel : ViewModelBase
    {
        public ProjectWorkspaceViewModel GlobalViewModel { get { return Utils.OnMemoryCache.GlobalViewModel; } }
        private Models.Timeline.Timeline _Timeline;
        public Models.Timeline.Timeline Timeline
        {
            get { return _Timeline; }
            set
            {
                _Timeline = value;
                RaisePropertyChanged();
            }
        }
    }
}
