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
    class Device : IDevice
    {
        readonly CBCentralManager _manager;
        internal CBPeripheral Peripheral { get; }

        public Device(CBCentralManager manager, CBPeripheral peripheral)
        {
            _manager = manager;
            Peripheral = peripheral;
        }

        public DeviceState State => throw new NotImplementedException();

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void DiscoverServices()
        {
            throw new NotImplementedException();
        }
    }
}