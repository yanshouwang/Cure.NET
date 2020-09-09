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
using System.Windows.Shapes;
using Cure.WPF.Media;

namespace Cure.WPF.Shapes
{
    /// <summary>
    /// 提供从平台形状中派生的形状的 WPF 实现。
    /// </summary>
    public abstract class PrimitiveShape : Shape, IGeometrySourceParameters, IShape
    {
        IGeometrySource _geometrySource;
        IGeometrySource GeometrySource
            => _geometrySource ??= CreateGeometrySource();

        static PrimitiveShape()
        {
            StretchProperty.OverrideMetadata(typeof(PrimitiveShape), new DrawingPropertyMetadata(Stretch.Fill, DrawingPropertyMetadataOptions.AffectsRender));
            StrokeThicknessProperty.OverrideMetadata(typeof(PrimitiveShape), new DrawingPropertyMetadata(1.0, DrawingPropertyMetadataOptions.AffectsRender));
        }

        void RealizeGeometry()
        {
            this.RenderedGeometryChanged?.Invoke(this, EventArgs.Empty);
        }

        #region 重写

        protected override sealed Geometry DefiningGeometry
            => GeometrySource.Geometry ?? Geometry.Empty;

        /// <summary>提供 Silverlight 布局传递的“度量值”部分的行为。类可以替代此方法以定义其自己的“度量值”传递行为。</summary>
        /// <returns>此对象根据其子对象分配大小的计算或者可能根据其他注意事项（如固定容器大小）确定的在布局过程中所需的大小。</returns>
        /// <param name="availableSize">此对象可以提供给子对象的可用大小。可以将值指定为无穷大 (<see cref="System.Double.PositiveInfinity" />)，以指明对象大小将调整为可用的任何内容的大小。</param>
        /// <remarks>
        /// 在 WPF 中，测量值替代利用 Shape.DefiningGeometry 进行工作，Shape.DefiningGeometry 并不始终跟预计的一样。有关详细信息，请参阅错误 99497，其中 WPF 默认情况下没有正确的测量值。
        ///
        /// 在 Silverlight 中，路径上的测量值替代的工作方式与原始形状的工作方式不相同。
        ///
        /// 返回的应该是此形状无需剪辑便可正确呈现的最小尺寸。默认情况下，呈现的形状可以小到一个点，因此会返回笔划粗细。
        /// </remarks>
        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(base.StrokeThickness, base.StrokeThickness);
        }

        /// <summary>提供 Silverlight 布局传递的“排列”部分的行为。类可以替代此方法以定义其自己的“排列”传递行为。</summary>
        /// <returns>在布局中排列该元素后使用的实际大小。</returns>
        /// <param name="finalSize">此对象排列自己及其子项所应使用的父项中的最终区域。</param>
        /// <remarks> <see cref="Cure.WPF.Shapes.PrimitiveShape" /> 将在 Geometry 失效后重新计算它，并更新 RenderedGeometry 和 GeometryMargin。</remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (GeometrySource.UpdateGeometry(this, finalSize.ToBounds()))
            {
                RealizeGeometry();
            }
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        #endregion

        /// <summary>
        /// 通过创建几何图形源来扩展形状的绘制方式。
        /// </summary>
        protected abstract IGeometrySource CreateGeometrySource();

        #region IShape

        /// <summary>
        /// 获取逻辑边界与实际几何图形边界之间的边距。这可能是正数（如在 <see cref="Cure.WPF.Shapes.Arc" /> 中），也可能是负数（如在 <see cref="Cure.WPF.Controls.Callout" /> 中）。
        /// </summary>
        public Thickness GeometryMargin
        {
            get
            {
                if (RenderedGeometry == null)
                {
                    return default;
                }
                return GeometrySource.LogicalBounds.Subtract(RenderedGeometry.Bounds);
            }
        }

        /// <summary>
        /// 在 RenderedGeometry 更改时发生。
        /// </summary>
        public event EventHandler RenderedGeometryChanged;

        /// <summary>
        /// 使 <see cref="Cure.WPF.Media.IShape" /> 的几何图形无效。在失效之后，<see cref="Cure.WPF.Media.IShape" /> 将重新计算该几何图形，这将以异步方式发生。
        /// </summary>
        public void InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if (GeometrySource.InvalidateGeometry(reasons))
            {
                InvalidateArrange();
            }
        }

        #endregion
    }
}
