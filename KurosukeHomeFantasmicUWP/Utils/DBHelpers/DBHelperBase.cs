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
    public class DBHelperBase
    {
        private protected JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new TimeSpanJsonConverter()
            }
        };

        private protected StorageFile assetDBFile;
        private protected async Task SaveObjectToJsonFile<T>(T target, int retry=3)
        {
            if (assetDBFile == null)
            {
                throw new InvalidOperationException("DB Asset File not correctly set. The DB helper should be initialized before the save operation.");
            }

            // save
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
                    await SaveObjectToJsonFile(target, retry);
                }
                else
                {
                    throw new Exception("Validation of saved file failed.", ex);
                }
            }
        }
    }
}
