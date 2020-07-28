using System;
using System.Collections.Generic;
using System.Text;

namespace Screw.Core.LE
{
    public interface IAdapter
    {
        event EventHandler<DeviceEventArgs> DeviceScanned;

        void StartScan(params Guid[] serviceUUIDs);
        void StopScan();
    }
}
