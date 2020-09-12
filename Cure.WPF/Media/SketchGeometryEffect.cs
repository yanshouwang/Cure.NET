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
        // TODO: 以下常量在反编译出来的代码中并未使用

        //private const double EXPECTED_LENGTH_MEAN = 8.0;
        //private const double NORMAL_DISTURB_VARIANCE = 0.5;
        //private const double TANGENT_DISTURB_VARIANCE = 1.0;
        //private const double BSP_LINE_WEIGHT = 0.05;

        /// <summary>
        /// 在创建时使用相同的随机种子以使视觉闪烁保持最小。
        /// </summary>
        private readonly long _randomSeed = DateTime.Now.Ticks;

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
            bool flag = false;
            PathGeometry inputPath = input.AsPathGeometry();
            if (inputPath != null)
                flag |= this.UpdateSketchGeometry(inputPath);
            else
                this.CachedGeometry = input;
            return flag;
        }

        private bool UpdateSketchGeometry(PathGeometry inputPath)
        {
            Geometry cachedGeometry = this.CachedGeometry;
            bool ensureGeometryType = GeometryUtil.EnsureGeometryType(out PathGeometry result, ref cachedGeometry, () => new PathGeometry());
            this.CachedGeometry = cachedGeometry;
            bool ensureListCount = result.Figures.EnsureListCount(inputPath.Figures.Count, () => new PathFigure());
            bool flag = false | ensureGeometryType | ensureListCount;
            RandomEngine random = new RandomEngine(this._randomSeed);
            for (int index = 0; index < inputPath.Figures.Count; ++index)
            {
                PathFigure figure = inputPath.Figures[index];
                bool closed = figure.IsClosed;
                bool filled = figure.IsFilled;
                if (figure.Segments.Count == 0)
                {
                    flag = flag | result.Figures[index].SetIfDifferent(PathFigure.StartPointProperty, figure.StartPoint) | result.Figures[index].Segments.EnsureListCount(0);
                }
                else
                {
                    List<Point> list = new List<Point>(figure.Segments.Count * 3);
                    foreach (SimpleSegment effectiveSegment in this.GetEffectiveSegments(figure))
                    {
                        List<Point> pointList1 = new List<Point>
                        {
                            effectiveSegment.Points[0]
                        };
                        SimpleSegment simpleSegment = effectiveSegment;
                        double num = 0.0;
                        IList<double> doubleList = null;
                        List<Point> pointList2 = pointList1;
                        double tolerance = num;
                        IList<double> resultParameters = doubleList;
                        simpleSegment.Flatten(pointList2, tolerance, resultParameters);
                        PolylineData polyline = new PolylineData(pointList1);
                        if (pointList1.Count > 1 && polyline.TotalLength > 4.0)
                        {
                            int sampleCount = (int)Math.Max(2.0, Math.Ceiling(polyline.TotalLength / 8.0));
                            double interval = polyline.TotalLength / sampleCount;
                            double scale = interval / 8.0;
                            List<Point> samplePoints = new List<Point>(sampleCount);
                            List<Vector> sampleNormals = new List<Vector>(sampleCount);
                            int sampleIndex = 0;
                            PolylineUtil.PathMarch(polyline, 0.0, 0.0, location =>
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
                            });
                            DisturbPoints(random, scale, samplePoints, sampleNormals);
                            list.AddRange(samplePoints);
                        }
                        else
                        {
                            list.AddRange(pointList1);
                            list.RemoveLast();
                        }
                    }
                    if (!closed)
                        list.Add(figure.Segments.Last().GetLastPoint());
                    flag |= PathFigureUtil.SyncPolylineFigure(result.Figures[index], list, closed, filled);
                }
            }
            if (flag)
                this.CachedGeometry = GeometryUtil.FixPathGeometryBoundary(this.CachedGeometry);
            return flag;
        }

        private static void DisturbPoints(RandomEngine random, double scale, IList<Point> points, IList<Vector> normals)
        {
            int count = points.Count;
            for (int index = 1; index < count; ++index)
            {
                double num1 = random.NextGaussian(0.0, 1.0 * scale);
                double num2 = random.NextUniform(-0.5, 0.5) * scale;
                points[index] = new Point(points[index].X + normals[index].X * num2 - normals[index].Y * num1, points[index].Y + normals[index].X * num1 + normals[index].Y * num2);
            }
        }

        /// <summary>
        /// 循环访问给定路径图中的所有简单段，包括右弦。
        /// </summary>
        private IEnumerable<SimpleSegment> GetEffectiveSegments(PathFigure pathFigure)
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