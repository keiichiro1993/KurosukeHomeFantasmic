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

        public DeviceInformation SelectedSerialDevice { get; set; }

        public async void Init(MediaPlayerElement mediaPlayer, CoreDispatcher dispatcher)
        {
            IsLoading = true;

            this.mediaPlayer = mediaPlayer;
            this.coreDispatcher = dispatcher;

            // prepare media player
            mediaPlayer.MediaPlayer.IsVideoFrameServerEnabled = true;
            mediaPlayer.MediaPlayer.VideoFrameAvailable += MediaPlayer_VideoFrameAvailable;

            // prepare I/O
            SerialDevices = await SerialClient.ListSerialDevices();
            AppGlobalVariables.VideoList = await VideoFileHelper.GetVideoList();
            BonjourHelper.PlayVideoRequested += BonjourHelper_PlayVideoRequested;
            BonjourHelper.StartServer();

            IsLoading = false;
        }

        private async void BonjourHelper_PlayVideoRequested(object sender, KurosukeBonjourService.Models.Json.PlayVideoEventArgs e)
        {
            // set path if not equal
            var path = (from item in AppGlobalVariables.VideoList
                        where item.Name == e.VideoName
                        select item.Path).FirstOrDefault();
            if (videoFilePath != path)
            {
                videoFilePath = path;
                VideoMediaSource = MediaSource.CreateFromUri(new Uri(path));
            }

            // adjust position if difference is more than 500ms
            var videoPosition = mediaPlayer.MediaPlayer.PlaybackSession.Position - e.VideoTime;
            if ((mediaPlayer.MediaPlayer.PlaybackSession.Position - videoPosition).Duration() > TimeSpan.FromMilliseconds(500))
            {
                mediaPlayer.MediaPlayer.PlaybackSession.Position = videoPosition;
            }

            if (e.VideoStatus == KurosukeBonjourService.Models.WebSocketServices.PlayVideoService.PlayVideoServiceStatus.Play)
            {
                if (!isPlaying)
                {
                    await Play();
                }
            }
            else
            {
                isPlaying = false;
                mediaPlayer.MediaPlayer.Pause();
            }
        }

        public async Task Play()
        {
            if (SelectedSerialDevice == null || client != null)
            {
                return;
            }
            
            client = await SerialClient.CreateFromId(SelectedSerialDevice.Id);

            isPlaying = true;
            SendSerialLoop();
            mediaPlayer.MediaPlayer.Play();
        }

        private int width = 64;
        private int height = 8;
        private int units = 2;
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
