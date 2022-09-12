using System;
using System.Collections.Generic;

namespace MyMathLib
{
    public class MethodResult
    {
        public MethodResult(double initialApproximation, int stepsCount, double root, double lastSecorLength, double residual)
        {
            InitialApproximation = initialApproximation;
            StepsCount = stepsCount;
            Root = root;
            LastSectorLength = lastSecorLength;
            Residual = residual;
        }

        public double InitialApproximation { get; }
        public int StepsCount { get; }
        public double Root { get; }
        public double LastSectorLength { get; }
        public double Residual { get; }
    }

    public static class NonLinearEquations
    {
        public enum Method
        {
            BisectionMetod, NewtonMethod, ModifiedNewtonMethod, SecantMethod
        }

        public static MethodResult FindRoot(Func<double, double> func,
            (double a, double b) section,
            double accuracy,
            Method method,
            Func<double, double> derivative = null)
        {
            if(section.b < section.a)
            {
                throw new ArgumentException("High border shouldn't be less than low border");
            }

            switch (method)
            {
                case Method.BisectionMetod:
                    return BisectionMethod(func, section, accuracy);
                case Method.NewtonMethod:
                    return NewtonMethod(func, derivative, section, accuracy);
                case Method.ModifiedNewtonMethod:
                    return ModifiedNewtonMethod(func, derivative, section, accuracy);
                case Method.SecantMethod:
                    return SecantMethod(func, section, accuracy);
                default:
                    throw new ArgumentException("Unknown method");
            }
        }

        public static IEnumerable<(double a, double b)> Split(Func<double, double> func, (double a, double b) section, int numberOfChunks)
        {
            var chunkLength = (section.b - section.a) / numberOfChunks;
            for(double a = section.a, b = a + chunkLength; a < section.b; a += chunkLength, b += chunkLength)
            {
                if(func(a) * func(b) <= 0)
                {
                    yield return (a, b);
                }
            }
        }

        #region BisectionMethod
        static (double a, double b) BisectionMethodCore(Func<double, double> func, (double a, double b) section, double accuracy, ref int counter)
        {
            while(section.b - section.a > 2 * accuracy)
            {
                counter++;
                var c = (section.a + section.b) / 2;
                section = func(section.a) * func(c) <= 0 ? (section.a, c) : (c, section.b);
            }
            return section;
        }

        static MethodResult BisectionMethod(Func<double, double> func, (double a, double b) section, double accuracy)
        {
            var counter = 0;
            var (a, b) = BisectionMethodCore(func, section, accuracy, ref counter);
            return new MethodResult((section.a + section.b) / 2, counter, (a + b) / 2, b - a, Math.Abs(func((a + b) / 2)));
        }
        #endregion

        #region NewtonMethod
        static (double x, double nextX) NewtonMethodCore(Func<double, double> func, Func<double, double> derivative, double x, double accuracy, ref int counter, int p = 1)
        {
            var prevPoint = 0d;
            var currentPoint = x;
            do
            {
                prevPoint = currentPoint;
                currentPoint = prevPoint - func(prevPoint) / derivative(prevPoint);
                counter++;
            } while(Math.Abs(currentPoint - prevPoint) > accuracy);
            return (prevPoint, currentPoint);
        }

        static MethodResult NewtonMethod(Func<double, double> func, Func<double, double> derivative, (double a, double b) section, double accuracy)
        {
            if (derivative == null)
            {
                throw new ArgumentException("Specify the derivative function to use Newthon method");
            }
            var counter = 0;
            var (x, nextX) = NewtonMethodCore(func, derivative, derivative(section.a) != 0 ? section.a : section.a + accuracy, accuracy, ref counter);
            return new MethodResult(section.a, counter, nextX, Math.Abs(x - nextX), Math.Abs(func(nextX)));
        }
        #endregion

        #region ModifiedNewtonMethod
        static MethodResult ModifiedNewtonMethod(Func<double, double> func, Func<double, double> derivative, (double a, double b) section, double accuracy)
        {
            if (derivative == null)
            {
                throw new ArgumentException("Specify the derivative function to use Newthon method");
            }
            var counter = 0;
            var (x, nextX) = NewtonMethodCore(func, x => derivative(section.a), derivative(section.a) != 0 ? section.a : section.a + accuracy, accuracy, ref counter);
            return new MethodResult(section.a, counter, nextX, Math.Abs(x - nextX), Math.Abs(func(nextX)));
        }
        #endregion

        #region SecantMethod
        static (double x, double nextX) SecantMethodCore(Func<double, double> func, double x, double nextX, double accuracy, ref int counter)
        {
            while(Math.Abs(x - nextX) > accuracy)
            {
                counter++;
                var temp = nextX;
                nextX = x - func(x) * (nextX - x) / (func(nextX) - func(x));
                x = temp;
            }
            return (x, nextX);
        }


        static MethodResult SecantMethod(Func<double, double> func, (double a, double b) section, double accuracy)
        {
            var counter = 0;
            var (x, nextX) = SecantMethodCore(func, section.a, section.b, accuracy, ref counter);
            return new MethodResult(section.a, counter, nextX, Math.Abs(x - nextX), Math.Abs(func(nextX)));
        } 
        #endregion
    }
}
