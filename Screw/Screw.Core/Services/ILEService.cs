using Screw.Core.LE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Screw.Core.Services
{
    public interface ILEService
    {
        IAdapter GetAdapter();
    }
}
