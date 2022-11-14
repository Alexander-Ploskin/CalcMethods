using System;
using System.Collections.Generic;
using System.Text;

namespace MyMathLib
{
    public static class QuadratureFormulasIntegration
    {
        public static double LeftRectangle(double a, double b, Func<double, double> func)
            => (b - a) * func(a);

        public static double RightRectangle(double a, double b, Func<double, double> func)
            => (b - a) * func(b);

        public static double CenterRectangle(double a, double b, Func<double, double> func)
            => (b - a) * func((b + a) / 2D);

        public static double Trapezoid(double a, double b, Func<double, double> func)
            => ((b - a) / 2D) * (func(a) + func(b));

        public static double Simpson(double a, double b, Func<double, double> func)
        {
            var coefficient = (b - a) / 6D;
            var element1 = func(a);
            var element2 = 4 * func((b + a) / 2D);
            var element3 = func(b);

            return coefficient * (element1 + element2 + element3);
        }

        public static double Formula3Slash8(double a, double b, Func<double, double> func)
        {
            var coefficient = b - a;
            var h = (b - a) / 3D;

            var element1 = (1D / 8D) * func(a);
            var element2 = (3D / 8D) * func(a + h);
            var element3 = (3D / 8D) * func(a + 2 * h);
            var element4 = (1D / 8D) * func(b);

            return coefficient * (element1 + element2 + element3 + element4);
        }
    }
}
