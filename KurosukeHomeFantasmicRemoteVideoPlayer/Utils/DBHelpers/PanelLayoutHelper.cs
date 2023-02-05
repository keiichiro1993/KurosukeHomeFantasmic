using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils.DBHelpers
{
    public class PanelLayoutHelper : DBHelperBase
    {
        string fileName = "ledpanelunitsets.db";
        List<LEDPanelUnitSet> dbContent;

        private async Task Init()
        {
            if (assetDBFile == null)
            {
                assetDBFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(".assetdb", CreationCollisionOption.OpenIfExists);
                if (!await assetDBFolder.FileExists(fileName))
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    dbContent = new List<LEDPanelUnitSet>();
                    await SaveObjectToJsonFile(dbContent);
                }
                else
                {
                    assetDBFile = await assetDBFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    using (var jsonStream = await assetDBFile.OpenReadAsync())
                    {
                        dbContent = await JsonSerializer.DeserializeAsync<List<LEDPanelUnitSet>>(jsonStream.AsStream(), serializerOptions);
                    }
                }
            }
        }


        internal async Task<List<LEDPanelUnitSet>> GetLEDPanelUnitSets()
        {
            await Init();
            return dbContent;
        }

        internal async Task SaveLEDPanelUnitSets(List<LEDPanelUnitSet> unitSets)
        {
            await Init();
            dbContent = unitSets;
            await SaveObjectToJsonFile(dbContent);
        }
    }
}
