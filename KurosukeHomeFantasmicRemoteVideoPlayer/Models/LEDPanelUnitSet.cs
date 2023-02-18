using CommonUtils;
using KurosukeHomeFantasmicRemoteVideoPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Media.Protection.PlayReady;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Models
{
    public class LEDPanelUnitSet
    {
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

        [JsonIgnore]
        public SerialClient SerialClient { get; set; }

        [JsonIgnore]
        public int UnitPixelWidth { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelWidth); } }
        [JsonIgnore]
        public int UnitPixelHeight { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitPixelHeight); } }
        [JsonIgnore]
        public int HorizontalUnitCount { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitHorizontalPanelCount); } }
        [JsonIgnore]
        public int VerticalUnitCount { get { return SettingsHelper.ReadSettings<int>(SettingNameMappings.UnitVerticalPanelCount); } }


        public async Task InitSerialClient()
        {
            if (SerialDeviceInformation == null)
            {
                var devices = await SerialClient.ListSerialDevices();
                var match = (from device in devices
                             where device.Id == SerialDeviceId
                             select device).FirstOrDefault();
                if (match != null)
                {
                    SerialDeviceInformation = match;
                }
                else
                {
                    // give up if the device is not available
                    return;
                }
            }

            if (SerialClient != null)
            {
                SerialClient.Dispose();
                SerialClient = null;
            }

            try
            {
                SerialClient = await SerialClient.CreateFromId(SerialDeviceInformation.Id);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteErrorLog(ex, $"Error occurred while initializing Serial Client for {SerialDeviceInformation.Name}({SerialDeviceInformation.Id})");
                // give up if Serial Client cannot be initialized
                return;
            }
        }
    }
}
