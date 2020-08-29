using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AOtter.WPF.Media
{
    public class GeometryCommand
    {
        public string Command { get; }
        public IList<Point> Args { get; }

        public GeometryCommand(string command, IList<Point> args)
        {
            Command = command;
            Args = args;
        }

        public override string ToString()
        {
            var args = Args.Aggregate(string.Empty, (total, next) => $"{total} {next}");
            return $"{Command}{args}";
        }
    }
}