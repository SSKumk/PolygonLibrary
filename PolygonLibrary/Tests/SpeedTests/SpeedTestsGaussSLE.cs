using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using DoubleDouble;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using NUnit.Framework.Internal;
using Tests.ToolsTests;

namespace Tests.SpeedTests;

public class SpeedTestsGaussSLE {

  private static Stopwatch InitTestTimer() {
    Stopwatch timer = new Stopwatch();
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    timer.Restart();
    {
      int p = 0;
      for (int i = 0; i < 10000; i++) { p++; }
    }
    timer.Stop();

    return timer;
  }

  [Test]
  public void PivotTests() {
    Stopwatch timer = InitTestTimer();
    StreamWriter writer = new StreamWriter
      (Directory.GetCurrentDirectory() + "/SpeedBench/LinearAlgebra/GaussSLE/PivotCompare-withCopyTime.txt");
    for (int k = 10; k <= 100; k *= 2) {
      ddouble[,]              A     = Matrix.GenNonSingular(k, -100, 100);
      ddouble[]               b     = GenArray(k, -100, 100);
      Func<int, int, ddouble> AFunc = (r, l) => A[r, l];
      Func<int, ddouble>      bFunc = r => b[r];

      writer.WriteLine($"Dim = {k}");

      GaussSLE gaussSLE = new GaussSLE(AFunc, bFunc, k, k);

      timer.Restart();
      gaussSLE.SetGaussChoice(GaussSLE.GaussChoice.No);
      gaussSLE.Solve();
      timer.Stop();
      writer.WriteLine($"No pivot: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      gaussSLE.SetSystem(AFunc, bFunc, k, k, GaussSLE.GaussChoice.RowWise);
      timer.Restart();
      gaussSLE.Solve();
      timer.Stop();
      writer.WriteLine($"Row-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      gaussSLE.SetSystem(AFunc, bFunc, k, k, GaussSLE.GaussChoice.ColWise);
      timer.Restart();
      gaussSLE.Solve();
      timer.Stop();
      writer.WriteLine($"Col-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      gaussSLE.SetSystem(AFunc, bFunc, k, k, GaussSLE.GaussChoice.All);
      timer.Restart();
      gaussSLE.Solve();
      timer.Stop();
      writer.WriteLine($"All-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      writer.WriteLine();
    }
  }

  [Test]
  public void CompareFunc() {
    Stopwatch timer = InitTestTimer();

    timer.Stop();
    StreamWriter writer = new StreamWriter
      (Directory.GetCurrentDirectory() + "/SpeedBench/LinearAlgebra/GaussSLE/CompareFunc.txt");

    const int N   = 1000;
    const int dim = 10;

    writer.WriteLine($"N = {N}, dim = {dim}");

    ddouble[,] A = Matrix.GenNonSingular(dim, -100, 100);
    ddouble[]  b = GenArray(dim, -100, 100);

    bool res;
    int  k = 0;
    writer.WriteLine("Flat solver:");
    timer.Restart();
    for (int i = 0; i < N; i++) {
      res = GaussSLE.Solve(A, b, GaussSLE.GaussChoice.RowWise, out ddouble[] _);
      if (res) { k++; }
    }
    timer.Stop();
    writer.WriteLine($"{timer.Elapsed.TotalSeconds / N:F8}");
    Console.WriteLine($"k = {k}");

    k = 0;
    writer.WriteLine("Function solver:");
    timer.Restart();
    for (int i = 0; i < N; i++) {
      res = GaussSLE.Solve((r, l) => A[r, l], r => b[r], dim, GaussSLE.GaussChoice.RowWise, out ddouble[] _);
      if (res) { k++; }
    }
    timer.Stop();
    writer.WriteLine($"{timer.Elapsed.TotalSeconds / N:F8}");
    Console.WriteLine($"k = {k}");

    writer.Close();
  }

}
