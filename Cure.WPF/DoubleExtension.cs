using System;

namespace Cure.WPF
{
    internal static class DoubleExtension
    {
        public const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

        public static bool IsZero(this double value) => Math.Abs(value) < 10.0 * DBL_EPSILON;
    }
}
