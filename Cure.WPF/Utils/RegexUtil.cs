using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cure.WPF.Utils
{
    static class RegexUtil
    {
        public static Regex Number
            => new Regex(@"[+-]?\d+(?:\.\d+)?");
        public static Regex GeometrySplit
            => new Regex(@"\s*[\s\,]\s*");
    }
}
