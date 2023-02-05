using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Models
{
    public class LEDPanelUnitSetCoordinate
    {
        public LEDPanelUnitSetCoordinate() { }
        public LEDPanelUnitSetCoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
