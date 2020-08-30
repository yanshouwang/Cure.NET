using System.ComponentModel;

namespace Cure.WPF
{
    static class StringExtensions
    {
        public static T GetValue<T>(this string str)
        {
            var type = typeof(T);
            var value = (T)TypeDescriptor.GetConverter(type).ConvertFromString(str);
            return value;
        }
    }
}
