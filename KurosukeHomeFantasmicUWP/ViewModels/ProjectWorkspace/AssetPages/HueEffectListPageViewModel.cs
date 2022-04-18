using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.AssetPages
{
    public class HueEffectListPageViewModel : ViewModelBase
    {
        public ObservableCollection<HueEffect> Effects { get { return Utils.OnMemoryCache.HueEffects; } }
        public HueEffect SelectedEffect { get; set; }
    }
}
