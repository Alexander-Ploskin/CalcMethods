using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyMathLib
{
    public static class AlgebraicInterpolaion
    {
        public enum Method
        {
            Lagrange, Newton
        }

        static double Lagrange(List<(double x, double fx)> sourceTable, int polynomDegree, double interpolationPoint)
            => Enumerable.Range(0, polynomDegree)
                         .Aggregate(0d, (res, i) => 
                            res += sourceTable[i].fx *
                                    Enumerable.Range(0, polynomDegree).Aggregate(1d, (res, j) =>
                                        i == j ? res : res * (interpolationPoint - sourceTable[j].x) / (sourceTable[i].x - sourceTable[j].x)));

        static double Newton(List<(double x, double fx)> sourceTable, int polynomDegree, double interpolationPoint)
        {
            var newtonTable = new double[polynomDegree + 2, polynomDegree + 1];
            for (var i = 0; i <= polynomDegree + 2; i++)
            {
                for (var j = 0; j < polynomDegree + 2 - i; j++)
                {
                    if (j == polynomDegree + 1)
                    {
                        continue;
                    }

                    if (i == 0)
                    {
                        newtonTable[i, j] = sourceTable[j].x;
                    }
                    else if (i == 1)
                    {
                        newtonTable[i, j] = sourceTable[j].fx;
                    }
                    else
                    {
                        newtonTable[i, j] = (newtonTable[i - 1, j + 1] - newtonTable[i - 1, j])
                          / (newtonTable[0, i + j - 1] - newtonTable[0, j]);
                    }
                }
            }

            return Enumerable.Range(1, polynomDegree + 1).Aggregate(0d, (res, i)
                => res + Enumerable.Range(1, i - 1).Aggregate(newtonTable[i, 0], (res, j)
                    => res * (interpolationPoint - sourceTable[j - 1].x)));
        }

        public static double GetInterpolationPolynome(List<(double, double)> sourceTable, int polynomDegree, double interpolationPoint, Method method)
        {
            return method switch
            {
                Method.Lagrange => Lagrange(sourceTable, polynomDegree, interpolationPoint),
                Method.Newton => Newton(sourceTable, polynomDegree, interpolationPoint),
                _ => throw new ArgumentException("Unknown value for Method")
            };
        }

        public static List<(double, double)> GetSourceTable(int sourceTableSize, (double a, double b) sector, Func<double, double> function)
        {
            if (sector.b <= sector.a)
            {
                throw new ArgumentException("Invalid sector");
            }
            var distance = (sector.b - sector.a) / sourceTableSize;
            return Enumerable.Range(0, sourceTableSize).Select(i =>
            {
                var point = sector.a + i * distance;
                return (point, function(point));
            }).ToList();
        }
    }
}
