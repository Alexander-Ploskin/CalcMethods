using System;

namespace MyMathLib
{
    internal static class PolynomeUtils
    {
        public static double CalculateValue(double[] polynome, double x)
        {
            var result = 0d;
            for (int i = 0; i < polynome.Length; ++i)
            {
                result += Math.Pow(x, i) * polynome[i];
            }
            return result;
        }
    }
}
