using ATooth.WPF.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ATooth.WPF.Services
{
    interface ILogService
    {
        event EventHandler<LogEventArgs> Logged;
        IList<LogModel> Logs { get; }
        void Log(string message);
    }
}
