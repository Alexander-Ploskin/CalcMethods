using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Math.Integration;

namespace MyMathLib
{
    public static class Integration
    {
        public static double CalcIntegralExactly(double a, double b, Func<double, double> function)
            => NonAdaptiveGaussKronrod.Integrate(function, a, b);

        public static double CalcIntegralByInterpolationFormula(double a, double b, List<double> nodes, Func<double, double> function, Func<double, double> weight)
        {
            var moments = nodes.Select((_, i) => CalculateMoment(a, b, i, weight)).ToList();
            var coefficients = CalculateCoefficients(nodes, moments);

            return Enumerable.Range(0, nodes.Count).Aggregate(0d, (res, k) => res += coefficients[k] * function(nodes[k]));
        }

        private static double CalculateMoment(double a, double b, int k, Func<double, double> weight)
        {
            var intFunc = new Func<double, double>(x => weight(x) * Math.Pow(x, k));
            return CalcIntegralExactly(a, b, intFunc);
        }

        private static double[] CalculateCoefficients(List<double> nodes, List<double> moments)
        {
            var matrix = new double[nodes.Count, nodes.Count];
            for(int k = 0; k < nodes.Count; ++k)
            {
                for(int i = 0; i < nodes.Count; ++i)
                {
                    matrix[k, i] = Math.Pow(nodes[i], k);
                }
            }

            return Matrix.Solve(matrix, moments.ToArray());
        }
    }
}
