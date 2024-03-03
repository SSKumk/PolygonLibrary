using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using System.IO;
namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  [Test]
  public void MakePointsOnSphereTest() {
    var x = Sphere_list(3, 2, 100, 1);
    var y = new GiftWrapping(x);
    // var y = MakePointsOnSphere_3D(3, 4);
  }
}


