using CommonUtils;
using KurosukeBonjourService;
using KurosukeBonjourService.Models;
using KurosukeBonjourService.Models.Json;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Utils;
using KurosukeHomeFantasmicUWP.Utils.RequestHelpers;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Search;
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
    public sealed partial class AddRemoteVideoAssetDialog : ContentDialog
    {
        public AddRemoteVideoAssetDialogViewModel ViewModel { get; set; } = new AddRemoteVideoAssetDialogViewModel();
        public AddRemoteVideoAssetDialog()
        {
            this.InitializeComponent();
            this.Loaded += AddRemoteVideoAssetDialog_Loaded;
        }

        private void AddRemoteVideoAssetDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Init();
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await ViewModel.AddRemoteVideo();
            }
            catch (Exception ex)
            {
                Hide();
                await DebugHelper.ShowErrorDialog(ex, "Failed to connect.");
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }

    public class AddRemoteVideoAssetDialogViewModel : ViewModelBase
    {
        public ObservableCollection<QueryResponseItem> BonjourDevices { get { return OnMemoryCache.BonjourDevices; } }
        public async void Init()
        {
            LoadingMessage = "Searching for local network devices...";
            IsLoading = true;
            try
            {
                await BonjourHelper.UpdateBonjourDevices();
            }
            catch (Exception ex)
            {
                DebugHelper.WriteErrorLog(ex, "Failed to find local network devices.");
            }
            IsLoading = false;
        }

        public async void RetrieveVideoList()
        {
            LoadingMessage = "Retrieving available videos from remote device...";
            IsLoading = true;
            try
            {
                AvailableVideos = await BonjourHelper.GetAvailableVideo(SelectedDevice);
                IsVideoSelectionEnabled = true;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteErrorLog(ex, "Failed to retrieve available Videos.");
            }
            IsLoading = false;
        }

        private QueryResponseItem _SelectedDevice;
        public QueryResponseItem SelectedDevice
        {
            get { return _SelectedDevice; }
            set
            {
                _SelectedDevice = value;
                RetrieveVideoList();
            }
        }

        private bool _IsVideoSelectionEnabled = false;
        public bool IsVideoSelectionEnabled
        {
            get { return _IsVideoSelectionEnabled; }
            set
            {
                _IsVideoSelectionEnabled = value;
                RaisePropertyChanged();
            }
        }

        private List<VideoInfo> _AvailableVideos;
        public List<VideoInfo> AvailableVideos
        {
            get { return _AvailableVideos; }
            set
            {
                _AvailableVideos = value;
                RaisePropertyChanged();
            }
        }

        private VideoInfo _SelectedVideo;
        public VideoInfo SelectedVideo
        {
            get { return _SelectedVideo; }
            set
            {
                _SelectedVideo = value;
                IsPrimaryButtonEnabled = true;
            }
        }

        public async Task AddRemoteVideo()
        {
            IsLoading = true;
            LoadingMessage = "Preparing connection to the device...";
            var bonjourClient = (from client in AppGlobalVariables.BonjourClients
                                 where client.Device.DomainName == SelectedDevice.DomainName
                                 select client).FirstOrDefault();

            if (bonjourClient == null)
            {
                bonjourClient = new BonjourClient(SelectedDevice);
                AppGlobalVariables.BonjourClients.Add(bonjourClient);
            }

            OnMemoryCache.RemoteVideoAssets.Add(
                new RemoteVideoAsset
                {
                    Info = SelectedVideo,
                    DomainName = SelectedDevice.DomainName,
                    HostName = SelectedDevice.InstanceName,
                    Id = Guid.NewGuid().ToString(),
                }
            );

            IsLoading = false;
        }
    }
}
