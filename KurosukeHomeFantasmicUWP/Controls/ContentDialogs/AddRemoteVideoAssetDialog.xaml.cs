using CommonUtils;
using KurosukeBonjourService;
using KurosukeBonjourService.Models;
using KurosukeBonjourService.Models.Json;
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
        public AddRemoteVideoAssetDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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

        public VideoInfo SelectedVideo { get; set; }
    }
}
