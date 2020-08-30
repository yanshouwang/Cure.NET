using System;
using System.Collections.Generic;
using System.Text;

namespace Cure.WPF.Media
{
    class GeometryValue
    {
        public int FillRule { get; }

        public GeometryValue(int fillRule)
        {
            FillRule = fillRule;
        }

        public static GeometryValue operator +(GeometryValue left, GeometryValue right)
        {
            return left;
        }
    }
}
