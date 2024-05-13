using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;

using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";



  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    // ConvexPolytop A       = ConvexPolytop.Cube01(4).RotateRND();
    // ConvexPolytop B       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop polytop = MinkowskiSum.BySandipDas(A, B);

    // ConvexPolytop polytop = ConvexPolytop.AsVPolytop(ConvexPolytop.Cube01(4).VRep.Union(new List<Vector>()
    //   {
    //     new Vector(new double[]{0.5,0.5,0.5,2}), // - в общем-то, это не нужно, чтобы сломать
    //     new Vector(new double[]{0.5,0.5,0.5,-2})
    //   }));
    // ConvexPolytop polytop = ConvexPolytop.Sphere(3,8,12,Vector.Zero(3), 1);

    // var           g       = ConvexPolytop.HRepToVRep_Geometric(polytop.HRep);
    // var           n       = ConvexPolytop.HRepToVRep_Naive(polytop.HRep);
    // Console.WriteLine("polytop == geometricH2V");
    // Console.WriteLine(polytop.VRep.SetEquals(g));
    // Console.WriteLine("polytop \\subset geometricH2V");
    // Console.WriteLine(polytop.VRep.IsSubsetOf(g));
    // Console.WriteLine("geometricH2V \\ polytop");
    // Console.WriteLine(string.Join('\n', g.Except(polytop.VRep)));
    // Console.WriteLine("polytop \\ geometricH2V");
    // Console.WriteLine(string.Join('\n', polytop.VRep.Except(g)));
    // Console.WriteLine(polytop.VRep.SetEquals(n));


    // ConvexPolytop A = ConvexPolytop.SimplexRND(6);
    // ConvexPolytop B = ConvexPolytop.SimplexRND(6);
    // ConvexPolytop sum = MinkowskiSum.BySandipDas(A, B);
    // ConvexPolytop sumCH = MinkowskiSum.ByConvexHull(A, B);
    // Console.WriteLine(string.Join(' ', A.FVector));
    // Console.WriteLine(string.Join(' ', B.FVector));
    // Console.WriteLine(string.Join(' ', sum.FVector));
    // Console.WriteLine(string.Join(' ', sumCH.FVector));
    // ConvexPolytop sumCutted = MinkowskiSum.BySandipDasCutted(A, B);
    // Console.WriteLine(sum.Vertices.SetEquals(sumCutted.Vertices));



    // SolverLDG.WriteSimplestTask_TerminalSet_GameItself(5, pathData);

    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simplestGame_5D");

    solverLdg.Solve(true);

  }


}
