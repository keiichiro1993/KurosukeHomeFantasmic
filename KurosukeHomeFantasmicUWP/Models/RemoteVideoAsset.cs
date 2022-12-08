using KurosukeBonjourService;
using KurosukeBonjourService.Models.Json;
using System.Text.Json.Serialization;

namespace KurosukeHomeFantasmicUWP.Models
{
    public class RemoteVideoAsset
    {
        public VideoInfo Info { get; set; }
        public string DomainName { get; set; }
        public string HostName { get; set; }
        public string Id { get; set; }
    }
}
