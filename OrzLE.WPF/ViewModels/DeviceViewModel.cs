using System;
using System.Collections.Generic;
using System.Text;
using Trisome.WPF.MVVM;
using Trisome.WPF.Services;

namespace OrzLE.WPF.ViewModels
{
    class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
