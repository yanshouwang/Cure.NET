// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.Windows;

namespace Cure.WPF
{
    static class SizeExtension
    {
        public static Rect ToBounds(this Size size)
            => new Rect(0.0, 0.0, size.Width, size.Height);

        public static bool HasValidArea(this Size size)
            => size.Width > 0.0 && size.Height > 0.0 && !double.IsInfinity(size.Width) && !double.IsInfinity(size.Height);
    }
}
