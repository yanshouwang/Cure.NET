// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.Reflection;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Cure.WPF.Shapes
{
    static class ShapeExtension
    {
        public static double GetStrokeThickness(this Shape shape)
        {
            var type = shape.GetType();
            var method = type.GetMethod("GetStrokeThickness", BindingFlags.Instance | BindingFlags.NonPublic);
            var obj = (double)method.Invoke(shape, null);
            return obj;
        }

        public static Pen GetPen(this Shape shape)
        {
            var type = shape.GetType();
            var method = type.GetMethod("GetPen", BindingFlags.Instance | BindingFlags.NonPublic);
            var obj = (Pen)method.Invoke(shape, null);
            return obj;
        }
    }
}
