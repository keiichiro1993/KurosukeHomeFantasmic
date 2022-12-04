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
    internal class RemoteVideoAssetHelper : DBHelperBase
    {
        string fileName = "remotevideoassets.db";
        List<RemoteVideoAsset> dbContent;

        private async Task Init()
        {
            if (assetDBFile == null)
            {
                assetDBFolder = await AppGlobalVariables.AssetsFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    dbContent = new List<RemoteVideoAsset>();
                    await SaveObjectToJsonFile(dbContent);
                }
                else
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await assetDBFile.OpenReadAsync())
                    {
                        dbContent = await JsonSerializer.DeserializeAsync<List<RemoteVideoAsset>>(jsonStream.AsStream(), serializerOptions);
                    }
                }
            }
        }


        internal async Task<List<RemoteVideoAsset>> GetRemoteVideoAssets()
        {
            await Init();
            return dbContent;
        }

        internal async Task SaveRemoteVideoAssets(List<RemoteVideoAsset> remoteVideos)
        {
            await Init();
            dbContent = remoteVideos;
            await SaveObjectToJsonFile(dbContent);
        }
    }
}
