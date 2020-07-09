using System;
using System.Collections.Generic;
using System.Text;
using Trisome.Core.MVVM;
using Windows.Devices.Bluetooth;

namespace OrzLE.WPF.Models
{
    class DeviceModel : ObservableObject
    {
        string _name;
        short _rssi;

        public ulong Address { get; }
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public short RSSI
        {
            get { return _rssi; }
            set { SetProperty(ref _rssi, value); }
        }

        public DeviceModel(ulong address, string name, short rssi)
        {
            Address = address;
            Name = name;
            RSSI = rssi;
        }
    }
}
