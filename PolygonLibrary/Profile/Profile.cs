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
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    var         s4      = ConvexPolytop.Sphere(4, 10, 12, Vector.Zero(4), 1);
    AffineBasis aBasis3 = new AffineBasis(Vector.Zero(4),new LinearBasis(3,4));
    var         s3      = ConvexPolytop.AsVPolytop(aBasis3.ProjectPoints(s4.Vertices).ToHashSet(),true);
    s3.WriteTXT_3D(pathData + "s4proj_s3");

    // SolverLDG ldg = new SolverLDG(pathData, "Spheres3D");
    // ldg.Solve(true);
  }

}
