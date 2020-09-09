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

namespace Cure.WPF.Media
{
    /// <summary>
    /// 指定调用 <see cref="InvalidateGeometry" /> 的原因。
    /// </summary>
    [Flags]
    public enum InvalidateGeometryReasons
    {
        /// <summary>
        /// 几何图形已经失效，因为属性已经发生更改。
        /// </summary>
        PropertyChanged = 0x1,
        /// <summary>
        /// 几何图形已经失效，因为正在动态显示属性。
        /// </summary>
        IsAnimated = 0x2,
        /// <summary>
        /// 几何图形已经失效，因为子项已经失效。
        /// </summary>
        ChildInvalidated = 0x4,
        /// <summary>
        /// 几何图形已经失效，因为父项已经失效。
        /// </summary>
        ParentInvalidated = 0x8,
        /// <summary>
        /// 几何图形已经失效，因为已经应用了新模板。
        /// </summary>
        TemplateChanged = 0x10
    }
}