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

        public List<ITimelineItemEntity> TimelineItemEntities { get; set; }

        public enum TimelineTypes { Video, Hue }
        public TimelineTypes TimelineType { get; set; }

        public string TargetDisplayId { get; set; }


        [JsonIgnore]
        public ObservableCollection<ITimelineItem> TimelineItems { get; set; } = new ObservableCollection<ITimelineItem>();

        public async Task DecodeTimelineItemEntities()
        {
            if (TimelineItemEntities != null)
            {
                foreach (var entity in TimelineItemEntities)
                {
                    if (TimelineType == TimelineTypes.Video)
                    {
                        var item = new TimelineVideoItem();
                        await item.Init(entity);
                        TimelineItems.Add(item);
                    }
                    else
                    {
                        throw new NotImplementedException("The entity type except TimelineVideoItemEntity is not implemented yet.");
                    }
                }
            }
        }

        public void EncodeTimelineItemToEntity()
        {
            TimelineItemEntities = new List<ITimelineItemEntity>();
            foreach (var entity in TimelineItems) { TimelineItemEntities.Add(entity.ToEntity()); }
        }
    }
}
