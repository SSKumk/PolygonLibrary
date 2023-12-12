using System.Diagnostics;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Profile;

// Всё сломалось!
// var P = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 4, 100, 1));
// var Q = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 100, 4, 1));
class Program {

  // private static void Sum
  static void Main(string[] args) {
    Stopwatch timer = new Stopwatch();
    timer.Restart();

    // var P = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 4, 100, 1).Select(s => s.ExpandTo(4)));
    // var P = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 4, 100, 1));
    // var Q = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 10, 20, 1));
    // var Q = Simplex4D_FL;

    var P = GiftWrapping.WrapFaceLattice(Cube5D_list);
    var Q = GiftWrapping.WrapFaceLattice(SimplexRND5D_list);
    Console.WriteLine($"GW = {timer.Elapsed.Milliseconds}");

    timer.Restart();
    var x = MinkSumSDas(P, Q);
    Console.WriteLine($"MinkSumSDas = {timer.Elapsed.Milliseconds}");

    timer.Restart();
    var y = MinkSumCH(P, Q);
    Console.WriteLine($"MinkSumCH = {timer.Elapsed.Milliseconds}");

    if (!x.Equals(y)) { throw new ArgumentException("AAAAAA"); }
  }

}
