using System;
using System.Collections.Generic;
using System.Text;
using Trisome.WPF.MVVM;
using Trisome.WPF.Services;

namespace OrzLE.WPF.ViewModels
{
    class DeviceViewModel : BaseViewModel
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

        public DeviceViewModel(INavigationService navigationService, ulong address, string name, short rssi)
            : base(navigationService)
        {
            Address = address;
            Name = name;
            RSSI = rssi;
        }
    }
}
