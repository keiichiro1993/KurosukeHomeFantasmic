using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public class TimelineVideoItem : ViewModels.ViewModelBase, ITimelineItem
    {
        private VideoAsset _VideoAsset;
        public VideoAsset VideoAsset
        {
            get { return _VideoAsset; }
            set
            {
                _VideoAsset = value;
                RaisePropertyChanged();
            }
        }

        private StorageFile videoFile;
        private VideoProperties videoProperties;

        public string VideoAssetId { get; set; }
        public bool IsResizable
        {
            get { return videoProperties.Duration > TimeSpan.FromSeconds(10); }
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
            set { throw new InvalidOperationException("Duration for Video Item is read-only."); }
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
                    _VideoEndPosition = value <= videoProperties.Duration ? (value >= VideoStartPosition.Add(TimeSpan.FromSeconds(10)) ? value : VideoStartPosition.Add(TimeSpan.FromSeconds(10))) : videoProperties.Duration;
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

        public MediaSource VideoMediaSource { get; set; }

        public string ItemId { get { return VideoAssetId; } }

        // newly added video
        public async Task Init(VideoAsset videoAsset)
        {
            IsLoading = true;

            _VideoAsset = videoAsset;
            VideoAssetId = videoAsset.VideoAssetEntity.Id;
            videoFile = await videoAsset.GetVideoAssetFile();
            VideoMediaSource = MediaSource.CreateFromStorageFile(videoFile);
            videoProperties = await videoFile.Properties.GetVideoPropertiesAsync();

            _VideoStartPosition = new TimeSpan(0, 0, 0);
            _VideoEndPosition = videoProperties.Duration;

            IsLoading = false;
        }

        // from saved json
        public async Task Init(ITimelineItemEntity entity)
        {
            var videoEntity = (TimelineVideoItemEntity)entity;
            var videoAssets = from item in Utils.OnMemoryCache.VideoAssetCache
                              where item.VideoAssetEntity.Id == videoEntity.VideoAssetId
                              select item;
            if (videoAssets.Any())
            {
                VideoAsset = videoAssets.First();
                videoFile = await VideoAsset.GetVideoAssetFile();
                VideoMediaSource = MediaSource.CreateFromStorageFile(videoFile);
                videoProperties = await videoFile.Properties.GetVideoPropertiesAsync();
            }
            else
            {
                throw new InvalidOperationException("Video Asset with ID " + videoEntity.VideoAssetId + " not found in Asset List.");
            }

            VideoAssetId = videoEntity.VideoAssetId;
            _StartTime = videoEntity.StartTime;
            _VideoStartPosition = videoEntity.VideoStartPosition;
            _VideoEndPosition = videoEntity.VideoEndPosition;
            Locked = videoEntity.Locked;
        }

        public ITimelineItemEntity ToEntity()
        {
            var entity = new TimelineVideoItemEntity();
            entity.VideoAssetId = VideoAssetId;
            entity.StartTime = StartTime;
            entity.VideoEndPosition = VideoEndPosition;
            entity.VideoStartPosition = VideoStartPosition;
            entity.Locked = Locked;
            return entity;
        }
    }
}
