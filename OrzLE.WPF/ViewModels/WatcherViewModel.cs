using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Trisome.WPF.MVVM;
using Trisome.WPF.Regions;
using Trisome.WPF.Services;
using Windows.Devices.Bluetooth.Advertisement;

namespace OrzLE.WPF.ViewModels
{
    class WatcherViewModel : BaseViewModel
    {
        readonly BluetoothLEAdvertisementWatcher _watcher;

        public ICollection<DeviceViewModel> Devices { get; }

        public WatcherViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _watcher = new BluetoothLEAdvertisementWatcher();
            _watcher.Received += OnWatcherReceived;

            Devices = new ObservableCollection<DeviceViewModel>();
        }

        void OnWatcherReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var address = args.BluetoothAddress;
            var name = args.Advertisement.LocalName;
            var rssi = args.RawSignalStrengthInDBm;

            var device = Devices.FirstOrDefault(i => i.Address == address);
            if (device != null)
            {
                device.Name = name;
                device.RSSI = rssi;
            }
            else
            {
                device = new DeviceViewModel(NavigationService, address, name, rssi);
                Application.Current.Dispatcher.Invoke(() => Devices.Add(device));
            }
        }

        public override void OnNavigatedTo(NavigationContext context)
        {
            base.OnNavigatedTo(context);

            _watcher.Start();
        }

        public override void OnNavigatedFrom(NavigationContext context)
        {
            base.OnNavigatedFrom(context);

            _watcher.Stop();
        }
    }
}
