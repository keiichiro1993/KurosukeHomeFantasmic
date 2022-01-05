using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public class ITimelineItemEntity
    {
        public TimeSpan StartTime { get; set; }
        public bool Locked { get; set; }
    }
}
