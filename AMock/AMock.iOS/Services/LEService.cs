using CoreBluetooth;
using AMock.Core.LE;
using AMock.Core.Services;
using AMock.iOS.LE;

namespace AMock.iOS.Services
{
    class LEService : ILEService
    {
        public IAdapter GetAdapter()
        {
            var manager = new CBCentralManager();
            return manager.ToCore();
        }
    }
}