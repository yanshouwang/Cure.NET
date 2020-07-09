using System;
using System.Collections.Generic;
using System.Text;
using Trisome.WPF.MVVM;
using Trisome.WPF.Regions;
using Trisome.WPF.Services;

namespace OrzLE.WPF.ViewModels
{
    class ShellViewModel : BaseViewModel
    {
        public ShellViewModel(INavigationService navigationService)
            : base(navigationService)
        {

        }
    }
}
