using System;
using System.Collections.Generic;
using System.Text;

namespace ATooth.WPF.Model
{
    class LogModel
    {
        public string Message { get; }
        public DateTime Time { get; }

        public LogModel(string message, DateTime time)
        {
            Message = message;
            Time = time;
        }

        public override string ToString()
            => $"[{Time}] {Message}";
    }
}
