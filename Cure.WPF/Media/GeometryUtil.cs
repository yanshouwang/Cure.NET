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
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 与几何图形相关的数据结构（点/矢量/大小/矩形）的扩展方法。
    /// </summary>
    internal static class GeometryUtil
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
            double width = rect.Width + thickness.Left + thickness.Right;
            double height = rect.Height + thickness.Top + thickness.Bottom;
            double x = rect.X - thickness.Left;
            if (width < 0.0)
            {
                x += width / 2.0;
                width = 0.0;
            }
            double y = rect.Y - thickness.Top;
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
            double num = degree * Math.PI / 180.0;
            return new Point(0.5 + 0.5 * Math.Sin(num), 0.5 - 0.5 * Math.Cos(num));
        }

        /// <summary>
        /// 利用给定的相对半径获取给定边界中的绝对弧点。
        /// </summary>
        public static Point GetArcPoint(double degree, Rect bound)
        {
            Point arcPoint = GetArcPoint(degree);
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
            Point point = logicalBound.Center();
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
            double num1 = lhs.X - rhs.X;
            double num2 = lhs.Y - rhs.Y;
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
            double num1 = lhs.X - rhs.X;
            double num2 = lhs.Y - rhs.Y;
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

        /// <summary>
        /// 将采用路径微型语言的字符串转换为 PathGeometry。
        /// </summary>
        /// <param name="abbreviatedGeometry">采用路径微型语言的字符串。</param>
        internal static PathGeometry ConvertToPathGeometry(string abbreviatedGeometry)
        {
            if (abbreviatedGeometry == null)
                throw new ArgumentNullException(nameof(abbreviatedGeometry));
            PathGeometry geometry = new PathGeometry { Figures = new PathFigureCollection() };
            int num = 0;
            while (num < abbreviatedGeometry.Length && char.IsWhiteSpace(abbreviatedGeometry, num))
                ++num;
            if (num < abbreviatedGeometry.Length && abbreviatedGeometry[num] == 'F')
            {
                int index = num + 1;
                while (index < abbreviatedGeometry.Length && char.IsWhiteSpace(abbreviatedGeometry, index))
                    ++index;
                if (index == abbreviatedGeometry.Length || abbreviatedGeometry[index] != '0' && abbreviatedGeometry[index] != '1')
                    throw new FormatException();
                geometry.FillRule = abbreviatedGeometry[index] == '0' ? FillRule.EvenOdd : FillRule.Nonzero;
                num = index + 1;
            }
            new AbbreviatedGeometryParser(geometry).Parse(abbreviatedGeometry, num);
            return geometry;
        }

        /// <summary>
        /// 用与给定点列表匹配的折线更新 PathGeometry 形式的给定几何图形。
        /// </summary>
        public static bool SyncPolylineGeometry(ref Geometry geometry, IList<Point> points, bool isClosed)
        {
            bool flag = false;
            PathFigure figure;
            if (!(geometry is PathGeometry pathGeometry) || pathGeometry.Figures.Count != 1 || (figure = pathGeometry.Figures[0]) == null)
            {
                ((PathGeometry)(geometry = new PathGeometry())).Figures.Add(figure = new PathFigure());
                flag = true;
            }
            return flag | PathFigureUtil.SyncPolylineFigure(figure, points, isClosed);
        }

        internal static Geometry FixPathGeometryBoundary(Geometry geometry)
            => geometry;

        /// <summary>
        /// 分析缩写的几何图形语法。
        /// </summary>
        private class AbbreviatedGeometryParser
        {
            private readonly PathGeometry _geometry;
            private PathFigure _figure;
            private Point _lastPoint;
            private Point _secondLastPoint;
            private string _buffer;
            private int _index;
            private int _length;
            private char _token;

            public AbbreviatedGeometryParser(PathGeometry geometry)
            {
                this._geometry = geometry;
            }

            public void Parse(string data, int startIndex)
            {
                this._buffer = data;
                this._length = data.Length;
                this._index = startIndex;
                bool flag = true;
                while (this.ReadToken())
                {
                    char token = this._token.ToUpper();
                    if (flag)
                    {
                        if (token != 'M')
                            throw new FormatException();
                        flag = false;
                    }
                    switch (token)
                    {
                        case 'A':
                            do
                            {
                                Size size = this.ReadSize(false);
                                double rotationAngle = this.ReadDouble(true);
                                bool isLargeArc = this.ReadBool01(true);
                                SweepDirection sweepDirection = this.ReadBool01(true) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                                this._lastPoint = this.ReadPoint(token, true);
                                this.ArcTo(size, rotationAngle, isLargeArc, sweepDirection, this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            this.EnsureFigure();
                            continue;
                        case 'C':
                            this.EnsureFigure();
                            do
                            {
                                Point point1 = this.ReadPoint(token, false);
                                this._secondLastPoint = this.ReadPoint(token, true);
                                this._lastPoint = this.ReadPoint(token, true);
                                this.BezierTo(point1, this._secondLastPoint, this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            continue;
                        case 'H':
                            this.EnsureFigure();
                            do
                            {
                                double num = this.ReadDouble(false);
                                if (token == 'h')
                                    num += this._lastPoint.X;
                                this._lastPoint.X = num;
                                this.LineTo(this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            continue;
                        case 'L':
                            this.EnsureFigure();
                            do
                            {
                                this._lastPoint = this.ReadPoint(token, false);
                                this.LineTo(this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            continue;
                        case 'M':
                            this._lastPoint = this.ReadPoint(token, false);
                            this.BeginFigure(this._lastPoint);
                            char command = 'M';
                            while (this.IsNumber(true))
                            {
                                this._lastPoint = this.ReadPoint(command, false);
                                this.LineTo(this._lastPoint);
                                command = 'L';
                            }
                            continue;
                        case 'Q':
                            this.EnsureFigure();
                            do
                            {
                                Point point1 = this.ReadPoint(token, false);
                                this._lastPoint = this.ReadPoint(token, true);
                                this.QuadraticBezierTo(point1, this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            continue;
                        case 'S':
                            this.EnsureFigure();
                            do
                            {
                                Point beizerFirstPoint = this.GetSmoothBeizerFirstPoint();
                                Point point2 = this.ReadPoint(token, false);
                                this._lastPoint = this.ReadPoint(token, true);
                                this.BezierTo(beizerFirstPoint, point2, this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            continue;
                        case 'V':
                            this.EnsureFigure();
                            do
                            {
                                double num = this.ReadDouble(false);
                                if (token == 'v')
                                    num += this._lastPoint.Y;
                                this._lastPoint.Y = num;
                                this.LineTo(this._lastPoint);
                            }
                            while (this.IsNumber(true));
                            continue;
                        case 'Z':
                            this.FinishFigure(true);
                            continue;
                        default:
                            throw new NotSupportedException();
                    }
                }
                this.FinishFigure(false);
            }

            private bool ReadToken()
            {
                this.SkipWhitespace(false);
                if (this._index >= this._length)
                    return false;
                this._token = this._buffer[this._index++];
                return true;
            }

            private Point ReadPoint(char command, bool allowComma)
            {
                double x = this.ReadDouble(allowComma);
                double y = this.ReadDouble(true);
                if (command >= 'a')
                {
                    x += this._lastPoint.X;
                    y += this._lastPoint.Y;
                }
                return new Point(x, y);
            }

            private Size ReadSize(bool allowComma)
               => new Size(this.ReadDouble(allowComma), this.ReadDouble(true));

            private bool ReadBool01(bool allowComma)
            {
                double num = this.ReadDouble(allowComma);
                if (num == 0.0)
                    return false;
                if (num == 1.0)
                    return true;
                throw new FormatException();
            }

            private double ReadDouble(bool allowComma)
            {
                if (!this.IsNumber(allowComma))
                    throw new FormatException();
                bool flag = true;
                int index = this._index;
                if (this._index < this._length && (this._buffer[this._index] == '-' || this._buffer[this._index] == '+'))
                    ++this._index;
                if (this._index < this._length && this._buffer[this._index] == 'I')
                {
                    this._index = Math.Min(this._index + 8, this._length);
                    flag = false;
                }
                else if (this._index < this._length && this._buffer[this._index] == 'N')
                {
                    this._index = Math.Min(this._index + 3, this._length);
                    flag = false;
                }
                else
                {
                    this.SkipDigits(false);
                    if (this._index < this._length && this._buffer[this._index] == '.')
                    {
                        flag = false;
                        ++this._index;
                        this.SkipDigits(false);
                    }
                    if (this._index < this._length && (this._buffer[this._index] == 'E' || this._buffer[this._index] == 'e'))
                    {
                        flag = false;
                        ++this._index;
                        this.SkipDigits(true);
                    }
                }
                if (flag && this._index <= index + 8)
                {
                    int num1 = 1;
                    if (this._buffer[index] == '+')
                        ++index;
                    else if (this._buffer[index] == '-')
                    {
                        ++index;
                        num1 = -1;
                    }
                    int num2 = 0;
                    for (; index < this._index; ++index)
                        num2 = num2 * 10 + (this._buffer[index] - 48);
                    return num2 * num1;
                }
                string str = this._buffer.Substring(index, this._index - index);
                try
                {
                    return Convert.ToDouble(str, CultureInfo.InvariantCulture);
                }
                catch (FormatException ex)
                {
                    throw ex;
                }
            }

            private void SkipDigits(bool signAllowed)
            {
                if (signAllowed && this._index < this._length && (this._buffer[this._index] == '-' || this._buffer[this._index] == '+'))
                    ++this._index;
                while (this._index < this._length && this._buffer[this._index] >= '0' && this._buffer[this._index] <= '9')
                    ++this._index;
            }

            private bool IsNumber(bool allowComma)
            {
                bool flag = this.SkipWhitespace(allowComma);
                if (this._index < this._length)
                {
                    this._token = this._buffer[this._index];
                    if (this._token == '.' || this._token == '-' || this._token == '+' || (this._token >= '0' && this._token <= '9' || (this._token == 'I' || this._token == 'N')))
                        return true;
                }
                if (flag)
                    throw new FormatException();
                return false;
            }

            private bool SkipWhitespace(bool allowComma)
            {
                bool flag = false;
                for (; this._index < this._length; ++this._index)
                {
                    char c = this._buffer[this._index];
                    switch (c)
                    {
                        case '\t':
                        case '\n':
                        case '\r':
                        case ' ':
                            continue;
                        case ',':
                            if (!allowComma)
                                throw new FormatException();
                            flag = true;
                            allowComma = false;
                            continue;
                        default:
                            if (c > ' ' && c <= 'z' || !char.IsWhiteSpace(c))
                                return flag;
                            continue;
                    }
                }
                return false;
            }

            private void BeginFigure(Point startPoint)
            {
                this.FinishFigure(false);
                this.EnsureFigure();
                this._figure.StartPoint = startPoint;
                this._figure.IsFilled = true;
            }

            private void EnsureFigure()
            {
                if (this._figure != null)
                    return;
                this._figure = new PathFigure
                {
                    Segments = new PathSegmentCollection()
                };
            }

            private void FinishFigure(bool figureExplicitlyClosed)
            {
                if (this._figure == null)
                    return;
                if (figureExplicitlyClosed)
                    this._figure.IsClosed = true;
                this._geometry.Figures.Add(this._figure);
                this._figure = null;
            }

            private void LineTo(Point point) => this._figure.Segments.Add(new LineSegment()
            {
                Point = point
            });

            private void BezierTo(Point point1, Point point2, Point point3) => this._figure.Segments.Add(new BezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            });

            private void QuadraticBezierTo(Point point1, Point point2) => this._figure.Segments.Add(new QuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2
            });

            private void ArcTo(
              Size size,
              double rotationAngle,
              bool isLargeArc,
              SweepDirection sweepDirection,
              Point point) => this._figure.Segments.Add(new ArcSegment()
              {
                  Size = size,
                  RotationAngle = rotationAngle,
                  IsLargeArc = isLargeArc,
                  SweepDirection = sweepDirection,
                  Point = point
              });

            private Point GetSmoothBeizerFirstPoint()
            {
                Point lastPoint = this._lastPoint;
                if (this._figure.Segments.Count > 0 && this._figure.Segments[this._figure.Segments.Count - 1] is BezierSegment segment)
                {
                    Point point2 = segment.Point2;
                    lastPoint.X += this._lastPoint.X - point2.X;
                    lastPoint.Y += this._lastPoint.Y - point2.Y;
                }
                return lastPoint;
            }
        }
    }
}
