using System.Diagnostics;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Profile;

// Всё сломалось!
// var P = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 4, 100, 1));
// var Q = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 100, 4, 1));
class Program {

  static void Main(string[] args) {
    Stopwatch timer = new Stopwatch();
    timer.Restart();

    // var P = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 4, 100, 1).Select(s => s.ExpandTo(4)));
    // var P = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 4, 100, 1));
    // var Q = GiftWrapping.WrapFaceLattice(MakePointsOnSphere(3, 10, 20, 1));
    // var Q = Simplex4D_FL;

    var P = GiftWrapping.WrapFaceLattice(Cube5D_list);
    var Q = GiftWrapping.WrapFaceLattice(SimplexRND5D_list);
    Console.WriteLine($"GW = {timer.ElapsedMilliseconds}");

    timer.Restart();
    var x = MinkSumSDas(P, Q);
    Console.WriteLine($"MinkSumSDas = {timer.ElapsedMilliseconds}");

    timer.Restart();
    var y = MinkSumCH(P, Q);
    Console.WriteLine($"MinkSumCH = {timer.ElapsedMilliseconds}");

    if (!x.Equals(y)) { throw new ArgumentException("AAAAAA"); }
  }
  // public static void Main(string[] args) {
  //   Stopwatch timer = new Stopwatch();
  //   var       swarm = Cube(5,out _, new []{1,2,3,4,5}, 500, null, true);
  //   // var swarm = MakePointsOnSphere(5, 5, 10, 1);
  //   timer.Restart();
  //   var x = GiftWrapping.WrapPolytop(swarm);
  //   Console.WriteLine($"GW: {timer.ElapsedMilliseconds}");
  //   Console.WriteLine(x.Vertices.Count);
  // }

}
