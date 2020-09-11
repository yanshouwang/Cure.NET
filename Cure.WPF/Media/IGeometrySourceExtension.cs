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
using System.Windows;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 提供帮助程序扩展方法以使用 IGeometrySource 和参数。
    /// </summary>
    internal static class IGeometrySourceExtension
    {
        public static double GetHalfStrokeThickness(this IGeometrySourceParameters parameter)
        {
            if (parameter.Stroke != null)
            {
                double strokeThickness = parameter.StrokeThickness;
                if (!double.IsNaN(strokeThickness) && !double.IsInfinity(strokeThickness))
                    return Math.Abs(strokeThickness) / 2.0;
            }
            return 0.0;
        }

        public static GeometryEffect GetGeometryEffect(this IGeometrySourceParameters parameters)
        {
            if (!(parameters is DependencyObject dependencyObject))
                return null;
            GeometryEffect geometryEffect = GeometryEffect.GetGeometryEffect(dependencyObject);
            return geometryEffect == null || !dependencyObject.Equals(geometryEffect.Parent) ? null : geometryEffect;
        }
    }
}
