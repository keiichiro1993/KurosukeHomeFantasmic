﻿using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace
{
    public class VideoAssetListPageViewModel : ViewModelBase
    {
        public ObservableCollection<VideoAsset> VideoAssets
        {
            get { return Utils.OnMemoryCache.VideoAssetCache; }
        }
        private VideoAsset _SelectedVideo;
        public VideoAsset SelectedVideo
        {
            get { return _SelectedVideo; }
            set
            {
                _SelectedVideo = value;
            }
        }

        public async Task AddVideo()
        {
            IsLoading = true;

            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
                };
                picker.FileTypeFilter.Add(".mp4");

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var videoAssetEntity = new VideoAssetEntity();
                    videoAssetEntity.Id = Guid.NewGuid().ToString();
                    videoAssetEntity.Name = System.IO.Path.GetFileNameWithoutExtension(file.Path);
                    videoAssetEntity.FilePath = file.Name;

                    var videoFolder = await Utils.AppGlobalVariables.AssetsFolder.CreateFolderAsync("Videos", Windows.Storage.CreationCollisionOption.OpenIfExists);
                    if (File.Exists(Path.Combine(videoFolder.Path, videoAssetEntity.FilePath)))
                    {
                        throw new InvalidOperationException("The video file with the same name" + videoAssetEntity.FilePath + " already exists.");
                    }

                    LoadingMessage = "Copying the video...";
                    await file.CopyAsync(videoFolder);
                    LoadingMessage = "Please wait...";

                    await Utils.AppGlobalVariables.VideoAssetDB.AddVideoAsset(videoAssetEntity);
                    VideoAssets.Add(new VideoAsset(videoAssetEntity));
                }
            }
            catch (Exception ex)
            {
                await DebugHelper.ShowErrorDialog(ex, "ビデオファイルのインポートに失敗しました。");
            }

            IsLoading = false;
        }
    }
}
