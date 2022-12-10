using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KurosukeBonjourService.Models.WebSocketServices.PlayVideoService;

namespace KurosukeBonjourService.Models.BonjourEventArgs
{
    public class PlayVideoEventArgs : EventArgs
    {
        public TimeSpan VideoTime { get; set; }
        public PlayVideoServiceStatus VideoStatus { get; set; }
        public string VideoPath { get; set; }
        /// <summary>
        /// The timestamp of the data send.
        /// For determining the invalid ordered data receive.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
