using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models
{
    /// <summary>
    /// A Scene can contains multiple Timelines.
    /// This is the top level object that the users create to place the things.
    /// </summary>
    public class ShowScene
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<ITimeline> Timelines { get; set; }
    }
}
