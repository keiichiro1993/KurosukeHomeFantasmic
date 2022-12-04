using CommonUtils;
using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Utils;
using System.Collections.ObjectModel;

namespace KurosukeHomeFantasmicUWP.ViewModels.ProjectWorkspace.TimelinePages
{
    public class SceneListPageViewModel : ViewModelBase
    {
        public ObservableCollection<ShowScene> Scenes
        {
            get { return OnMemoryCache.Scenes; }
        }


    }
}
