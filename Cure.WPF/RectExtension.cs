// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.Windows;

namespace Cure.WPF
{
    internal static class RectExtension
    {
        public static Thickness Subtract(this Rect lhs, Rect rhs)
        {
            double left = rhs.Left - lhs.Left;
            double top = rhs.Top - lhs.Top;
            double right = lhs.Right - rhs.Right;
            double bottom = lhs.Bottom - rhs.Bottom;
            return new Thickness(left, top, right, bottom);
        }

        public static Point Center(this Rect rect)
            => new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);

        /// <summary>
        /// 将矩形大小调整为相对大小，同时保持中心不变。
        /// </summary>
        public static Rect Resize(this Rect rect, double ratio)
            => rect.Resize(ratio, ratio);

        public static Rect Resize(this Rect rect, double ratioX, double ratioY)
        {
            Point point = rect.Center();
            double width = rect.Width * ratioX;
            double height = rect.Height * ratioY;
            return new Rect(point.X - width / 2.0, point.Y - height / 2.0, width, height);
        }
    }
}
