using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Views
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SplashScreen : Page
    {
        public SplashScreen()
        {
            this.InitializeComponent();
            this.Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            var ledpanelHelper = new Utils.DBHelpers.PanelLayoutHelper();
            AppGlobalVariables.LEDPanelUnitSets = new ObservableCollection<LEDPanelUnitSet>(await ledpanelHelper.GetLEDPanelUnitSets());

            try
            {
                var notFoundDevices = new List<string>();
                foreach (var unitSet in AppGlobalVariables.LEDPanelUnitSets)
                {
                    await unitSet.InitSerialClient();

                    if(unitSet.SerialClient == null)
                    {
                        notFoundDevices.Add(unitSet.SerialDeviceId);
                    }
                }

                if (notFoundDevices.Count > 0)
                {
                    await new MessageDialog($"The IDs of devices not found: {string.Join(", ", notFoundDevices)}", "Serial Device Not Connected").ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "Failed to load serial devices.");
            }

            Frame.Navigate(typeof(MainPage));
        }
    }
}
