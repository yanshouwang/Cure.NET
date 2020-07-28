using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBluetooth;
using Foundation;
using Screw.Core.LE;
using UIKit;

namespace Screw.iOS.LE
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
    }
}