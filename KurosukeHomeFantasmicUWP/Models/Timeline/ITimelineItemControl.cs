using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public interface ITimelineItemControl
    {
        ITimelineItem TimelineItem { get; set; }
    }
}
