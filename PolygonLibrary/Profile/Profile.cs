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

    // SolverLDG.WriteSimplestTask_TerminalSet_GameItself(3, pathData);
    // SolverLDG solverLdg1 = new SolverLDG(pathData, "simplestGame");
    // solverLdg1.Solve(true);
    // //
    // SolverLDG.WriteSimplestTask_Payoff_Supergraphic_2D(pathData);
    // SolverLDG solverLdg2 = new SolverLDG(pathData, "simplestSupergraphic");
    // solverLdg2.Solve(true);


    // todo ПРОТЕСТИРОВАТЬ различные сочетания GoalType и MType !!!



    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-1-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-1-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "Spheres3D");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");


    // solverLdg.Solve(true);

  }

}
