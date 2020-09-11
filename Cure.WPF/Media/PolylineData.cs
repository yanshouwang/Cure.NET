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

namespace Cure.WPF.Media
{
    /// <summary>
    /// 表示由一系列点连接而成的折线。闭合多边形是通过将第一个点与最后一个点重合来表示的。根据需要计算差值、法线、角度和长度。
    /// </summary>
    internal class PolylineData
    {
        private IList<Vector> _normals;
        private IList<double> _angles;
        private IList<double> _lengths;
        private IList<double> _accumulates;
        private double? _totalLength;

        /// <summary>
        /// 用两个或更多个点构成折线。
        /// </summary>
        /// <param name="points"></param>
        public PolylineData(IList<Point> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            this.Points = points.Count > 1 ? points : throw new ArgumentOutOfRangeException(nameof(points));
        }

        /// <summary>
        /// 第一个点与最后一个点重叠时，折线发生闭合。
        /// </summary>
        public bool IsClosed
            => this.Points[0] == this.Points.Last();
        /// <summary>
        /// 此折线中的点的计数。
        /// </summary>
        public int Count
            => this.Points.Count;
        /// <summary>
        /// 此折线的总弧长。
        /// </summary>
        public double TotalLength
            => this._totalLength ?? this.ComputeTotalLength();
        /// <summary>
        /// 此折线的点数组。
        /// </summary>
        public IList<Point> Points { get; }
        /// <summary>
        /// 线段（Points[i] 到 Points[i+1]）的长度。
        /// </summary>
        public IList<double> Lengths
            => this._lengths ?? this.ComputeLengths();
        /// <summary>
        /// 每个线段的法向矢量的列表。Normals[i] 是线段（p[i] 到 p[i + 1]）的法线。Normals[N-1] == Normals[N-2]。
        /// </summary>
        public IList<Vector> Normals
            => this._normals ?? this.ComputeNormals();
        /// <summary>
        /// 以点 p[i] 为顶点的两个线段的夹角的 Cos(angle) 列表。注意：值为 cos(angle) = Dot(u, v)。单位不是度。
        /// </summary>
        public IList<double> Angles
            => this._angles ?? this.ComputeAngles();
        /// <summary>
        /// 从 points[i] 到 points[0] 之间的累计长度的列表。
        /// </summary>
        public IList<double> AccumulatedLength
            => this._accumulates ?? this.ComputeAccumulatedLength();

        /// <summary>
        /// 折线的前向差分矢量。Points[i] + Differences[i] = Points[i+1]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector Difference(int index)
            => this.Points[(index + 1) % this.Count].Subtract(this.Points[index]);

        /// <summary>
        /// 计算给定位置的法向矢量 (lerp(index, index+1, fraction)。如果该位置位于 cornerRadius 范围内，请内插法线方向。
        /// </summary>
        /// <param name="cornerRadius">法线平滑度的范围。如果为零，则没有平滑度并针对索引返回确切的法线。</param>
        public Vector SmoothNormal(int index, double fraction, double cornerRadius)
        {
            if (cornerRadius > 0.0)
            {
                double length = this.Lengths[index];
                if (MathUtil.IsVerySmall(length))
                {
                    int index1 = index - 1;
                    if (index1 < 0 && this.IsClosed)
                        index1 = this.Count - 1;
                    int index2 = index + 1;
                    if (this.IsClosed && index2 >= this.Count - 1)
                        index2 = 0;
                    return index1 < 0 || index2 >= this.Count ? this.Normals[index] : GeometryUtil.Lerp(this.Normals[index2], this.Normals[index1], 0.5).Normalized();
                }
                double num = Math.Min(cornerRadius / length, 0.5);
                if (fraction <= num)
                {
                    int index1 = index - 1;
                    if (this.IsClosed && index1 == -1)
                        index1 = this.Count - 1;
                    if (index1 >= 0)
                    {
                        double alpha = (num - fraction) / (2.0 * num);
                        return GeometryUtil.Lerp(this.Normals[index], this.Normals[index1], alpha).Normalized();
                    }
                }
                else if (fraction >= 1.0 - num)
                {
                    int index1 = index + 1;
                    if (this.IsClosed && index1 >= this.Count - 1)
                        index1 = 0;
                    if (index1 < this.Count)
                    {
                        double alpha = (fraction + num - 1.0) / (2.0 * num);
                        return GeometryUtil.Lerp(this.Normals[index], this.Normals[index1], alpha).Normalized();
                    }
                }
            }
            return this.Normals[index];
        }

        private IList<double> ComputeLengths()
        {
            this._lengths = new double[this.Count];
            for (int index = 0; index < this.Count; ++index)
                this._lengths[index] = this.Difference(index).Length;
            return this._lengths;
        }

        private IList<Vector> ComputeNormals()
        {
            this._normals = (new Vector[this.Points.Count]);
            for (int index = 0; index < this.Count - 1; ++index)
                this._normals[index] = GeometryUtil.Normal(this.Points[index], this.Points[index + 1]);
            this._normals[this.Count - 1] = this._normals[this.Count - 2];
            return this._normals;
        }

        private IList<double> ComputeAngles()
        {
            this._angles = new double[this.Count];
            for (int index = 1; index < this.Count - 1; ++index)
                this._angles[index] = -GeometryUtil.Dot(this.Normals[index - 1], this.Normals[index]);
            this._angles[0] = !this.IsClosed ? (this._angles[this.Count - 1] = 1.0) : (this._angles[this.Count - 1] = -GeometryUtil.Dot(this.Normals[0], this.Normals[this.Count - 2]));
            return this._angles;
        }

        private IList<double> ComputeAccumulatedLength()
        {
            this._accumulates = new double[this.Count];
            this._accumulates[0] = 0.0;
            for (int index = 1; index < this.Count; ++index)
                this._accumulates[index] = this._accumulates[index - 1] + this.Lengths[index - 1];
            this._totalLength = this._accumulates.Last();
            return this._accumulates;
        }

        private double ComputeTotalLength()
        {
            this.ComputeAccumulatedLength();
            return this._totalLength.Value;
        }
    }
}