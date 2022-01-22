using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public class TimelineHueItem : ITimelineItem
    {
        public TimeSpan TotalCanvasDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double CanvasWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TimelineVideoItemEntity ToEntity()
        {
            throw new NotImplementedException();
        }
    }
}
