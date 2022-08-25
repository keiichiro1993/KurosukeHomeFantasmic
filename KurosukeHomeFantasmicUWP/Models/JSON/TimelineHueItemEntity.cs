using System;
using static KurosukeHomeFantasmicUWP.Models.Timeline.TimelineHueItem;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public class TimelineHueItemEntity : ITimelineItemEntity
    {
        public TimeSpan StartTime { get; set; }
        public bool Locked { get; set; }
        public bool Loop { get; set; }

        public TimelineHueItemTypes HueItemType { get; set; }
        public string HueItemId { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
