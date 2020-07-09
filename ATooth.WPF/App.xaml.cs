using ATooth.WPF.ViewModels;
using ATooth.WPF.Views;
using System.Windows;
using Trisome.Core.IoC;
using Trisome.DryIoC.WPF;
using Trisome.WPF.IoC;
using Trisome.WPF.Services;

namespace ATooth.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : BaseApplication
    {
        protected override Window CreateShell()
            => Container.Resolve<ShellView>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WatcherView>();
            containerRegistry.RegisterForNavigation<DeviceView>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigationService.Navigate<WatcherViewModel>("Shell");
        }
    }
}
