﻿using AMock.WPF.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AMock.WPF.Services
{
    class DebugLogService : BaseLogService
    {
        public override IList<LogModel> Logs
            => throw new NotImplementedException();

        protected override void Log(LogModel log)
            => Debug.WriteLine(log);
    }
}