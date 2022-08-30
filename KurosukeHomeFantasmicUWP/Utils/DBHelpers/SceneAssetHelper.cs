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
    public class SceneAssetHelper : DBHelperBase
    {
        string fileName = "scenes.db";
        List<ShowScene> sceneAssetsDBContent;

        private async Task Init()
        {
            if (assetDBFile == null)
            {
                assetDBFolder = await AppGlobalVariables.AssetsFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    sceneAssetsDBContent = new List<ShowScene>();
                    await SaveObjectToJsonFile(sceneAssetsDBContent);
                }
                else
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await assetDBFile.OpenReadAsync())
                    {
                        sceneAssetsDBContent = await JsonSerializer.DeserializeAsync<List<ShowScene>>(jsonStream.AsStream(), serializerOptions);
                    }
                }

                foreach (var scene in sceneAssetsDBContent)
                {
                    foreach (var timeline in scene.Timelines)
                    {
                        await timeline.DecodeTimelineItemEntities();
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
            await SaveObjectToJsonFile(scenes);
            sceneAssetsDBContent = scenes;
        }
    }
}
