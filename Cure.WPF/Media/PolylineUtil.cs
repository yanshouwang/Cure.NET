// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/11: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 要使用点列表的帮助程序类
    /// </summary>
    internal static class PolylineUtil
    {
        /// <summary>
        /// 按给定间隔伸展给定折线，并通过回调输出每个停止点。
        /// </summary>
        /// <param name="polyline">要继续伸展的折线点。</param>
        /// <param name="startArcLength">在第一个点处停止之前要伸展的弧长。</param>
        /// <param name="cornerThreshold">要被视为顶角的最大边缘夹角。</param>
        /// <param name="stopCallback">伸展算法停止在某个点时进行回调。回调针对下一个停止点返回弧长。如果要求的长度为负值，则向后伸展。如果回调返回 NaN，则完成伸展。</param>
        public static void PathMarch(
            PolylineData polyline,
            double startArcLength,
            double cornerThreshold,
            Func<MarchLocation, double> stopCallback)
        {
            int num1 = polyline != null ? polyline.Count : throw new ArgumentNullException(nameof(polyline));
            if (num1 <= 1)
                throw new ArgumentOutOfRangeException(nameof(polyline));
            bool flag = false;
            double num2 = startArcLength;
            double before = 0.0;
            int index = 0;
            double num3 = Math.Cos(cornerThreshold * Math.PI / 180.0);
            while (true)
            {
                do
                {
                    double length1;
                    do
                    {
                        length1 = polyline.Lengths[index];
                        if (!MathUtil.IsFiniteDouble(num2))
                            return;
                        if (MathUtil.IsVerySmall(num2))
                        {
                            num2 = stopCallback(MarchLocation.Create(MarchStopReason.CompleteStep, index, before, length1 - before, num2));
                            flag = true;
                        }
                        else if (MathUtil.GreaterThan(num2, 0.0))
                        {
                            if (MathUtil.LessThanOrClose(num2 + before, length1))
                            {
                                before += num2;
                                num2 = stopCallback(MarchLocation.Create(MarchStopReason.CompleteStep, index, before, length1 - before, 0.0));
                                flag = true;
                            }
                            else if (index < num1 - 2)
                            {
                                ++index;
                                double num4 = length1 - before;
                                num2 -= num4;
                                before = 0.0;
                                if (flag && num3 != 1.0 && polyline.Angles[index] > num3)
                                {
                                    double length2 = polyline.Lengths[index];
                                    num2 = stopCallback(MarchLocation.Create(MarchStopReason.CornerPoint, index, before, length2 - before, num2));
                                }
                            }
                            else
                            {
                                double num4 = length1 - before;
                                double remain = num2 - num4;
                                double length2 = polyline.Lengths[index];
                                before = polyline.Lengths[index];
                                num2 = stopCallback(MarchLocation.Create(MarchStopReason.CompletePolyline, index, before, length2 - before, remain));
                                flag = true;
                            }
                        }
                    }
                    while (!MathUtil.LessThan(num2, 0.0));
                    if (MathUtil.GreaterThanOrClose(num2 + before, 0.0))
                    {
                        before += num2;
                        num2 = stopCallback(MarchLocation.Create(MarchStopReason.CompleteStep, index, before, length1 - before, 0.0));
                        flag = true;
                    }
                    else if (index > 0)
                    {
                        --index;
                        num2 += before;
                        before = polyline.Lengths[index];
                    }
                    else
                        goto label_21;
                }
                while (!flag || num3 == 1.0 || polyline.Angles[index + 1] <= num3);
                double length3 = polyline.Lengths[index];
                num2 = stopCallback(MarchLocation.Create(MarchStopReason.CornerPoint, index, before, length3 - before, num2));
                continue;
            label_21:
                double remain1 = num2 + before;
                double length4 = polyline.Lengths[index];
                before = 0.0;
                num2 = stopCallback(MarchLocation.Create(MarchStopReason.CompletePolyline, index, before, length4 - before, remain1));
                flag = true;
            }
        }

        /// <summary>
        /// 对给定的折线列表重新进行排序，使具有给定弧长的折线在列表中排在第一位。在此线条之前的折线将级联到列表末尾，第一条折线位于最后。
        /// </summary>
        /// <param name="lines">折线的列表。</param>
        /// <param name="startArcLength">整个折线列表中查找起始线所依据的弧长。在此变量中会返回该线条中的弧长。</param>
        /// <returns>重新排序和回绕后的列表。</returns>
        public static IEnumerable<PolylineData> GetWrappedPolylines(IList<PolylineData> lines, ref double startArcLength)
        {
            int num = 0;
            for (int index = 0; index < lines.Count; ++index)
            {
                num = index;
                startArcLength -= lines[index].TotalLength;
                if (MathUtil.LessThanOrClose(startArcLength, 0.0))
                    break;
            }
            if (!MathUtil.LessThanOrClose(startArcLength, 0.0))
                throw new ArgumentOutOfRangeException(nameof(startArcLength));
            startArcLength += lines[num].TotalLength;
            return lines.Skip(num).Concat(lines.Take(num + 1));
        }
    }
}