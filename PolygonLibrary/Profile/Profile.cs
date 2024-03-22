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

    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-1-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-1-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "Spheres3D");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");

    ConvexPolytop x = ConvexPolytop.ReadAsPoints
      (
       pathData +
       "Ep_MaterialDot-1-0.9_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2/5.80)materialDot1-0.9-supG"
      );

// x.WriteTXT_3D(
//               pathData +
//               "Ep_MaterialDot-1-0.9_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2/__7.00)materialDot1-0.9-supG"
//              );
    // solverLdg.Solve(true);
  }

}
