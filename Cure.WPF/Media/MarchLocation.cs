// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/11: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Collections.Generic;
using System.Windows;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 要与 PathMarch 算法进行通信的数据结构。
    /// </summary>
    internal class MarchLocation
    {
        public static MarchLocation Create(
            MarchStopReason reason,
            int index,
            double before,
            double after,
            double remain)
        {
            double rhs = before + after;
            return new MarchLocation()
            {
                Reason = reason,
                Index = index,
                Remain = remain,
                Before = MathUtil.EnsureRange(before, new double?(0.0), new double?(rhs)),
                After = MathUtil.EnsureRange(after, new double?(0.0), new double?(rhs)),
                Ratio = MathUtil.EnsureRange(MathUtil.SafeDivide(before, rhs, 0.0), new double?(0.0), new double?(1.0))
            };
        }

        /// <summary>
        /// 对此位置取样的原因。
        /// </summary>
        public MarchStopReason Reason { get; private set; }

        /// <summary>
        /// 折线点列表上的点的索引。
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 比率：[0，1]，始终为“之前”/“（之前和之后）”。
        /// </summary>
        public double Ratio { get; private set; }

        /// <summary>
        /// 停止点之前的弧长。非负数且小于 Length[index]。
        /// </summary>
        public double Before { get; private set; }

        /// <summary>
        /// 停止点之后的弧长。非负数且小于 Length[index]。
        /// </summary>
        public double After { get; private set; }

        /// <summary>
        /// 在步骤中要达到下一个停止点的剩余长度。正数表示前进。负数表示后退。
        /// </summary>
        public double Remain { get; private set; }

        /// <summary>
        /// 获取给定点列表上此 MarchLocation 的内插位置。
        /// </summary>
        public Point GetPoint(IList<Point> points)
            => GeometryUtil.Lerp(points[this.Index], points[this.Index + 1], this.Ratio);

        /// <summary>
        /// 获取给定法向矢量列表上此 MarchLocation 的内插法线方向。
        /// </summary>
        public Vector GetNormal(PolylineData polyline, double cornerRadius = 0.0)
            => polyline.SmoothNormal(this.Index, this.Ratio, cornerRadius);

        /// <summary>
        /// 获取此 MarchLocation 到整个折线的起点的弧长。
        /// </summary>
        public double GetArcLength(IList<double> accumulatedLengths)
            => MathUtil.Lerp(accumulatedLengths[this.Index], accumulatedLengths[this.Index + 1], this.Ratio);
    }
}