using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using AMock.Core.ViewModels;
using AMock.Core.Views;
using Xamarin.Forms;

namespace AMock.Core
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

            await NavigationService.NavigateAsync("NavigationView/WatcherView");
        }
    }
}
