using FFmpegInterop;
using KurosukeHomeFantasmicUWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace KurosukeHomeFantasmicUWP.Views.ProjectWorkspace.VideoAssetPages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class VideoPlaybackPage : Page
    {
        public Models.VideoAsset VideoAsset { get; set; }
        private MediaSource mediaSource;

        public VideoPlaybackPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VideoAsset = e.Parameter as VideoAsset;
            this.Loaded += VideoPlaybackPage_Loaded;
        }

        private async void VideoPlaybackPage_Loaded(object sender, RoutedEventArgs e)
        {
            var file = await VideoAsset.GetVideoAssetFile();
            mediaSource = MediaSource.CreateFromStorageFile(file);
            mediaPlayerElement.Source = mediaSource;
            mediaPlayerElement.MediaPlayer.Play();
            //SetMediaPlayerWithFFMpeg(file);
        }

        private async void SetMediaPlayerWithFFMpeg(StorageFile file)
        {
            // FFmpeg
            var config = new FFmpegInteropConfig();
            config.VideoDecoderMode = FFmpegInterop.VideoDecoderMode.Automatic;
            var stream = (await file.OpenAsync(FileAccessMode.Read)).AsStream();
            var ffmpegStream = await FFmpegInteropMSS.CreateFromStreamAsync(stream.AsRandomAccessStream(), config);
            var mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = ffmpegStream.CreateMediaPlaybackItem();
            mediaPlayerElement.SetMediaPlayer(mediaPlayer);
            mediaPlayerElement.MediaPlayer.Play();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                if (mediaSource != null)
                {
                    mediaPlayerElement.MediaPlayer.Pause();
                    mediaSource.Dispose();
                }
                Frame.GoBack();
            }
        }
    }
}
