using KurosukeHomeFantasmicUWP.Models.JSON;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public class Timeline
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // interface is not supported for Json Deserialization...
        public List<TimelineVideoItemEntity> TimelineVideoItemEntities { get; set; }
        public List<TimelineHueItemEntity> TimelineHueItemEntities { get; set; }

        public enum TimelineTypes { Video, Hue, RemoteVideo }
        public TimelineTypes TimelineType { get; set; }

        /// <summary>
        /// for local video playback
        /// </summary>
        public string TargetDisplayId { get; set; }
        /// <summary>
        /// for remote video playback
        /// </summary>
        public string TargetDomainName { get; set; }


        [JsonIgnore]
        public ObservableCollection<ITimelineItem> TimelineItems { get; set; } = new ObservableCollection<ITimelineItem>();

        public async Task DecodeTimelineItemEntities()
        {
            switch (TimelineType)
            {
                case TimelineTypes.Video:
                    if (TimelineVideoItemEntities != null)
                    {
                        foreach (var item in TimelineVideoItemEntities)
                        {
                            var timelineItem = new TimelineVideoItem();
                            await timelineItem.Init(item);
                            TimelineItems.Add(timelineItem);
                        }
                    }
                    break;
                case TimelineTypes.Hue:
                    if (TimelineHueItemEntities != null)
                    {
                        foreach (var item in TimelineHueItemEntities)
                        {
                            var timelineItem = new TimelineHueItem();
                            await timelineItem.Init(item);
                            TimelineItems.Add(timelineItem);
                        }
                    }
                    break;
                case TimelineTypes.RemoteVideo:
                    if (TimelineVideoItemEntities != null)
                    {
                        foreach (var item in TimelineVideoItemEntities)
                        {
                            var timelineItem = new TimelineRemoteVideoItem();
                            await timelineItem.Init(item);
                            TimelineItems.Add(timelineItem);
                        }
                    }
                    break;
            }
        }

        public void EncodeTimelineItemToEntity()
        {
            switch (TimelineType)
            {
                case TimelineTypes.RemoteVideo:
                case TimelineTypes.Video:
                    TimelineVideoItemEntities = new List<TimelineVideoItemEntity>();
                    foreach (var item in TimelineItems)
                    {
                        TimelineVideoItemEntities.Add((TimelineVideoItemEntity)item.ToEntity());
                    }
                    break;
                case TimelineTypes.Hue:
                    TimelineHueItemEntities = new List<TimelineHueItemEntity>();
                    foreach (var item in TimelineItems)
                    {
                        TimelineHueItemEntities.Add((TimelineHueItemEntity)item.ToEntity());
                    }
                    break;
            }
        }
    }
}
