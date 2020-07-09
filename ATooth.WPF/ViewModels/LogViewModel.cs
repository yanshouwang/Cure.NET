using System;
using System.Collections.Generic;
using System.Text;
using Trisome.WPF.MVVM;
using Trisome.WPF.Services;

namespace ATooth.WPF.ViewModels
{
    class LogViewModel : BaseViewModel
    {
        public string Sender { get; }
        public string Message { get; }
        public DateTime Time { get; }

        public LogViewModel(INavigationService navigationService, string sender, string message)
            : base(navigationService)
        {
            Sender = sender;
            Message = message;
            Time = DateTime.Now;
        }

        public override string ToString()
            => $"[{Time}] [{Sender}] {Message}";
    }
}
