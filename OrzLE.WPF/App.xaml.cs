using OrzLE.WPF.ViewModels;
using OrzLE.WPF.Views;
using System.Windows;
using Trisome.Core.IoC;
using Trisome.DryIoC.WPF;
using Trisome.WPF.IoC;
using Trisome.WPF.Services;

namespace OrzLE.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BaseApplication
    {
        protected override Window CreateShell()
            => Container.Resolve<ShellView>();

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterForNavigation<WatcherView>();
            container.RegisterForNavigation<DeviceView>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigationService.Navigate<WatcherViewModel>("Shell");
        }
    }
}
