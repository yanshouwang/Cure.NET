using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Cure.WPF
{
    static class SizeExtension
    {
        public static Rect ToBounds(this Size size)
        {
            return new Rect(0.0, 0.0, size.Width, size.Height);
        }
    }
}
