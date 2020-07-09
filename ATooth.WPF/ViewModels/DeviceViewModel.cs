using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Trisome.Core.Commands;
using Trisome.Core.IoC;
using Trisome.WPF.MVVM;
using Trisome.WPF.Regions;
using Trisome.WPF.Services;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;

namespace ATooth.WPF.ViewModels
{
    class DeviceViewModel : BaseViewModel
    {
        ulong _address;
        BluetoothLEDevice _device;
        GattDeviceService _communicationService;
        GattCharacteristic _notifyCharacteristic;
        GattCharacteristic _writeCharacteristic;

        IContainerProvider Container { get; }
        public short VId { get; }
        public short PId { get; }
        public byte MId { get; }
        public string MAC { get; }
        public byte MTU => 20;
        Guid CommunicationId { get; }
        Guid NotifyId { get; }
        Guid WriteId { get; }
        public IList<LogViewModel> Logs { get; }
        bool CanWriteWithResponse
            => _writeCharacteristic?.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write) ?? false;
        bool CanWriteWithoutResponse
            => _writeCharacteristic?.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse) ?? false;
        bool CanWrite
            => CanWriteWithResponse || CanWriteWithoutResponse;

        private short _rssi;
        public short RSSI
        {
            get { return _rssi; }
            set { SetProperty(ref _rssi, value); }
        }

        private bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set { SetProperty(ref _connected, value); }
        }

        public DeviceViewModel(INavigationService navigationService, IContainerProvider container, ulong address, short vid, short pid, byte mid, string mac, short rssi)
            : base(navigationService)
        {
            _address = address;

            Container = container;
            VId = vid;
            PId = pid;
            MId = mid;
            MAC = mac;
            RSSI = rssi;

            CommunicationId = UUID.GetCommunicationId(vid, pid, mid);
            NotifyId = UUID.GetNotityId(vid, pid, mid);
            WriteId = UUID.GetWriteId(vid, pid, mid);

            Logs = new ObservableCollection<LogViewModel>();
        }

        public override bool IsNavigationTarget(NavigationContext context)
        {
            var mac = (string)context.Args["MAC"];
            return mac == MAC;
        }

        DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand
            => _connectCommand ??= new DelegateCommand(ExecuteConnectCommand, CanExecuteConnectCommand)
            .ObservesProperty(() => Connected);

        bool CanExecuteConnectCommand()
            => !Connected;

        async void ExecuteConnectCommand()
        {
            _device = await BluetoothLEDevice.FromBluetoothAddressAsync(_address);
            _device.ConnectionStatusChanged += OnConnectionStateChanged;
            var r1 = await _device.GetGattServicesForUuidAsync(CommunicationId);
            if (r1.Status != GattCommunicationStatus.Success)
            {
                throw new NotImplementedException();
            }
            _communicationService = r1.Services[0];
            var r2 = await _communicationService.GetCharacteristicsForUuidAsync(NotifyId);
            if (r2.Status != GattCommunicationStatus.Success)
            {
                throw new NotImplementedException();
            }
            _notifyCharacteristic = r2.Characteristics[0];
            _notifyCharacteristic.ValueChanged += OnValueChanged;
            var r3 = await _communicationService.GetCharacteristicsForUuidAsync(WriteId);
            if (r3.Status != GattCommunicationStatus.Success)
            {
                throw new NotImplementedException();
            }
            _writeCharacteristic = r3.Characteristics[0];
            RaisePropertyChanged(nameof(CanWrite));
            // 开启通知
            var status = await _notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            if (status != GattCommunicationStatus.Success)
            {
                throw new NotImplementedException();
            }
        }

        private void OnConnectionStateChanged(BluetoothLEDevice sender, object args)
        {
            Connected = sender.ConnectionStatus == BluetoothConnectionStatus.Connected;
        }

        byte[] _buffer;

        void OnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out var value);
            if (_buffer == null)
            {
                _buffer = value;
            }
            else
            {
                var values = new List<byte>(_buffer);
                values.AddRange(value);
                _buffer = values.ToArray();
            }
            if (_buffer.Length < 2)
                return;
            var code1 = _buffer[^2];
            var code2 = _buffer[^1];
            if (code1 != 0x0D || code2 != 0x0A)
                return;
            var str = Encoding.ASCII.GetString(_buffer).TrimEnd();
            _buffer = null;

            DealWithStr(str);
        }

        private async void DealWithStr(string str)
        {
            var a0 = (typeof(string), "RECEIVE");
            var a1 = (str.GetType(), str);
            var log = Container.Resolve<LogViewModel>(a0, a1);
            Application.Current.Dispatcher.Invoke(() => Logs.Add(log));
            // 握手
            if (str == "CODE?")
            {
                await WriteAsync("@GrzBLE");
            }
        }

        DelegateCommand _disconnectCommand;
        public DelegateCommand DisconnectCommand
            => _disconnectCommand ??= new DelegateCommand(ExecuteDisconnectCommand, CanExecuteDisconnectCommand)
            .ObservesProperty(() => Connected);

        bool CanExecuteDisconnectCommand()
            => Connected;

        void ExecuteDisconnectCommand()
        {
            _notifyCharacteristic = null;
            _writeCharacteristic = null;
            _communicationService.Dispose();
            _communicationService = null;
            _device.Dispose();
            _device = null;
            Connected = false;
        }

        DelegateCommand<string> _writeCommand;
        public DelegateCommand<string> WriteCommand
            => _writeCommand ??= new DelegateCommand<string>(ExecuteWriteCommand, CanExecuteWriteCommand)
            .ObservesProperty(() => CanWrite);

        bool CanExecuteWriteCommand(string str)
            => CanWrite && str != null;

        async void ExecuteWriteCommand(string str)
            => await WriteAsync(str);

        async Task<bool> WriteAsync(string str)
        {
            var data = Encoding.ASCII.GetBytes($"{str}\r\n");

            var option = CanWriteWithoutResponse ? GattWriteOption.WriteWithoutResponse : GattWriteOption.WriteWithResponse;
            // 大于 20 字节分包发送（最大可以支持 512 字节）
            // https://stackoverflow.com/questions/53313117/cannot-write-large-byte-array-to-a-ble-device-using-uwp-apis-e-g-write-value
            var count = data.Length / MTU;
            var remainder = data.Length % MTU;
            var carriage = new byte[MTU];
            for (int i = 0; i < count; i++)
            {
                Array.Copy(data, i * MTU, carriage, 0, MTU);
                var value = CryptographicBuffer.CreateFromByteArray(carriage);
                var status = await _writeCharacteristic.WriteValueAsync(value, option);
                //var result = await mCharacteristic.WriteValueWithResultAsync(value, option);
                //var status = result.Status;
                if (status != GattCommunicationStatus.Success)
                    return false;
            }
            if (remainder > 0)
            {
                carriage = new byte[remainder];
                Array.Copy(data, count * MTU, carriage, 0, remainder);
                var value = CryptographicBuffer.CreateFromByteArray(carriage);
                var status = await _writeCharacteristic.WriteValueAsync(value, option);
                if (status != GattCommunicationStatus.Success)
                    return false;
            }

            var a0 = (typeof(string), "SEND");
            var a1 = (str.GetType(), str);
            var log = Container.Resolve<LogViewModel>(a0, a1);
            Application.Current.Dispatcher.Invoke(() => Logs.Add(log));

            return true;
        }
    }
}
