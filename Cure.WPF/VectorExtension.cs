using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Cure.WPF
{
    static class VectorExtension
    {
        internal static Vector Normalized(this Vector vector)
        {
            var vector1 = new Vector(vector.X, vector.Y);
            var length = vector1.Length;
            return MathUtil.IsVerySmall(length) ? new Vector(0.0, 1.0) : vector1 / length;
        }
    }
}
