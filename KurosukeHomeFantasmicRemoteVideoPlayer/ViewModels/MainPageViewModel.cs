using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.ViewModels
{
    internal class MainPageViewModel : ViewModelBase
    {
        private string videoFilePath;
        private bool isPlaying = false;
        private MediaSource _VideoMediaSource;
        public MediaSource VideoMediaSource
        {
            get { return _VideoMediaSource; }
            set
            {
                _VideoMediaSource = value;
                RaisePropertyChanged();
            }
        }
        private CoreDispatcher coreDispatcher;
        private byte[] currentFrameBytes;
        private MediaPlayerElement mediaPlayer;
        private SoftwareBitmapSource _VideoBitmap;
        public SoftwareBitmapSource VideoBitmap
        {
            get { return _VideoBitmap; }
            set
            {
                _VideoBitmap = value;
                RaisePropertyChanged();
            }
        }

        public async void Init(MediaPlayerElement mediaPlayer, CoreDispatcher dispatcher)
        {
            IsLoading = true;

            this.mediaPlayer = mediaPlayer;
            this.coreDispatcher = dispatcher;

            AppGlobalVariables.VideoList = await VideoFileHelper.GetVideoList();
            BonjourHelper.PlayVideoRequested += BonjourHelper_PlayVideoRequested;
            await BonjourHelper.StartServer();

            IsLoading = false;
        }

        /// <summary>
        /// Re-initialize Serial Client/Re-calculate image size when the Player (MainPage) is reloaded
        /// </summary>
        public async void InitSerialClient()
        {
            foreach (var unitSet in AppGlobalVariables.LEDPanelUnitSets)
            {
                await unitSet.InitSerialClient();
            }

            _unitWidth = SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelWidth) * SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitHorizontalPanelCount);
            _unitHeight = SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelHeight) * SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitVerticalPanelCount);
            _byteCountPerUnit = _unitWidth * _unitHeight * 4;

            if (AppGlobalVariables.LEDPanelUnitSets.Count > 0)
            {
                _unitCount = AppGlobalVariables.LEDPanelUnitSets.Max(panel => panel.Coordinate.Y) + 1;
                _imageWidth = _unitWidth;
                _imageHeight = _unitHeight * _unitCount;
            }
        }

        object serialClientLock = new object();
        DateTime lastTimestamp;
        private async void BonjourHelper_PlayVideoRequested(object sender, KurosukeBonjourService.Models.BonjourEventArgs.PlayVideoEventArgs e)
        {
            if (lastTimestamp != null && e.Timestamp < lastTimestamp)
            {
                // skip if old request received
                return;
            }
            lastTimestamp = e.Timestamp;

            DebugHelper.WriteDebugLog($"Data recieve... video position:{e.VideoTime} timestamp:{e.Timestamp} video status:{e.VideoStatus}");

            // set path if not equal
            if (!string.IsNullOrEmpty(e.VideoPath) && videoFilePath != e.VideoPath)
            {
                videoFilePath = e.VideoPath;

                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var storageFile = await StorageFile.GetFileFromPathAsync(videoFilePath);
                    var stream = await storageFile.OpenAsync(FileAccessMode.Read);
                    VideoMediaSource = MediaSource.CreateFromStream(stream, storageFile.ContentType);

                    // prepare media player
                    mediaPlayer.MediaPlayer.IsVideoFrameServerEnabled = true;
                    mediaPlayer.MediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;
                });
            }

            bool isMediaPlayerNull = false;
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (mediaPlayer.MediaPlayer == null)
                {
                    isMediaPlayerNull = true;
                    return;
                }

                // adjust position if difference is more than 500ms
                if ((mediaPlayer.MediaPlayer.PlaybackSession.Position - e.VideoTime).Duration() > TimeSpan.FromMilliseconds(500))
                {
                    DebugHelper.WriteDebugLog($"Adjust video timing... from {mediaPlayer.MediaPlayer.PlaybackSession.Position} to {e.VideoTime}");
                    mediaPlayer.MediaPlayer.PlaybackSession.Position = e.VideoTime;
                }
            });

            if (isMediaPlayerNull)
            {
                DebugHelper.WriteDebugLog("MediaPlayer not ready.");
                return;
            }

            if (e.VideoStatus == KurosukeBonjourService.Models.WebSocketServices.PlayVideoService.PlayVideoServiceStatus.Play)
            {
                bool needToPlay = false;
                lock (serialClientLock)
                {
                    if (!isPlaying)
                    {
                        isPlaying = true;
                        SendSerialLoop();
                        needToPlay = true;
                    }
                }
                if (needToPlay)
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mediaPlayer.MediaPlayer.Play();
                    });
                }
            }
            else
            {
                isPlaying = false;
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    mediaPlayer.MediaPlayer.Pause();
                });
            }
        }


        private int _unitWidth;
        private int _unitHeight;
        private int _byteCountPerUnit;
        private int _unitCount;

        private int _imageWidth;
        private int _imageHeight;

        private async void MediaPlayer_VideoFrameAvailable(MediaPlayer sender, object args)
        {
            CanvasDevice canvasDevice = CanvasDevice.GetSharedDevice();
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SoftwareBitmap frameServerDest = new SoftwareBitmap(BitmapPixelFormat.Rgba8, _imageWidth, _imageHeight, BitmapAlphaMode.Premultiplied);

                using (CanvasBitmap canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, frameServerDest))
                {
                    sender.CopyFrameToVideoSurface(canvasBitmap);
                    currentFrameBytes = canvasBitmap.GetPixelBytes(); //to send via serial
                }

                var bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(SoftwareBitmap.Convert(SoftwareBitmap.CreateCopyFromBuffer(currentFrameBytes.AsBuffer(), BitmapPixelFormat.Rgba8, _imageWidth, _imageHeight), BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied));
                VideoBitmap = bitmapSource; //to display result
            });
        }

        private List<Task> _sendSerialJobList = new List<Task>();
        private async void SendSerialLoop()
        {
            while (isPlaying)
            {
                if (currentFrameBytes != null)
                {
                    foreach (var panel in AppGlobalVariables.LEDPanelUnitSets)
                    {
                        if (panel.SerialClient == null)
                        {
                            // skip if the device is unavailable
                            continue;
                        }

                        _sendSerialJobList.Add(SendSerialJobPerUnit(panel));
                    }

                    await Task.WhenAll(_sendSerialJobList);
                }
                await Task.Delay(20);
            }
        }

        private async Task SendSerialJobPerUnit(Models.LEDPanelUnitSet panel)
        {
            var start = _byteCountPerUnit * panel.Coordinate.Y;
            var end = start + _byteCountPerUnit;
            
            try
            {
                await panel.SerialClient.WriteByteAsync(currentFrameBytes.Skip(start).Take(end).ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // dispose and recover client
                await panel.InitSerialClient();
            }
        }
    }
}
