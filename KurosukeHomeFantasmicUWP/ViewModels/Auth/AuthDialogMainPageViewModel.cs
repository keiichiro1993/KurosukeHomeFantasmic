using CommonUtils;

namespace KurosukeHomeFantasmicUWP.ViewModels.Auth
{
    public class AuthDialogMainPageViewModel : ViewModelBase
    {
        private bool _IsButtonAvailable = true;
        public bool IsButtonAvailable
        {
            get { return _IsButtonAvailable; }
            set
            {
                _IsButtonAvailable = value;
                RaisePropertyChanged();
            }
        }
    }
}
