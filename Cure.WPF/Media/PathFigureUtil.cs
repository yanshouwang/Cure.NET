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
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 要使用 PathFigure 的帮助程序类。
    /// </summary>
    static class PathFigureUtil
    {
        /// <summary>
        /// 平展给定图形并将结果点添加到给定点列表中。
        /// </summary>
        /// <param name="tolerance">容错。必须是正数。可以为零。回退到默认容差。</param>
        public static void FlattenFigure(PathFigure figure, IList<Point> points, double tolerance, bool removeRepeat)
        {
            if (figure == null)
                throw new ArgumentNullException(nameof(figure));
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            if (tolerance < 0.0)
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            var points1 = removeRepeat ? new List<Point>() : points;
            points1.Add(figure.StartPoint);
            foreach (var allSegment in figure.AllSegments())
                allSegment.PathSegment.FlattenSegment(points1, allSegment.StartPoint, tolerance);
            if (figure.IsClosed)
                points1.Add(figure.StartPoint);
            if (!removeRepeat || points1.Count <= 0)
                return;
            points.Add(points1[0]);
            for (var index = 1; index < points1.Count; ++index)
            {
                if (!MathUtil.IsVerySmall(GeometryUtil.SquaredDistance(points.Last(), points1[index])))
                    points.Add(points1[index]);
            }
        }

        /// <summary>
        /// 将图形作为单一折线段与给定点列表同步。尝试使更改保持最小程度，如果未进行任何更改，则返回 false。
        /// </summary>
        public static bool SyncPolylineFigure(PathFigure figure, IList<Point> points, bool closed, bool filled = true)
        {
            if (figure == null)
                throw new ArgumentNullException(nameof(figure));
            var flag1 = false;
            var flag21 = figure.ClearIfSet(PathFigure.StartPointProperty);
            var flag22 = figure.Segments.EnsureListCount(0);
            var flag23 = figure.SetIfDifferent(PathFigure.StartPointProperty, points[0]);
            var flag24 = figure.Segments.EnsureListCount(1, () => new PolyLineSegment());
            var flag25 = PathSegmentUtil.SyncPolylineSegment(figure.Segments, 0, points, 1, points.Count - 1);
            var flag2 = points == null || points.Count == 0
                ? flag1 | flag21 | flag22
                : flag1 | flag23 | flag24 | flag25;
            var flag3 = figure.SetIfDifferent(PathFigure.IsClosedProperty, closed);
            var flag4 = figure.SetIfDifferent(PathFigure.IsFilledProperty, filled);
            return flag2 | flag3 | flag4;
        }

        /// <summary>
        /// 将要成为闭合椭圆的给定图形与两段弧同步。
        /// </summary>
        public static bool SyncEllipseFigure(PathFigure figure, Rect bounds, SweepDirection sweepDirection, bool filled = true)
        {
            var flag = false;
            var pointArray = new Point[2];
            var size = new Size(bounds.Width / 2.0, bounds.Height / 2.0);
            var point = bounds.Center();
            if (size.Width > size.Height)
            {
                pointArray[0] = new Point(bounds.Left, point.Y);
                pointArray[1] = new Point(bounds.Right, point.Y);
            }
            else
            {
                pointArray[0] = new Point(point.X, bounds.Top);
                pointArray[1] = new Point(point.X, bounds.Bottom);
            }
            var flag1 = figure.SetIfDifferent(PathFigure.IsClosedProperty, true);
            var flag2 = figure.SetIfDifferent(PathFigure.IsFilledProperty, filled);
            var flag3 = figure.SetIfDifferent(PathFigure.StartPointProperty, pointArray[0]);
            var flag4 = figure.Segments.EnsureListCount(2, () => new ArcSegment());
            var flag5 = GeometryUtil.EnsureSegmentType(out var result, figure.Segments, 0, () => new ArcSegment());
            var flag6 = result.SetIfDifferent(ArcSegment.PointProperty, pointArray[1]);
            var flag7 = result.SetIfDifferent(ArcSegment.SizeProperty, size);
            var flag8 = result.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            var flag9 = result.SetIfDifferent(ArcSegment.SweepDirectionProperty, sweepDirection);
            var flag10 = GeometryUtil.EnsureSegmentType(out result, figure.Segments, 1, () => new ArcSegment());
            var flag11 = result.SetIfDifferent(ArcSegment.PointProperty, pointArray[0]);
            var flag12 = result.SetIfDifferent(ArcSegment.SizeProperty, size);
            var flag13 = result.SetIfDifferent(ArcSegment.IsLargeArcProperty, false);
            var flag14 = result.SetIfDifferent(ArcSegment.SweepDirectionProperty, sweepDirection);
            return flag | flag1 | flag2 | flag3 | flag4 | flag5 | flag6 | flag7 | flag8 | flag9 | flag10 | flag11 | flag12 | flag13 | flag14;
        }
    }
}