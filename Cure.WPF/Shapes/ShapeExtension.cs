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
    internal static class ShapeExtension
    {
        public static double GetStrokeThickness(this Shape shape)
        {
            System.Type type = shape.GetType();
            MethodInfo method = type.GetMethod("GetStrokeThickness", BindingFlags.Instance | BindingFlags.NonPublic);
            double obj = (double)method.Invoke(shape, null);
            return obj;
        }

        public static Pen GetPen(this Shape shape)
        {
            System.Type type = shape.GetType();
            MethodInfo method = type.GetMethod("GetPen", BindingFlags.Instance | BindingFlags.NonPublic);
            Pen obj = (Pen)method.Invoke(shape, null);
            return obj;
        }
    }
}
