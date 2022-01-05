using KurosukeHomeFantasmicUWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.Utils.DBHelpers
{
    public class SceneAssetHelper
    {
        string fileName = "scenes.db";
        StorageFile sceneAssetDBFile;
        List<ShowScene> sceneAssetsDBContent;

        private async Task Init()
        {
            if (sceneAssetDBFile == null)
            {
                var assetDBFolder = await AppGlobalVariables.AssetsFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    sceneAssetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    sceneAssetsDBContent = new List<ShowScene>();

                    await WriteJsonAsync(sceneAssetsDBContent);
                }
                else
                {
                    sceneAssetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await sceneAssetDBFile.OpenReadAsync())
                    {
                        sceneAssetsDBContent = await JsonSerializer.DeserializeAsync<List<ShowScene>>(jsonStream.AsStream());
                    }
                }
            }
        }

        public async Task<List<ShowScene>> GetSceneAssets()
        {
            await Init();
            return sceneAssetsDBContent;
        }

        public async Task SaveSceneAssets(List<ShowScene> scenes)
        {
            await Init();
            await WriteJsonAsync(scenes);
            sceneAssetsDBContent = scenes;
        }

        private async Task WriteJsonAsync(List<ShowScene> scenes)
        {
            using (var stream = await sceneAssetDBFile.OpenStreamForWriteAsync())
            {
                await JsonSerializer.SerializeAsync(stream, scenes);
            }
        }
    }
}
