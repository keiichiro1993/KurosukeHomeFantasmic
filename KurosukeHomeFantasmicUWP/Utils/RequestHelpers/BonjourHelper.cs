using KurosukeBonjourService;
using KurosukeBonjourService.Models;
using KurosukeBonjourService.Models.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Utils.RequestHelpers
{
    internal class BonjourHelper
    {
        public static async Task UpdateBonjourDevices()
        {
            if (OnMemoryCache.BonjourDevices == null)
            {
                OnMemoryCache.BonjourDevices = new ObservableCollection<QueryResponseItem>();
            }

            var devices = await BonjourClient.QueryServiceAsync();
            if (devices.Any())
            {
                OnMemoryCache.BonjourDevices.Clear();
                foreach (var device in devices)
                {
                    OnMemoryCache.BonjourDevices.Add(device);
                }
            }
        }

        public static async Task<List<VideoInfo>> GetAvailableVideo(QueryResponseItem device)
        {
            var bonjourClient = new BonjourClient(device);
            var result = await bonjourClient.Get("/videolist");
            return JsonSerializer.Deserialize<List<VideoInfo>>(result);
        }
    }
}
