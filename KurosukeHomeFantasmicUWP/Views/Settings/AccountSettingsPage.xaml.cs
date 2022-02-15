using AuthCommon.Models;
using KurosukeHomeFantasmicUWP.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.Settings
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AccountSettingsPage : Page
    {
        public AccountSettingsPageViewModel ViewModel { get; set; } = new AccountSettingsPageViewModel();

        public AccountSettingsPage()
        {
            this.InitializeComponent();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Controls.ContentDialogs.AuthDialog();
            await dialog.ShowAsync();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Are you sure to delete account?", "Delete user");
            dialog.Commands.Add(new UICommand("Delete"));
            dialog.Commands.Add(new UICommand("Cancel"));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            var result = await dialog.ShowAsync();

            if (result.Label == "Delete")
            {
                IUser user = ((Button)sender).DataContext as IUser;
                Utils.Auth.AccountManager.DeleteUser(user);
                Utils.AppGlobalVariables.DeviceUsers.Remove(user);
            }
        }
    }
}
