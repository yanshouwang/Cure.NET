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
using System.Linq;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 要将 ArcSegment 转换成 BezierSegment 的帮助程序类。
    /// 要使用 PathSegment 和所有变体的帮助程序类。
    /// 用于处理不同类型的 PathSegment 的策略类。
    /// </summary>
    static class PathSegmentUtil
    {
        /// <summary>
        /// 将弧段转换成贝塞尔格式。返回 BezierSegment、PolyBezierSegment、LineSegment 或 null。返回 null 时，弧将退化为起点。
        /// </summary>
        public static PathSegment ArcToBezierSegments(ArcSegment arcSegment, Point startPoint)
        {
            var stroked = arcSegment.IsStroked;
            ArcToBezierUtil.ArcToBezier(startPoint.X, startPoint.Y, arcSegment.Size.Width, arcSegment.Size.Height, arcSegment.RotationAngle, arcSegment.IsLargeArc, arcSegment.SweepDirection == SweepDirection.Clockwise, arcSegment.Point.X, arcSegment.Point.Y, out var pPt, out var cPieces);
            return cPieces switch
            {
                -1 => null,
                0 => CreateLineSegment(arcSegment.Point, stroked),
                1 => CreateBezierSegment(pPt[0], pPt[1], pPt[2], stroked),
                _ => CreatePolyBezierSegment(pPt, 0, cPieces * 3, stroked),
            };
        }

        public static LineSegment CreateLineSegment(Point point, bool stroked = true)
        {
            var segment = new LineSegment { Point = point };
            segment.SetIsStroked(stroked);
            return segment;
        }

        public static QuadraticBezierSegment CreateQuadraticBezierSegment(Point point1, Point point2, bool stroked = true)
        {
            var segment = new QuadraticBezierSegment
            {
                Point1 = point1,
                Point2 = point2
            };
            segment.SetIsStroked(stroked);
            return segment;
        }

        public static BezierSegment CreateBezierSegment(Point point1, Point point2, Point point3, bool stroked = true)
        {
            BezierSegment segment = new BezierSegment
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3
            };
            segment.SetIsStroked(stroked);
            return segment;
        }

        public static PolyBezierSegment CreatePolyBezierSegment(IList<Point> points, int start, int count, bool stroked = true)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            count = count / 3 * 3;
            if (count < 0 || points.Count < start + count)
                throw new ArgumentOutOfRangeException(nameof(count));
            var segment = new PolyBezierSegment { Points = new PointCollection() };
            for (var index = 0; index < count; ++index)
                segment.Points.Add(points[start + index]);
            segment.SetIsStroked(stroked);
            return segment;
        }

        public static PolyQuadraticBezierSegment CreatePolyQuadraticBezierSegment(IList<Point> points, int start, int count, bool stroked = true)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            count = count / 2 * 2;
            if (count < 0 || points.Count < start + count)
                throw new ArgumentOutOfRangeException(nameof(count));
            var segment = new PolyQuadraticBezierSegment { Points = new PointCollection() };
            for (var index = 0; index < count; ++index)
                segment.Points.Add(points[start + index]);
            segment.SetIsStroked(stroked);
            return segment;
        }

        public static PolyLineSegment CreatePolylineSegment(IList<Point> points, int start, int count, bool stroked = true)
        {
            if (count < 0 || points.Count < start + count)
                throw new ArgumentOutOfRangeException(nameof(count));
            var segment = new PolyLineSegment { Points = new PointCollection() };
            for (var index = 0; index < count; ++index)
                segment.Points.Add(points[start + index]);
            segment.SetIsStroked(stroked);
            return segment;
        }

        public static ArcSegment CreateArcSegment(Point point, Size size, bool largeArc, bool clockwise, double rotationAngle = 0.0, bool stroked = true)
        {
            ArcSegment arcSegment = new ArcSegment();
            arcSegment.SetIfDifferent(ArcSegment.PointProperty, point);
            arcSegment.SetIfDifferent(ArcSegment.SizeProperty, size);
            arcSegment.SetIfDifferent(ArcSegment.IsLargeArcProperty, largeArc);
            arcSegment.SetIfDifferent(ArcSegment.SweepDirectionProperty, (SweepDirection)(clockwise ? 1 : 0));
            arcSegment.SetIfDifferent(ArcSegment.RotationAngleProperty, rotationAngle);
            arcSegment.SetIsStroked(stroked);
            return arcSegment;
        }

        /// <summary>
        /// 用与给定点列表匹配的给定折线更新 SegmentCollection。尝试使更改保持最小程度，如果未进行任何更改，则返回 false。
        /// </summary>
        public static bool SyncPolylineSegment(PathSegmentCollection collection, int index, IList<Point> points, int start, int count)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (points.Count < start + count)
                throw new ArgumentOutOfRangeException(nameof(count));
            var flag1 = false;
            if (!(collection[index] is PolyLineSegment polyLineSegment))
            {
                collection[index] = polyLineSegment = new PolyLineSegment();
                flag1 = true;
            }
            var flag2 = flag1 | polyLineSegment.Points.EnsureListCount(count);
            for (var index1 = 0; index1 < count; ++index1)
            {
                if (polyLineSegment.Points[index1] != points[index1 + start])
                {
                    polyLineSegment.Points[index1] = points[index1 + start];
                    flag2 = true;
                }
            }
            return flag2;
        }

        /// <summary>
        /// 用与给定点列表匹配的多贝塞尔线段更新 collection[index] 线段。给定点列表必须包含 N 个贝塞尔线段的 3*N 个点。
        /// </summary>
        public static bool SyncPolyBezierSegment(PathSegmentCollection collection, int index, IList<Point> points, int start, int count)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (index < 0 || index >= collection.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (points.Count < start + count)
                throw new ArgumentOutOfRangeException(nameof(count));
            var flag = false;
            count = count / 3 * 3;
            if (!(collection[index] is PolyBezierSegment polyBezierSegment))
            {
                collection[index] = polyBezierSegment = new PolyBezierSegment();
                flag = true;
            }
            polyBezierSegment.Points.EnsureListCount(count);
            for (int index1 = 0; index1 < count; ++index1)
            {
                if (polyBezierSegment.Points[index1] != points[index1 + start])
                {
                    polyBezierSegment.Points[index1] = points[index1 + start];
                    flag = true;
                }
            }
            return flag;
        }

        static class ArcToBezierUtil
        {
            /// <summary>
            /// ArcToBezier，计算弧的贝塞尔近似值。
            /// </summary>
            /// <remarks>
            /// 此实用工具计算 SVG 弧详细说明中定义的椭圆弧的贝塞尔近似值。从中划分弧的椭圆在其自己的坐标系中与轴平行，并通过其 x 和 y 半径来定义。旋转角度定义了椭圆的轴相对于 x 轴旋转的角度。起点和终点定义了 4 个可能的弧中的一个弧；扫描和大弧标志决定了将选择这些弧中的哪一个。
            /// 如果返回 cPieces = 0，则表示一条线而不是弧；如果返回 cPieces = -1，则表示弧退化为了点
            /// </remarks>
            public static void ArcToBezier(
                double xStart,
                double yStart,
                double xRadius,
                double yRadius,
                double rRotation,
                bool fLargeArc,
                bool fSweepUp,
                double xEnd,
                double yEnd,
                out Point[] pPt,
                out int cPieces)
            {
                var num1 = 1E-06;
                pPt = new Point[12];
                var rFuzz2 = num1 * num1;
                var flag = false;
                cPieces = -1;
                var num2 = 0.5 * (xEnd - xStart);
                var num3 = 0.5 * (yEnd - yStart);
                var rHalfChord2 = num2 * num2 + num3 * num3;
                if (rHalfChord2 < rFuzz2)
                    return;
                if (!AcceptRadius(rHalfChord2, rFuzz2, ref xRadius) || !AcceptRadius(rHalfChord2, rFuzz2, ref yRadius))
                {
                    cPieces = 0;
                }
                else
                {
                    double num4;
                    double num5;
                    if (Math.Abs(rRotation) < num1)
                    {
                        num4 = 1.0;
                        num5 = 0.0;
                    }
                    else
                    {
                        rRotation = -rRotation * Math.PI / 180.0;
                        num4 = Math.Cos(rRotation);
                        num5 = Math.Sin(rRotation);
                        var num6 = num2 * num4 - num3 * num5;
                        num3 = num2 * num5 + num3 * num4;
                        num2 = num6;
                    }
                    var num7 = num2 / xRadius;
                    var num8 = num3 / yRadius;
                    var d = num7 * num7 + num8 * num8;
                    double num9;
                    double num10;
                    if (d > 1.0)
                    {
                        var num6 = Math.Sqrt(d);
                        xRadius *= num6;
                        yRadius *= num6;
                        num10 = num9 = 0.0;
                        flag = true;
                        num7 /= num6;
                        num8 /= num6;
                    }
                    else
                    {
                        var num6 = Math.Sqrt((1.0 - d) / d);
                        if (fLargeArc != fSweepUp)
                        {
                            num10 = -num6 * num8;
                            num9 = num6 * num7;
                        }
                        else
                        {
                            num10 = num6 * num8;
                            num9 = -num6 * num7;
                        }
                    }
                    var point1 = new Point(-num7 - num10, -num8 - num9);
                    var point2 = new Point(num7 - num10, num8 - num9);
                    var matrix = new Matrix(num4 * xRadius, -num5 * xRadius, num5 * yRadius, num4 * yRadius, 0.5 * (xEnd + xStart), 0.5 * (yEnd + yStart));
                    if (!flag)
                    {
                        matrix.OffsetX += matrix.M11 * num10 + matrix.M21 * num9;
                        matrix.OffsetY += matrix.M12 * num10 + matrix.M22 * num9;
                    }
                    GetArcAngle(point1, point2, fLargeArc, fSweepUp, out var rCosArcAngle, out var rSinArcAngle, out cPieces);
                    var num11 = GetBezierDistance(rCosArcAngle);
                    if (!fSweepUp)
                        num11 = -num11;
                    var rhs1 = new Point(-num11 * point1.Y, num11 * point1.X);
                    var num12 = 0;
                    pPt = new Point[cPieces * 3];
                    Point rhs2;
                    for (var index1 = 1; index1 < cPieces; ++index1)
                    {
                        var point3 = new Point(point1.X * rCosArcAngle - point1.Y * rSinArcAngle, point1.X * rSinArcAngle + point1.Y * rCosArcAngle);
                        rhs2 = new Point(-num11 * point3.Y, num11 * point3.X);
                        var pointArray1 = pPt;
                        var index2 = num12;
                        var num6 = index2 + 1;
                        pointArray1[index2] = matrix.Transform(point1.Plus(rhs1));
                        var pointArray2 = pPt;
                        var index3 = num6;
                        var num13 = index3 + 1;
                        pointArray2[index3] = matrix.Transform(point3.Minus(rhs2));
                        var pointArray3 = pPt;
                        var index4 = num13;
                        num12 = index4 + 1;
                        pointArray3[index4] = matrix.Transform(point3);
                        point1 = point3;
                        rhs1 = rhs2;
                    }
                    rhs2 = new Point(-num11 * point2.Y, num11 * point2.X);
                    var pointArray4 = pPt;
                    var index5 = num12;
                    var num14 = index5 + 1;
                    pointArray4[index5] = matrix.Transform(point1.Plus(rhs1));
                    var pointArray5 = pPt;
                    var index6 = num14;
                    var index7 = index6 + 1;
                    pointArray5[index6] = matrix.Transform(point2.Minus(rhs2));
                    pPt[index7] = new Point(xEnd, yEnd);
                }
            }

            /// <summary>
            /// 获取贝塞尔弧的数量以及每个弧的正弦值/余弦值。
            /// </summary>
            /// <remarks>
            /// 这是 ArcToBezier 使用的专用实用工具。将弧分为若干段，使任何一段弧扫过的角度都不超过 90 度。输入点在单位圆上。
            /// </remarks>
            static void GetArcAngle(
                Point ptStart,
                Point ptEnd,
                bool fLargeArc,
                bool fSweepUp,
                out double rCosArcAngle,
                out double rSinArcAngle,
                out int cPieces)
            {
                rCosArcAngle = GeometryUtil.Dot(ptStart, ptEnd);
                rSinArcAngle = GeometryUtil.Determinant(ptStart, ptEnd);
                if (rCosArcAngle >= 0.0)
                {
                    if (fLargeArc)
                    {
                        cPieces = 4;
                    }
                    else
                    {
                        cPieces = 1;
                        return;
                    }
                }
                else
                    cPieces = !fLargeArc ? 2 : 3;
                var num1 = Math.Atan2(rSinArcAngle, rCosArcAngle);
                if (fSweepUp)
                {
                    if (num1 < 0.0)
                        num1 += 2.0 * Math.PI;
                }
                else if (num1 > 0.0)
                    num1 -= 2.0 * Math.PI;
                var num2 = num1 / (double)cPieces;
                rCosArcAngle = Math.Cos(num2);
                rSinArcAngle = Math.Sin(num2);
            }

            /// <summary>
            /// GetBezierDistance 返回的距离为半径的一小部分。
            /// </summary>
            /// <remarks>
            /// 获取从圆弧终点到与该圆弧近似的贝塞尔弧的控制点之间的距离（弧半径的一小部分）。
            /// 
            /// 由于结果是相对于弧半径的一个值，所以它完全取决于弧的角度。由于假设弧为 90 度或不到 90 度，因此该角度由其余弦来决定，这是从 rDot = 两个半径矢量的点积中推导出来的。我们需要在终点和中点处与弧的点和切线保持一致的贝塞尔曲线。在此，我们将计算曲线的终点到其控制点之间的距离。
            /// 
            /// 由于需要的是相对距离，所以可以使用单位圆。将圆心放在原点处，并使 X 轴成为这两个矢量的平分线。让 a 成为矢量夹角。那么第一个点和最后一个点的 X 坐标为 cos(a/2)。让 x 成为第二个点和第三个点的 X 坐标。当 t = 1/2 时，有一个点在 (1,0) 处。但是此处的多项式的项都相等：
            /// 
            /// (1-t)^3 = t*(1-t)^2 = t^2*(1-t) = t^3 = 1/8，
            /// 
            /// 因此，从贝塞尔公式中可以得出：
            /// 
            /// 1 = (1/8) * (cos(a/2) + 3x + 3x + cos(a/2))，
            /// 
            /// 因此
            /// 
            /// x = (4 - cos(a/2)) / 3
            /// 
            /// 这与第一个点之间的 X 差值为：
            /// 
            /// DX = x - cos(a/2) = 4(1 - cos(a/2)) / 3。
            /// 
            /// 但是 DX = 距离 / sin(a/2)，因此距离为
            /// 
            /// dist = (4/3)*(1 - cos(a/2)) / sin(a/2)。
            /// 
            /// 由于给出的是 rDot = R^2 * cos(a)，而不是角 a，因此应将最大值和最小值乘以 R：
            /// 
            /// dist = (4/3)*(R - Rcos(a/2)) / Rsin(a/2)
            /// 
            /// 并使用以下三角公式：________________ cos(a/2)   = \/(1 + cos(a)) / 2 ______________________ R*cos(a/2) = \/(R^2 + R^2 cos(a)) / 2 ________________ = \/(R^2 + rDot) / 2
            /// 
            /// 假设 A = (R^2 + rDot)/2。____________________ R*sin(a/2) = \/R^2 - R^2 cos^2(a/2) _______ = \/R^2 - A
            /// 
            /// 因此：_ 4      R - \/A dist = - * ------------ 3      _______ \/R^2 - A
            /// 
            /// 历史记录：2001-5-29，其创建者为 MichKa。
            /// </remarks>
            static double GetBezierDistance(double rDot, double rRadius = 1.0)
            {
                var num1 = rRadius * rRadius;
                var num2 = 0.0;
                var d1 = 0.5 * (num1 + rDot);
                if (d1 >= 0.0)
                {
                    var d2 = num1 - d1;
                    if (d2 > 0.0)
                    {
                        var num3 = Math.Sqrt(d2);
                        var num4 = 4.0 * (rRadius - Math.Sqrt(d1)) / 3.0;
                        num2 = num4 > num3 * 1E-06 ? num4 / num3 : 0.0;
                    }
                }
                return num2;
            }

            /// <summary>
            /// 如果半径与弦长相比太小，则返回 false（针对 NaN 会返回 true），并且半径会修改为接受的值。
            /// </summary>
            static bool AcceptRadius(double rHalfChord2, double rFuzz2, ref double rRadius)
            {
                var flag = rRadius * rRadius > rHalfChord2 * rFuzz2;
                if (flag && rRadius < 0.0)
                    rRadius = -rRadius;
                return flag;
            }
        }

        public abstract class PathSegmentImplementation
        {
            public Point Start { get; private set; }

            public abstract void Flatten(IList<Point> points, double tolerance);

            public abstract Point GetPoint(int index);

            public abstract IEnumerable<SimpleSegment> GetSimpleSegments();

            public static PathSegmentImplementation Create(PathSegment segment, Point start)
            {
                var segmentImplementation = Create(segment);
                segmentImplementation.Start = start;
                return segmentImplementation;
            }

            public static PathSegmentImplementation Create(PathSegment segment)
            {
                var segmentImplementation = BezierSegmentImplementation.Create(segment as BezierSegment);
                if (segmentImplementation == null)
                {
                    segmentImplementation = LineSegmentImplementation.Create(segment as LineSegment);
                }
                if (segmentImplementation == null)
                {
                    segmentImplementation = ArcSegmentImplementation.Create(segment as ArcSegment);
                }
                if (segmentImplementation == null)
                {
                    segmentImplementation = PolyLineSegmentImplementation.Create(segment as PolyLineSegment);
                }
                if (segmentImplementation == null)
                {
                    segmentImplementation = PolyBezierSegmentImplementation.Create(segment as PolyBezierSegment);
                }
                if (segmentImplementation == null)
                {
                    segmentImplementation = QuadraticBezierSegmentImplementation.Create(segment as QuadraticBezierSegment);
                }
                if (segmentImplementation == null)
                {
                    segmentImplementation = PolyQuadraticBezierSegmentImplementation.Create(segment as PolyQuadraticBezierSegment);
                }
                if (segmentImplementation == null)
                {
                    throw new NotImplementedException();
                }
                return segmentImplementation;
            }
        }

        class BezierSegmentImplementation : PathSegmentImplementation
        {
            BezierSegment _segment;

            public static PathSegmentImplementation Create(BezierSegment source)
            {
                if (source == null)
                    return null;
                return new BezierSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                Point[] controlPoints = new Point[4]
                {
                    Start,
                    _segment.Point1,
                    _segment.Point2,
                    _segment.Point3
                };
                var points1 = new List<Point>();
                BezierCurveFlattener.FlattenCubic(controlPoints, tolerance, points1, true);
                points.AddRange(points1);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 2)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (index == 0)
                    return _segment.Point1;
                return index == 1 ? _segment.Point2 : _segment.Point3;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                yield return SimpleSegment.Create(Start, _segment.Point1, _segment.Point2, _segment.Point3);
            }
        }

        class QuadraticBezierSegmentImplementation : PathSegmentImplementation
        {
            QuadraticBezierSegment _segment;

            public static PathSegmentImplementation Create(QuadraticBezierSegment source)
            {
                if (source == null)
                    return null;
                return new QuadraticBezierSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                var controlPoints = new Point[3]
                {
                    Start,
                    _segment.Point1,
                    _segment.Point2
                };
                var points1 = new List<Point>();
                BezierCurveFlattener.FlattenQuadratic(controlPoints, tolerance, points1, true);
                points.AddRange(points1);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return index == 0 ? _segment.Point1 : _segment.Point2;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                yield return SimpleSegment.Create(Start, _segment.Point1, _segment.Point2);
            }
        }

        class PolyBezierSegmentImplementation : PathSegmentImplementation
        {
            private PolyBezierSegment _segment;

            public static PathSegmentImplementation Create(PolyBezierSegment source)
            {
                if (source == null)
                    return null;
                return new PolyBezierSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                var point = Start;
                var num = _segment.Points.Count / 3 * 3;
                for (var index = 0; index < num; index += 3)
                {
                    var controlPoints = new Point[4]
                    {
                        point,
                        _segment.Points[index],
                        _segment.Points[index + 1],
                        _segment.Points[index + 2]
                    };
                    var points1 = new List<Point>();
                    BezierCurveFlattener.FlattenCubic(controlPoints, tolerance, points1, true);
                    points.AddRange(points1);
                    point = _segment.Points[index + 2];
                }
            }

            public override Point GetPoint(int index)
            {
                var num = _segment.Points.Count / 3 * 3;
                if (index < -1 || index > num - 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return index != -1 ? _segment.Points[index] : _segment.Points[num - 1];
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                var point0 = Start;
                var points = _segment.Points;
                var count = _segment.Points.Count / 3;
                for (var i = 0; i < count; ++i)
                {
                    var i3 = i * 3;
                    yield return SimpleSegment.Create(point0, points[i3], points[i3 + 1], points[i3 + 2]);
                    point0 = points[i3 + 2];
                }
            }
        }

        class PolyQuadraticBezierSegmentImplementation : PathSegmentImplementation
        {
            PolyQuadraticBezierSegment _segment;

            public static PathSegmentImplementation Create(PolyQuadraticBezierSegment source)
            {
                if (source == null)
                    return null;
                return new PolyQuadraticBezierSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                var point = Start;
                var num = _segment.Points.Count / 2 * 2;
                for (var index = 0; index < num; index += 2)
                {
                    var controlPoints = new Point[3]
                    {
                        point,
                        _segment.Points[index],
                        _segment.Points[index + 1]
                    };
                    var pointList = new List<Point>();
                    BezierCurveFlattener.FlattenQuadratic(controlPoints, tolerance, pointList, true);
                    points.AddRange(pointList);
                    point = _segment.Points[index + 1];
                }
            }

            public override Point GetPoint(int index)
            {
                var num = _segment.Points.Count / 2 * 2;
                if (index < -1 || index > num - 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return index != -1 ? _segment.Points[index] : _segment.Points[num - 1];
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                var point0 = Start;
                var points = _segment.Points;
                var count = _segment.Points.Count / 2;
                for (var i = 0; i < count; ++i)
                {
                    var i2 = i * 2;
                    yield return SimpleSegment.Create(point0, points[i2], points[i2 + 1]);
                    point0 = points[i2 + 1];
                }
            }
        }

        class ArcSegmentImplementation : PathSegmentImplementation
        {
            ArcSegment _segment;

            public static PathSegmentImplementation Create(ArcSegment source)
            {
                if (source == null)
                    return null;
                return new ArcSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
            {
                var bezierSegments = ArcToBezierSegments(_segment, Start);
                if (bezierSegments == null)
                    return;
                bezierSegments.FlattenSegment(points, Start, tolerance);
            }

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _segment.Point;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                var bezierSegments = ArcToBezierSegments(_segment, Start);
                return bezierSegments != null
                    ? bezierSegments.GetSimpleSegments(Start)
                    : Enumerable.Empty<SimpleSegment>();
            }
        }

        class LineSegmentImplementation : PathSegmentImplementation
        {
            LineSegment _segment;

            public static PathSegmentImplementation Create(LineSegment source)
            {
                if (source == null)
                    return null;
                return new LineSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
                => points.Add(_segment.Point);

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _segment.Point;
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                yield return SimpleSegment.Create(Start, _segment.Point);
            }
        }

        class PolyLineSegmentImplementation : PathSegmentImplementation
        {
            PolyLineSegment _segment;

            public static PathSegmentImplementation Create(PolyLineSegment source)
            {
                if (source == null)
                    return null;
                return new PolyLineSegmentImplementation() { _segment = source };
            }

            public override void Flatten(IList<Point> points, double tolerance)
                => points.AddRange(_segment.Points);

            public override Point GetPoint(int index)
            {
                if (index < -1 || index > _segment.Points.Count - 1)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return index != -1 ? _segment.Points[index] : _segment.Points.Last();
            }

            public override IEnumerable<SimpleSegment> GetSimpleSegments()
            {
                var point0 = Start;
                foreach (var point in _segment.Points)
                {
                    yield return SimpleSegment.Create(point0, point);
                    point0 = point;
                }
            }
        }
    }
}