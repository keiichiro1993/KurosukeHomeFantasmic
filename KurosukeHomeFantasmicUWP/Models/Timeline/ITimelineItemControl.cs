using KurosukeHomeFantasmicUWP.Utils.UIHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KurosukeHomeFantasmicUWP.Controls.Timeline.Items.TimelineHueItemControl;

namespace KurosukeHomeFantasmicUWP.Models.Timeline
{
    public interface ITimelineItemControl
    {
        ITimelineItem TimelineItem { get; set; }
        event DeleteButtonClickedEventHandler<ITimelineItem> DeleteButtonClicked;
    }
}
