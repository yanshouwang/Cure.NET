using System;
using System.Linq;
using CoreBluetooth;
using AMock.Core.LE;
using System.Collections.Generic;

namespace AMock.iOS.LE
{
    class Adapter : IAdapter
    {
        public event EventHandler<DeviceEventArgs> DeviceScanned;

        readonly CBCentralManager _manager;
        readonly IList<Device> _devices;

        public AdapterState State
            => _manager.State.ToCore();

        public Adapter(CBCentralManager manager)
        {
            _manager = manager;
            _devices = new List<Device>();

            _manager.UpdatedState += OnUpdatedState;
            _manager.DiscoveredPeripheral += OnDiscoveredPeripheral;
            _manager.ConnectedPeripheral += OnConnectedPeripheral;
            _manager.FailedToConnectPeripheral += OnFailedToConnectPeripheral;
            _manager.RetrievedConnectedPeripherals += OnRetrievedConnectedPeripherals;
            _manager.DisconnectedPeripheral += OnDisconnectedPeripheral;
        }

        void OnDisconnectedPeripheral(object sender, CBPeripheralErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnRetrievedConnectedPeripherals(object sender, CBPeripheralsEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnFailedToConnectPeripheral(object sender, CBPeripheralErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnConnectedPeripheral(object sender, CBPeripheralEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnUpdatedState(object sender, EventArgs e)
        {

        }

        void OnDiscoveredPeripheral(object sender, CBDiscoveredPeripheralEventArgs e)
        {
            var device = _devices.FirstOrDefault(i => i.UUID == e.Peripheral.UUID);
            var device = e.Peripheral.ToCore();
            var rssi = e.RSSI.Int16Value;
            var advertisements = Util.GetAdvertisements(e.AdvertisementData);
            var args = new DeviceEventArgs(device, rssi, advertisements);
            DeviceScanned?.Invoke(this, args);
        }

        public void StartScan(params Guid[] serviceUUIDs)
        {
            var uuids = serviceUUIDs.Select(i => i.ToiOS()).ToArray();
            _manager.ScanForPeripherals(uuids);
        }

        public void StopScan()
        {
            _manager.StopScan();
        }
    }
}