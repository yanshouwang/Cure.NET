using System;
using System.Collections.Generic;

namespace AMock.Core.LE
{
    public class DeviceEventArgs : EventArgs
    {
        public IDevice Device { get; }
        public short RSSI { get; }
        public IList<Advertisement> Advertisements { get; }

        public DeviceEventArgs(IDevice device, short rssi, IList<Advertisement> advertisements)
        {
            Device = device;
            RSSI = rssi;
            Advertisements = advertisements;
        }
    }
}