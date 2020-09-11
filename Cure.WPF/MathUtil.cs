// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System;

namespace Cure.WPF
{
    /// <summary>
    /// 用于提供与浮点运算相关的静态属性和方法的帮助程序类。
    /// </summary>
    internal static class MathUtil
    {
        /// <summary>
        /// 考虑到两个值相同时的最小距离。注意：MIL/SL 中的内部浮点是浮点型，不是双精度型。
        /// </summary>
        public const double EPSILON = 1E-06;
        /// <summary>
        /// 全圆角度值。
        /// </summary>
        public const double TWO_PI = 6.28318530717959;
        /// <summary>
        /// 五边形的内径，精确度为以百分比为单位的三位数字。(1 - Sin36 * Sin72 / Sin54) / (Cos36) ^ 2，即 0.47210998990512996761913067272407
        /// </summary>
        public const double PENTAGRAM_INNER_RADIUS = 0.47211;

        /// <summary>
        /// 确定 <see cref="double"/> 值小得是否可以视为零。
        /// </summary>
        /// <param name="value"></param>
        /// <returns>如果值小于 <see cref="DoubleTolerance"/>，则为 <see cref="true"/>；否则为 <see cref="false"/>。</returns>
        public static bool IsVerySmall(double value)
            => Math.Abs(value) < 1E-06;

        public static bool AreClose(double value1, double value2)
            => value1 == value2 || IsVerySmall(value1 - value2);

        public static bool GreaterThan(double value1, double value2)
            => value1 > value2 && !AreClose(value1, value2);

        public static bool GreaterThanOrClose(double value1, double value2)
            => value1 > value2 || AreClose(value1, value2);

        public static bool LessThan(double value1, double value2)
            => value1 < value2 && !AreClose(value1, value2);

        public static bool LessThanOrClose(double value1, double value2)
            => value1 < value2 || AreClose(value1, value2);

        public static double SafeDivide(double lhs, double rhs, double fallback)
            => !IsVerySmall(rhs) ? lhs / rhs : fallback;

        public static double Lerp(double x, double y, double alpha)
            => x * (1.0 - alpha) + y * alpha;

        /// <summary>
        /// 返回给定范围内的值。等于 null 的给定的最小值/最大值表示无限制。
        /// </summary>
        public static double EnsureRange(double value, double? min, double? max)
        {
            if (min.HasValue && value < min.Value)
                return min.Value;
            return max.HasValue && value > max.Value ? max.Value : value;
        }

        /// <summary>
        /// 计算矢量 (x，y) 的欧几里得范数。
        /// </summary>
        /// <param name="x">第一个分量。</param>
        /// <param name="y">第二个分量。</param>
        /// <returns>矢量 (x，y) 的欧几里得范数。</returns>
        public static double Hypotenuse(double x, double y)
            => Math.Sqrt(x * x + y * y);

        /// <summary>
        /// 利用尾数和指数计算实数。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="exp"></param>
        /// <returns>如果成功，则为 x * 2^exp 的值。</returns>
        public static double DoubleFromMantissaAndExponent(double x, int exp)
            => x * Math.Pow(2.0, exp);

        /// <summary>测试双精度值。</summary>
        /// <param name="x">要测试的双精度值。</param>
        /// <returns>如果 x 不是 NaN 并且不等于正无穷大或负无穷大，则为 <c>True</c>；否则，为 <c>False</c>。</returns>
        public static bool IsFiniteDouble(double x)
            => !double.IsInfinity(x) && !double.IsNaN(x);
    }
}