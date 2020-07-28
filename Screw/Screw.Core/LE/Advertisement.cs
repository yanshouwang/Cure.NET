using System;
using System.Collections.Generic;
using System.Text;

namespace Screw.Core.LE
{
    public class Advertisement
    {
        public byte Type { get; }
        public byte[] Value { get; }

        public Advertisement(byte type, byte[] value)
        {
            Type = type;
            Value = value;
        }
    }
}
