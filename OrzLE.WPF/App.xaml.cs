using OrzLE.WPF.Views;
using System.Windows;
using Trisome.Core.IoC;
using Trisome.DryIoC.WPF;

namespace OrzLE.WPF
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
            throw new System.NotImplementedException();
        }
    }
}
