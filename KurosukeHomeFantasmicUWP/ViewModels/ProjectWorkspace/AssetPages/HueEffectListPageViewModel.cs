using CommonUtils;
using KurosukeHueClient.Models.HueObjects;
using System.Collections.ObjectModel;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.AssetPages
{
    public class HueEffectListPageViewModel : ViewModelBase
    {
        public ObservableCollection<HueEffect> Effects { get { return Utils.OnMemoryCache.HueEffects; } }
        public HueEffect SelectedEffect { get; set; }
    }
}
