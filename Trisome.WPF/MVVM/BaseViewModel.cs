using Trisome.Core.Commands;
using Trisome.Core.MVVM;
using Trisome.WPF.Regions;
using Trisome.WPF.Services;

namespace Trisome.WPF.MVVM
{
    public abstract class BaseViewModel : ObservableObject, INavigationAware
    {
        IRegionNavigationJournal _journal;

        protected INavigationService NavigationService { get; }

        public BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand
            => _goBackCommand ??= new DelegateCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand);

        bool CanExecuteGoBackCommand()
           => _journal != null && _journal.CanGoBack;

        void ExecuteGoBackCommand()
            => _journal.GoBack();

        DelegateCommand _goForwardCommand;
        public DelegateCommand GoForwardCommand
            => _goForwardCommand ??= new DelegateCommand(ExecuteGoForwardCommand, CanExecuteGoForwardCommand);

        bool CanExecuteGoForwardCommand()
           => _journal != null && _journal.CanGoForward;

        void ExecuteGoForwardCommand()
            => _journal.GoForward();

        public virtual void OnNavigatedTo(NavigationContext context)
            => _journal = context.NavigationService.Journal;

        public virtual bool IsNavigationTarget(NavigationContext context)
            => true;

        public virtual void OnNavigatedFrom(NavigationContext context) { }
    }
}
