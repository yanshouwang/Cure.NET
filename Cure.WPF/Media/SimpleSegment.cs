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

namespace Cure.WPF.Media
{
    class SimpleSegment
    {
        public SegmentType Type { get; private set; }

        /// <summary>
        /// 路径段的控制点。长度为变量。线段有 2 个点，三次方贝塞尔有 4 个点。
        /// </summary>
        public Point[] Points { get; private set; }

        public void Flatten(IList<Point> resultPolyline, double tolerance, IList<double> resultParameters)
        {
            switch (Type)
            {
                case SegmentType.Line:
                    resultPolyline.Add(Points[1]);
                    resultParameters?.Add(1.0);
                    break;
                case SegmentType.CubicBeizer:
                    BezierCurveFlattener.FlattenCubic(Points, tolerance, resultPolyline, true, resultParameters);
                    break;
            }
        }

        /// <summary>
        /// 专用构造函数。强制使用工厂方法。
        /// </summary>
        SimpleSegment(SegmentType type, Point[] points)
        {
            Type = type;
            Points = points;
        }

        /// <summary>
        /// 创建一条线段
        /// </summary>
        public static SimpleSegment Create(Point point0, Point point1)
        {
            var points = new Point[2] { point0, point1 };
            return new SimpleSegment(SegmentType.Line, points);
        }

        /// <summary>
        /// 利用二次曲线（3 个控制点）创建一条三次方贝塞尔线段
        /// </summary>
        public static SimpleSegment Create(Point point0, Point point1, Point point2)
        {
            var point3 = GeometryUtil.Lerp(point0, point1, 2.0 / 3.0);
            var point4 = GeometryUtil.Lerp(point1, point2, 1.0 / 3.0);
            var points = new Point[4] { point0, point3, point4, point2 };
            return new SimpleSegment(SegmentType.CubicBeizer, points);
        }

        /// <summary>
        /// 利用 4 个控制点创建一条三次方贝塞尔线段。
        /// </summary>
        public static SimpleSegment Create(Point point0, Point point1, Point point2, Point point3)
        {
            var points = new Point[4] { point0, point1, point2, point3 };
            return new SimpleSegment(SegmentType.CubicBeizer, points);
        }

        public enum SegmentType
        {
            Line,
            CubicBeizer,
        }
    }
}