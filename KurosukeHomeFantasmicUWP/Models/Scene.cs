using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models
{
    /// <summary>
    /// A Scene can contains multiple Stages.
    /// This is the top level object that the users create to place the things.
    /// </summary>
    public class Scene
    {
        public ObservableCollection<VideoTimeline> Stages { get; set; }
    }
}
