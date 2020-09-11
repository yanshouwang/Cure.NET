// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// PathSegment 和相应 StartPoint 的元组数据结构。
    /// </summary>
    internal class PathSegmentData
    {
        public PathSegmentData(Point startPoint, PathSegment pathSegment)
        {
            this.PathSegment = pathSegment;
            this.StartPoint = startPoint;
        }

        public Point StartPoint { get; }
        public PathSegment PathSegment { get; }
    }
}