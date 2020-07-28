using Prism;
using Prism.Ioc;
using Screw.Core.Services;
using Screw.iOS.Services;

namespace Screw.iOS
{
    class Initializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            container.Register<ILEService, LEService>();
        }
    }
}