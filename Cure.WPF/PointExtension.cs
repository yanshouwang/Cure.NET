// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Windows;

namespace Cure.WPF
{
    static class PointExtension
    {
        /// <summary>
        /// 获取两个点之间的差异矢量。
        /// </summary>
        public static Vector Subtract(this Point lhs, Point rhs)
            => new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);

        /// <summary>
        /// 逐个添加点成员。
        /// </summary>
        public static Point Plus(this Point lhs, Point rhs)
            => new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);

        /// <summary>
        /// 逐个减去点成员。
        /// </summary>
        public static Point Minus(this Point lhs, Point rhs)
            => new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }
}
