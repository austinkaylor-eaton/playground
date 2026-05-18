using System.Diagnostics.CodeAnalysis;

namespace Core.Tests;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This class is used as a test dependency and should be instantiated for each test.")]
[SuppressMessage("SonarAnalyzer", "S2325", Justification = "This class is used as a test dependency and should be instantiated for each test.")]
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Subtract(int a, int b) => a - b;
    public int Multiply(int a, int b) => a * b;

    public double Divide(int a, int b) =>
        b == 0 ? throw new DivideByZeroException() : (double)a / b;
}