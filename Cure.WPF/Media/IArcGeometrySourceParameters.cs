// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/10: yanshouwang Created.
//
// ---------------------------------------------------------------------------

namespace Cure.WPF.Media
{
    internal interface IArcGeometrySourceParameters : IGeometrySourceParameters
    {
        double StartAngle { get; }
        double EndAngle { get; }
        double ArcThickness { get; }
        UnitType ArcThicknessUnit { get; }
    }
}