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
    internal class DrawingPropertyChangedEventArgs : EventArgs
    {
        public DrawingPropertyMetadata Metadata { get; set; }
        public bool Animated { get; set; }
    }
}