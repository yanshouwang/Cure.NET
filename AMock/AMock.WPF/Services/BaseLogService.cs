using AMock.WPF.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMock.WPF.Services
{
    abstract class BaseLogService : ILogService
    {
        public abstract IList<LogModel> Logs { get; }

        public event EventHandler<LogEventArgs> Logged;

        public void Log(string message)
        {
            var log = new LogModel(message, DateTime.Now);
            Log(log);
            Logged?.Invoke(this, new LogEventArgs(log));
        }

        protected abstract void Log(LogModel log);
    }
}
