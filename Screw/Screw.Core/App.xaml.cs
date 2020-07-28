using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Screw.Core.ViewModels;
using Screw.Core.Views;
using Xamarin.Forms;

namespace Screw.Core
{
    public partial class App : PrismApplication
    {
        public App()
            : base()
        {
        }

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterForNavigation<NavigationPage>("NavigationView");
            container.RegisterForNavigation<WatcherView, WatcherViewModel>();
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("WatcherView");
        }
    }
}
