using System;
using System.Collections.Generic;
using System.Text;

namespace AMock.Core.LE
{
    public interface IAdapter
    {
        event EventHandler<DeviceEventArgs> DeviceScanned;

        AdapterState State { get; }

        void StartScan(params Guid[] serviceUUIDs);
        void StopScan();
    }
}
