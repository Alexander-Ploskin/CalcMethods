using System.Linq;
using System.Collections.Generic;
using System;

namespace MyMathLib
{
    public static class NumericalDifferentiation
    {
        public static List<(double, double)> FindDerivatives(List<(double, double)> sourceTable)
        {
            var h = sourceTable[1].Item1 - sourceTable[0].Item1;
            var lastIndex = sourceTable.Count - 1;
            return sourceTable.Skip(1).SkipLast(1).Select((x, i) => (x.Item1, (sourceTable[i + 2].Item2 - sourceTable[i].Item2) / (2 * h)))
                .Prepend((sourceTable[0].Item1, (-3 * sourceTable[0].Item2 + 4 * sourceTable[1].Item2 - sourceTable[2].Item2) / (2 * h)))
                .Append((sourceTable.Last().Item1, (3 * sourceTable.Last().Item2 - 4 * sourceTable[lastIndex - 1].Item2 + sourceTable[lastIndex - 2].Item2) / (2 * h)))
                .ToList();
        }

        public static List<(double, double?)> FindSecondDerivatives(List<(double, double)> sourceTable) 
        {
            var h = sourceTable[1].Item1 - sourceTable[0].Item1;
            return sourceTable.Skip(1).SkipLast(1)
                .Select<(double, double), (double, double?)>((x, i) => (x.Item1, (sourceTable[i + 2].Item2 - 2 * sourceTable[i + 1].Item2 + sourceTable[i].Item2) / (h * h)))
                .Prepend((sourceTable.First().Item1, null))
                .Append((sourceTable.Last().Item1, null))
                .ToList();
        }

        public static List<(double, double)> GetSourceTable(double startPoint, double step, int numberOfPoints, Func<double, double> function)
            => Enumerable.Range(0, numberOfPoints).Select(x => {
                var point = startPoint + step * x;
                return (point, function(point));
            }).ToList();
    }
}
