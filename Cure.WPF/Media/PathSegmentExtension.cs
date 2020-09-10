// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    static class PathSegmentExtension
    {
        /// <summary>
        /// 避免调用包含三个参数的构造函数，因为它始终会为 IsStroked 设置本地值。
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="stroked"></param>
        public static void SetIsStroked(this PathSegment segment, bool stroked)
        {
            if (segment.IsStroked == stroked)
                return;
            segment.IsStroked = stroked;
        }

        /// <summary>
        /// 测试给定路径段是否为空。
        /// </summary>
        public static bool IsEmpty(this PathSegment segment)
            => segment.GetPointCount() == 0;

        /// <summary>
        /// 获取给定路径段中的点数。
        /// </summary>
        public static int GetPointCount(this PathSegment segment)
            => segment switch
            {
                ArcSegment _ => 1,
                LineSegment _ => 1,
                QuadraticBezierSegment _ => 2,
                BezierSegment _ => 3,
                PolyLineSegment polyLineSegment => polyLineSegment.Points.Count,
                PolyQuadraticBezierSegment quadraticBezierSegment => quadraticBezierSegment.Points.Count / 2 * 2,
                PolyBezierSegment polyBezierSegment => polyBezierSegment.Points.Count / 3 * 3,
                _ => 0,
            };

        /// <summary>
        /// 获取给定路径段的最后一个点。
        /// </summary>
        public static Point GetLastPoint(this PathSegment segment)
            => segment.GetPoint(-1);

        /// <summary>
        /// 获取给定线段中给定索引的点。如果输入为 (-1)，则返回最后一个点。
        /// </summary>
        public static Point GetPoint(this PathSegment segment, int index)
            => PathSegmentUtil.PathSegmentImplementation.Create(segment).GetPoint(index);

        /// <summary>
        /// 平展给定线段并将所生成的点添加到给定点列表中。
        /// </summary>
        /// <param name="segment">要平展的线段。</param>
        /// <param name="points">所生成的点列表。</param>
        /// <param name="start">线段的起点。</param>
        /// <param name="tolerance">容错。必须是正数。可以为零。回退到默认容差。</param>
        public static void FlattenSegment(this PathSegment segment, IList<Point> points, Point start, double tolerance)
            => PathSegmentUtil.PathSegmentImplementation.Create(segment, start).Flatten(points, tolerance);

        public static IEnumerable<SimpleSegment> GetSimpleSegments(this PathSegment segment, Point start)
        {
            var implementation = PathSegmentUtil.PathSegmentImplementation.Create(segment, start);
            foreach (var simpleSegment in implementation.GetSimpleSegments())
                yield return simpleSegment;
        }
    }
}
