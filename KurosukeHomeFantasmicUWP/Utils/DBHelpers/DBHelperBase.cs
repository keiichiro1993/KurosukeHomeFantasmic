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
    public abstract class DBHelperBase
    {
        private protected JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new TimeSpanJsonConverter()
            }
        };

        private protected StorageFile assetDBFile;
        private protected StorageFolder assetDBFolder;
        private protected async Task SaveObjectToJsonFile<T>(T target, int retry = 3, bool isRetry = false)
        {
            if (assetDBFile == null)
            {
                throw new InvalidOperationException("DB Asset File not correctly set. The DB helper should be initialized before the save operation.");
            }

            // backup
            var backupName = assetDBFile.Name + ".backup";
            if (!isRetry)
            {
                await assetDBFile.CopyAsync(assetDBFolder, backupName, NameCollisionOption.ReplaceExisting);
            }

            // save
            await assetDBFile.DeleteAsync();
            assetDBFile = await assetDBFolder.CreateFileAsync(assetDBFile.Name, CreationCollisionOption.OpenIfExists);
            using (var stream = await assetDBFile.OpenStreamForWriteAsync())
            {
                await JsonSerializer.SerializeAsync(stream, target, serializerOptions);
            }

            // validate if the save operation succeeded
            try
            {
                using (var jsonStream = await assetDBFile.OpenReadAsync())
                {
                    await JsonSerializer.DeserializeAsync<T>(jsonStream.AsStream(), serializerOptions);
                }
            }
            catch (Exception ex)
            {
                retry--;
                if (retry > 0)
                {
                    await SaveObjectToJsonFile(target, retry, true);
                }
                else
                {
                    throw new Exception($"Validation of saved file failed. The previous content is available as {backupName}", ex);
                }
            }
        }
    }
}
