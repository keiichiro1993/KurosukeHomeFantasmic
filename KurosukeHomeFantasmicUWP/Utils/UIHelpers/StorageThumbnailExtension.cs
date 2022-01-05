using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace KurosukeHomeFantasmicUWP.Utils.UIHelpers
{
    public static class StorageThumbnailExtention
    {
        public static async Task SaveToStorageFile(this StorageItemThumbnail thumbnail, StorageFile storageFile)
        {
            Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer(Convert.ToUInt32(thumbnail.Size));
            IBuffer iBuf = await thumbnail.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);
            using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                await stream.WriteAsync(iBuf);
            }
        }
    }
}
