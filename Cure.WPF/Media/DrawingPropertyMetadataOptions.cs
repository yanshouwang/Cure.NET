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

namespace Cure.WPF.Media
{
    [Flags]
    enum DrawingPropertyMetadataOptions
    {
        None = 0,
        AffectsMeasure = 1,
        AffectsRender = 16, // 0x00000010
    }
}