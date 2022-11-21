using System;
using System.Linq;

namespace MyMathLib
{
    public static class CompositeQuadratureFormulas
    {
        public static double CalcTheoreticalErrorForGrowingFunction(double constant, int accuracyDegree, double a, double b, double h, Func<double, double> func)
        {
            return constant * func(b) * (b - a) * Math.Pow(h, accuracyDegree);
        }

        public static double LeftRectangle(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return step * Enumerable.Range(0, partitionsCount).Aggregate(0d, (res, i) => res + func(a + i * step));
        }

        public static double RightRectangle(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return step * Enumerable.Range(0, partitionsCount).Aggregate(0d, (res, i) => res + func(a + (i + 1) * step));
        }

        public static double CenterRectangle(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return step * Enumerable.Range(0, partitionsCount).Aggregate(0d, (res, i) => res + func(a + (i + 0.5) * step));
        }

        public static double Trapezoid(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return (step / 2d) * Enumerable.Range(1, partitionsCount - 1).Aggregate(func(a) + func(b), (res, i) => res + 2 * func(a + i * step));
        }

        public static double Simpson(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            var midTermResult = Enumerable.Range(1, partitionsCount - 1).Aggregate(func(a) + func(b), (res, i) => res + 2 * func(a + i * step));
            return (step / 6d) * Enumerable.Range(0, partitionsCount)
                .Aggregate(midTermResult, (res, i) => res + 4 * func(a + (i + 0.5) * step));
        }
    }
}
