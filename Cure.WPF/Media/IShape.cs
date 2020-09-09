// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 提供定义 Shape 所需的接口。虽然原始形状和复合形状可能派生自不同类型的 FrameworkElement，但它们都需要与此接口匹配。
    /// </summary>
    public interface IShape
    {
        /// <summary>获取或设置指定如何绘制形状内部的 <see cref="System.Windows.Media.Brush" />。</summary>
        /// <returns>一个描述如何绘制形状内部的 <see cref="System.Windows.Media.Brush" />。默认值为 null。</returns>
        Brush Fill { get; set; }

        /// <summary>获取或设置指定如何绘制 <see cref="System.Windows.Shapes.Shape" /> 轮廓的 <see cref="System.Windows.Media.Brush" />。</summary>
        /// <returns>一个指定如何绘制 <see cref="System.Windows.Shapes.Shape" /> 轮廓的 <see cref="System.Windows.Media.Brush" />。</returns>
        Brush Stroke { get; set; }

        /// <summary>获取或设置 <see cref="System.Windows.Shapes.Shape" /> 笔划轮廓的宽度。 </summary>
        /// <returns><see cref="System.Windows.Shapes.Shape" /> 轮廓的宽度（以像素为单位）。</returns>
        double StrokeThickness { get; set; }

        /// <summary>获取或设置一个描述形状如何填充其分配空间的 <see cref="System.Windows.Media.Stretch" /> 枚举值。</summary>
        /// <returns><see cref="System.Windows.Media.Stretch" /> 枚举值之一。运行时的默认值取决于 <see cref="System.Windows.Shapes.Shape" /> 的类型。</returns>
        Stretch Stretch { get; set; }

        /// <summary>
        /// 获取呈现引擎提供的已呈现几何图形。
        /// </summary>
        Geometry RenderedGeometry { get; }

        /// <summary>
        /// 获取逻辑边界与实际几何图形边界之间的边距。这可能是正数（如在 <see cref="Microsoft.Expression.Shapes.Arc" /> 中），也可能是负数（如在 <see cref="Microsoft.Expression.Controls.Callout" /> 中）。
        /// </summary>
        Thickness GeometryMargin { get; }

        /// <summary>
        /// 在 RenderedGeometry 更改时发生。
        /// </summary>
        event EventHandler RenderedGeometryChanged;

        /// <summary>
        /// 使 <see cref="Microsoft.Expression.Media.IShape" /> 的几何图形无效。在失效之后，<see cref="Microsoft.Expression.Media.IShape" /> 将重新计算该几何图形，这将以异步方式发生。
        /// </summary>
        void InvalidateGeometry(InvalidateGeometryReasons reasons);
    }
}
