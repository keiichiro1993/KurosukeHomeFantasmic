using KurosukeBonjourService.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Search;
using Windows.Storage;
using Windows.Media.Core;
using Windows.Storage.FileProperties;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils
{
    internal class VideoFileHelper
    {
        public static async Task<List<VideoInfo>> GetVideoList()
        {
            QueryOptions queryOption = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { ".wmv", ".flv", ".mp4" });
            queryOption.FolderDepth = FolderDepth.Deep;
            var files = await KnownFolders.VideosLibrary.CreateFileQueryWithOptions(queryOption).GetFilesAsync();

            var list = new List<VideoInfo>();
            foreach (var file in files)
            {
                var videoProperties = await file.Properties.GetVideoPropertiesAsync();
                list.Add(new VideoInfo(file.DisplayName, videoProperties.Duration, file.Path));
            }

            return list;
        }
    }
}
