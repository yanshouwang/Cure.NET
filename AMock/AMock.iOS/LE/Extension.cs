using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using AMock.Core.LE;
using UIKit;

namespace AMock.iOS.LE
{
    static class Extension
    {
        public static Adapter ToCore(this CBCentralManager manager)
        {
            return new Adapter(manager);
        }

        public static CBUUID ToiOS(this Guid uuid)
        {
            var str = uuid.ToString();
            return CBUUID.FromString(str);
        }

        public static Device ToCore(this CBPeripheral peripheral)
        {
            return new Device(peripheral);
        }

        public static AdapterState ToCore(this CBCentralManagerState state)
        {
            switch (state)
            {
                case CBCentralManagerState.PoweredOff:
                    return AdapterState.Off;
                case CBCentralManagerState.PoweredOn:
                    return AdapterState.On;
                case CBCentralManagerState.Unknown:
                case CBCentralManagerState.Resetting:
                case CBCentralManagerState.Unsupported:
                case CBCentralManagerState.Unauthorized:
                default:
                    return AdapterState.None;
            }
        }
    }
}