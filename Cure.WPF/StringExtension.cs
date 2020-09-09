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
    static class StringExtension
    {
        public static T GetValue<T>(this string str)
        {
            var type = typeof(T);
            var value = (T)TypeDescriptor.GetConverter(type).ConvertFromString(str);
            return value;
        }
    }
}
