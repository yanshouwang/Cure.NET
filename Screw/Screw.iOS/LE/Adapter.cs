using System;
using System.Linq;
using CoreBluetooth;
using Screw.Core.LE;

namespace Screw.iOS.LE
{
    class Adapter : IAdapter
    {
        public event EventHandler<DeviceEventArgs> DeviceScanned;

        readonly CBCentralManager _manager;

        public Adapter(CBCentralManager manager)
        {
            _manager = manager;
            _manager.DiscoveredPeripheral += OnDiscoveredPeripheral;
        }

        void OnDiscoveredPeripheral(object sender, CBDiscoveredPeripheralEventArgs e)
        {
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