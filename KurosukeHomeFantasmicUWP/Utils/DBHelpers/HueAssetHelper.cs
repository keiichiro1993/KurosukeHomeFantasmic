using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHueClient.Models.HueObjects;
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
    public class HueAssetHelper : DBHelperBase
    {
        string fileName = "hueassets.db";
        HueAssetEntity dbContent;

        private async Task Init()
        {
            if (assetDBFile == null)
            {
                assetDBFolder = await AppGlobalVariables.AssetsFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    dbContent = new HueAssetEntity(new List<HueAction>(), new List<HueEffect>());
                    await SaveObjectToJsonFile(dbContent);
                }
                else
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await assetDBFile.OpenReadAsync())
                    {
                        dbContent = await JsonSerializer.DeserializeAsync<HueAssetEntity>(jsonStream.AsStream(), serializerOptions);
                    }
                }
            }
        }


        public async Task<HueAssetEntity> GetHueAssets()
        {
            await Init();
            return dbContent;
        }

        public async Task SaveHueAssets(List<HueAction> actions, List<HueEffect> effects)
        {
            await Init();
            dbContent.HueActions = (from action in actions
                                   select new HueActionEntity(action)).ToList();
            dbContent.HueEffects = (from effect in effects
                                    select new HueEffectEntity(effect)).ToList();
            await SaveObjectToJsonFile(dbContent);
        }
    }
}
