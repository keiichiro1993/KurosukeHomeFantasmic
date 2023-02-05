using KurosukeBonjourService;
using KurosukeBonjourService.Models.Json;
using KurosukeHomeFantasmicRemoteVideoPlayer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils
{
    internal static class AppGlobalVariables
    {
        public static BonjourServer BonjourServer { get; set; }
        public static List<VideoInfo> VideoList { get; set; }
        public static ObservableCollection<LEDPanelUnitSet> LEDPanelUnitSets { get; set; } 
    }
}
