using CoreBluetooth;
using Screw.Core.LE;
using Screw.Core.Services;
using Screw.iOS.LE;

namespace Screw.iOS.Services
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