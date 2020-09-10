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
    /// 像在 SketchFlow 中那样将任何几何图形转换为草图样式的几何图形效果。
    /// </summary>
    public sealed class SketchGeometryEffect : GeometryEffect
    {
        const double EXPECTED_LENGTH_MEAN = 8.0;
        const double NORMAL_DISTURB_VARIANCE = 0.5;
        const double TANGENT_DISTURB_VARIANCE = 1.0;
        const double BSP_LINE_WEIGHT = 0.05;
        /// <summary>
        /// 在创建时使用相同的随机种子以使视觉闪烁保持最小。
        /// </summary>
        readonly long _randomSeed = DateTime.Now.Ticks;

        /// <summary>
        /// 对几何图形效果进行深层复制。
        /// </summary>
        /// <returns>几何图形效果的当前实例的克隆。</returns>
        protected override GeometryEffect DeepCopy()
            => new SketchGeometryEffect();

        /// <summary>
        /// 测试给定几何图形效果是否等效于当前实例。
        /// </summary>
        /// <param name="geometryEffect">用来比较的几何图形效果。</param>
        /// <returns>当两种效果呈现相同的外观时返回 true。</returns>
        public override bool Equals(GeometryEffect geometryEffect)
            => geometryEffect is SketchGeometryEffect;

        /// <summary>
        /// 基于给定输入几何图形更新 cachedGeometry。
        /// </summary>
        /// <param name="input">输入的几何图形。</param>
        /// <returns>当 cachedGeometry 上的任何内容已更新时返回 true。</returns>
        protected override bool UpdateCachedGeometry(Geometry input)
        {
            var flag = false;
            var inputPath = input.AsPathGeometry();
            if (inputPath != null)
                flag |= UpdateSketchGeometry(inputPath);
            else
                CachedGeometry = input;
            return flag;
        }

        bool UpdateSketchGeometry(PathGeometry inputPath)
        {
            var cachedGeometry = CachedGeometry;
            var ensureGeometryType = GeometryUtil.EnsureGeometryType(out var result, ref cachedGeometry, () => new PathGeometry());
            CachedGeometry = cachedGeometry;
            var ensureListCount = result.Figures.EnsureListCount(inputPath.Figures.Count, () => new PathFigure());
            var flag = false | ensureGeometryType | ensureListCount;
            var random = new RandomEngine(_randomSeed);
            for (var index = 0; index < inputPath.Figures.Count; ++index)
            {
                var figure = inputPath.Figures[index];
                var closed = figure.IsClosed;
                var filled = figure.IsFilled;
                if (figure.Segments.Count == 0)
                {
                    flag = flag | result.Figures[index].SetIfDifferent(PathFigure.StartPointProperty, figure.StartPoint) | result.Figures[index].Segments.EnsureListCount(0);
                }
                else
                {
                    var list = new List<Point>(figure.Segments.Count * 3);
                    foreach (var effectiveSegment in GetEffectiveSegments(figure))
                    {
                        var pointList1 = new List<Point>
                        {
                            effectiveSegment.Points[0]
                        };
                        var simpleSegment = effectiveSegment;
                        var num = 0.0;
                        IList<double> doubleList = null;
                        var pointList2 = pointList1;
                        var tolerance = num;
                        var resultParameters = doubleList;
                        simpleSegment.Flatten(pointList2, tolerance, resultParameters);
                        PolylineData polyline = new PolylineData((IList<Point>)pointList1);
                        if (pointList1.Count > 1 && polyline.TotalLength > 4.0)
                        {
                            var sampleCount = (int)Math.Max(2.0, Math.Ceiling(polyline.TotalLength / 8.0));
                            double interval = polyline.TotalLength / (double)sampleCount;
                            var scale = interval / 8.0;
                            List<Point> samplePoints = new List<Point>(sampleCount);
                            List<Vector> sampleNormals = new List<Vector>(sampleCount);
                            int sampleIndex = 0;
                            PolylineHelper.PathMarch(polyline, 0.0, 0.0, (Func<MarchLocation, double>)(location =>
                            {
                                if (location.Reason == MarchStopReason.CompletePolyline)
                                    return double.NaN;
                                if (location.Reason != MarchStopReason.CompleteStep)
                                    return location.Remain;
                                if (sampleIndex++ == sampleCount)
                                    return double.NaN;
                                samplePoints.Add(location.GetPoint(polyline.Points));
                                sampleNormals.Add(location.GetNormal(polyline));
                                return interval;
                            }));
                            SketchGeometryEffect.DisturbPoints(random, scale, (IList<Point>)samplePoints, (IList<Vector>)sampleNormals);
                            list.AddRange((IEnumerable<Point>)samplePoints);
                        }
                        else
                        {
                            list.AddRange((IEnumerable<Point>)pointList1);
                            list.RemoveLast<Point>();
                        }
                    }
                    if (!closed)
                        list.Add(figure.Segments.Last<PathSegment>().GetLastPoint());
                    flag |= PathFigureUtil.SyncPolylineFigure(result.Figures[index], (IList<Point>)list, closed, filled);
                }
            }
            if (flag)
                this.cachedGeometry = PathGeometryHelper.FixPathGeometryBoundary(this.cachedGeometry);
            return flag;
        }

        static void DisturbPoints(RandomEngine random, double scale, IList<Point> points, IList<Vector> normals)
        {
            var count = points.Count;
            for (var index = 1; index < count; ++index)
            {
                double num1 = random.NextGaussian(0.0, 1.0 * scale);
                double num2 = random.NextUniform(-0.5, 0.5) * scale;
                points[index] = new Point(points[index].X + normals[index].X * num2 - normals[index].Y * num1, points[index].Y + normals[index].X * num1 + normals[index].Y * num2);
            }
        }

        /// <summary>
        /// 循环访问给定路径图中的所有简单段，包括右弦。
        /// </summary>
        IEnumerable<SimpleSegment> GetEffectiveSegments(PathFigure pathFigure)
        {
            Point lastPoint = pathFigure.StartPoint;
            foreach (PathSegmentData allSegment in pathFigure.AllSegments())
            {
                foreach (SimpleSegment simpleSegment in allSegment.PathSegment.GetSimpleSegments(allSegment.StartPoint))
                {
                    yield return simpleSegment;
                    lastPoint = ((IList<Point>)simpleSegment.Points).Last<Point>();
                }
            }
            if (pathFigure.IsClosed)
            {
                yield return SimpleSegment.Create(lastPoint, pathFigure.StartPoint);
            }
        }
    }
}