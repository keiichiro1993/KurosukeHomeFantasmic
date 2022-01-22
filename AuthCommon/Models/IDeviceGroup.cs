using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthCommon.Models
{
    public interface IDeviceGroup
    {
        string DeviceGroupName { get; }
        List<IDevice> Devices { get; set; }
    }
}
