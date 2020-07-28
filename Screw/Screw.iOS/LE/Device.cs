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
    class Device : IDevice
    {
        readonly CBPeripheral _peripheral;

        public Device(CBPeripheral peripheral)
        {
            _peripheral = peripheral;
        }
    }
}