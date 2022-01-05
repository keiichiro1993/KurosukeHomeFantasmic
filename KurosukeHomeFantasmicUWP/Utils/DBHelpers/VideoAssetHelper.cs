using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.Utils.DBHelpers
{
    public class VideoAssetsHelper
    {
        string fileName = "videoasset.db";
        StorageFolder assetDBFolder;
        StorageFile videoAssetDBFile;
        List<VideoAssetEntity> videoAssetDBContent;

        private async Task Init()
        {
            if (assetDBFolder == null)
            {
                assetDBFolder = await AppGlobalVariables.AssetsFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                var item = await assetDBFolder.TryGetItemAsync(fileName);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    //create file if not exist
                    videoAssetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    videoAssetDBContent = new List<VideoAssetEntity>();
                    var json = JsonSerializer.Serialize(videoAssetDBContent);
                    await FileIO.WriteTextAsync(videoAssetDBFile, json);
                }
                else
                {
                    videoAssetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await videoAssetDBFile.OpenReadAsync())
                    {
                        videoAssetDBContent = await JsonSerializer.DeserializeAsync<List<VideoAssetEntity>>(jsonStream.AsStream());
                    }
                }
            }
        }

        public async Task<List<VideoAssetEntity>> GetVideoAssets()
        {
            await Init();
            return videoAssetDBContent;
        }

        public async Task<int> AddVideoAsset(VideoAssetEntity entity)
        {
            videoAssetDBContent.Add(entity);
            var json = JsonSerializer.Serialize(videoAssetDBContent);
            await FileIO.WriteTextAsync(videoAssetDBFile, json);
            return videoAssetDBContent.Count;
        }

        public async Task<int> RemoveVideoAsset(VideoAssetEntity entity)
        {
            var removeitem = from item in videoAssetDBContent
                             where item.Id == entity.Id
                             select item;
            videoAssetDBContent.Remove(removeitem.First());
            var json = JsonSerializer.Serialize(videoAssetDBContent);
            await FileIO.WriteTextAsync(videoAssetDBFile, json);
            return videoAssetDBContent.Count;
        }
    }
}
