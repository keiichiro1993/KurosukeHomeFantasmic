using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Models
{
    internal class LEDPanelUnitSet
    {
        // should be global
        public int UnitPixelWidth { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelWidth); } }
        public int UnitPixelHeight { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelHeight); } }
        public int HorizontalUnitCount { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitHorizontalPanelCount); } }
        public int VerticalUnitCount { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitVerticalPanelCount); } }

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

        [JsonIgnore]
        public DeviceInformation SerialDeviceInformation { get; set; }
    }
}
