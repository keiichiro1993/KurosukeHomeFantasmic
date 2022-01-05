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
    }
}
