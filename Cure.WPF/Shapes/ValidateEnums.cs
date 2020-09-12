// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/11: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.Windows.Media;

namespace Cure.WPF.Shapes
{
    internal static class ValidateEnums
    {
        ///  Returns whether or not an enumeration instance a valid value.
        ///  This method is designed to be used with ValidateValueCallback, and thus
        ///  matches it's prototype. 
        ///  
        ///  Enumeration value to validate. 
        /// 
        ///  'true' if the enumeration contains a valid value, 'false' otherwise.  
        public static bool IsPenLineCapValid(object valueObject)
        {
            PenLineCap value = (PenLineCap)valueObject;

            return (value == PenLineCap.Flat) ||
                   (value == PenLineCap.Square) ||
                   (value == PenLineCap.Round) ||
                   (value == PenLineCap.Triangle);
        }

        ///  Returns whether or not an enumeration instance a valid value.
        ///  This method is designed to be used with ValidateValueCallback, and thus
        ///  matches it's prototype.
        ///  
        ///  Enumeration value to validate. 
        /// 
        ///  'true' if the enumeration contains a valid value, 'false' otherwise.  
        public static bool IsPenLineJoinValid(object valueObject)
        {
            PenLineJoin value = (PenLineJoin)valueObject;

            return (value == PenLineJoin.Miter) ||
                   (value == PenLineJoin.Bevel) ||
                   (value == PenLineJoin.Round);
        }
    }
}