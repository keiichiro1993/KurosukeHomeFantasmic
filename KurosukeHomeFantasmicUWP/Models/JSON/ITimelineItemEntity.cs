using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static KurosukeHomeFantasmicUWP.Models.Timeline.Timeline;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public interface ITimelineItemEntity
    {
        TimeSpan StartTime { get; set; }
        bool Locked { get; set; }
    }
}
