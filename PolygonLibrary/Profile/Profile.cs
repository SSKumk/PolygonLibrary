using System.Diagnostics;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Profile;
class Program {
  static void Main(string[] args) {
    Debug.Assert(false, "Очевидный дебаг ассерт");

    // var s = MakePointsOnSphere(4, 4, 100, 1);
    var s = Cube(5, out _, new List<int>() { 3 }, 100);
    var x = new GiftWrapping(s);
    System.Console.WriteLine("Hello!");
  }
}