using System.Diagnostics;
using System.Globalization;
using Tests.ToolsTests;

using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;

using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    // "E:\\Work\\CGLibrary\\CGLibrary\\Tests\\OtherTests\\LDG_Computations";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations";

  static void Main(string[] args) {
    // Tools.Eps = 1e-8;
    Tools.Eps = 1e-16;

    Console.WriteLine($"{0.GetHashCode()}");

     int dim = 5;

    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // string    t         = "5.10";
    // ParamReader prR = new ParamReader
    // ($"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{ftype}/Geometric/{eps}/{t}){solverLdg.FileName}.cpolytop");
    // ConvexPolytop polytop = ConvexPolytop.CreateFromReader(prR);
    // ConvexPolytop polytop = ConvexPolytop.Cube01_VRep(dim).GetInFLrep();
    // var           _       = polytop.Hrep;
    // ConvexPolytop polytop = MinkowskiSum.BySandipDas
    // (ConvexPolytop.SimplexRND(dim), ConvexPolytop.SimplexRND(dim));
    // HyperPlane hp = HyperPlane.Make3D_xyParallel(3);
    // Vector v = Vector.MakeOrth(dim, 1);
    // Console.WriteLine($"{hp.AffBasis.ProjectPointToSubSpace_in_OrigSpace(v)}");
    // Console.WriteLine($"{x.Equals(res)}");

    // Console.WriteLine($"{Vector.Ones(6).ToStringBraceAndDelim(null, null, ' ')}");

    // SolverLDG solverLdg = new SolverLDG(pathData, "SomeRND", false);
    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator");
    // // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");
    //
    // // solverLdg.Solve(true);
    // solverLdg.LoadGame(6,7);
    // var x = solverLdg.Euler(new Vector(new ddouble[]{4,0,2}), 6, 7);
    // Console.WriteLine($"{string.Join('\n', x)}");

    // solverLdg.WorkOutControl(1*Vector.MakeOrth(3,1), 0, out Vector p, out Vector q);
    // Console.WriteLine($"Управление P = {p}");
    // Console.WriteLine($"Управление Q = {q}");

  }

}


// string      t   = "3.10";
// ParamReader prP = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{t}) P {solverLdg.FileName}.cpolytop");
// ParamReader prQ = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{t}) Q {solverLdg.FileName}.cpolytop");
// ParamReader prW = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{ftype}/Geometric/{eps}/{t}){solverLdg.FileName}.cpolytop");
//
// ConvexPolytop P = ConvexPolytop.CreateFromReader(prP);
// ConvexPolytop Q = ConvexPolytop.CreateFromReader(prQ);
// ConvexPolytop W = ConvexPolytop.CreateFromReader(prW);
//
// var         x  = solverLdg.DoNextSection(W, P, Q);
// string tNext = "3.00)";
// ParamWriter pr = new ParamWriter($"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/ddouble/Geometric/1e-016/{tNext}{solverLdg.FileName}.cpolytop");
// x.WriteIn(pr);
