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
    /// 提供几何图形的源的基类。基于输入参数和布局边界生成并缓存几何图形。
    /// </summary>
    /// <remarks>
    /// 典型的实现将扩展 UpdateCachedGeometry() 以更新 this.cachedGeometry。然后，此基类将处理失效的内容，以管道方式传递到几何图形效果中，然后相对于布局边界进行缓存。实现应该尝试尽可能重新使用缓存的几何图形，以避免在呈现线程中进行重新构建。实现可以扩展 ComputeLogicalBounds，从而以不同的方式处理拉伸。
    /// </remarks>
    /// <typeparam name="TParameters">基类将要处理的几何图形源参数的类型。</typeparam>
    public abstract class GeometrySource<TParameters> : IGeometrySource where TParameters : IGeometrySourceParameters
    {
        bool _geometryInvalidated;
        /// <summary>
        /// 指定上次处理几何图形效果后产生的几何图形。
        /// </summary>
        protected Geometry CachedGeometry { get; set; }

        /// <summary>
        /// 将通过执行此函数来扩展提供几何图形的方法。在任何几何图形发生更改时返回 true。
        /// </summary>
        protected abstract bool UpdateCachedGeometry(TParameters parameters);

        /// <summary>
        /// 扩展处理拉伸模式的方法。默认值是始终使用 Stretch.Fill 和中心笔划。
        /// </summary>
        protected virtual Rect ComputeLogicalBounds(
          Rect layoutBounds,
          IGeometrySourceParameters parameters)
            => GeometryUtil.Inflate(layoutBounds, -parameters.GetHalfStrokeThickness());

        /// <summary>
        /// 在不清晰或不得已的情况下应用几何图形效果并更新 this.Geometry。否则将 this.Geometry 保留为 this.cachedGeometry。
        /// </summary>
        bool ApplyGeometryEffect(IGeometrySourceParameters parameters, bool force)
        {
            var flag = false;
            var geometry = CachedGeometry;
            var geometryEffect = parameters.GetGeometryEffect();
            if (geometryEffect != null)
            {
                if (force)
                {
                    flag = true;
                    geometryEffect.InvalidateGeometry(InvalidateGeometryReasons.ParentInvalidated);
                }
                if (geometryEffect.ProcessGeometry(CachedGeometry))
                {
                    flag = true;
                    geometry = geometryEffect.OutputGeometry;
                }
            }
            if (Geometry != geometry)
            {
                flag = true;
                Geometry = geometry;
            }
            return flag;
        }

        #region IGeometrySource

        /// <summary>
        /// 获取或设置在最新 UpdateGeometry() 之后生成的几何图形。
        /// </summary>
        public Geometry Geometry { get; private set; }
        /// <summary>
        /// 获取几何图形应拉伸到的范围框。实际的几何图形可能比这小，也可能比这大。<see cref="Cure.WPF.Media.GeometrySource`1.LogicalBounds" /> 应该已经考虑了笔划粗细和拉伸。
        /// </summary>
        public Rect LogicalBounds { get; private set; }
        /// <summary>
        /// 获取 FrameworkElement 的实际边界。<see cref="Cure.WPF.Media.GeometrySource`1.LayoutBounds" /> 包括逻辑边界、拉伸和笔划粗细。
        /// </summary>
        public Rect LayoutBounds { get; private set; }

        /// <summary>
        /// 通知几何图形因外部更改已经失效。
        /// </summary>
        /// <remarks>
        /// 当参数发生更改时几何图形通常会失效。无论布局边界是否发生更改，只要任何几何图形已在外部失效，就会重新计算该几何图形。
        /// </remarks>
        public bool InvalidateGeometry(InvalidateGeometryReasons reasons)
        {
            if ((reasons & InvalidateGeometryReasons.TemplateChanged) != 0)
                CachedGeometry = null;
            if (_geometryInvalidated)
                return false;
            _geometryInvalidated = true;
            return true;
        }

        /// <summary>
        /// 基于给定的参数和 layoutBounds 更新几何图形。如果几何图形未发生更改，则返回 false。
        /// </summary>
        public bool UpdateGeometry(IGeometrySourceParameters parameters, Rect layoutBounds)
        {
            var flag1 = false;
            if (parameters is TParameters newParameters)
            {
                var logicalBounds = ComputeLogicalBounds(layoutBounds, parameters);
                flag1 = ((flag1 ? 1 : 0) | (LayoutBounds != layoutBounds ? 1 : (LogicalBounds != logicalBounds ? 1 : 0))) != 0;
                if (_geometryInvalidated || flag1)
                {
                    LayoutBounds = layoutBounds;
                    LogicalBounds = logicalBounds;
                    var flag2 = flag1 | UpdateCachedGeometry(newParameters);
                    var num1 = flag2 ? 1 : 0;
                    var force = flag2;
                    var num2 = ApplyGeometryEffect(parameters, force) ? 1 : 0;
                    flag1 = (num1 | num2) != 0;
                }
            }
            _geometryInvalidated = false;
            return flag1;
        }

        #endregion
    }
}