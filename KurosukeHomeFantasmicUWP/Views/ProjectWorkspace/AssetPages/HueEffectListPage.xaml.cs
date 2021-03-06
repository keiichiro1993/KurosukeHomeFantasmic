using KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.AssetPages;
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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.AssetPages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class HueEffectListPage : Page
    {
        public HueEffectListPageViewModel ViewModel { get; set; } = new HueEffectListPageViewModel();
        public HueEffectListPage()
        {
            this.InitializeComponent();
        }

        private async void AddHueEffectButton_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).IsEnabled = false;
            var dialog = new Controls.ContentDialogs.AddHueEffectDialog();
            await dialog.ShowAsync();
            ((Button)sender).IsEnabled = true;
        }
    }
}
