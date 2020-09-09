// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 提供一个描述几何图形的源的接口。
    /// </summary>
    /// <remarks>
    /// 此接口旨在以非泛型方法公开几何图形源。典型的实现应实现子类 GeometrySource，而不是直接实现此接口。
    /// </remarks>
    public interface IGeometrySource
    {
        /// <summary>
        /// 获取或设置在最新 UpdateGeometry() 之后生成的几何图形。
        /// </summary>
        Geometry Geometry { get; }
        /// <summary>
        /// 获取几何图形应拉伸到的范围框。实际的几何图形可能比这小，也可能比这大。<see cref="P:Microsoft.Expression.Media.IGeometrySource.LogicalBounds" /> 应该已经考虑了笔划粗细和拉伸。
        /// </summary>
        Rect LogicalBounds { get; }
        /// <summary>
        /// 获取 FrameworkElement 的实际边界。<see cref="P:Microsoft.Expression.Media.IGeometrySource.LayoutBounds" /> 包括逻辑边界、拉伸和笔划粗细。
        /// </summary>
        Rect LayoutBounds { get; }
        /// <summary>
        /// 通知几何图形因外部更改已经失效。
        /// </summary>
        /// <remarks>
        /// 当参数发生更改时几何图形通常会失效。如果任何几何图形已在外部失效，就会重新计算该几何图形，即使布局边界发生更改也不例外。
        /// </remarks>
        bool InvalidateGeometry(InvalidateGeometryReasons reasons);
        /// <summary>
        /// 使用给定的参数和布局边界更新几何图形。如果未更新任何内容，则返回 false。
        /// </summary>
        bool UpdateGeometry(IGeometrySourceParameters parameters, Rect layoutBounds);
    }
}
