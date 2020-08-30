using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    static class GeometryValueExtensions
    {
        public static Geometry ToGeometry(this GeometryValue value)
        {
            var source = value.ToString();
            return Geometry.Parse(source);
        }
    }
}
