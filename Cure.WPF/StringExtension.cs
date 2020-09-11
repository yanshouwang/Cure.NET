// ---------------------------------------------------------------------------
//
// Copyright (c) 2020 yanshouwang.
//
// History:
//
// 2020/09/09: yanshouwang - Created.
//
// ---------------------------------------------------------------------------

using System.ComponentModel;

namespace Cure.WPF
{
    internal static class StringExtension
    {
        public static T GetValue<T>(this string str)
        {
            System.Type type = typeof(T);
            T value = (T)TypeDescriptor.GetConverter(type).ConvertFromString(str);
            return value;
        }
    }
}
