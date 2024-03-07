using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using System.IO;
using NUnit.Framework;

// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.OtherTests;

public class LDG_Tests {

  private static string pathData =
    "F:\\Works\\IMM\\Аспирантура\\_PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_computations";

  [Test]
  public void SimplestCube3D() {
    SolverLDG ldg = new SolverLDG(pathData, "simplest.c");
    ldg.Solve();
  }

}
