using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMathLib
{
    public class CompoundGaussFormula
    {
        private readonly double leftBorder;
        private readonly double rightBorder;
        private readonly int nodesCount;
        private readonly int partitionsCount;
        private readonly Func<double, double> func;
        private readonly Func<double, double> weight;
        private readonly GaussianQuadrature gaussianQuadrature;
        private Func<double, double> fullFunction => x => func(x) * weight(x);
        private double step => (rightBorder - leftBorder) / (partitionsCount * 1d);

        public List<double> Nodes => gaussianQuadrature.Nodes;
        public List<double> Coefficients => gaussianQuadrature.Coefficients.Select(x => 0.5 * step * x).ToList();

        public CompoundGaussFormula(
            double leftBorder,
            double rightBorder,
            int nodesCount,
            int partitionsCount,
            Func<double, double> func,
            Func<double, double> weight)
        {
            this.leftBorder = leftBorder;
            this.rightBorder = rightBorder;
            this.nodesCount = nodesCount;
            this.partitionsCount = partitionsCount;
            this.func = func;
            this.weight = weight;
            gaussianQuadrature = new GaussianQuadrature(nodesCount, -1, 1);
        }

        public double CalculateIntegral()
        {
            var result = 0d;
            for (int j = 0; j < partitionsCount; ++j)
            {
                for (int k = 0; k < nodesCount; ++k)
                {
                    var coefficient = Coefficients[k];
                    var node = getNode(k, j);
                    result += coefficient * fullFunction(node);
                } 
            }
            return result;
        }

        private double getNode(int k, int j)
        {
            return 0.5 * step * Nodes[k] + 0.5 * (getZj(j) + getZj(j + 1));
        }

        private double getZj(int j) => leftBorder + j * step;
    }
}
