using CommonUtils;
using KurosukeHueClient.Models.HueObjects;
using System.Collections.ObjectModel;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.AssetPages
{
    public class HueActionListPageViewModel : ViewModelBase
    {
        public ObservableCollection<HueAction> Actions { get { return Utils.OnMemoryCache.HueActions; } }
        public HueAction SelectedAction { get; set; }
    }
}
