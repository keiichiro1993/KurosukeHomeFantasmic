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

        public Models.Timeline.Timeline RemoteVideoTimeline
        {
            get => (Models.Timeline.Timeline)GetValue(RemoteVideoTimelineProperty);
            set => SetValue(RemoteVideoTimelineProperty, value);
        }

        public static readonly DependencyProperty RemoteVideoTimelineProperty =
            DependencyProperty.Register(nameof(RemoteVideoTimeline), typeof(Models.Timeline.Timeline), typeof(RemoteVideoTimelinePlayer),
                new PropertyMetadata(null, RemoteVideoTimelineChanged));

        private static void RemoteVideoTimelineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
