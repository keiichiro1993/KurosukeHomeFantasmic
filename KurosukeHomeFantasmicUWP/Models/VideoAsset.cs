using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHomeFantasmicUWP.Utils.DBHelpers;
using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace KurosukeHomeFantasmicUWP.Models
{
    public class VideoAsset : ViewModels.ViewModelBase
    {
        private VideoAssetEntity _VideoAssetEntity;
        public VideoAssetEntity VideoAssetEntity
        {
            get { return _VideoAssetEntity; }
            set
            {
                _VideoAssetEntity = value;
                RaisePropertyChanged();
            }
        }

        private BitmapImage _ThumbnailUri = new BitmapImage(new Uri("ms-appx:///Assets/StoreLogo.png"));
        public BitmapImage ThumbnailUri
        {
            get { return _ThumbnailUri; }
            set
            {
                _ThumbnailUri = value;
                RaisePropertyChanged();
            }
        }

        public VideoAsset(VideoAssetEntity entity)
        {
            Init(entity);
        }

        public async Task<StorageFile> GetVideoAssetFile()
        {
            var videoFolder = await Utils.AppGlobalVariables.AssetsFolder.CreateFolderAsync("Videos", Windows.Storage.CreationCollisionOption.OpenIfExists);
            if (!await videoFolder.FileExists(VideoAssetEntity.FilePath))
            {
                throw new InvalidOperationException("Video file " + VideoAssetEntity.FilePath + " doesn't exist.");
            }
            return await videoFolder.CreateFileAsync(VideoAssetEntity.FilePath, CreationCollisionOption.OpenIfExists);
        }

        public void Init(VideoAssetEntity entity)
        {
            VideoAssetEntity = entity;
            InitThumbnail(); //Run in background
        }

        private async void InitThumbnail()
        {
            IsLoading = true;

            try
            {
                var thumbnailFolder = await Utils.AppGlobalVariables.AssetsFolder.CreateFolderAsync(".thumbnails", Windows.Storage.CreationCollisionOption.OpenIfExists);
                var thumbnailFileName = VideoAssetEntity.Name + VideoAssetEntity.Id.Substring(0, 8) + ".thumb";
                var exists = await thumbnailFolder.FileExists(thumbnailFileName);

                var thumbnailFile = await thumbnailFolder.CreateFileAsync(thumbnailFileName, Windows.Storage.CreationCollisionOption.OpenIfExists);

                if (!exists)
                {
                    var thumbnail = await (await GetVideoAssetFile()).GetScaledImageAsThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.VideosView, (uint)256);
                    await thumbnail.SaveToStorageFile(thumbnailFile);
                }

                var bitmap = new BitmapImage();
                using (var stream = await thumbnailFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    bitmap.SetSource(stream);
                }
                ThumbnailUri = bitmap;
            }
            catch (Exception)
            {
                //TODO: decide what to do
            }

            IsLoading = false;
        }
    }
}
