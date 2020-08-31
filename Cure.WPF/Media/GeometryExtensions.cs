using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    static class GeometryExtensions
    {
        public static GeometryValue ToValue(this Geometry geometry)
        {
            //var geo = PathGeometry.CreateFromGeometry(geometry);
            var str = geometry.ToString();
            var command = @"\w";
            var number = @"[+-]?\d+(?:\.\d+)?";
            var split = @"\s*[\s\,]\s*";
            var point = $@"{number}{split}{number}";
            var segment = $@"{command}\s*(?:{point}(?:{split}{point})*)?";
            var pattern = $@"\A\s*(?<FILLRULE>F[01])?\s*(?:(?<SEGMENT>{segment})\s*)*\z";
            var match = Regex.Match(str, @"[+-]?\d+(?:\.\d+)?");
            var fillGroup = match.Groups["FILLRULE"];
            var fillRule = fillGroup.Success ? fillGroup.Captures[0].Value : string.Empty;
            var captures = match.Groups["SEGMENT"].Captures;
            var commands = new List<GeometryCommand>();
            foreach (Capture capture in captures)
            {
                var item = capture.Value;
                var cmd = item[..1];
                var items = item[1..].Split(',', ' ');
                var args = new List<Point>();
                if (string.Compare(cmd, "Z", true) != 0)
                {
                    for (int i = 0; i < items.Length; i += 2)
                    {
                        var x = double.Parse(items[i]);
                        var y = double.Parse(items[i + 1]);
                        var arg = new Point(x, y);
                        args.Add(arg);
                    }
                }
                var ccc = new GeometryCommand(cmd, args);
                commands.Add(ccc);
            }
            var value = new GeometryValue(fillRule, commands);
            return value;
        }
    }
}
