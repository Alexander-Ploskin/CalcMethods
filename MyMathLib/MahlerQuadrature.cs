using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMathLib
{
    public class ChebyshevGaussQuadrature
    {
        public const double LeftBorder = -1;
        public const double RightBorder = 1;
        public static readonly Func<double, double> Weight = x => 1 / Math.Sqrt(1 - Math.Pow(x, 2));

        public List<double> Nodes
            => Enumerable.Range(1, nodesCount).Select(x => Math.Cos(Math.PI * (2 * x - 1) / (2d * nodesCount))).ToList();

        public List<double> Coefficients => Enumerable.Range(0, nodesCount).Select(x => Math.PI / nodesCount * 1d).ToList();

        private readonly Func<double, double> function;
        private readonly int nodesCount;

        public ChebyshevGaussQuadrature(int nodesCount, Func<double, double> function)
        {
            this.function = function;
            this.nodesCount = nodesCount;
        }

        public double CalcIntegral()
        {
            var result = 0d;
            var coefficiens = Coefficients;
            for (int i = 0; i < Nodes.Count; ++i)
            {
                result += coefficiens[i] * function(Nodes[i]);
            }
            return result;
        }
    }
}
