using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using DoubleDouble;
using Tests.ToolsTests;

// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";



  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    // ConvexPolytop A       = ConvexPolytop.Cube01(4).RotateRND();
    // ConvexPolytop B       = ConvexPolytop.Cube01(4).RotateRND();
    // ConvexPolytop polytop = MinkowskiSum.BySandipDas(A, B);

    ConvexPolytop polytop = ConvexPolytop.AsVPolytop(ConvexPolytop.Cube01(4).VRep.Union(new List<Vector>()
      {
        // new Vector(new double[]{0.5,0.5,0.5,2}),
        new Vector(new double[]{0.5,0.5,0.5,-2})
      }));
    // ConvexPolytop polytop = ConvexPolytop.Cube01(4);

    var           g       = ConvexPolytop.HRepToVRep_Geometric(polytop.HRep);
    // var           n       = ConvexPolytop.HRepToVRep_Naive(polytop.HRep);
    Console.WriteLine("p == g");
    Console.WriteLine(polytop.VRep.SetEquals(g));
    Console.WriteLine("p \\subset g");
    Console.WriteLine(polytop.VRep.IsSubsetOf(g));
    Console.WriteLine("g \\ p");
    Console.WriteLine(string.Join('\n', g.Except(polytop.VRep)));
    Console.WriteLine("p \\ g");
    Console.WriteLine(string.Join('\n', polytop.VRep.Except(g)));
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


    //
    // ConvexPolytop P = ConvexPolytop.Cube01(2);
    // ConvexPolytop Q = ConvexPolytop.AsFLPolytop(new List<Vector>()
    //   {
    //     Vector.Zero(2),
    //     Vector.Ones(2)
    //   });
    //
    // ConvexPolytop sum = MinkowskiSum.BySandipDas(P, Q);

    // for (int i = 3; i <= 7; i++) {
    //   ConvexPolytop P = ConvexPolytop.Cube01(i).RotateRND(true);
    //   ConvexPolytop Q = ConvexPolytop.Cube01(i).RotateRND(true);
    //   Console.WriteLine($"{i} | {string.Join(' ', MinkowskiSum.BySandipDas(P, Q).FVector)}");
    //   Console.Out.Flush();
    // }

    // SolverLDG.WriteSimplestTask_TerminalSet_GameItself(5, pathData);

    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simplestGame_5D");

    // solverLdg.Solve(true);

  }


}
