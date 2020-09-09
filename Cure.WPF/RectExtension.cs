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
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Cure.WPF
{
    static class RectExtension
    {
        public static Thickness Subtract(this Rect lhs, Rect rhs)
        {
            var left = rhs.Left - lhs.Left;
            var top = rhs.Top - lhs.Top;
            var right = lhs.Right - rhs.Right;
            var bottom = lhs.Bottom - rhs.Bottom;
            return new Thickness(left, top, right, bottom);
        }
    }
}
