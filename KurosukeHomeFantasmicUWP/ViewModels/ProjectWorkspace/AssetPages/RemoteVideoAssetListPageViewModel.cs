using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using KurosukeHomeFantasmicUWP.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Popups;
using System.Linq;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace
{
    public class RemoteVideoAssetListPageViewModel : ViewModelBase
    {
        public ObservableCollection<RemoteVideoAsset> RemoteVideoAssets
        {
            get { return OnMemoryCache.RemoteVideoAssets; }
        }
    }
}
