using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public interface ITimelineItem
    {
        TimeSpan TotalCanvasDuration { get; set; }
        double CanvasWidth { get; set; }
        bool Locked { get; set; }
        Visibility IsResizable { get; }

        double Left { get; }
        double Width { get; }

        TimeSpan StartTime { get; set; }
        TimeSpan EndTime { get; }
        TimeSpan Duration { get; set; }

        string ItemId { get; }

        ITimelineItemEntity ToEntity();
        Task Init(ITimelineItemEntity entity);
    }
}
