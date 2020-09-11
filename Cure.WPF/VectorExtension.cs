using System.Windows;

namespace Cure.WPF
{
    internal static class VectorExtension
    {
        internal static Vector Normalized(this Vector vector)
        {
            Vector vector1 = new Vector(vector.X, vector.Y);
            double length = vector1.Length;
            return MathUtil.IsVerySmall(length) ? new Vector(0.0, 1.0) : vector1 / length;
        }
    }
}
