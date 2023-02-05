using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// コンテンツ ダイアログの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Controls.ContentDialogs
{
    public sealed partial class AddUnitDialog : ContentDialog
    {
        internal AddUnitDialogViewModel ViewModel { get; set; } = new AddUnitDialogViewModel();
        public AddUnitDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddUnitDialog_Loaded;
        }

        private void AddUnitDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ViewModel.AddLEDPanelUnitSet();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }

    internal class AddUnitDialogViewModel : ViewModelBase
    {
        private List<DeviceInformation> _SerialDevices;
        public List<DeviceInformation> SerialDevices
        {
            get { return _SerialDevices; }
            set
            {
                _SerialDevices = value;
                RaisePropertyChanged();
            }
        }

        private DeviceInformation _SelectedSerialDevice;
        public DeviceInformation SelectedSerialDevice
        {
            get { return _SelectedSerialDevice; }
            set
            {
                if (value != null && value != _SelectedSerialDevice)
                {
                    _SelectedSerialDevice = value;
                    IsAddButtonEnabled = true;
                }
            }
        }

        private bool _IsAddButtonEnabled = false;
        public bool IsAddButtonEnabled
        {
            get { return _IsAddButtonEnabled; }
            set
            {
                _IsAddButtonEnabled = value;
                RaisePropertyChanged();
            }
        }

        public async void Init()
        {
            IsLoading = true;
            try
            {
                SerialDevices = await SerialClient.ListSerialDevices();
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "Failed to load serial devices.");
            }
            IsLoading = false;
        }

        public async void AddLEDPanelUnitSet()
        {
            var width = SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelWidth);
            var height = SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelHeight);
            AppGlobalVariables.LEDPanelUnitSets.Add(new Models.LEDPanelUnitSet
            {
                SerialDeviceId = SelectedSerialDevice.Id,
                SerialDeviceInformation = SelectedSerialDevice,
                Coordinate = new Models.LEDPanelUnitSetCoordinate(0, AppGlobalVariables.LEDPanelUnitSets.Count + 1),
                EnabledUnitMatrix = Enumerable.Repeat(Enumerable.Repeat(true, width).ToList(), height).ToList()
            });

            try 
            {
                var ledUnitSetsHelper = new Utils.DBHelpers.PanelLayoutHelper();
                await ledUnitSetsHelper.SaveLEDPanelUnitSets(AppGlobalVariables.LEDPanelUnitSets.ToList());
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "Failed to save panel layout.");
            }
        }
    }
}
