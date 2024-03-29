﻿using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using Accord.Math.Integration;
using MathNet.Numerics;

namespace MyMathLib
{
    public static class Integration
    {
        public static List<double> FindNodes(List<double> moments)
        {
            var nodesCount = moments.Count / 2;
            var matrix = new double[nodesCount, nodesCount];
            for (int k = 0; k < nodesCount; ++k)
            {
                for (int i = 0; i < nodesCount; ++i)
                {
                    matrix[k, i] = moments[nodesCount - 1 + k - i];
                }
            }

            var rightSide = moments.Skip(nodesCount).Select(x => -x).ToArray();
            var polynomeCoefficients = Matrix.Solve(matrix, rightSide).Prepend(1).Reverse().ToList();
            return FindRoots.Polynomial(polynomeCoefficients.ToArray()).Select(x => x.Real).ToList();
        }

        public static List<double> CalculateMoments(double a, double b, int numberOfNodes, Func<double, double> weight)
            => Enumerable.Range(0, numberOfNodes).Select(k => CalculateMoment(a, b, k, weight)).ToList();

        public static double CalcIntegralExactly(double a, double b, Func<double, double> function)
            => NonAdaptiveGaussKronrod.Integrate(function, a, b);

        public static double CalcIntegralByInterpolationFormula(double a, double b, List<double> nodes, Func<double, double> function, Func<double, double> weight)
        {
            var moments = nodes.Select((_, i) => CalculateMoment(a, b, i, weight)).ToList();
            var coefficients = CalculateCoefficients(nodes, moments);

            return CalcIntegral(nodes, coefficients.ToList(), function);
        }

        public static double CalcIntegral(List<double> nodes, List<double> coefficients, Func<double, double> function)
        {
            return Enumerable.Range(0, nodes.Count).Aggregate(0d, (res, k) => res += coefficients[k] * function(nodes[k]));
        }

        private static double CalculateMoment(double a, double b, int k, Func<double, double> weight)
        {
            var intFunc = new Func<double, double>(x => weight(x) * Math.Pow(x, k));
            return CalcIntegralExactly(a, b, intFunc);
        }

        public static double[] CalculateCoefficients(List<double> nodes, List<double> moments)
        {
            var matrix = new double[nodes.Count, nodes.Count];
            for (int k = 0; k < nodes.Count; ++k)
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    matrix[k, i] = Math.Pow(nodes[i], k);
                }
            }

            return Matrix.Solve(matrix, moments.Take(nodes.Count).ToArray());
        }
    }
}
