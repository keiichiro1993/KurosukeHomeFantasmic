using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.Utils.DBHelpers
{
    public class HueBridgeCacheHelper : DBHelperBase
    {
        string fileName = "huebridge.db";
        HueBridgeCacheEntity dbContent;

        private async Task Init()
        {
            if (assetDBFile == null)
            {
                var assetDBFolder = await AppGlobalVariables.AssetsFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    dbContent = new HueBridgeCacheEntity(new List<HueBridgeCacheItem>());
                    await SaveObjectToJsonFile(dbContent);
                }
                else
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await assetDBFile.OpenReadAsync())
                    {
                        dbContent = await JsonSerializer.DeserializeAsync<HueBridgeCacheEntity>(jsonStream.AsStream(), serializerOptions);
                    }
                }
            }
        }

        public async Task<string> GetHueBridgeCachedIp(string bridgeId)
        {
            await Init();
            var hit = from item in dbContent.HueBridgeCache
                      where item.Id == bridgeId
                      select item;
            if (hit.Any())
            {
                return hit.First().IpAddress;
            }
            else
            {
                return null;
            }
        }

        public async Task SaveHueBridgeCache(string bridgeId, string ipAddress)
        {
            await Init();
            var hit = from item in dbContent.HueBridgeCache
                      where item.Id == bridgeId
                      select item;
            if (hit.Any())
            {
                hit.First().IpAddress = ipAddress;
            }
            else
            {
                dbContent.HueBridgeCache.Add(new HueBridgeCacheItem(bridgeId, ipAddress));
            }
            await SaveObjectToJsonFile(dbContent);
        }
    }
}
