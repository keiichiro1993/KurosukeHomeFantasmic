using KurosukeHomeFantasmicUWP.Views.Auth;
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

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Controls.ContentDialogs
{
    public sealed partial class AuthDialog : ContentDialog
    {
        public AuthDialog()
        {
            this.InitializeComponent(); this.Loaded += AuthDialog_Loaded;
        }

        private void AuthDialog_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(AuthDialogMainPage), this);
        }
    }
}
