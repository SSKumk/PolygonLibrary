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
  public void LowerGW() {
    HashSet<Point> S = new HashSet<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0, 0})
      , new Point(new double[] { 1, 0, 0, 0, 0})
      , new Point(new double[] { 0, 0, 1, 0, 0})
      , new Point(new double[] { 0, 0, 0, 1, 0})
      , new Point(new double[] { 1, 0, 1, 0, 0})
      , new Point(new double[] { 0, 0, 1, 1, 0})
      , new Point(new double[] { 1, 0, 0, 1, 0})
      , new Point(new double[] { 1, 0, 1, 1, 0})
      };

    GiftWrapping P = new GiftWrapping(S);

  }

}
