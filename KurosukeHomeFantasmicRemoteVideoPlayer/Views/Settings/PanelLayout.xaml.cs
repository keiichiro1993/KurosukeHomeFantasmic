using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Controls.ContentDialogs;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils.DBHelpers;
using KurosukeHomeFantasmicRemoteVideoPlayer.ViewModels.Settings;
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

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Views.Settings
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class PanelLayout : Page
    {
        internal PanelLayoutPageViewModel ViewModel { get; set; } = new PanelLayoutPageViewModel();
        public PanelLayout()
        {
            this.InitializeComponent();
            this.Loaded += PanelLayout_Loaded;
        }

        private void PanelLayout_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.Dispose();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddUnitDialog();
            await dialog.ShowAsync();
        }

        private async void PanelLayoutListItem_DeleteButtonClicked(object sender, ItemDeleteButtonClickedEventArgs<Models.LEDPanelUnitSet> args)
        {
            try
            {
                AppGlobalVariables.LEDPanelUnitSets.Remove(args.DeleteItem);
                var helper = new PanelLayoutHelper();
                await helper.SaveLEDPanelUnitSets(AppGlobalVariables.LEDPanelUnitSets.ToList());
            }
            catch(Exception ex) {
                await DebugHelper.ShowErrorDialog(ex, "Failed to remove or save panel unit data.");
            }
        }
    }
}
