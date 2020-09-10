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
using System.Windows.Media;

namespace Cure.WPF.Media
{
    static class PathFigureExtension
    {
        /// <summary>
        /// 迭代给定图形内部的所有线段，并返回每个线段的正确起点。
        /// </summary>
        public static IEnumerable<PathSegmentData> AllSegments(this PathFigure figure)
        {
            if (figure != null && figure.Segments.Count > 0)
            {
                var startPoint = figure.StartPoint;
                foreach (var segment in figure.Segments)
                {
                    var lastPoint = segment.GetLastPoint();
                    yield return new PathSegmentData(startPoint, segment);
                    startPoint = lastPoint;
                }
            }
        }
    }
}
