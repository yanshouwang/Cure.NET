using ATooth.WPF.Model;
using System;

namespace ATooth.WPF.Services
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