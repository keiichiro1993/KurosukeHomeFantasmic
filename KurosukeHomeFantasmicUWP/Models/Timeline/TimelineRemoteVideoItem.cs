using CommonUtils;
using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHomeFantasmicUWP.Utils;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public class TimelineRemoteVideoItem : ViewModelBase, ITimelineItem
    {
        private RemoteVideoAsset _RemoteVideoAsset;
        public RemoteVideoAsset RemoteVideoAsset
        {
            get { return _RemoteVideoAsset; }
            set
            {
                _RemoteVideoAsset = value;
                RaisePropertyChanged();
            }
        }

        public bool IsResizable
        {
            get { return RemoteVideoAsset.Info.Length > TimeSpan.FromSeconds(10); }
        }

        // Timeline Position
        private TimeSpan _StartTime;
        public TimeSpan StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime != value)
                {
                    _StartTime = value >= TimeSpan.Zero ? (value <= TotalCanvasDuration - Duration ? value : TotalCanvasDuration - Duration) : TimeSpan.Zero;
                    RaisePositionPropertyChanged();
                }
            }
        }
        public TimeSpan Duration
        {
            get { return VideoEndPosition - VideoStartPosition; }
            set { throw new Exception("Duration of Remote Video Asset is read-only"); }
        }
        public TimeSpan EndTime { get { return StartTime + Duration; } }

        // Video Position
        private TimeSpan _VideoStartPosition;
        public TimeSpan VideoStartPosition
        {
            get { return _VideoStartPosition; }
            set
            {
                if (_VideoStartPosition != value)
                {
                    var previousValue = VideoStartPosition;
                    var validatedValue = value >= TimeSpan.Zero ? (value <= VideoEndPosition.Add(TimeSpan.FromSeconds(-10)) ? value : VideoEndPosition.Add(TimeSpan.FromSeconds(-10))) : TimeSpan.Zero;
                    _VideoStartPosition = validatedValue;
                    StartTime += validatedValue - previousValue;
                    RaisePositionPropertyChanged();
                }
            }
        }

        private TimeSpan _VideoEndPosition;
        public TimeSpan VideoEndPosition
        {
            get { return _VideoEndPosition; }
            set
            {
                if (_VideoEndPosition != value)
                {
                    _VideoEndPosition = value <= RemoteVideoAsset.Info.Length ? (value >= VideoStartPosition.Add(TimeSpan.FromSeconds(10)) ? value : VideoStartPosition.Add(TimeSpan.FromSeconds(10))) : RemoteVideoAsset.Info.Length;
                    RaisePositionPropertyChanged();
                }
            }
        }

        private void RaisePositionPropertyChanged()
        {
            RaisePropertyChanged("StartTime");
            RaisePropertyChanged("Duration");
            RaisePropertyChanged("EndTime");
            RaisePropertyChanged("Width");
            RaisePropertyChanged("Left");
            RaisePropertyChanged("VideoStartPosition");
            RaisePropertyChanged("VideoEndPosition");
        }

        // View Position
        public double Left { get { return CanvasWidth * (StartTime / TotalCanvasDuration); } }
        public double Width { get { return CanvasWidth * (Duration / TotalCanvasDuration); } }
        private double _CanvasWidth;
        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set
            {
                _CanvasWidth = value;
                RaisePositionPropertyChanged();
            }
        }

        private TimeSpan _TotalCanvasDuration;
        public TimeSpan TotalCanvasDuration
        {
            get { return _TotalCanvasDuration; }
            set
            {
                _TotalCanvasDuration = value;
                RaisePropertyChanged("Left");
                RaisePropertyChanged("Width");
            }
        }

        public bool Locked { get; set; }

        public string ItemId { get { return RemoteVideoAsset.Id; } }

        // newly added video
        public async Task Init(RemoteVideoAsset remoteVideoAsset)
        {
            _RemoteVideoAsset = remoteVideoAsset;
            _VideoStartPosition = new TimeSpan(0, 0, 0);
            _VideoEndPosition = RemoteVideoAsset.Info.Length;
        }

        // from saved json
        public async Task Init(ITimelineItemEntity entity)
        {
            var videoEntity = (TimelineVideoItemEntity)entity;
            var videoAssets = from item in OnMemoryCache.RemoteVideoAssets
                              where item.Id == videoEntity.VideoAssetId
                              select item;
            if (!videoAssets.Any())
            {
                throw new InvalidOperationException("Remote Video Asset with ID " + videoEntity.VideoAssetId + " not found in Asset List.");
            }

            RemoteVideoAsset = videoAssets.First();
            var bonjourClient = from client in AppGlobalVariables.BonjourClients
                                where client.Device.DomainName == RemoteVideoAsset.DomainName
                                select client;

            if (!bonjourClient.Any())
            {
                var bonjourDevice = from device in OnMemoryCache.BonjourDevices
                                    where device.DomainName == RemoteVideoAsset.DomainName
                                    select device;

                // Not throwing even if device is not available. This should be expected if the devices are not ready. Player should retry connection when necessary.
                if (bonjourDevice.Any())
                {
                    AppGlobalVariables.BonjourClients.Add(new KurosukeBonjourService.BonjourClient(bonjourDevice.First()));
                }
            }

            _StartTime = videoEntity.StartTime;
            _VideoStartPosition = videoEntity.VideoStartPosition;
            _VideoEndPosition = videoEntity.VideoEndPosition;
            Locked = videoEntity.Locked;
        }

        public ITimelineItemEntity ToEntity()
        {
            var entity = new TimelineVideoItemEntity();
            entity.VideoAssetId = RemoteVideoAsset.Id;
            entity.StartTime = StartTime;
            entity.VideoEndPosition = VideoEndPosition;
            entity.VideoStartPosition = VideoStartPosition;
            entity.Locked = Locked;
            return entity;
        }
    }
}
