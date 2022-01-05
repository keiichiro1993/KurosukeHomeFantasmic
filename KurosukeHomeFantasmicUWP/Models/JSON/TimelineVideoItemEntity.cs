using System;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public class TimelineVideoItemEntity : ITimelineItemEntity
    {
        public string VideoAssetId { get; set; }
        public TimeSpan VideoStartPosition { get; set; }
        public TimeSpan VideoEndPosition { get; set; }
    }
}
