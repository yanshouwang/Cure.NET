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
    class ArcGeometrySource : GeometrySource<IArcGeometrySourceParameters>
    {
        double _relativeThickness;
        double _absoluteThickness;

        /// <summary>
        /// 假定纵横比为 1:1，则弧会认为 Stretch.None 与 Stretch.Fill 相同。
        /// </summary>
        protected override Rect ComputeLogicalBounds(Rect layoutBounds, IGeometrySourceParameters parameters)
            => GeometryUtil.GetStretchBound(base.ComputeLogicalBounds(layoutBounds, parameters), parameters.Stretch, new Size(1.0, 1.0));

        /// <summary>
        /// 使相对于范围框的粗细以及大小为绝对像素的粗细标准化。相对粗细 = 0 -&gt; 全圆半径或固定值。相对粗细 = 1 -&gt; 缩小为点或已退化。
        /// </summary>
        void NormalizeThickness(IArcGeometrySourceParameters parameters)
        {
            var rhs = Math.Min(LogicalBounds.Width / 2.0, LogicalBounds.Height / 2.0);
            var lhs = parameters.ArcThickness;
            if (parameters.ArcThicknessUnit == UnitType.Pixel)
                lhs = MathUtil.SafeDivide(lhs, rhs, 0.0);
            _relativeThickness = MathUtil.EnsureRange(lhs, 0.0, 1.0);
            _absoluteThickness = rhs * _relativeThickness;
        }

        /// <summary>
        /// 弧退化为指向内心/内部法线的线条。
        /// </summary>
        bool UpdateZeroAngleGeometry(bool relativeMode, double angle)
        {
            var flag = false;
            var arcPoint = GeometryUtil.GetArcPoint(angle, LogicalBounds);
            var logicalBounds = LogicalBounds;
            var radiusX = logicalBounds.Width / 2.0;
            var radiusY = logicalBounds.Height / 2.0;
            Point point;
            if (relativeMode || MathUtil.AreClose(radiusX, radiusY))
            {
                var bound = LogicalBounds.Resize(1.0 - _relativeThickness);
                point = GeometryUtil.GetArcPoint(angle, bound);
            }
            else
            {
                var intersect = InnerCurveSelfIntersect(radiusX, radiusY, _absoluteThickness);
                var angleRanges = ComputeAngleRanges(radiusX, radiusY, intersect, angle, angle);
                var num = angleRanges[0] * Math.PI / 180.0;
                var vector = new Vector(radiusY * Math.Sin(num), -radiusX * Math.Cos(num));
                point = GeometryUtil.GetArcPoint(angleRanges[0], LogicalBounds) - vector.Normalized() * _absoluteThickness;
            }
            var cachedGeometry = CachedGeometry;
            var flag1 = GeometryUtil.EnsureGeometryType(out var result, ref cachedGeometry, () => new LineGeometry());
            CachedGeometry = cachedGeometry;
            var flag2 = result.SetIfDifferent(LineGeometry.StartPointProperty, arcPoint);
            var flag3 = result.SetIfDifferent(LineGeometry.EndPointProperty, point);
            return flag | flag1 | flag2 | flag3;
        }

        bool UpdateEllipseGeometry(bool filled)
        {
            var flag1 = false;
            var y = MathUtil.Lerp(LogicalBounds.Top, LogicalBounds.Bottom, 0.5);
            var point1 = new Point(LogicalBounds.Left, y);
            var point2 = new Point(LogicalBounds.Right, y);
            var cachedGeometry = CachedGeometry;
            var flag21 = GeometryUtil.EnsureGeometryType(out var result1, ref cachedGeometry, () => new PathGeometry());
            CachedGeometry = cachedGeometry;
            var flag22 = result1.Figures.EnsureListCount(1, () => new PathFigure());
            var flag2 = flag1 | flag21 | flag22;
            var figure = result1.Figures[0];
            var flag31 = figure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            var flag32 = figure.SetIfDifferent(PathFigure.IsFilledProperty, filled);
            var flag33 = figure.Segments.EnsureListCount(2, () => new ArcSegment());
            var flag34 = figure.SetIfDifferent(PathFigure.StartPointProperty, point1);
            var flag35 = GeometryUtil.EnsureSegmentType(out var result2, figure.Segments, 0, () => new ArcSegment());
            var flag36 = GeometryUtil.EnsureSegmentType(out var result3, figure.Segments, 1, () => new ArcSegment());
            var flag3 = flag2 | flag31 | flag32 | flag33 | flag34 | flag35 | flag36;
            var size = new Size(LogicalBounds.Width / 2.0, LogicalBounds.Height / 2.0);
            var flag41 = result2.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            var flag42 = result2.SetIfDifferent(ArcSegment.SizeProperty, size);
            var flag43 = result2.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            var flag44 = result2.SetIfDifferent(ArcSegment.PointProperty, point2);
            var flag45 = result3.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            var flag46 = result3.SetIfDifferent(ArcSegment.SizeProperty, size);
            var flag47 = result3.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            var flag48 = result3.SetIfDifferent(ArcSegment.PointProperty, point1);
            return flag3 | flag41 | flag42 | flag43 | flag44 | flag45 | flag46 | flag47 | flag48;
        }

        bool UpdateFullRingGeometry(bool relativeMode)
        {
            var cachedGeometry = CachedGeometry;
            var flag11 = GeometryUtil.EnsureGeometryType(out var result, ref cachedGeometry, () => new PathGeometry());
            CachedGeometry = cachedGeometry;
            var flag12 = result.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.EvenOdd);
            var flag13 = result.Figures.EnsureListCount(2, () => new PathFigure());
            var flag14 = PathFigureUtil.SyncEllipseFigure(result.Figures[0], LogicalBounds, SweepDirection.Clockwise);
            var flag1 = false | flag11 | flag12 | flag13 | flag14;
            var logicalBounds = LogicalBounds;
            var radiusX = logicalBounds.Width / 2.0;
            var radiusY = logicalBounds.Height / 2.0;
            bool flag2;
            if (relativeMode || MathUtil.AreClose(radiusX, radiusY))
            {
                var bounds = this.LogicalBounds.Resize(1.0 - this._relativeThickness);
                flag2 = flag1 | PathFigureUtil.SyncEllipseFigure(result.Figures[1], bounds, SweepDirection.Counterclockwise);
            }
            else
            {
                var flag3 = flag1 | result.Figures[1].SetIfDifferent(PathFigure.IsClosedProperty, true) | result.Figures[1].SetIfDifferent(PathFigure.IsFilledProperty, (object)true);
                var firstPoint = new Point();
                var intersect = InnerCurveSelfIntersect(radiusX, radiusY, _absoluteThickness);
                var angleRanges = ComputeAngleRanges(radiusX, radiusY, intersect, 360.0, 0.0);
                flag2 = flag3 | SyncPieceWiseInnerCurves(result.Figures[1], 0, ref firstPoint, angleRanges) | result.Figures[1].SetIfDifferent(PathFigure.StartPointProperty, (object)firstPoint);
            }
            return flag2;
        }

        static void IncreaseDuplicatedIndex(IList<double> values, ref int index)
        {
            while (index < values.Count - 1 && values[index] == values[index + 1])
                ++index;
        }

        static void DecreaseDuplicatedIndex(IList<double> values, ref int index)
        {
            while (index > 0 && values[index] == values[index - 1])
                --index;
        }

        /// <summary>
        /// 计算角对的列表，并定义应在其中查找弧示例的范围。返回值具有 2、4 或 6 对值，每对值都定义了一个范围，并且它们按顺序从给定的起始角向终止角跨越。在自相交角处将会超出范围。如果起始/终止输入在自相交角之间的无效范围内，则它将移到邻近的自相交范围内。
        /// </summary>
        internal static double[] ComputeAngleRanges(
            double radiusX,
            double radiusY,
            double intersect,
            double start,
            double end)
        {
            var doubleList = new List<double>()
            {
                start,
                end,
                intersect,
                180.0 - intersect,
                180.0 + intersect,
                360.0 - intersect,
                360.0 + intersect,
                540.0 - intersect,
                540.0 + intersect,
                720.0 - intersect
            };
            doubleList.Sort();
            var index1 = doubleList.IndexOf(start);
            var index2 = doubleList.IndexOf(end);
            if (index2 == index1)
                ++index2;
            else if (start < end)
            {
                IncreaseDuplicatedIndex(doubleList, ref index1);
                DecreaseDuplicatedIndex(doubleList, ref index2);
            }
            else if (start > end)
            {
                DecreaseDuplicatedIndex(doubleList, ref index1);
                IncreaseDuplicatedIndex(doubleList, ref index2);
            }
            var list = new List<double>();
            if (index1 < index2)
            {
                for (var index3 = index1; index3 <= index2; ++index3)
                    list.Add(doubleList[index3]);
            }
            else
            {
                for (var index3 = index1; index3 >= index2; --index3)
                    list.Add(doubleList[index3]);
            }
            var num = EnsureFirstQuadrant((list[0] + list[1]) / 2.0);
            if (radiusX < radiusY && num < intersect || radiusX > radiusY && num > intersect)
                list.RemoveAt(0);
            if (list.Count % 2 == 1)
                list.RemoveLast();
            if (list.Count == 0)
            {
                var index3 = Math.Min(index1, index2) - 1;
                if (index3 < 0)
                    index3 = Math.Max(index1, index2) + 1;
                list.Add(doubleList[index3]);
                list.Add(doubleList[index3]);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 将角移到 0-90 范围内。
        /// </summary>
        internal static double EnsureFirstQuadrant(double angle)
        {
            angle = Math.Abs(angle % 180.0);
            return angle <= 90.0 ? angle : 180.0 - angle;
        }

        bool UpdatePieGeometry(double start, double end)
        {
            var flag = false;
            PathFigure dependencyObject1;
            if (!(CachedGeometry is PathGeometry cachedGeometry) || cachedGeometry.Figures.Count != 1 || (dependencyObject1 = cachedGeometry.Figures[0]).Segments.Count != 2 || !(dependencyObject1.Segments[0] is ArcSegment dependencyObject2) || !(dependencyObject1.Segments[1] is LineSegment dependencyObject3))
            {
                ((PathGeometry)(CachedGeometry = new PathGeometry())).Figures.Add(dependencyObject1 = new PathFigure());
                dependencyObject2 = new ArcSegment() { SweepDirection = SweepDirection.Clockwise };
                dependencyObject1.Segments.Add(dependencyObject2);
                dependencyObject3 = new LineSegment();
                dependencyObject1.Segments.Add(dependencyObject3);
                flag = true;
            }
            return flag | dependencyObject1.SetIfDifferent(PathFigure.StartPointProperty, GeometryUtil.GetArcPoint(start, LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(end, LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0) | dependencyObject2.SetIfDifferent(LineSegment.PointProperty, LogicalBounds.Center());
        }

        bool UpdateOpenArcGeometry(double start, double end)
        {
            var flag = false;
            PathFigure dependencyObject1;
            if (!(CachedGeometry is PathGeometry cachedGeometry) || cachedGeometry.Figures.Count != 1 || ((dependencyObject1 = cachedGeometry.Figures[0]).Segments.Count != 1 || !(dependencyObject1.Segments[0] is ArcSegment dependencyObject2)))
            {
                ((PathGeometry)(CachedGeometry = new PathGeometry())).Figures.Add(dependencyObject1 = new PathFigure());
                dependencyObject1.Segments.Add(dependencyObject2 = new ArcSegment());
                dependencyObject1.IsClosed = false;
                dependencyObject2.SweepDirection = SweepDirection.Clockwise;
                flag = true;
            }
            return flag | dependencyObject1.SetIfDifferent(PathFigure.StartPointProperty, GeometryUtil.GetArcPoint(start, LogicalBounds)) | dependencyObject1.SetIfDifferent(PathFigure.IsFilledProperty, false) | dependencyObject2.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(end, LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0);
        }

        bool UpdateRingArcGeometry(bool relativeMode, double start, double end)
        {
            var cachedGeometry = CachedGeometry;
            var flag11 = GeometryUtil.EnsureGeometryType(out var result1, ref cachedGeometry, () => new PathGeometry());
            CachedGeometry = cachedGeometry;
            var flag12 = result1.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.Nonzero);
            var flag13 = result1.Figures.EnsureListCount(1, () => new PathFigure());
            var flag1 = false | flag11 | flag12 | flag13;
            var figure = result1.Figures[0];
            var flag2 = flag1 | figure.SetIfDifferent(PathFigure.IsClosedProperty, true) | figure.SetIfDifferent(PathFigure.IsFilledProperty, true) | figure.SetIfDifferent(PathFigure.StartPointProperty, (object)GeometryUtil.GetArcPoint(start, this.LogicalBounds)) | figure.Segments.EnsureListCountAtLeast(3, () => new ArcSegment()) | GeometryUtil.EnsureSegmentType(out var result2, figure.Segments, 0, () => new ArcSegment()) | result2.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(end, LogicalBounds)) | result2.SetIfDifferent(ArcSegment.SizeProperty, new Size(LogicalBounds.Width / 2.0, LogicalBounds.Height / 2.0)) | result2.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0) | result2.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise) | GeometryUtil.EnsureSegmentType(out var result3, figure.Segments, 1, () => new LineSegment());
            var logicalBounds = LogicalBounds;
            var radiusX = logicalBounds.Width / 2.0;
            var radiusY = logicalBounds.Height / 2.0;
            bool flag3;
            if (relativeMode || MathUtil.AreClose(radiusX, radiusY))
            {
                var bound = LogicalBounds.Resize(1.0 - _relativeThickness);
                flag3 = flag2 | result3.SetIfDifferent(LineSegment.PointProperty, (object)GeometryUtil.GetArcPoint(end, bound)) | figure.Segments.EnsureListCount(3, () => new ArcSegment()) | GeometryUtil.EnsureSegmentType(out var result4, figure.Segments, 2, () => new ArcSegment()) | result4.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(start, bound)) | result4.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(bound)) | result4.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0) | result4.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Counterclockwise);
            }
            else
            {
                var firstPoint = new Point();
                var intersect = InnerCurveSelfIntersect(radiusX, radiusY, this._absoluteThickness);
                var angleRanges = ComputeAngleRanges(radiusX, radiusY, intersect, end, start);
                flag3 = flag2 | SyncPieceWiseInnerCurves(figure, 2, ref firstPoint, angleRanges) | result3.SetIfDifferent(LineSegment.PointProperty, firstPoint);
            }
            return flag3;
        }

        /// <summary>
        /// 用每对输入角计算内部曲线的所有部分，并将其与多贝塞尔线段连接。新线段根据给定的索引输出到给定的 figure.Segments 列表中。起点是单独输出的。
        /// </summary>
        bool SyncPieceWiseInnerCurves(PathFigure figure, int index, ref Point firstPoint, params double[] angles)
        {
            var flag1 = false;
            var length = angles.Length;
            var logicalBounds = LogicalBounds;
            var absoluteThickness = _absoluteThickness;
            var flag2 = flag1 | figure.Segments.EnsureListCount(index + length / 2, () => new PolyBezierSegment());
            for (var index1 = 0; index1 < length / 2; ++index1)
            {
                var oneInnerCurve = ComputeOneInnerCurve(angles[index1 * 2], angles[index1 * 2 + 1], logicalBounds, absoluteThickness);
                if (index1 == 0)
                    firstPoint = oneInnerCurve[0];
                flag2 |= PathSegmentUtil.SyncPolyBezierSegment(figure.Segments, index + index1, oneInnerCurve, 1, oneInnerCurve.Count - 1);
            }
            return flag2;
        }

        /// <summary>
        /// 用给定的角范围计算一段内部曲线，并以多贝塞尔线段形式输出一段平滑曲线。
        /// </summary>
        static IList<Point> ComputeOneInnerCurve(double start, double end, Rect bounds, double offset)
        {
            var num1 = bounds.Width / 2.0;
            var num2 = bounds.Height / 2.0;
            var point1 = bounds.Center();
            start = start * Math.PI / 180.0;
            end = end * Math.PI / 180.0;
            var num3 = Math.PI / 18.0;
            var capacity = Math.Max(2, (int)Math.Ceiling(Math.Abs(end - start) / num3));
            var pointList1 = new List<Point>(capacity);
            var vectorList = new List<Vector>(capacity);
            var point2 = new Point();
            var point3 = new Point();
            var vector1 = new Vector();
            var vector2 = new Vector();
            var vector3 = new Vector();
            var vector4 = new Vector();
            for (var index = 0; index < capacity; ++index)
            {
                var num4 = MathUtil.Lerp(start, end, (double)index / (capacity - 1));
                var num5 = Math.Sin(num4);
                var num6 = Math.Cos(num4);
                point2.X = point1.X + num1 * num5;
                point2.Y = point1.Y - num2 * num6;
                vector1.X = num1 * num6;
                vector1.Y = num2 * num5;
                vector2.X = -num2 * num5;
                vector2.Y = num1 * num6;
                var d = num2 * num2 * num5 * num5 + num1 * num1 * num6 * num6;
                var num7 = Math.Sqrt(d);
                var num8 = 2.0 * num5 * num6 * (num2 * num2 - num1 * num1);
                vector3.X = -num2 * num6;
                vector3.Y = -num1 * num5;
                point3.X = point2.X + offset * vector2.X / num7;
                point3.Y = point2.Y + offset * vector2.Y / num7;
                vector4.X = vector1.X + offset / num7 * (vector3.X - 0.5 * vector2.X / d * num8);
                vector4.Y = vector1.Y + offset / num7 * (vector3.Y - 0.5 * vector2.Y / d * num8);
                pointList1.Add(point3);
                vectorList.Add(-vector4.Normalized());
            }
            var pointList2 = new List<Point>(capacity * 3 + 1) { pointList1[0] };
            for (var index = 1; index < capacity; ++index)
            {
                var lhs = pointList1[index - 1];
                var rhs = pointList1[index];
                var num4 = GeometryUtil.Distance(lhs, rhs) / 3.0;
                pointList2.Add(lhs + vectorList[index - 1] * num4);
                pointList2.Add(rhs - vectorList[index] * num4);
                pointList2.Add(rhs);
            }
            return pointList2;
        }

        /// <summary>
        /// 针对给定粗细的给定椭圆计算自相交点的参数（角）。结果始终在第一象限，并且可能为 0 或 90（表示无自相交）。基本算法是执行二进制搜索以搜索取样点不在第一象限的角。
        /// </summary>
        internal static double InnerCurveSelfIntersect(double radiusX, double radiusY, double thickness)
        {
            var angleA1 = 0.0;
            var angleB = Math.PI / 2.0;
            var flag = radiusX <= radiusY;
            var vector = new Vector();
            while (!AreCloseEnough(angleA1, angleB))
            {
                var num1 = (angleA1 + angleB) / 2.0;
                var num2 = Math.Cos(num1);
                var num3 = Math.Sin(num1);
                vector.X = radiusY * num3;
                vector.Y = radiusX * num2;
                vector.Normalize();
                if (flag)
                {
                    var num4 = radiusX * num3 - vector.X * thickness;
                    if (num4 > 0.0)
                        angleB = num1;
                    else if (num4 < 0.0)
                        angleA1 = num1;
                }
                else
                {
                    var num4 = radiusY * num2 - vector.Y * thickness;
                    if (num4 < 0.0)
                        angleB = num1;
                    else if (num4 > 0.0)
                        angleA1 = num1;
                }
            }
            var angleA2 = (angleA1 + angleB) / 2.0;
            if (AreCloseEnough(angleA2, 0.0))
                return 0.0;
            return !AreCloseEnough(angleA2, Math.PI / 2.0) ? angleA2 * 180.0 / Math.PI : 90.0;
        }

        static bool AreCloseEnough(double angleA, double angleB)
            => Math.Abs(angleA - angleB) < 0.001;

        static Size GetArcSize(Rect bound)
            => new Size(bound.Width / 2.0, bound.Height / 2.0);

        static double NormalizeAngle(double degree)
        {
            if (degree < 0.0 || degree > 360.0)
            {
                degree %= 360.0;
                if (degree < 0.0)
                    degree += 360.0;
            }
            return degree;
        }

        #region GeometrySource

        protected override bool UpdateCachedGeometry(IArcGeometrySourceParameters parameters)
        {
            var flag1 = false;
            NormalizeThickness(parameters);
            var relativeMode = parameters.ArcThicknessUnit == UnitType.Percent;
            var flag2 = MathUtil.AreClose(parameters.StartAngle, parameters.EndAngle);
            var num = NormalizeAngle(parameters.StartAngle);
            var end = NormalizeAngle(parameters.EndAngle);
            if (end < num)
                end += 360.0;
            var filled = _relativeThickness == 1.0;
            var flag3 = _relativeThickness == 0.0;
            return !flag2 ? (!MathUtil.IsVerySmall((end - num) % 360.0) ? (!filled ? (!flag3 ? flag1 | UpdateRingArcGeometry(relativeMode, num, end) : flag1 | UpdateOpenArcGeometry(num, end)) : flag1 | UpdatePieGeometry(num, end)) : (flag3 || filled ? flag1 | UpdateEllipseGeometry(filled) : flag1 | UpdateFullRingGeometry(relativeMode))) : flag1 | UpdateZeroAngleGeometry(relativeMode, num);
        }

        #endregion
    }
}