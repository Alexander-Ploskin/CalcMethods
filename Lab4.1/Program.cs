namespace Lab4._1;

class Program
{
    const double DefaultLeftBorder = 0;
    const double DefaultRightBorder = 1;
    static readonly List<double> defaultNodes = new List<double> { 1D / 4D, 3D / 4D };
    static readonly Func<double, double> givenFunction = x => Math.Cos(x) * Math.Pow(x, 1D / 4D);
    static readonly Func<double, double> givenFunctionFx = x => Math.Cos(x);
    static readonly Func<double, double> givenFunctionPx = x => Math.Pow(x, 1D / 4D);

    static void Main(string[] args)
    {
        CUIHelpers.CUIHelpers.Introduce("4.1", "Integration problem", new string[] {
                "Given function: cos(x) * (x)^(1/4)",
                $"Default value for the left border: {DefaultLeftBorder}",
                $"Default value for the right border: {DefaultRightBorder}",
            });
        Console.WriteLine("Default nodes:");
        defaultNodes.ForEach(x => Console.WriteLine($"X_{defaultNodes.IndexOf(x)}: {x}"));

        Console.WriteLine();

        var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
            (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
        var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
            (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");
        var nodes = CUIHelpers.CUIHelpers.EnterParameters(i => double.Parse(i), x => double.IsFinite(x) && x > leftBorder && x < rightBorder,
            (ex, res) => Console.WriteLine("The value is not correct"), defaultNodes, "Enter nodes:").ToList();

        var approximateIntegral = MyMathLib.Integration.CalcIntegralByInterpolationFormula(leftBorder, rightBorder, nodes, givenFunctionFx, givenFunctionPx);
        var exactIntegral = MyMathLib.Integration.CalcIntegralExactly(leftBorder, rightBorder, givenFunction);
        
        Console.WriteLine($"Exact integral value: {exactIntegral}");
        Console.WriteLine($"Approximate integral value: {approximateIntegral}");

        var absoluteError = Math.Abs(exactIntegral - approximateIntegral);
        var relativeError = absoluteError / Math.Abs(approximateIntegral);

        Console.WriteLine($"Absolute error: {absoluteError}");
        Console.WriteLine($"Relative error: {relativeError}");
    }
}