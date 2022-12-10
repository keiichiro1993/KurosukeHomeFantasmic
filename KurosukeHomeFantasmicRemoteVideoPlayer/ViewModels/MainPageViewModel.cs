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
        private SerialClient client;
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
                    var localSettings = ApplicationData.Current.LocalSettings;
                    if (localSettings.Values.ContainsKey("selectedSerialDeviceName"))
                    {
                        localSettings.Values["selectedSerialDeviceName"] = _SelectedSerialDevice.Name;
                    }
                    else
                    {
                        localSettings.Values.Add("selectedSerialDeviceName", _SelectedSerialDevice.Name);
                    }

                    InitSerialClient();
                }
            }
        }

        public async void Init(MediaPlayerElement mediaPlayer, CoreDispatcher dispatcher)
        {
            IsLoading = true;

            this.mediaPlayer = mediaPlayer;
            this.coreDispatcher = dispatcher;

            // prepare I/O
            SerialDevices = await SerialClient.ListSerialDevices();
            var localSettings = ApplicationData.Current.LocalSettings;
            var selected = localSettings.Values["selectedSerialDeviceName"];
            if (selected != null)
            {
                var match = (from device in SerialDevices
                             where device.Name == (string)selected
                             select device).FirstOrDefault();
                if (match != null)
                {
                    SelectedSerialDevice = match;
                    RaisePropertyChanged("SelectedSerialDevice");
                }
            }

            AppGlobalVariables.VideoList = await VideoFileHelper.GetVideoList();
            BonjourHelper.PlayVideoRequested += BonjourHelper_PlayVideoRequested;
            await BonjourHelper.StartServer();

            IsLoading = false;
        }

        public async void InitSerialClient()
        {
            if (client != null)
            {
                client.Dispose();
            }
            client = await SerialClient.CreateFromId(SelectedSerialDevice.Id);
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

            DebugHelper.WriteDebugLog($"Data recieve... video position:{e.VideoTime} timestamp:{e.Timestamp}");

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

        private int width = 64;
        private int height = 8;
        private int units = 4;
        private async void MediaPlayer_VideoFrameAvailable(MediaPlayer sender, object args)
        {
            CanvasDevice canvasDevice = CanvasDevice.GetSharedDevice();
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                SoftwareBitmap frameServerDest = new SoftwareBitmap(BitmapPixelFormat.Rgba8, width, height * units, BitmapAlphaMode.Premultiplied);

                using (CanvasBitmap canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(canvasDevice, frameServerDest))
                {
                    sender.CopyFrameToVideoSurface(canvasBitmap);
                    currentFrameBytes = canvasBitmap.GetPixelBytes(); //to send via serial
                }

                var bitmapSource = new SoftwareBitmapSource();
                await bitmapSource.SetBitmapAsync(SoftwareBitmap.Convert(SoftwareBitmap.CreateCopyFromBuffer(currentFrameBytes.AsBuffer(), BitmapPixelFormat.Rgba8, width, height * units), BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied));
                VideoBitmap = bitmapSource; //to display result
            });
        }

        private async void SendSerialLoop()
        {
            while (isPlaying)
            {
                if (currentFrameBytes != null)
                {
                    try
                    {
                        Debug.WriteLine($"Source frame bytes: {currentFrameBytes.Length}");
                        await client.WriteByteAsync(currentFrameBytes);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        // dispose and recover client
                        client.Dispose();
                        var serialDevices = await SerialClient.ListSerialDevices();
                        var sameNameDevice = (from serialdevice in serialDevices
                                              where serialdevice.Name == SelectedSerialDevice.Name
                                              select serialdevice).FirstOrDefault();
                        client = await SerialClient.CreateFromId(sameNameDevice.Id);
                    }
                }
                await Task.Delay(20);
            }
        }
    }
}
