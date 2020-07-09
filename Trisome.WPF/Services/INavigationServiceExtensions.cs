using System;
using System.Collections.Generic;
using System.Text;
using Trisome.WPF.MVVM;

namespace Trisome.WPF.Services
{
    public static class INavigationServiceExtensions
    {
        public static void Navigate<T>(this INavigationService navigationService, string regionName) where T : BaseViewModel
        {
            var uri = typeof(T).Name[0..^5];
            navigationService.Navigate(regionName, uri);
        }

        public static void Navigate<T>(this INavigationService navigationService, string regionName, IDictionary<string, object> args) where T : BaseViewModel
        {
            var uri = typeof(T).Name[0..^5];
            navigationService.Navigate(regionName, uri, args);
        }
    }
}
