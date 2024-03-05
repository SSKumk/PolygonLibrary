using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;

namespace Profile;

class Program {

  public static double[] MakeOneVector(int dim) {
    double[] v = new double[dim];
    for (int i = 0; i < dim; i++) {
      v[i] = 1;
    }

    return v;
  }

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    var F = ConvexPolytop.Sphere(4, 5, 5, 2);
    var G = ConvexPolytop.Sphere(4, 4, 4, 1);

    ConvexPolytop? diff = MinkowskiDiff.Naive(F, G);
    Console.WriteLine($"diff is null: {diff is null}");
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
    // var P = GiftWrapping.WrapFaceLattice(Cube5D_list);
    // var Q = GiftWrapping.WrapFaceLattice(SimplexRND5D_list);
    // Console.WriteLine($"GW = {timer.ElapsedMilliseconds}");
    //
    // const int N = 100;
    //
    // FaceLattice? x = null;
    // timer.Restart();
    // for (int i = 0; i < N; i++) {
    //   x = MinkSumSDas(P, Q);
    // }
    // Console.WriteLine($"MinkSumSDas = {timer.ElapsedMilliseconds}");
    //
    // FaceLattice? y = null;
    // timer.Restart();
    // for (int i = 0; i < N; i++) {
    //   y = MinkSumCH(P, Q);
    // }
    // Console.WriteLine($"ByConvexHull = {timer.ElapsedMilliseconds}");
    //
    // if (!x!.Equals(y)) { throw new ArgumentException("AAAAAA"); }
  }

}
