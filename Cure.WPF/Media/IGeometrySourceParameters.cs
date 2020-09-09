// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.Windows.Media;

namespace Cure.WPF.Media
{
    /// <summary>
    /// 提供一个接口以描述 Shape 的参数。
    /// </summary>
    /// <remarks>
    /// 此接口是用于 Shape 和 GeometrySource 之间的通信的数据。通常，IShape 的具体实现将实现此接口，并将其传递到 GeometrySource.UpdateGeometry() 中，然后后者会将形状用作只读数据提供程序。
    /// </remarks>
    public interface IGeometrySourceParameters
    {
        Stretch Stretch { get; }
        Brush Stroke { get; }
        double StrokeThickness { get; }
    }
}