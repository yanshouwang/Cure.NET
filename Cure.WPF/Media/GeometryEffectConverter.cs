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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 提供字符串与几何图形效果之间的转换。
    /// </summary>
    /// <remarks>
    /// 此类启用类似 XAML 中的简要语法<code>GeometryEffect="Sketch"</code>. 创建几何图形效果实例的克隆，以便可将其用作资源。
    /// </remarks>
    public sealed class GeometryEffectConverter : TypeConverter
    {
        /// <summary>生成受支持几何图形效果的预设列表。</summary>
        static readonly Dictionary<string, GeometryEffect> _registeredEffects = new Dictionary<string, GeometryEffect>()
        {
            ["None"] = GeometryEffect.DefaultGeometryEffect,
            ["Sketch"] = (GeometryEffect)new SketchGeometryEffect()
        };

        /// <summary>
        /// 可以从字符串类型转换的 GeometryEffect。
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => typeof(string).IsAssignableFrom(sourceType);

        /// <summary>
        /// 可以转换为字符串类型的 GeometryEffect。
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => typeof(string).IsAssignableFrom(destinationType);

        /// <summary>
        /// 将字符串转换为几何图形效果。回退值是 null。
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            => value is string key && _registeredEffects.TryGetValue(key, out var geometryEffect)
            ? geometryEffect.CloneCurrentValue()
            : null;

        /// <summary>
        /// 将几何图形效果转换为字符串。回退值是 null。
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string).IsAssignableFrom(destinationType))
            {
                foreach (var registeredEffect in _registeredEffects)
                {
                    if ((registeredEffect.Value == null ? (value == null ? 1 : 0) : (registeredEffect.Value.Equals(value as GeometryEffect) ? 1 : 0)) != 0)
                        return registeredEffect.Key;
                }
            }
            return null;
        }
    }
}