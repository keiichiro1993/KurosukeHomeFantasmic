using AuthCommon.Models;
using CommonUtils;
using System.Collections.ObjectModel;

namespace KurosukeHomeFantasmicUWP.ViewModels.Settings
{
    public class AccountSettingsPageViewModel : ViewModelBase
    {
        public ObservableCollection<IUser> Users { get { return Utils.AppGlobalVariables.DeviceUsers; } }
    }
}
