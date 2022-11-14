namespace Lab4._2;

class Program
{
    const double DefaultLeftBorder = 1.5;
    const double DefaultRightBorder = 2.5;

    const string StringF = "f(x) = Ln(x) * Exp(x)";
    static readonly Func<double, double> F = x => Math.Log(x) * Math.Exp(x);
    const string StringF_0 = "f_0(x) = 4";
    static readonly Func<double, double> F_0 = x => 4;
    const string StringF_1 = "f_1(x) = 4 * x";
    static readonly Func<double, double> F_1 = x => 4 * x;
    const string StringF_2 = "f_2(x) = 4 * x^2 + 16 * x";
    static readonly Func<double, double> F_2 = x => 4 * Math.Pow(x, 2) + 16 * x;
    const string StringF_3 = "f_3(x) = 4 * x^3 + 16 * x^2 + 64 * x";
    static readonly Func<double, double> F_3 = x => 4 * Math.Pow(x, 3) + 16 * Math.Pow(x, 2) + 64 * Math.Pow(x, 3);

    static void Main(string[] args)
    {
        CUIHelpers.CUIHelpers.Introduce("4.2", "Integration problem solving with quadrature formulas", new string[] {
                StringF,
                "p(x) = 1",
                StringF_0,
                StringF_1,
                StringF_2,
                StringF_3,
                $"Default value for the left border: {DefaultLeftBorder}",
                $"Default value for the right border: {DefaultRightBorder}",
            });

        var leftBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x),
            (ex, res) => Console.WriteLine("The value is not correct"), DefaultLeftBorder, "Enter the value for the left border");
        var rightBorder = CUIHelpers.CUIHelpers.EnterParameter(i => double.Parse(i), x => double.IsFinite(x) && x >= leftBorder,
            (ex, res) => Console.WriteLine("The value is not correct"), DefaultRightBorder, "Enter the value for the right border");

        Console.WriteLine();
        CalcIntegral(leftBorder, rightBorder, StringF, F);
        CalcIntegral(leftBorder, rightBorder, StringF_0, F_0);
        CalcIntegral(leftBorder, rightBorder, StringF_1, F_1);
        CalcIntegral(leftBorder, rightBorder, StringF_2, F_2);
        CalcIntegral(leftBorder, rightBorder, StringF_3, F_3);
    }

    static void CalcIntegral(double a, double b, string stringFunc, Func<double, double> func)
    {
        Console.WriteLine($"Results for {stringFunc}");

        var exactValue = MyMathLib.Integration.CalcIntegralExactly(a, b, func);

        var leftRect = MyMathLib.QuadratureFormulasIntegration.LeftRectangle(a, b, func);
        var rightRect = MyMathLib.QuadratureFormulasIntegration.RightRectangle(a, b, func);
        var centerRect = MyMathLib.QuadratureFormulasIntegration.CenterRectangle(a, b, func);
        var trapezoid = MyMathLib.QuadratureFormulasIntegration.Trapezoid(a, b, func);
        var simpson = MyMathLib.QuadratureFormulasIntegration.Simpson(a, b, func);
        var formula3Slash8 = MyMathLib.QuadratureFormulasIntegration.Formula3Slash8(a, b, func);

        var leftRectError = Math.Abs(exactValue - leftRect);
        var rightRectError = Math.Abs(exactValue - rightRect);
        var centerRectError = Math.Abs(exactValue - centerRect);
        var trapezoidError = Math.Abs(exactValue - trapezoid);
        var simpsonError = Math.Abs(exactValue - simpson);
        var formula3slash8Error = Math.Abs(exactValue - formula3Slash8);

        Console.WriteLine($"Exact value: {exactValue}");

        Console.WriteLine($"Left rectangle: {leftRect}");
        Console.WriteLine($"Absolute error: {leftRectError}");
        Console.WriteLine($"Right rectangle: {rightRect}");
        Console.WriteLine($"Absolute error: {rightRectError}");
        Console.WriteLine($"Center rectangle: {centerRect}");
        Console.WriteLine($"Absolute error: {centerRectError}");
        Console.WriteLine($"Trapezoid: {trapezoid}");
        Console.WriteLine($"Absolute error: {trapezoidError}");
        Console.WriteLine($"Simpson: {simpson}");
        Console.WriteLine($"Absolute error: {simpsonError}");
        Console.WriteLine($"Formula 3/8: {formula3Slash8}");
        Console.WriteLine($"Absolute error: {formula3slash8Error}");

        Console.WriteLine();
    }
}