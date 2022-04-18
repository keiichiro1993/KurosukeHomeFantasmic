using CommonUtils;
using KurosukeHomeFantasmicUWP.Controls.ContentDialogs;
using KurosukeHomeFantasmicUWP.Utils;
using KurosukeHomeFantasmicUWP.Utils.Auth;
using KurosukeHueClient.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.Auth
{
    public class AuthDialogHuePageViewModel : ViewModelBase
    {
        private AuthDialog dialogHost;

        public async void Init(AuthDialog dialogHost)
        {
            this.dialogHost = dialogHost;

            IsLoading = true;
            LoadingMessage = "Please press Link button on your Hue Bridge within 1 minute... We will discover it automatically for you.";

            try
            {
                var user = await HueAuthClient.RegisterHueBridge();
                AccountManager.SaveUserToVault(user);
                AppGlobalVariables.DeviceUsers.Add(user);

                dialogHost.Hide();
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "Failed to add Hue Bridge.");

                dialogHost.Hide();
            }
        }

    }
}
