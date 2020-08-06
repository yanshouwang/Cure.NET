using AMock.WPF.Model;
using System;

namespace AMock.WPF.Services
{
    class LogEventArgs : EventArgs
    {
        public LogModel Log { get; }

        public LogEventArgs(LogModel log)
        {
            Log = log;
        }
    }
}