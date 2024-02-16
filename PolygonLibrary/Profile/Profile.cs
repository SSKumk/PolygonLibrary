using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;

namespace Profile;

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    // {
    //   double[,] A = new double[,] { { 10, 6, 2, 0 }, { 5, 1, -2, 4 }, { 3, 5, 1, -1 }, { 0, 6, -2, 2 } };
    //   double[]  b = new double[] { 25, 14, 10, 8 };
    //   GaussNaive((double[,])A.Clone(), (double[])b.Clone());
    //   GaussRowWise((double[,])A.Clone(), (double[])b.Clone());
    //   GaussColWise((double[,])A.Clone(), (double[])b.Clone());
    //   GaussAll((double[,])A.Clone(), (double[])b.Clone());
    // }
    {
      double[,] A = new double[,] { { 2, -9, 5 }, { 1.2, -5.3999, 6}, { 1, -1, -7.5 }};
      double[]  b = new double[] { -4, 0.6001, -8.5 };
      GaussNaive((double[,])A.Clone(), (double[])b.Clone());
      GaussRowWise((double[,])A.Clone(), (double[])b.Clone());
      GaussColWise((double[,])A.Clone(), (double[])b.Clone());
      GaussAll((double[,])A.Clone(), (double[])b.Clone());
    }
  }

  private static void GaussAll(double[,] A, double[] b) {
    Console.WriteLine("All-wise");
    GaussSLE.Solve(A, b, GaussSLE.GaussChoice.All, out double[] x);
    Console.WriteLine(string.Join(' ', x));
  }
  private static void GaussColWise(double[,] A, double[] b) {
    Console.WriteLine("Col-wise");
    GaussSLE.Solve(A, b, GaussSLE.GaussChoice.ColWise, out double[] x);
    Console.WriteLine(string.Join(' ', x));
  }

  private static void GaussRowWise(double[,] A, double[] b) {
    Console.WriteLine("Row-wise");
    GaussSLE.Solve(A, b, GaussSLE.GaussChoice.RowWise, out double[] x);
    Console.WriteLine(string.Join(' ', x));
  }

  private static void GaussNaive(double[,] A, double[] b) {
    GaussSLE.Solve(A, b, GaussSLE.GaussChoice.No, out double[] x);
    Console.WriteLine("Naive:");
    Console.WriteLine(string.Join(' ', x));
  }

  private static void CompareValues_AlgLib_naive() {
    const int dim   = 3;
    const int theta = 3;
    const int phi   = 4;
    const int r     = 1;

    var x = Sphere_list(dim, theta, phi, r);

    var y = GiftWrapping.WrapPolytop(x);


    y.WriteTXT($@"F:\Temp\LP-tests\Alglib\pic\S{dim}-{theta}-{phi}-{r}.txt");
    foreach (var hp in y.Faces.Select(F => F.HPlane).ToList()) {
      var p1 = new Point(hp.Normal).GetAsList();
      p1.Add(hp.ConstantTerm);
      Console.WriteLine(new Vector(p1.ToArray()).ToStringWithDiffBraces('{', '}'));
      Console.Write(", ");
    }
    Console.WriteLine("\n");

    foreach (var p in y.Vertices) {
      Console.WriteLine($"new double[] {new Vector(p).ToStringWithDiffBraces('{', '}')}");
      Console.Write(",");
    }
    Console.WriteLine("\n");
    Console.WriteLine(Vector.GenVector(dim));
  }

  private static void ProfileMinkSum() {
    Stopwatch timer = new Stopwatch();
    timer.Restart();

    // var P = GiftWrapping.WrapFaceLattice(Sphere_list(3, 4, 100, 1).Select(s => s.ExpandTo(4)));
    // var P = GiftWrapping.WrapFaceLattice(Sphere_list(3, 4, 100, 1));
    // var Q = GiftWrapping.WrapFaceLattice(Sphere_list(3, 100, 4, 1));
    // var Q = Simplex4D_FL;
    // var P = GiftWrapping.WrapFaceLattice(Sphere_list(3, 10, 10, 1));
    // var Q = GiftWrapping.WrapFaceLattice(Sphere_list(3, 20, 4, 1));
    var P = GiftWrapping.WrapFaceLattice(Cube5D_list);
    var Q = GiftWrapping.WrapFaceLattice(SimplexRND5D_list);
    Console.WriteLine($"GW = {timer.ElapsedMilliseconds}");

    const int N = 100;

    FaceLattice? x = null;
    timer.Restart();
    for (int i = 0; i < N; i++) {
      x = MinkSumSDas(P, Q);
    }
    Console.WriteLine($"MinkSumSDas = {timer.ElapsedMilliseconds}");

    FaceLattice? y = null;
    timer.Restart();
    for (int i = 0; i < N; i++) {
      y = MinkSumCH(P, Q);
    }
    Console.WriteLine($"MinkSumCH = {timer.ElapsedMilliseconds}");

    if (!x!.Equals(y)) { throw new ArgumentException("AAAAAA"); }
  }

}
