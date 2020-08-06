using AMock.WPF.Services;
using DryIoc;
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

namespace AMock.WPF.ViewModels
{
    class DeviceViewModel : BaseViewModel
    {
        short _rssi;
        BluetoothLEDevice _device;
        GattDeviceService _communicationService;
        GattCharacteristic _notifyCharacteristic;
        GattCharacteristic _writeCharacteristic;

        ulong Address { get; }
        ILogService LogService { get; }
        public DeviceCategory Category { get; }
        public short VId { get; }
        public short PId { get; }
        public byte MId { get; }
        public string MAC { get; }
        public byte MTU => 20;
        Guid CommunicationUUID { get; }
        Guid NotifyUUID { get; }
        Guid WriteUUID { get; }
        bool CanWriteWithResponse
            => _writeCharacteristic != null && _writeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write);
        bool CanWriteWithoutResponse
            => _writeCharacteristic != null && _writeCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
        public bool CanWrite
            => CanWriteWithResponse || CanWriteWithoutResponse;

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

        public DeviceViewModel(INavigationService navigationService, ILogService logService, DeviceArgs args)
            : base(navigationService)
        {
            LogService = logService;
            Category = args.Category;
            Address = args.Address;
            RSSI = args.RSSI;
            VId = args.VId;
            PId = args.PId;
            MId = args.MId;
            MAC = args.MAC;
            CommunicationUUID = args.CommunicatonUUID;
            NotifyUUID = args.NotifyUUID;
            WriteUUID = args.WriteUUID;
        }

        DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand
            => _connectCommand ??= new DelegateCommand(ExecuteConnectCommand, CanExecuteConnectCommand)
            .ObservesProperty(() => Connected);

        bool CanExecuteConnectCommand()
            => !Connected;

        async void ExecuteConnectCommand()
        {
            _device = await BluetoothLEDevice.FromBluetoothAddressAsync(Address);
            _device.ConnectionStatusChanged += OnConnectionStateChanged;
            var r1 = await _device.GetGattServicesForUuidAsync(CommunicationUUID);
            if (r1.Status != GattCommunicationStatus.Success || r1.Services.Count == 0)
            {
                LogService.Log("获取服务失败");
                ExecuteDisconnectCommand();
                return;
            }
            _communicationService = r1.Services[0];
            var r2 = await _communicationService.GetCharacteristicsForUuidAsync(NotifyUUID);
            if (r2.Status != GattCommunicationStatus.Success || r2.Characteristics.Count == 0)
            {
                LogService.Log("获取通知特征值失败");
                ExecuteDisconnectCommand();
                return;
            }
            _notifyCharacteristic = r2.Characteristics[0];
            _notifyCharacteristic.ValueChanged += OnValueChanged;
            var r3 = await _communicationService.GetCharacteristicsForUuidAsync(WriteUUID);
            if (r3.Status != GattCommunicationStatus.Success || r3.Characteristics.Count == 0)
            {
                LogService.Log("获取写入特征值失败");
                ExecuteDisconnectCommand();
                return;
            }
            _writeCharacteristic = r3.Characteristics[0];
            RaisePropertyChanged(nameof(CanWrite));
            // 开启通知
            try
            {
                var status = await _notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                if (status != GattCommunicationStatus.Success)
                {
                    LogService.Log("开启通知失败");
                    ExecuteDisconnectCommand();
                }
            }
            catch (Exception ex)
            {
                LogService.Log($"开启通知失败 {ex.Message}");
                ExecuteDisconnectCommand();
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
            var message = $"RECEIVE : {str}";
            LogService.Log(message);
            // 握手
            if (str == "CODE?")
            {
                await WriteAsync("@ATooth");
            }
        }

        DelegateCommand _disconnectCommand;
        public DelegateCommand DisconnectCommand
            => _disconnectCommand ??= new DelegateCommand(ExecuteDisconnectCommand, CanExecuteDisconnectCommand)
            .ObservesProperty(() => Connected);

        bool CanExecuteDisconnectCommand()
            => Connected;

        async void ExecuteDisconnectCommand()
        {
            if (_notifyCharacteristic != null)
            {
                try
                {
                    var status = await _notifyCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (status != GattCommunicationStatus.Success)
                    {
                        LogService.Log("关闭通知失败");
                    }
                }
                catch (Exception ex)
                {
                    LogService.Log(ex.Message);
                }
                _notifyCharacteristic.ValueChanged -= OnValueChanged;
                _notifyCharacteristic = null;
                _buffer = null;
            }
            _writeCharacteristic = null;
            if (_communicationService != null)
            {
                _communicationService.Dispose();
                _communicationService = null;
            }
            _device.Dispose();
            _device = null;
            Connected = false;
            RaisePropertyChanged(nameof(CanWrite));
            _buffer = null;
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

            var message = $"SEND : {str}";
            LogService.Log(message);

            return true;
        }
    }
}
