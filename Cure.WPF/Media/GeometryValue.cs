using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Cure.WPF.Media
{
    class GeometryValue
    {
        public string FillRule { get; }

        public IList<GeometryCommand> Commands { get; }

        public GeometryValue(string fillRule, IList<GeometryCommand> commands)
        {
            FillRule = fillRule;
            Commands = commands;
        }

        public override string ToString()
        {
            return Commands.Aggregate(FillRule, (total, next) => $"{total} {next}").TrimStart();
        }
    }
}
