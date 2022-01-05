using KurosukeHomeFantasmicUWP.Models;
using KurosukeHomeFantasmicUWP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
