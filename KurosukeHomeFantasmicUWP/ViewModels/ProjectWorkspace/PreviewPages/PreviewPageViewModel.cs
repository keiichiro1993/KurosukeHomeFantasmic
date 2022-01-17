using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.PreviewPages
{
    public class PreviewPageViewModel : ViewModelBase
    {
        public ProjectWorkspaceViewModel GlobalViewModel { get { return Utils.AppGlobalVariables.GlobalViewModel; } }

        public ObservableCollection<ITimeline> Timelines { get { return Utils.OnMemoryCache.Scenes.First().Timelines; } }

        private Visibility _PlayButtonVisibility = Visibility.Visible;
        public Visibility PlayButtonVisibility
        {
            get { return _PlayButtonVisibility; }
            set
            {
                _PlayButtonVisibility = value;
                RaisePropertyChanged();
                RaisePropertyChanged("PauseButtonVisibility");
            }
        }

        public Visibility PauseButtonVisibility
        { get { return PlayButtonVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible; } }
    }
}
