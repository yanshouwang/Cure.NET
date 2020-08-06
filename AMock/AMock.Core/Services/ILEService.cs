using AMock.Core.LE;
using System;
using System.Collections.Generic;
using System.Text;

namespace AMock.Core.Services
{
    public interface ILEService
    {
        IAdapter GetAdapter();
    }
}
