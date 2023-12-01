using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;

namespace Profile;
class Program {
  static void Main(string[] args) {
    var s = MakePointsOnSphere(4, 4, 1000, 1);
    // var s = Cube(5, out _, new List<int>() { 3 }, 100000);
    var x = new GiftWrapping(s);
  }
}