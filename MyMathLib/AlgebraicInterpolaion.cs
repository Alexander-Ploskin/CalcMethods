using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyMathLib
{
    public class Polynom
    {
        public Polynom(int[] coefficients)
        {
            Coefficients = coefficients;
        }

        public static Polynom operator +(Polynom a, Polynom b)
        {
            if (b.Degree > a.Degree)
            {
                var temp = a;
                a = b;
                b = temp;
            }

            return a.Сoefficients.Take(a.Degree - b.Degree).Concat(a.Take)
        }

        public double[] Сoefficients { get; set; }
        public int Degree => Сoefficients.Length;
        public override string ToString() => Сoefficients.Aggregate((res, x) => res + $"+ {x}X^{Сoefficients.IndexOf(Degree - x)}").Trim(2);
        public double Result(double x) => Сoefficients.Aggregate((res, c) => res * x + c);
    }

    public static class AlgebraicInterpolaion
    {
        public enum Method
        {
            Lagrange, Newthon
        }

        static Polynom Lagrange(List<(double x, double fx)> sourceTable, int polynomDegree, double interpolationPoint)
        {
            var answer = 0d;
            for (var i = 0; i <= polynomDegree; i++)
            {
                double temp = 1.0;
                for (var j = 0; j <= polynomDegree; j++)
                {
                    if (j != i)
                    {
                        var number = sourceTable[j].x;
                        temp = temp * (interpolationPoint - number) / (sourceTable[i].x - number);
                    }
                }
                answer += temp * sourceTable[i].Item2;
            }
            return answer;
        }

        static Polynom Newthon()

        public static Polynom GetInterpolationPolynome(List<List<double>> sourceTable, int polynomDegree, double interpolationPoint, Method method)
        {

        }
    }
}
