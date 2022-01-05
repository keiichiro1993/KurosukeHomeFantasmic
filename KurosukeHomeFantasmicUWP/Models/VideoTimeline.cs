using KurosukeHomeFantasmicUWP.Models.JSON;
using KurosukeHomeFantasmicUWP.Models.Timeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicUWP.Models
{
    /// <summary>
    /// VideoTimeline is a single timeline combined with one window which can have a target display. (The window will be always placed to a specified display device.)
    /// One VideoTimeline can contain multiple videos but the videos can only be placed serially.
    /// </summary>
    public class VideoTimeline : ITimeline
    {
        public string TargetDisplayId { get; set; }
    }
}
