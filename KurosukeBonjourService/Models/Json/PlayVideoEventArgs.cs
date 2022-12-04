using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KurosukeBonjourService.Models.WebSocketServices.PlayVideoService;

namespace KurosukeBonjourService.Models.Json
{
    public class PlayVideoEventArgs : EventArgs
    {
        public TimeSpan VideoTime { get; set; }
        public PlayVideoServiceStatus VideoStatus { get; set; }
        public string VideoName { get; set; }
    }
}
