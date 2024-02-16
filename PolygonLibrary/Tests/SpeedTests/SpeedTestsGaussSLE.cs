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
      ddouble[,] A = Matrix.GenNonSingular(k, -100, 100);
      ddouble[]  b = GenArray(k, -100, 100);

      writer.WriteLine($"Dim = {k}");

      timer.Restart();
      GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.No, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"No pivot: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      timer.Restart();
      GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.RowWise, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"Row-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      timer.Restart();
      GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.ColWise, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"Col-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      timer.Restart();
      GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.ColWise, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"All-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      writer.WriteLine();
    }
  }

  [Test]
  public void PivotTests2() {
    Stopwatch timer = InitTestTimer();

    StreamWriter writer = new StreamWriter
      (Directory.GetCurrentDirectory() + "/SpeedBench/LinearAlgebra/GaussSLE/PivotCompare-NoCopyTime.txt");
    for (int k = 10; k <= 100; k *= 2) {
      ddouble[,] A     = Matrix.GenNonSingular(k, -100, 100);
      ddouble[]  b     = GenArray(k, -100, 100);
      ddouble[,] ASave = (ddouble[,])A.Clone();
      ddouble[]  bSave = (ddouble[])b.Clone();

      writer.WriteLine($"Dim = {k}");

      timer.Restart();
      GaussSLE.Solve(A, b, GaussSLE.GaussChoice.No, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"No pivot: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      A = (ddouble[,])ASave.Clone();
      b = (ddouble[])bSave.Clone();
      timer.Restart();
      GaussSLE.Solve(A, b, GaussSLE.GaussChoice.RowWise, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"Row-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      A = (ddouble[,])ASave.Clone();
      b = (ddouble[])bSave.Clone();
      timer.Restart();
      GaussSLE.Solve(A, b, GaussSLE.GaussChoice.ColWise, out ddouble[] _);
      timer.Stop();
      writer.WriteLine($"Col-wise: {timer.Elapsed.TotalSeconds:F8}s");
      writer.Flush();

      A = (ddouble[,])ASave.Clone();
      b = (ddouble[])bSave.Clone();
      timer.Restart();
      GaussSLE.Solve(A, b, GaussSLE.GaussChoice.ColWise, out ddouble[] _);
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
    // ddouble[,] ASave = (ddouble[,])A.Clone();
    // ddouble[]  bSave = (ddouble[])b.Clone();

    writer.WriteLine("Flat solver:");
    timer.Restart();
    for (int i = 0; i < N; i++) {
      GaussSLE.SolveImmutable(A, b, GaussSLE.GaussChoice.RowWise, out ddouble[] _);
    }
    timer.Stop();
    writer.WriteLine($"{timer.Elapsed.TotalSeconds / N:F8}");

    writer.WriteLine("Function solver:");
    timer.Restart();
    for (int i = 0; i < N; i++) {
      GaussSLE.Solve((r, l) => A[r, l], r => b[r], dim, GaussSLE.GaussChoice.RowWise, out ddouble[] _);
    }
    timer.Stop();
    writer.WriteLine($"{timer.Elapsed.TotalSeconds / N:F8}");

    writer.Close();
  }


}
