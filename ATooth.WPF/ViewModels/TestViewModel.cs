using ATooth.WPF.Model;
using ATooth.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Trisome.WPF.MVVM;
using Trisome.WPF.Regions;
using Trisome.WPF.Services;

namespace ATooth.WPF.ViewModels
{
    class TestViewModel : BaseViewModel
    {
        DeviceViewModel _device;
        int _interval;

        ILogService LogService { get; }
        public IList<LogModel> Logs { get; }
        public DeviceViewModel Device
        {
            get { return _device; }
            set { SetProperty(ref _device, value); }
        }
        public int Interval
        {
            get { return _interval; }
            set { SetProperty(ref _interval, value); }
        }

        public TestViewModel(INavigationService navigationService, ILogService logService)
            : base(navigationService)
        {
            LogService = logService;
            Logs = new ObservableCollection<LogModel>();
            Interval = 1000;
        }

        public override void OnNavigatedTo(NavigationContext context)
        {
            base.OnNavigatedTo(context);

            Device = (DeviceViewModel)context.Args["Device"];
            LogService.Logged += OnLogged;
        }

        private void OnLogged(object sender, LogEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Logs.Add(e.Log));
        }

        public override void OnNavigatedFrom(NavigationContext context)
        {
            base.OnNavigatedFrom(context);

            LogService.Logged -= OnLogged;
            // 断开连接
            if (Device.DisconnectCommand.CanExecute())
            {
                Device.DisconnectCommand.Execute();
            }
        }

        public override bool IsNavigationTarget(NavigationContext context)
        {
            var device = (DeviceViewModel)context.Args["Device"];
            return _device == device;
        }
    }
}
