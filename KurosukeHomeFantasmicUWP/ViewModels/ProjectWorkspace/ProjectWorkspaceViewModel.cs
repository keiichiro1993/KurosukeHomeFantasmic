using KurosukeHomeFantasmicUWP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.System.Threading;
using Windows.UI.Core;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace
{
    public class ProjectWorkspaceViewModel : ViewModelBase
    {
        private MediaPlaybackState _GlobalPlaybackState = MediaPlaybackState.Paused;
        public MediaPlaybackState GlobalPlaybackState
        {
            get { return _GlobalPlaybackState; }
            set
            {
                _GlobalPlaybackState = value;
                RaisePropertyChanged();
                UpdateTimerState();
            }
        }

        private TimeSpan _GlobalCurrentPosition = TimeSpan.Zero;
        public TimeSpan GlobalCurrentPosition
        {
            get { return _GlobalCurrentPosition; }
            set
            {
                if (_GlobalCurrentPosition != value)
                {
                    /*if (basePosition != null)
                    {
                        basePosition = value;
                    }*/
                    _GlobalCurrentPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ShowScene _CurrentScene;
        public ShowScene CurrentScene
        {
            get { return _CurrentScene; }
            set
            {
                _CurrentScene = value;
                RaisePropertyChanged();
            }
        }


        private TimeSpan period = TimeSpan.FromMilliseconds(50);
        private DateTime? playStartTime = null;
        private TimeSpan? basePosition = null;
        private ThreadPoolTimer currentPositionTimer;

        private void UpdateTimerState()
        {
            if (GlobalPlaybackState == MediaPlaybackState.Playing && currentPositionTimer == null)
            {
                currentPositionTimer = ThreadPoolTimer.CreatePeriodicTimer(
                    async (source) =>
                    {
                        if (playStartTime == null)
                        {
                            playStartTime = DateTime.Now;
                            basePosition = GlobalCurrentPosition;
                        }

                        _GlobalCurrentPosition = DateTime.Now - (DateTime)playStartTime + (TimeSpan)basePosition;

                        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High,
                            () =>
                            {
                                RaisePropertyChanged("GlobalCurrentPosition");
                            });

                    }, period,
                    (source) =>
                    {
                        playStartTime = null;
                        basePosition = null;
                        currentPositionTimer = null;
                    }
                );
            }
            else if (GlobalPlaybackState != MediaPlaybackState.Playing && currentPositionTimer != null)
            {
                currentPositionTimer.Cancel();
            }
        }

    }
}
