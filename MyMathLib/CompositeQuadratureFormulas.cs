using System;
using System.Linq;

namespace MyMathLib
{
    public enum CalculationMethod
    {
        LeftRectangle,
        RightRectangle,
        CenterRectangle,
        Trapezoid,
        Simpson
    }

    public static class CompositeQuadratureFormulas
    {
        public static int GetAccuracyDegree(this CalculationMethod method)
        {
            return method switch
            {
                CalculationMethod.LeftRectangle => 0,
                CalculationMethod.RightRectangle => 0,
                CalculationMethod.CenterRectangle => 1,
                CalculationMethod.Trapezoid => 1,
                CalculationMethod.Simpson => 3,
                _ => throw new ArgumentException()
            };
        }

        public static double CalcIntegral(CalculationMethod method, double a, double b, int partitionsCount, Func<double, double> func)
        {
            return method switch
            {
                CalculationMethod.LeftRectangle => LeftRectangle(a, b, partitionsCount, func),
                CalculationMethod.RightRectangle => RightRectangle(a, b, partitionsCount, func),
                CalculationMethod.CenterRectangle => CenterRectangle(a, b, partitionsCount, func),
                CalculationMethod.Trapezoid => Trapezoid(a, b, partitionsCount, func),
                CalculationMethod.Simpson => Simpson(a, b, partitionsCount, func),
                _ => throw new ArgumentException()
            };
        }

        public static double CalcTheoreticalErrorForGrowingFunction(CalculationMethod method, double a, double b, double h, Func<double, double> func)
        {
            var constant = method switch
            {
                CalculationMethod.LeftRectangle => 1 / 2d,
                CalculationMethod.RightRectangle => 1 / 2d,
                CalculationMethod.CenterRectangle => 1 / 24d,
                CalculationMethod.Trapezoid => 1 / 12d,
                CalculationMethod.Simpson => 1 / 2880d,
                _ => throw new ArgumentException()
            };
            var accuracyDegree = method.GetAccuracyDegree();
            return constant * func(b) * (b - a) * Math.Pow(h, accuracyDegree);
        }

        static double LeftRectangle(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return step * Enumerable.Range(0, partitionsCount).Aggregate(0d, (res, i) => res + func(a + i * step));
        }

        static double RightRectangle(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return step * Enumerable.Range(0, partitionsCount).Aggregate(0d, (res, i) => res + func(a + (i + 1) * step));
        }

        static double CenterRectangle(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return step * Enumerable.Range(0, partitionsCount).Aggregate(0d, (res, i) => res + func(a + (i + 0.5) * step));
        }

        static double Trapezoid(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            return (step / 2d) * Enumerable.Range(1, partitionsCount - 1).Aggregate(func(a) + func(b), (res, i) => res + 2 * func(a + i * step));
        }

        static double Simpson(double a, double b, int partitionsCount, Func<double, double> func)
        {
            var step = (b - a) / partitionsCount;
            var midTermResult = Enumerable.Range(1, partitionsCount - 1).Aggregate(func(a) + func(b), (res, i) => res + 2 * func(a + i * step));
            return (step / 6d) * Enumerable.Range(0, partitionsCount)
                .Aggregate(midTermResult, (res, i) => res + 4 * func(a + (i + 0.5) * step));
        }

        public static double Refine(int d, double oldValue, double newValue, int partitionsCountFactor)
            => (Math.Pow(partitionsCountFactor, d + 1) * newValue - oldValue) / (Math.Pow(partitionsCountFactor, d + 1) - 1);
    }
}
