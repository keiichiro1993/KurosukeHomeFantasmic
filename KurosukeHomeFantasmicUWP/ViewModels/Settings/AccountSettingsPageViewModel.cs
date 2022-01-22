using AuthCommon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.Settings
{
    public class AccountSettingsPageViewModel : ViewModelBase
    {
        public ObservableCollection<IUser> Users { get { return Utils.AppGlobalVariables.DeviceUsers; } }
    }
}
