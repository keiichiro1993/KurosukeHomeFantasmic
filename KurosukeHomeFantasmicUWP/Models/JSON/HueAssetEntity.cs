using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public class HueAssetEntity
    {
        public HueAssetEntity() { }
        public HueAssetEntity(List<HueAction> hueActions, List<HueEffect> hueEffects)
        {
            HueActions = hueActions;
            HueEffects = hueEffects;
        }

        public List<HueAction> HueActions { get; set; }
        public List<HueEffect> HueEffects { get; set; }
    }
}
