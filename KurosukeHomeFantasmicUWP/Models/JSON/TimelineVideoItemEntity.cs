using System;

namespace KurosukeHomeFantasmicUWP.Models.JSON
{
    public class TimelineVideoItemEntity : ITimelineItemEntity
    {
        public TimeSpan VideoStartPosition { get; set; }
        public TimeSpan VideoEndPosition { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool Locked { get; set; }
        public string VideoAssetId { get; set; }
    }
}
