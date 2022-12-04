using CommonUtils;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.PreviewPages
{
    public class PreviewPageViewModel : ViewModelBase
    {
        public ProjectWorkspaceViewModel GlobalViewModel { get { return Utils.OnMemoryCache.GlobalViewModel; } }

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
