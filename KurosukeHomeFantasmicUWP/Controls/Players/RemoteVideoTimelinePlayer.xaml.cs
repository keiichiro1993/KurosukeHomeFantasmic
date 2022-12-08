using CommonUtils;
using KurosukeHomeFantasmicUWP.Controls.Players;
using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace;
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

// ユーザー コントロールの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.Players
{
    public sealed partial class RemoteVideoTimelinePlayer : UserControl
    {
        RemoteVideoTimelinePlayerControlViewModel ViewModel { get; set; } = new RemoteVideoTimelinePlayerControlViewModel();
        public RemoteVideoTimelinePlayer()
        {
            this.InitializeComponent();
        }

        public Models.Timeline.Timeline HueTimeline
        {
            get => (Models.Timeline.Timeline)GetValue(HueTimelineProperty);
            set => SetValue(HueTimelineProperty, value);
        }

        public static readonly DependencyProperty HueTimelineProperty =
            DependencyProperty.Register(nameof(HueTimeline), typeof(Models.Timeline.Timeline), typeof(HueTimelinePlayer),
                new PropertyMetadata(null, HueTimelineChanged));

        private static void HueTimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var playerControl = (RemoteVideoTimelinePlayer)d;
            playerControl.ViewModel.Timeline = (Models.Timeline.Timeline)e.NewValue;
        }
    }

    public class RemoteVideoTimelinePlayerControlViewModel : ViewModelBase
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
