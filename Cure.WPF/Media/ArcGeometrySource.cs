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
    internal class ArcGeometrySource : GeometrySource<IArcGeometrySourceParameters>
    {
        private double _relativeThickness;
        private double _absoluteThickness;

        /// <summary>
        /// 假定纵横比为 1:1，则弧会认为 Stretch.None 与 Stretch.Fill 相同。
        /// </summary>
        protected override Rect ComputeLogicalBounds(Rect layoutBounds, IGeometrySourceParameters parameters)
            => GeometryUtil.GetStretchBound(base.ComputeLogicalBounds(layoutBounds, parameters), parameters.Stretch, new Size(1.0, 1.0));

        /// <summary>
        /// 使相对于范围框的粗细以及大小为绝对像素的粗细标准化。相对粗细 = 0 -&gt; 全圆半径或固定值。相对粗细 = 1 -&gt; 缩小为点或已退化。
        /// </summary>
        private void NormalizeThickness(IArcGeometrySourceParameters parameters)
        {
            double rhs = Math.Min(this.LogicalBounds.Width / 2.0, this.LogicalBounds.Height / 2.0);
            double lhs = parameters.ArcThickness;
            if (parameters.ArcThicknessUnit == UnitType.Pixel)
                lhs = MathUtil.SafeDivide(lhs, rhs, 0.0);
            this._relativeThickness = MathUtil.EnsureRange(lhs, 0.0, 1.0);
            this._absoluteThickness = rhs * this._relativeThickness;
        }

        /// <summary>
        /// 弧退化为指向内心/内部法线的线条。
        /// </summary>
        private bool UpdateZeroAngleGeometry(bool relativeMode, double angle)
        {
            bool flag = false;
            Point arcPoint = GeometryUtil.GetArcPoint(angle, this.LogicalBounds);
            Rect logicalBounds = this.LogicalBounds;
            double radiusX = logicalBounds.Width / 2.0;
            double radiusY = logicalBounds.Height / 2.0;
            Point point;
            if (relativeMode || MathUtil.AreClose(radiusX, radiusY))
            {
                Rect bound = this.LogicalBounds.Resize(1.0 - this._relativeThickness);
                point = GeometryUtil.GetArcPoint(angle, bound);
            }
            else
            {
                double intersect = InnerCurveSelfIntersect(radiusX, radiusY, this._absoluteThickness);
                double[] angleRanges = ComputeAngleRanges(radiusX, radiusY, intersect, angle, angle);
                double num = angleRanges[0] * Math.PI / 180.0;
                Vector vector = new Vector(radiusY * Math.Sin(num), -radiusX * Math.Cos(num));
                point = GeometryUtil.GetArcPoint(angleRanges[0], this.LogicalBounds) - vector.Normalized() * this._absoluteThickness;
            }
            Geometry cachedGeometry = this.CachedGeometry;
            bool flag1 = GeometryUtil.EnsureGeometryType(out LineGeometry result, ref cachedGeometry, () => new LineGeometry());
            this.CachedGeometry = cachedGeometry;
            bool flag2 = result.SetIfDifferent(LineGeometry.StartPointProperty, arcPoint);
            bool flag3 = result.SetIfDifferent(LineGeometry.EndPointProperty, point);
            return flag | flag1 | flag2 | flag3;
        }

        private bool UpdateEllipseGeometry(bool filled)
        {
            bool flag1 = false;
            double y = MathUtil.Lerp(this.LogicalBounds.Top, this.LogicalBounds.Bottom, 0.5);
            Point point1 = new Point(this.LogicalBounds.Left, y);
            Point point2 = new Point(this.LogicalBounds.Right, y);
            Geometry cachedGeometry = this.CachedGeometry;
            bool flag21 = GeometryUtil.EnsureGeometryType(out PathGeometry result1, ref cachedGeometry, () => new PathGeometry());
            this.CachedGeometry = cachedGeometry;
            bool flag22 = result1.Figures.EnsureListCount(1, () => new PathFigure());
            bool flag2 = flag1 | flag21 | flag22;
            PathFigure figure = result1.Figures[0];
            bool flag31 = figure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            bool flag32 = figure.SetIfDifferent(PathFigure.IsFilledProperty, filled);
            bool flag33 = figure.Segments.EnsureListCount(2, () => new ArcSegment());
            bool flag34 = figure.SetIfDifferent(PathFigure.StartPointProperty, point1);
            bool flag35 = GeometryUtil.EnsureSegmentType(out ArcSegment result2, figure.Segments, 0, () => new ArcSegment());
            bool flag36 = GeometryUtil.EnsureSegmentType(out ArcSegment result3, figure.Segments, 1, () => new ArcSegment());
            bool flag3 = flag2 | flag31 | flag32 | flag33 | flag34 | flag35 | flag36;
            Size size = new Size(this.LogicalBounds.Width / 2.0, this.LogicalBounds.Height / 2.0);
            bool flag41 = result2.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            bool flag42 = result2.SetIfDifferent(ArcSegment.SizeProperty, size);
            bool flag43 = result2.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            bool flag44 = result2.SetIfDifferent(ArcSegment.PointProperty, point2);
            bool flag45 = result3.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            bool flag46 = result3.SetIfDifferent(ArcSegment.SizeProperty, size);
            bool flag47 = result3.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise);
            bool flag48 = result3.SetIfDifferent(ArcSegment.PointProperty, point1);
            return flag3 | flag41 | flag42 | flag43 | flag44 | flag45 | flag46 | flag47 | flag48;
        }

        private bool UpdateFullRingGeometry(bool relativeMode)
        {
            Geometry cachedGeometry = this.CachedGeometry;
            bool flag11 = GeometryUtil.EnsureGeometryType(out PathGeometry result, ref cachedGeometry, () => new PathGeometry());
            this.CachedGeometry = cachedGeometry;
            bool flag12 = result.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.EvenOdd);
            bool flag13 = result.Figures.EnsureListCount(2, () => new PathFigure());
            bool flag14 = PathFigureUtil.SyncEllipseFigure(result.Figures[0], this.LogicalBounds, SweepDirection.Clockwise);
            bool flag1 = false | flag11 | flag12 | flag13 | flag14;
            Rect logicalBounds = this.LogicalBounds;
            double radiusX = logicalBounds.Width / 2.0;
            double radiusY = logicalBounds.Height / 2.0;
            bool flag2;
            if (relativeMode || MathUtil.AreClose(radiusX, radiusY))
            {
                Rect bounds = this.LogicalBounds.Resize(1.0 - this._relativeThickness);
                flag2 = flag1 | PathFigureUtil.SyncEllipseFigure(result.Figures[1], bounds, SweepDirection.Counterclockwise);
            }
            else
            {
                bool flag3 = flag1 | result.Figures[1].SetIfDifferent(PathFigure.IsClosedProperty, true) | result.Figures[1].SetIfDifferent(PathFigure.IsFilledProperty, true);
                Point firstPoint = new Point();
                double intersect = InnerCurveSelfIntersect(radiusX, radiusY, this._absoluteThickness);
                double[] angleRanges = ComputeAngleRanges(radiusX, radiusY, intersect, 360.0, 0.0);
                flag2 = flag3 | this.SyncPieceWiseInnerCurves(result.Figures[1], 0, ref firstPoint, angleRanges) | result.Figures[1].SetIfDifferent(PathFigure.StartPointProperty, firstPoint);
            }
            return flag2;
        }

        private static void IncreaseDuplicatedIndex(IList<double> values, ref int index)
        {
            while (index < values.Count - 1 && values[index] == values[index + 1])
                ++index;
        }

        private static void DecreaseDuplicatedIndex(IList<double> values, ref int index)
        {
            while (index > 0 && values[index] == values[index - 1])
                --index;
        }

        /// <summary>
        /// 计算角对的列表，并定义应在其中查找弧示例的范围。返回值具有 2、4 或 6 对值，每对值都定义了一个范围，并且它们按顺序从给定的起始角向终止角跨越。在自相交角处将会超出范围。如果起始/终止输入在自相交角之间的无效范围内，则它将移到邻近的自相交范围内。
        /// </summary>
        internal static double[] ComputeAngleRanges(double radiusX, double radiusY, double intersect, double start, double end)
        {
            List<double> doubleList = new List<double>()
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
            int index1 = doubleList.IndexOf(start);
            int index2 = doubleList.IndexOf(end);
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
            List<double> list = new List<double>();
            if (index1 < index2)
            {
                for (int index3 = index1; index3 <= index2; ++index3)
                    list.Add(doubleList[index3]);
            }
            else
            {
                for (int index3 = index1; index3 >= index2; --index3)
                    list.Add(doubleList[index3]);
            }
            double num = EnsureFirstQuadrant((list[0] + list[1]) / 2.0);
            if (radiusX < radiusY && num < intersect || radiusX > radiusY && num > intersect)
                list.RemoveAt(0);
            if (list.Count % 2 == 1)
                list.RemoveLast();
            if (list.Count == 0)
            {
                int index3 = Math.Min(index1, index2) - 1;
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

        private bool UpdatePieGeometry(double start, double end)
        {
            bool flag = false;
            PathFigure dependencyObject1;
            if (!(this.CachedGeometry is PathGeometry cachedGeometry) || cachedGeometry.Figures.Count != 1 || (dependencyObject1 = cachedGeometry.Figures[0]).Segments.Count != 2 || !(dependencyObject1.Segments[0] is ArcSegment dependencyObject2) || !(dependencyObject1.Segments[1] is LineSegment dependencyObject3))
            {
                dependencyObject2 = new ArcSegment()
                {
                    SweepDirection = SweepDirection.Clockwise
                };
                dependencyObject3 = new LineSegment();
                dependencyObject1 = new PathFigure();
                dependencyObject1.Segments.Add(dependencyObject2);
                dependencyObject1.Segments.Add(dependencyObject3);
                cachedGeometry = new PathGeometry();
                cachedGeometry.Figures.Add(dependencyObject1);
                this.CachedGeometry = cachedGeometry;
                flag = true;
            }
            return flag | dependencyObject1.SetIfDifferent(PathFigure.StartPointProperty, GeometryUtil.GetArcPoint(start, this.LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(end, this.LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(this.LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0) | dependencyObject3.SetIfDifferent(LineSegment.PointProperty, this.LogicalBounds.Center());
        }

        private bool UpdateOpenArcGeometry(double start, double end)
        {
            bool flag = false;
            PathFigure dependencyObject1;
            if (!(this.CachedGeometry is PathGeometry cachedGeometry) || cachedGeometry.Figures.Count != 1 || (dependencyObject1 = cachedGeometry.Figures[0]).Segments.Count != 1 || !(dependencyObject1.Segments[0] is ArcSegment dependencyObject2))
            {
                dependencyObject2 = new ArcSegment
                {
                    SweepDirection = SweepDirection.Clockwise
                };
                dependencyObject1 = new PathFigure()
                {
                    IsClosed = false
                };
                dependencyObject1.Segments.Add(dependencyObject2);
                cachedGeometry = new PathGeometry();
                cachedGeometry.Figures.Add(dependencyObject1);
                this.CachedGeometry = cachedGeometry;
                flag = true;
            }
            return flag | dependencyObject1.SetIfDifferent(PathFigure.StartPointProperty, GeometryUtil.GetArcPoint(start, this.LogicalBounds)) | dependencyObject1.SetIfDifferent(PathFigure.IsFilledProperty, false) | dependencyObject2.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(end, this.LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(this.LogicalBounds)) | dependencyObject2.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0);
        }

        private bool UpdateRingArcGeometry(bool relativeMode, double start, double end)
        {
            Geometry cachedGeometry = this.CachedGeometry;
            bool flag11 = GeometryUtil.EnsureGeometryType(out PathGeometry result1, ref cachedGeometry, () => new PathGeometry());
            this.CachedGeometry = cachedGeometry;
            bool flag12 = result1.SetIfDifferent(PathGeometry.FillRuleProperty, FillRule.Nonzero);
            bool flag13 = result1.Figures.EnsureListCount(1, () => new PathFigure());
            bool flag1 = false | flag11 | flag12 | flag13;
            PathFigure figure = result1.Figures[0];
            bool flag2 = flag1 | figure.SetIfDifferent(PathFigure.IsClosedProperty, true) | figure.SetIfDifferent(PathFigure.IsFilledProperty, true) | figure.SetIfDifferent(PathFigure.StartPointProperty, GeometryUtil.GetArcPoint(start, this.LogicalBounds)) | figure.Segments.EnsureListCountAtLeast(3, () => new ArcSegment()) | GeometryUtil.EnsureSegmentType(out ArcSegment result2, figure.Segments, 0, () => new ArcSegment()) | result2.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(end, this.LogicalBounds)) | result2.SetIfDifferent(ArcSegment.SizeProperty, new Size(this.LogicalBounds.Width / 2.0, this.LogicalBounds.Height / 2.0)) | result2.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0) | result2.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Clockwise) | GeometryUtil.EnsureSegmentType(out LineSegment result3, figure.Segments, 1, () => new LineSegment());
            Rect logicalBounds = this.LogicalBounds;
            double radiusX = logicalBounds.Width / 2.0;
            double radiusY = logicalBounds.Height / 2.0;
            bool flag3;
            if (relativeMode || MathUtil.AreClose(radiusX, radiusY))
            {
                Rect bound = this.LogicalBounds.Resize(1.0 - this._relativeThickness);
                flag3 = flag2 | result3.SetIfDifferent(LineSegment.PointProperty, GeometryUtil.GetArcPoint(end, bound)) | figure.Segments.EnsureListCount(3, () => new ArcSegment()) | GeometryUtil.EnsureSegmentType(out ArcSegment result4, figure.Segments, 2, () => new ArcSegment()) | result4.SetIfDifferent(ArcSegment.PointProperty, GeometryUtil.GetArcPoint(start, bound)) | result4.SetIfDifferent(ArcSegment.SizeProperty, GetArcSize(bound)) | result4.SetIfDifferent(ArcSegment.IsLargeArcProperty, end - start > 180.0) | result4.SetIfDifferent(ArcSegment.SweepDirectionProperty, SweepDirection.Counterclockwise);
            }
            else
            {
                Point firstPoint = new Point();
                double intersect = InnerCurveSelfIntersect(radiusX, radiusY, this._absoluteThickness);
                double[] angleRanges = ComputeAngleRanges(radiusX, radiusY, intersect, end, start);
                flag3 = flag2 | this.SyncPieceWiseInnerCurves(figure, 2, ref firstPoint, angleRanges) | result3.SetIfDifferent(LineSegment.PointProperty, firstPoint);
            }
            return flag3;
        }

        /// <summary>
        /// 用每对输入角计算内部曲线的所有部分，并将其与多贝塞尔线段连接。新线段根据给定的索引输出到给定的 figure.Segments 列表中。起点是单独输出的。
        /// </summary>
        private bool SyncPieceWiseInnerCurves(PathFigure figure, int index, ref Point firstPoint, params double[] angles)
        {
            bool flag1 = false;
            int length = angles.Length;
            Rect logicalBounds = this.LogicalBounds;
            double absoluteThickness = this._absoluteThickness;
            bool flag2 = flag1 | figure.Segments.EnsureListCount(index + length / 2, () => new PolyBezierSegment());
            for (int index1 = 0; index1 < length / 2; ++index1)
            {
                IList<Point> oneInnerCurve = ComputeOneInnerCurve(angles[index1 * 2], angles[index1 * 2 + 1], logicalBounds, absoluteThickness);
                if (index1 == 0)
                    firstPoint = oneInnerCurve[0];
                flag2 |= PathSegmentUtil.SyncPolyBezierSegment(figure.Segments, index + index1, oneInnerCurve, 1, oneInnerCurve.Count - 1);
            }
            return flag2;
        }

        /// <summary>
        /// 用给定的角范围计算一段内部曲线，并以多贝塞尔线段形式输出一段平滑曲线。
        /// </summary>
        private static IList<Point> ComputeOneInnerCurve(double start, double end, Rect bounds, double offset)
        {
            double num1 = bounds.Width / 2.0;
            double num2 = bounds.Height / 2.0;
            Point point1 = bounds.Center();
            start = start * Math.PI / 180.0;
            end = end * Math.PI / 180.0;
            double num3 = Math.PI / 18.0;
            int capacity = Math.Max(2, (int)Math.Ceiling(Math.Abs(end - start) / num3));
            List<Point> pointList1 = new List<Point>(capacity);
            List<Vector> vectorList = new List<Vector>(capacity);
            Point point2 = new Point();
            Point point3 = new Point();
            Vector vector1 = new Vector();
            Vector vector2 = new Vector();
            Vector vector3 = new Vector();
            Vector vector4 = new Vector();
            for (int index = 0; index < capacity; ++index)
            {
                double num4 = MathUtil.Lerp(start, end, (double)index / (capacity - 1));
                double num5 = Math.Sin(num4);
                double num6 = Math.Cos(num4);
                point2.X = point1.X + num1 * num5;
                point2.Y = point1.Y - num2 * num6;
                vector1.X = num1 * num6;
                vector1.Y = num2 * num5;
                vector2.X = -num2 * num5;
                vector2.Y = num1 * num6;
                double d = num2 * num2 * num5 * num5 + num1 * num1 * num6 * num6;
                double num7 = Math.Sqrt(d);
                double num8 = 2.0 * num5 * num6 * (num2 * num2 - num1 * num1);
                vector3.X = -num2 * num6;
                vector3.Y = -num1 * num5;
                point3.X = point2.X + offset * vector2.X / num7;
                point3.Y = point2.Y + offset * vector2.Y / num7;
                vector4.X = vector1.X + offset / num7 * (vector3.X - 0.5 * vector2.X / d * num8);
                vector4.Y = vector1.Y + offset / num7 * (vector3.Y - 0.5 * vector2.Y / d * num8);
                pointList1.Add(point3);
                vectorList.Add(-vector4.Normalized());
            }
            List<Point> pointList2 = new List<Point>(capacity * 3 + 1) { pointList1[0] };
            for (int index = 1; index < capacity; ++index)
            {
                Point lhs = pointList1[index - 1];
                Point rhs = pointList1[index];
                double num4 = GeometryUtil.Distance(lhs, rhs) / 3.0;
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
            double angleA1 = 0.0;
            double angleB = Math.PI / 2.0;
            bool flag = radiusX <= radiusY;
            Vector vector = new Vector();
            while (!AreCloseEnough(angleA1, angleB))
            {
                double num1 = (angleA1 + angleB) / 2.0;
                double num2 = Math.Cos(num1);
                double num3 = Math.Sin(num1);
                vector.X = radiusY * num3;
                vector.Y = radiusX * num2;
                vector.Normalize();
                if (flag)
                {
                    double num4 = radiusX * num3 - vector.X * thickness;
                    if (num4 > 0.0)
                        angleB = num1;
                    else if (num4 < 0.0)
                        angleA1 = num1;
                }
                else
                {
                    double num4 = radiusY * num2 - vector.Y * thickness;
                    if (num4 < 0.0)
                        angleB = num1;
                    else if (num4 > 0.0)
                        angleA1 = num1;
                }
            }
            double angleA2 = (angleA1 + angleB) / 2.0;
            if (AreCloseEnough(angleA2, 0.0))
                return 0.0;
            return !AreCloseEnough(angleA2, Math.PI / 2.0) ? angleA2 * 180.0 / Math.PI : 90.0;
        }

        private static bool AreCloseEnough(double angleA, double angleB)
            => Math.Abs(angleA - angleB) < 0.001;

        private static Size GetArcSize(Rect bound)
            => new Size(bound.Width / 2.0, bound.Height / 2.0);

        private static double NormalizeAngle(double degree)
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
            bool flag1 = false;
            this.NormalizeThickness(parameters);
            bool relativeMode = parameters.ArcThicknessUnit == UnitType.Percent;
            bool flag2 = MathUtil.AreClose(parameters.StartAngle, parameters.EndAngle);
            double num = NormalizeAngle(parameters.StartAngle);
            double end = NormalizeAngle(parameters.EndAngle);
            if (end < num)
                end += 360.0;
            bool filled = this._relativeThickness == 1.0;
            bool flag3 = this._relativeThickness == 0.0;
            return !flag2 ? (!MathUtil.IsVerySmall((end - num) % 360.0) ? (!filled ? (!flag3 ? flag1 | this.UpdateRingArcGeometry(relativeMode, num, end) : flag1 | this.UpdateOpenArcGeometry(num, end)) : flag1 | this.UpdatePieGeometry(num, end)) : (flag3 || filled ? flag1 | this.UpdateEllipseGeometry(filled) : flag1 | this.UpdateFullRingGeometry(relativeMode))) : flag1 | this.UpdateZeroAngleGeometry(relativeMode, num);
        }

        #endregion
    }
}