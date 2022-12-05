using KurosukeBonjourService.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using KurosukeBonjourService;

namespace KurosukeHomeFantasmicUWP.Models
{
    public class RemoteVideoAsset
    {
        public VideoInfo Info { get; set; }
        public string DomainName { get; set; }
        public string HostName { get; set; }
        public string Id { get; set; }

        [JsonIgnore]
        public BonjourClient Client { get; set; }
    }
}
