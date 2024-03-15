using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  static void Main(string[] args) {
    // CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    // var         s4      = ConvexPolytop.Sphere(4, 10, 12, Vector.Zero(4), 1);
    // AffineBasis aBasis3 = new AffineBasis(Vector.Zero(4),new LinearBasis(3,4));
    // var         s3      = ConvexPolytop.AsVPolytop(aBasis3.ProjectPoints(s4.Vertices).ToHashSet(),true);
    // s3.WriteTXT_3D(pathData + "s4proj_s3");

    // ConvexPolytop s4_1 = ConvexPolytop.Sphere(4,10,20);

    // SolverLDG ldg = new SolverLDG(pathData, "Spheres3D");
    // ldg.Solve(true);

    // ConvexPolytop.Sphere(5, 20, 30, Vector.Zero(5), 1);
    SolverLDG.WriteSimplestFile(3, pathData);
    SolverLDG solverLdg = new SolverLDG(pathData, "simplest");
    solverLdg.Solve(true);

    // ConvexPolytop x = ConvexPolytop.AsVPolytop(new HashSet<Vector>() { Vector.Ones(3) });
  }

}
