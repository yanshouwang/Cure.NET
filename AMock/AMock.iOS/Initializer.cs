using Prism;
using Prism.Ioc;
using AMock.Core.Services;
using AMock.iOS.Services;

namespace AMock.iOS
{
    class Initializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            container.Register<ILEService, LEService>();
        }
    }
}