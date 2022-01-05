using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KurosukeHomeFantasmicUWP.Utils.DBHelpers
{
    public static class StorageFolderExtention
    {
        public static async Task<bool> FileExists(this StorageFolder folder, string fileName)
        {
            var item = await folder.TryGetItemAsync(fileName);
            return item != null;
        }
    }
}
