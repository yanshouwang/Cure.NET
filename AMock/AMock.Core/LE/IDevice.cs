namespace AMock.Core.LE
{
    public interface IDevice
    {
        DeviceState State { get; }

        void Connect();
        void Disconnect();
        void DiscoverServices();
    }
}