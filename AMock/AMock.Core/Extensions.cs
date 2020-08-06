using System;
using System.Collections.Generic;
using System.Text;

namespace AMock.Core
{
    static class Extensions
    {
        public static byte GetHigher(this byte value)
            => (byte)((value & 0xF0) >> 4);

        public static byte GetLower(this byte value)
            => (byte)(value & 0x0F);
    }
}
