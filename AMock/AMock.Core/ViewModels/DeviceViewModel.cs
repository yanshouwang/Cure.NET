using AMock.Core.LE;
using Prism.Commands;
using Prism.Navigation;
using System;

namespace AMock.Core.ViewModels
{
    class DeviceViewModel : BaseViewModel
    {
        IDevice _device;
        IService _communicationService;
        ICharacteristic _notifyCharacteristic;
        ICharacteristic _writeCharacteristic;

        public DeviceCategory Category { get; }
        public short VId { get; }
        public short PId { get; }
        public byte MId { get; }
        public string MAC { get; }
        public byte MTU { get; }

        Guid CommunicationUUID { get; }
        Guid NotifyUUID { get; }
        Guid WriteUUID { get; }

        bool CanWriteWithResponse
            => _writeCharacteristic != null && _writeCharacteristic.Properties.HasFlag(CharacteristicProperties.Write);
        bool CanWriteWithoutResponse
            => _writeCharacteristic != null && _writeCharacteristic.Properties.HasFlag(CharacteristicProperties.WriteWithoutResponse);
        bool CanWrite
            => CanWriteWithoutResponse || CanWriteWithResponse;

        short _rssi;
        public short RSSI
        {
            get => _rssi;
            set => SetProperty(ref _rssi, value);
        }

        bool _connected;
        public bool Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, value);
        }

        public DeviceViewModel(INavigationService navigationService, IDevice device, DeviceArgs args)
            : base(navigationService)
        {
            _device = device;

            Category = args.Category;
            VId = args.VId;
            PId = args.PId;
            MId = args.MId;
            MAC = args.MAC;
            MTU = 20;
            CommunicationUUID = args.CommunicatonUUID;
            NotifyUUID = args.NotifyUUID;
            WriteUUID = args.WriteUUID;
            RSSI = args.RSSI;
        }

        DelegateCommand _connectCommand;
        public DelegateCommand ConnectCommand =>
            _connectCommand ?? (_connectCommand = new DelegateCommand(ExecuteConnectCommand, CanExecuteConnectCommand).ObservesProperty(() => Connected));

        void ExecuteConnectCommand()
        {
            _device.Connect();
        }

        bool CanExecuteConnectCommand()
        {
            return !Connected;
        }
    }
}