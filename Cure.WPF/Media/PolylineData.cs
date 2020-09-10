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

namespace Cure.WPF.Media
{
    /// <summary>
    /// 表示由一系列点连接而成的折线。闭合多边形是通过将第一个点与最后一个点重合来表示的。根据需要计算差值、法线、角度和长度。
    /// </summary>
    class PolylineData
    {
        IList<Vector> _normals;
        IList<double> _angles;
        IList<double> _lengths;
        IList<double> _accumulates;
        double? _totalLength;

        /// <summary>
        /// 用两个或更多个点构成折线。
        /// </summary>
        /// <param name="points"></param>
        public PolylineData(IList<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            Points = points.Count > 1 ? points : throw new ArgumentOutOfRangeException(nameof(points));
        }

        /// <summary>
        /// 第一个点与最后一个点重叠时，折线发生闭合。
        /// </summary>
        public bool IsClosed
            => Points[0] == Points.Last();
        /// <summary>
        /// 此折线中的点的计数。
        /// </summary>
        public int Count
            => Points.Count;
        /// <summary>
        /// 此折线的总弧长。
        /// </summary>
        public double TotalLength
            => _totalLength ?? ComputeTotalLength();
        /// <summary>
        /// 此折线的点数组。
        /// </summary>
        public IList<Point> Points { get; }
        /// <summary>
        /// 线段（Points[i] 到 Points[i+1]）的长度。
        /// </summary>
        public IList<double> Lengths
            => _lengths ?? ComputeLengths();
        /// <summary>
        /// 每个线段的法向矢量的列表。Normals[i] 是线段（p[i] 到 p[i + 1]）的法线。Normals[N-1] == Normals[N-2]。
        /// </summary>
        public IList<Vector> Normals
            => _normals ?? ComputeNormals();
        /// <summary>
        /// 以点 p[i] 为顶点的两个线段的夹角的 Cos(angle) 列表。注意：值为 cos(angle) = Dot(u, v)。单位不是度。
        /// </summary>
        public IList<double> Angles
            => _angles ?? ComputeAngles();
        /// <summary>
        /// 从 points[i] 到 points[0] 之间的累计长度的列表。
        /// </summary>
        public IList<double> AccumulatedLength
            => _accumulates ?? ComputeAccumulatedLength();

        /// <summary>
        /// 折线的前向差分矢量。Points[i] + Differences[i] = Points[i+1]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector Difference(int index)
            => Points[(index + 1) % Count].Subtract(Points[index]);

        /// <summary>
        /// 计算给定位置的法向矢量 (lerp(index, index+1, fraction)。如果该位置位于 cornerRadius 范围内，请内插法线方向。
        /// </summary>
        /// <param name="cornerRadius">法线平滑度的范围。如果为零，则没有平滑度并针对索引返回确切的法线。</param>
        public Vector SmoothNormal(int index, double fraction, double cornerRadius)
        {
            if (cornerRadius > 0.0)
            {
                var length = Lengths[index];
                if (MathUtil.IsVerySmall(length))
                {
                    var index1 = index - 1;
                    if (index1 < 0 && IsClosed)
                        index1 = Count - 1;
                    var index2 = index + 1;
                    if (IsClosed && index2 >= Count - 1)
                        index2 = 0;
                    return index1 < 0 || index2 >= Count ? Normals[index] : GeometryUtil.Lerp(Normals[index2], Normals[index1], 0.5).Normalized();
                }
                var num = Math.Min(cornerRadius / length, 0.5);
                if (fraction <= num)
                {
                    var index1 = index - 1;
                    if (IsClosed && index1 == -1)
                        index1 = Count - 1;
                    if (index1 >= 0)
                    {
                        var alpha = (num - fraction) / (2.0 * num);
                        return GeometryUtil.Lerp(Normals[index], Normals[index1], alpha).Normalized();
                    }
                }
                else if (fraction >= 1.0 - num)
                {
                    var index1 = index + 1;
                    if (IsClosed && index1 >= Count - 1)
                        index1 = 0;
                    if (index1 < Count)
                    {
                        var alpha = (fraction + num - 1.0) / (2.0 * num);
                        return GeometryUtil.Lerp(Normals[index], Normals[index1], alpha).Normalized();
                    }
                }
            }
            return Normals[index];
        }

        IList<double> ComputeLengths()
        {
            _lengths = new double[Count];
            for (var index = 0; index < Count; ++index)
                _lengths[index] = Difference(index).Length;
            return _lengths;
        }

        IList<Vector> ComputeNormals()
        {
            _normals = (new Vector[Points.Count]);
            for (var index = 0; index < Count - 1; ++index)
                _normals[index] = GeometryUtil.Normal(Points[index], Points[index + 1]);
            _normals[Count - 1] = _normals[Count - 2];
            return _normals;
        }

        IList<double> ComputeAngles()
        {
            _angles = new double[Count];
            for (var index = 1; index < Count - 1; ++index)
                _angles[index] = -GeometryUtil.Dot(Normals[index - 1], Normals[index]);
            _angles[0] = !IsClosed ? (_angles[Count - 1] = 1.0) : (_angles[Count - 1] = -GeometryUtil.Dot(Normals[0], Normals[Count - 2]));
            return _angles;
        }

        IList<double> ComputeAccumulatedLength()
        {
            _accumulates = new double[Count];
            _accumulates[0] = 0.0;
            for (int index = 1; index < Count; ++index)
                _accumulates[index] = _accumulates[index - 1] + Lengths[index - 1];
            _totalLength = _accumulates.Last();
            return _accumulates;
        }

        double ComputeTotalLength()
        {
            ComputeAccumulatedLength();
            return _totalLength.Value;
        }
    }
}