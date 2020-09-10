// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

using System.Windows.Media;

namespace Cure.WPF.Media
{
    static class GeometryExtension
    {
        /// <summary>
        /// 将给定的几何图形转换成单个 PathGeometry。
        /// </summary>
        public static PathGeometry AsPathGeometry(this Geometry original)
        {
            if (!(original is PathGeometry pathGeometry))
                pathGeometry = PathGeometry.CreateFromGeometry(original);
            return pathGeometry;
        }
    }
}
