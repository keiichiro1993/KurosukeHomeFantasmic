using KurosukeHueClient.Models.HueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.AssetPages
{
    public class HueActionListPageViewModel : ViewModelBase
    {
        public ObservableCollection<HueAction> Actions { get { return Utils.OnMemoryCache.HueActions; } }
        public HueAction SelectedAction { get; set; }
    }
}
