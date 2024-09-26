using System.Globalization;
using Tests.ToolsTests;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static CGLibrary.Geometry<double, Tests.DConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    // Tools.Eps = 1e-8;
    // string eps = "1e-008";
    // string ftype = "double";

    Tools.Eps = 1e-16;
    string eps   = "1e-016";
    string ftype = "ddouble";

    bool isDouble = ftype == "double";


    SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");

    solverLdg.Solve(false, false, isDouble);

    // Console.WriteLine($"LinBasis.ProjVector count = {LinearBasis.projCount}");

    // Console.WriteLine($"Average Contains in AffBasis = {AffineBasis.useContains / AffineBasis.createAff}");
  }

}



// string      t   = "3.10";
// ParamReader prP = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{t}) P {solverLdg.fileName}.cpolytop");
// ParamReader prQ = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{t}) Q {solverLdg.fileName}.cpolytop");
// ParamReader prW = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/{ftype}/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");
//
// ConvexPolytop P = ConvexPolytop.CreateFromReader(prP);
// ConvexPolytop Q = ConvexPolytop.CreateFromReader(prQ);
// ConvexPolytop W = ConvexPolytop.CreateFromReader(prW);
//
// var         x  = solverLdg.DoNextSection(W, P, Q);
// string tNext = "3.00)";
// ParamWriter pr = new ParamWriter($"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/ddouble/Geometric/1e-016/{tNext}{solverLdg.fileName}.cpolytop");
// x.WriteIn(pr);
