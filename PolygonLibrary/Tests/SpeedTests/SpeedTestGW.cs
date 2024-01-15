using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using DoubleDouble;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using System.Numerics;
using NUnit.Framework.Internal;
using Tests.ToolsTests;


namespace Tests.SpeedTests;

// dotnet test Tests\bin\Release\net8.0\Tests.dll --filter SpeedTestGW

[TestFixture]
public class SpeedTestGW {

  // [Test]
  // public void Circle2D_Double() { SphereBench<double, DConvertor>(2, 1, (int)1e6, "Circle2D-double"); }

  [Test]
  public void Circle2D_DoubleDouble() { SphereBench<ddouble, DDConvertor>(2, 1, (int)1e6, "Circle2D-double-double"); }

  // [Test]
  // public void Sphere3D_Double() { SphereBench<double, DConvertor>(3, 2, (int)1e3, "Sphere3D-double"); }

  [Test]
  public void Sphere3D_DoubleDouble() { SphereBench<ddouble, DDConvertor>(3, 2, (int)1e3, "Sphere3D-double-double"); }

  // [Test]
  // public void Sphere4D_Double() { SphereBench<double, DConvertor>(4, 2, (int)1e3, "Sphere4D-double"); }

  [Test]
  public void Sphere4D_DoubleDouble() { SphereBench<ddouble, DDConvertor>(4, 2, (int)1e3, "Sphere4D-double-double"); }

  // [Test]
  // public void Sphere5D_Double() { SphereBench<double, DConvertor>(5, 2, (int)1e3, "Sphere5D-double"); }

  [Test]
  public void Sphere5D_DoubleDouble() { SphereBench<ddouble, DDConvertor>(5, 2, (int)1e3, "Sphere5D-double-double"); }

  // [Test]
  // public void Cube2D_Double() { CubeBench<double, DConvertor>(2, (int)1e6, "Cube2D-double"); }

