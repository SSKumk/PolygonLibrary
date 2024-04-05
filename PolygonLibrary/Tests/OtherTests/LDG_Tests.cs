using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using System.IO;
using NUnit.Framework;

using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.OtherTests;

public class LDG_Tests {

  private static readonly string pathData = "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  [Test]
  public void SimplestCube3D() {
    SolverLDG ldg = new SolverLDG(pathData, "Cubes3D");
    ldg.Solve(true);
  }

  [Test]
  public void SimplestCube4D() {
    SolverLDG ldg = new SolverLDG(pathData, "Cubes4D");
    ldg.Solve(false);
  }

  // (6 10 1.2)(7 11 1)(8 12 1)
  [Test]
  public void Spheres3D() {
    SolverLDG ldg = new SolverLDG(pathData, "Spheres3D");
    ldg.Solve(true);
  }

  // (12 20 2)(14 22 1)(16 24 1)
  [Test]
  public void Spheres3D_accurate() {
    SolverLDG ldg = new SolverLDG(pathData, "Spheres3D-accurate");
    ldg.Solve(true);
  }
}
