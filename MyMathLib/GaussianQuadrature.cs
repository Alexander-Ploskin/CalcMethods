using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMathLib
{
    public class GaussianQuadrature
    {
        public const double DefaultLeftBorder = -1;
        public const double DefaultRightBorder = 1;

        private readonly double leftBorder;
        private readonly double rightBorder;

        public List<double> Nodes
            => FindRoots.Polynomial(legendrePolynome).Select(x => x.Real)
                        .Select(x => 0.5 * (rightBorder - leftBorder) * x + 0.5 * (rightBorder + leftBorder)).ToList();

        public List<double> Coefficients
        {
            get
            {
                var nodes = FindRoots.Polynomial(legendrePolynome).Select(x => x.Real).ToList();
                var reducedDegreePolynome = CreateLegendrePolynome(nodes.Count - 1);
                var polynomeSquare = new Func<double, double>(x => Math.Pow(PolynomeUtils.CalculateValue(reducedDegreePolynome, x), 2));
                var coefficients = new List<double>(nodes.Count);
                for (int i = 0; i < nodes.Count; ++i)
                {
                    coefficients.Insert(i, 2 * (1 - Math.Pow(nodes[i], 2)) / (Math.Pow(nodes.Count, 2) * polynomeSquare(nodes[i])));
                }
                return coefficients.Select(x => 0.5 * (rightBorder - leftBorder) * x).ToList();
            }
        }

        private readonly double[] legendrePolynome;

        public GaussianQuadrature(int nodesCount, double leftBorder, double rightBorder)
        {
            legendrePolynome = CreateLegendrePolynome(nodesCount);
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
        }

        public double CalcIntegral(Func<double, double> func)
        {
            var nodes = Nodes;
            var coefficients = Coefficients;
            var result = 0d;
            for (int i = 0; i < nodes.Count; ++i)
            {
                result += coefficients[i] * func(nodes[i]);
            }
            return result;
        }


        private double[] CreateLegendrePolynome(int nodesCount)
        {
            return nodesCount == 0 ? new double[] { 1d } : CreateLegendrePolynomeRecursive(2, new double[1] { 1d }, new double[2] { 0d, 1d }, nodesCount);
        }

        private double[] CreateLegendrePolynomeRecursive(int counter, double[] first, double[] second, int nodesCount)
        {
            if (counter == nodesCount + 1)
            {
                return second;
            }

            var newPolynom = new double[counter + 1];
            var firstNumber = ((double)counter - 1) / counter;
            var secondNumber = (2 * (double)counter - 1) / counter;

            newPolynom[counter] = secondNumber * second[counter - 1];
            newPolynom[counter - 1] = secondNumber * second[counter - 2];
            for (var i = counter - 2; i > 0; i--)
            {
                newPolynom[i] = secondNumber * second[i - 1] - firstNumber * first[i];
            }

            newPolynom[0] = firstNumber * first[0] * (-1);

            return CreateLegendrePolynomeRecursive(counter + 1, second, newPolynom, nodesCount);
        }
    }
}
