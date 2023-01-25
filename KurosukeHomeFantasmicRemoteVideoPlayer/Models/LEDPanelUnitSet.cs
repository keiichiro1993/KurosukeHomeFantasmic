using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Models
{
    internal class LEDPanelUnitSet
    {
        // UnitPixelWidth/Height should be global
        public int UnitPixelWidth { get; set; }
        public int UnitPixelHeight { get; set; }

        public int HorizontalUnitCount { get; set; }
        public int VerticalUnitCount { get; set; }

        /// <summary>
        /// Serial Device ID of ESP32 board
        /// </summary>
        public string SerialDeviceId { get; set; }
        /// <summary>
        /// Matrix of the Unit exists. Serial Client will skip the data applied to the disabled panels.
        /// Coordinate should be in (x,y) format. (EnabledUnitMatrix[x][y])
        /// </summary>
        public List<List<bool>> EnabledUnitMatrix { get; set; }
        /// <summary>
        /// Coordinate of this Unit Set. Starts from Upper Right with (0, 0).
        /// (Because the client reads/sends data in that order and I don't wanna change it.)
        /// </summary>
        public LEDPanelUnitSetCoordinate Coordinate { get; set; }
    }
}
