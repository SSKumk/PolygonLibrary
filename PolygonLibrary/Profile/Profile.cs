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


    SolverLDG solverLdg = new SolverLDG(pathData, "materialDot");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator-1-0.9");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");

    // Tools.Eps = 1e-16;
    // solverLdg.Solve(false, true, false);
    Tools.Eps = 1e-8;
    // solverLdg.Solve(false, false, true);

    string      t   = "5.30)";
    ParamReader prP = new ParamReader(pathData + $"{solverLdg.gd.ProblemName}/{t} P materialDot.cpolytop");
    ParamReader prQ = new ParamReader(pathData + $"{solverLdg.gd.ProblemName}/{t} Q materialDot.cpolytop");
    ParamReader prW = new ParamReader(pathData + $"{solverLdg.gd.ProblemName}/double/Geometric/1e-008/{t}materialDot.cpolytop");

    ConvexPolytop P = ConvexPolytop.CreateFromReader(prP);
    ConvexPolytop Q = ConvexPolytop.CreateFromReader(prQ);
    ConvexPolytop W = ConvexPolytop.CreateFromReader(prW);

    var wp_full = MinkowskiSum.BySandipDas(W, P);
    var wp_cutt = MinkowskiSum.BySandipDas(W, P, true);
    var wp_gw   = ConvexPolytop.CreateFromHalfSpaces(wp_cutt.Hrep);

    Console.WriteLine(wp_full.Equals(wp_gw));

    var sortedSet = wp_full.Hrep.Select(s => s.Normal).ToSortedSet();
    var set       = wp_cutt.Hrep.Select(s => s.Normal).ToSortedSet();
    var set2      = wp_gw.Hrep.Select(s => s.Normal).ToSortedSet();

    SortedSet<Vector> y = new SortedSet<Vector>(sortedSet);
    y.SymmetricExceptWith(set);
    Console.WriteLine(set.SetEquals(sortedSet));
    Console.WriteLine(set2.SetEquals(sortedSet));

    // var x = solverLdg.DoNextSection(W,P,Q);
    // Console.WriteLine(x.Vrep.Count);
  }

}
