using KurosukeBonjourService.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models
{
    internal class RemoteVideoAsset
    {
        public VideoInfo Info { get; set; }
        public string Host { get; set; }
        public string Id { get; set; }
    }
}