  [Test]
  public void Cube2D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(2, (int)1e6, "Cube2D-double-double"); }

  // [Test]
  // public void Cube3D_Double() { CubeBench<double, DConvertor>(3, (int)1e5, "Cube3D-double"); }

  [Test]
  public void Cube3D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(3, (int)1e5, "Cube3D-double-double"); }

  // [Test]
  // public void Cube4D_Double() { CubeBench<double, DConvertor>(4, (int)1e5, "Cube4D-double"); }

  [Test]
  public void Cube4D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(4, (int)1e5, "Cube4D-double-double"); }

  // [Test]
  // public void Cube5D_Double() { CubeBench<double, DConvertor>(5, (int)1e5, "Cube5D-double"); }

  [Test]
  public void Cube5D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(5, (int)1e5, "Cube5D-double-double"); }

  [Test]
  public void Cube6D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(6, (int)1e5, "Cube6D-double-double"); }

  [Test]
  public void Cube7D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(7, (int)1e5, "Cube7D-double-double"); }

  [Test]
  public void Cube8D_DoubleDouble() { CubeBench<ddouble, DDConvertor>(8, (int)1e5, "Cube8D-double-double"); }

  [Test]
  public void Cubes_DoubleDouble() {
    for (int i = 7; i <= 8; i++) {
      CubeBench<ddouble, DDConvertor>(i, (int)1e5, $"Cube{i}D");
    }
  }

  [Test]
  public void SimplicesRND_DoubleDouble() {
    for (int i = 2; i <= 8; i++) {
      SimplexRNDBench<ddouble, DDConvertor>(i, (int)1e5, $"SimplexRND{i}D");
    }
  }

  [Test]
  public void Spheres_DoubleDouble() {
    const int theta = 2;
    for (int i = 8; i <= 8; i++) {
      SphereBench<ddouble, DDConvertor>(i, theta, (int)1e2, $"Sphere{i}D-{theta}-n-1");
    }
  }

  public void SphereBench<TNum, TConv>(int dim, int thetaPoints, int maxAmount, string fileName)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>
    where TConv : INumConvertor<TNum> {
    Stopwatch          timer  = new Stopwatch();
    using StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/SpeedBench/" + fileName + ".txt");

    const int N = 10;
    // Берём окружность и начинаем туда напихивать точки
    for (int n = 10; n <= maxAmount; n *= 10) {
      // writer.WriteLine($"n = {n,-8}");

      // for (int k = 1; k <= maxAmount; k *= 10) {
      // var circleSmall = TestsPolytopes<TNum, TConv>.Sphere_list(dim, thetaPoints, k, TConv.FromDouble(0.5));
      var circle = TestsPolytopes<TNum, TConv>.Sphere_list(dim, thetaPoints, n, TConv.FromInt(1));
      // circle.AddRange(circleSmall);
      Geometry<TNum, TConv>.GiftWrapping? Circle = null;
      timer.Restart();
      for (int i = 0; i < N; i++) { Circle = new Geometry<TNum, TConv>.GiftWrapping(circle); }
      timer.Stop();

      writer.WriteLine
        (
         $"n = {n,-8} sec: {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture),-12}" +
         $" Total: {circle.Count,-8} inCH = {Circle!.Vertices.Count}"
        );
      writer.Flush();
    }
  }

  public void CubeBench<TNum, TConv>(int dim, int maxAmount, string fileName)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>
    where TConv : INumConvertor<TNum> {
    Stopwatch    timer  = new Stopwatch();
    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/SpeedBench/" + fileName + ".txt");

    for (int k = 1; k <= maxAmount; k *= 10) {
      var cube = TestsPolytopes<TNum, TConv>.Cube(dim, out _, new int[] { dim }, k);
      timer.Restart();
      var Cube = new Geometry<TNum, TConv>.GiftWrapping(cube);
      timer.Stop();
      writer.WriteLine
        (
         $"k = {k,-8} sec: {timer.Elapsed.TotalSeconds.ToString("F5", CultureInfo.InvariantCulture),-12}" +
         $" Total: {cube.Count,-8} inCH = {Cube.Vertices.Count}"
        );
      writer.Flush();
    }
  }


  public void SimplexRNDBench<TNum, TConv>(int dim, int maxAmount, string fileName)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>
    where TConv : INumConvertor<TNum> {
    Stopwatch    timer  = new Stopwatch();
    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/SpeedBench/" + fileName + ".txt");

    for (int k = 1; k <= maxAmount; k *= 10) {
      var simplexRND = TestsPolytopes<TNum, TConv>.SimplexRND(dim, out _, new int[] { dim }, k);
      timer.Restart();
      var Simplex = new Geometry<TNum, TConv>.GiftWrapping(simplexRND);
      timer.Stop();
      writer.WriteLine
        (
         $"k = {k,-8} sec: {timer.Elapsed.TotalSeconds.ToString("F5", CultureInfo.InvariantCulture),-12}" +
         $" Total: {simplexRND.Count,-8} inCH = {Simplex.Vertices.Count}"
        );
      writer.Flush();
    }
  }

  [Test]
  public void CubesWithPointsOnFaces() {
    const int nPoints = 100;
    const int N       = 5;

    Stopwatch timer = new Stopwatch();
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    for (int dim = 3; dim <= 6; dim++) {
      StreamWriter writer = new StreamWriter
        (Directory.GetCurrentDirectory() + "/SpeedBench/" + $"Cube{dim}D-withPoints" + ".txt");
      List<int> fIDs = Enumerable.Range(1, dim).ToList();

      for (int n = 1; n <= nPoints; n *= 10) {
        List<Point>   S = RotateRND(Cube(dim, out _, fIDs, n, 255, true));
        GiftWrapping? P = null;
        timer.Restart();
        for (int i = 0; i < N; i++) { P = new GiftWrapping(S); }
        timer.Stop();
        writer.WriteLine($"{P!.FaceLattice.NonZeroKFacesAmount}-{S.Count, -5} & {timer.Elapsed.TotalSeconds / N,-8:F5}");
        writer.Flush();
      }
    }
  }

}
