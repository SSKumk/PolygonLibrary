using System.Globalization;
using Tests.ToolsTests;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


    string materialdot_path =
      pathData + "Ep_MaterialDot-1-0.9_T[4,7]_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2/";



    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator-1-0.9");
    SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");

    // Tools.Eps = 1e-16;
    // solverLdg.Solve(false, true, false);
    Tools.Eps = 1e-8;
    solverLdg.Solve(false, true, true);
  }

}
