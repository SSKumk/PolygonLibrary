using CGLibrary;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiSum {

  [Test]
  public void Cube_3D() {
    List<Point> p1 = Cube(4, out _);

    var sum = MinkSum(p1, p1);

    Console.WriteLine(string.Join(',', sum));
  }

}
