using CommonUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace KurosukeHomeFantasmicRemoteVideoPlayer.Utils
{
    public class SerialClient : IDisposable
    {
        private SerialDevice device;
        DataWriter dw;
        DataReader dr;

        public static async Task<List<DeviceInformation>> ListSerialDevices()
        {
            DeviceInformationCollection serialDeviceInfos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
            return serialDeviceInfos.ToList();
        }

        public static async Task<SerialClient> CreateFromId(string serialDeviceId, uint speed = 500000)
        {
            var client = new SerialClient();
            client.device = await SerialDevice.FromIdAsync(serialDeviceId);

            if (client.device == null)
            {
                throw new Exception("device null!!!");
            }

            client.device.BaudRate = speed;
            client.device.WriteTimeout = new TimeSpan(0, 0, 1);
            client.device.DataBits = 8;
            client.device.Parity = SerialParity.None;
            client.device.StopBits = SerialStopBitCount.One;
            client.device.Handshake = SerialHandshake.XOnXOff;

            client.dw = new DataWriter(client.device.OutputStream);
            client.dr = new DataReader(client.device.InputStream);
            return client;
        }

        public async Task WriteByteAsync(byte[] bytes)
        {
            var bytelist = new List<byte>();
            for (var i = 0; i < bytes.Length / 4; i++)
            {
                bytelist.Add(bytes[4 * i]);
                bytelist.Add(bytes[4 * i + 1]);
                bytelist.Add(bytes[4 * i + 2]);
                // drop A from RGBA
            }
            for (var i = 0; i < bytelist.Count; i++)
            {
                if (bytelist[i] < 0x10) { bytelist[i] = 0x00; }
                if (bytelist[i] == 0x30) { bytelist[i] = 0x31; }
            }


            // ESP32
            dw.WriteBytes(bytelist.ToArray());
            await dw.StoreAsync();
            dw.WriteByte(0x30);
            await dw.StoreAsync();
            //await Task.Delay(10);
            //Debug.WriteLine($"Sent bytes: {bytelist.Count}");
        }

        public void Dispose()
        {
            dw.Dispose();
            dr.Dispose();
            device.Dispose();
        }
    }
}
