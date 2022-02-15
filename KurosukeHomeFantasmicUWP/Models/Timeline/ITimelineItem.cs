using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public interface ITimelineItem
    {
        TimeSpan TotalCanvasDuration { get; set; }
        double CanvasWidth { get; set; }
        bool Locked { get; set; }

        double Left { get; }
        double Width { get; }

        TimeSpan StartTime { get; set; }
        TimeSpan EndTime { get; }
        TimeSpan Duration { get; set; }

        TimelineVideoItemEntity ToEntity();
    }
}
