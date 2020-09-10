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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 与几何图形相关的数据结构（点/矢量/大小/矩形）的扩展方法。
    /// </summary>
    static class GeometryUtil
    {
        public static Point Lerp(Point pointA, Point pointB, double alpha)
            => new Point(MathUtil.Lerp(pointA.X, pointB.X, alpha), MathUtil.Lerp(pointA.Y, pointB.Y, alpha));

        public static Vector Lerp(Vector vectorA, Vector vectorB, double alpha)
            => new Vector(MathUtil.Lerp(vectorA.X, vectorB.X, alpha), MathUtil.Lerp(vectorA.Y, vectorB.Y, alpha));

        public static Rect Inflate(Rect rect, double offset)
            => Inflate(rect, new Thickness(offset));

        public static Rect Inflate(Rect rect, double offsetX, double offsetY)
            => Inflate(rect, new Thickness(offsetX, offsetY, offsetX, offsetY));

        public static Rect Inflate(Rect rect, Size size)
            => Inflate(rect, new Thickness(size.Width, size.Height, size.Width, size.Height));

        public static Rect Inflate(Rect rect, Thickness thickness)
        {
            var width = rect.Width + thickness.Left + thickness.Right;
            var height = rect.Height + thickness.Top + thickness.Bottom;
            var x = rect.X - thickness.Left;
            if (width < 0.0)
            {
                x += width / 2.0;
                width = 0.0;
            }
            var y = rect.Y - thickness.Top;
            if (height < 0.0)
            {
                y += height / 2.0;
                height = 0.0;
            }
            return new Rect(x, y, width, height);
        }

        /// <summary>
        /// 获取 (0,0)(1,1) 框中的标准化弧。零度按顺时针方向映射到 [0.5，0]（向上）。
        /// </summary>
        public static Point GetArcPoint(double degree)
        {
            var num = degree * Math.PI / 180.0;
            return new Point(0.5 + 0.5 * Math.Sin(num), 0.5 - 0.5 * Math.Cos(num));
        }

        /// <summary>
        /// 利用给定的相对半径获取给定边界中的绝对弧点。
        /// </summary>
        public static Point GetArcPoint(double degree, Rect bound)
        {
            var arcPoint = GetArcPoint(degree);
            return RelativeToAbsolutePoint(bound, arcPoint);
        }

        /// <summary>
        /// 获取相对于 (0,0)(1,1) 框的弧的角度。零度按顺时针方向映射到 [0.5，0]（向上）。
        /// </summary>
        public static double GetArcAngle(Point point)
            => Math.Atan2(point.Y - 0.5, point.X - 0.5) * 180.0 / Math.PI + 90.0;

        /// <summary>
        /// 利用相对于边界的给定绝对点获取弧的角度。
        /// </summary>
        public static double GetArcAngle(Point point, Rect bound)
            => GetArcAngle(AbsoluteToRelativePoint(bound, point));

        /// <summary>
        /// 使用从给定边界到 (0,0)(1,1) 框的映射将相对点映射到绝对点。
        /// </summary>
        public static Point RelativeToAbsolutePoint(Rect bound, Point relative)
            => new Point(bound.X + relative.X * bound.Width, bound.Y + relative.Y * bound.Height);

        /// <summary>
        /// 使用从 (0,0)(1,1) 框到给定边界的映射将绝对点映射到相对点。
        /// </summary>
        public static Point AbsoluteToRelativePoint(Rect bound, Point absolute)
            => new Point(MathUtil.SafeDivide(absolute.X - bound.X, bound.Width, 1.0), MathUtil.SafeDivide(absolute.Y - bound.Y, bound.Height, 1.0));

        /// <summary>
        /// 在给定的逻辑边界中拉伸之后计算边界。如果拉伸到统一值，请使用给定的 aspectRatio。如果 aspectRatio 为空，则它等效于“填充”。如果拉伸为“无”，则它等效于“填充”或“统一”。
        /// </summary>
        public static Rect GetStretchBound(Rect logicalBound, Stretch stretch, Size aspectRatio)
        {
            if (stretch == Stretch.None)
                stretch = Stretch.Fill;
            if (stretch == Stretch.Fill || !aspectRatio.HasValidArea())
                return logicalBound;
            var point = logicalBound.Center();
            switch (stretch)
            {
                case Stretch.Uniform:
                    if (aspectRatio.Width * logicalBound.Height < logicalBound.Width * aspectRatio.Height)
                    {
                        logicalBound.Width = logicalBound.Height * aspectRatio.Width / aspectRatio.Height;
                        break;
                    }
                    logicalBound.Height = logicalBound.Width * aspectRatio.Height / aspectRatio.Width;
                    break;
                case Stretch.UniformToFill:
                    if (aspectRatio.Width * logicalBound.Height < logicalBound.Width * aspectRatio.Height)
                    {
                        logicalBound.Height = logicalBound.Width * aspectRatio.Height / aspectRatio.Width;
                        break;
                    }
                    logicalBound.Width = logicalBound.Height * aspectRatio.Width / aspectRatio.Height;
                    break;
            }
            return new Rect(point.X - logicalBound.Width / 2.0, point.Y - logicalBound.Height / 2.0, logicalBound.Width, logicalBound.Height);
        }

        /// <summary>返回 2 个点的中点。</summary>
        /// <param name="lhs">第一个点。</param>
        /// <param name="rhs">第二个点。</param>
        /// <returns>以下两点之间的中点：<paramref name="lhs" /> 和  <paramref name="rhs" />.</returns>
        public static Point Midpoint(Point lhs, Point rhs)
            => new Point((lhs.X + rhs.X) / 2.0, (lhs.Y + rhs.Y) / 2.0);

        /// <summary>
        /// 返回两个矢量的点积。
        /// </summary>
        /// <param name="lhs">第一个矢量。</param>
        /// <param name="rhs">第二个矢量。</param>
        /// <returns>以下两个矢量的点积：<paramref name="lhs" /> 和  <paramref name="rhs" />.</returns>
        public static double Dot(Vector lhs, Vector rhs)
            => lhs.X * rhs.X + lhs.Y * rhs.Y;

        /// <summary>
        /// 返回两个点的点积。
        /// </summary>
        public static double Dot(Point lhs, Point rhs)
            => lhs.X * rhs.X + lhs.Y * rhs.Y;

        /// <summary>
        /// 返回两个点之间的距离。
        /// </summary>
        /// <param name="lhs">第一个点。</param>
        /// <param name="rhs">第二个点。</param>
        /// <returns>以下两个点之间的距离：<paramref name="lhs" /> 和  <paramref name="rhs" />.</returns>
        public static double Distance(Point lhs, Point rhs)
        {
            var num1 = lhs.X - rhs.X;
            var num2 = lhs.Y - rhs.Y;
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }

        /// <summary>
        /// 返回两个点之间的距离的平方。
        /// </summary>
        /// <param name="lhs">第一个点。</param>
        /// <param name="rhs">第二个点。</param>
        /// <returns>以下两个点之间的距离的平方：<paramref name="lhs" /> 和  <paramref name="rhs" />.</returns>
        public static double SquaredDistance(Point lhs, Point rhs)
        {
            var num1 = lhs.X - rhs.X;
            var num2 = lhs.Y - rhs.Y;
            return num1 * num1 + num2 * num2;
        }

        /// <summary>
        /// 叉积的决定因子。等效于方向区域。
        /// </summary>
        public static double Determinant(Point lhs, Point rhs)
            => lhs.X * rhs.Y - lhs.Y * rhs.X;

        /// <summary>
        /// 计算给定线段的法线方向矢量。
        /// </summary>
        public static Vector Normal(Point lhs, Point rhs)
            => new Vector(lhs.Y - rhs.Y, rhs.X - lhs.X).Normalized();

        /// <summary>
        /// 确保值为结果类型 (T) 的实例。如果不是，则更换为类型 (T) 的新实例。
        /// </summary>
        public static bool EnsureGeometryType<T>(out T result, ref Geometry value, Func<T> factory) where T : Geometry
        {
            result = value as T;
            if (result != null)
                return false;
            value = result = factory();
            return true;
        }

        /// <summary>
        /// 确保 list[index] 为结果类型 (T) 的实例。如果不是，则更换为类型 (T) 的新实例。
        /// </summary>
        public static bool EnsureSegmentType<T>(out T result, IList<PathSegment> list, int index, Func<T> factory) where T : PathSegment
        {
            result = list[index] as T;
            if (result != null)
                return false;
            list[index] = result = factory();
            return true;
        }
    }
}
