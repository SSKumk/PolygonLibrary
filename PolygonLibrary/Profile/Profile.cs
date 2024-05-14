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

    int        dim = 3;
    ConvexPolytop polytop = ConvexPolytop.Sphere(dim,2,4,Vector.Zero(dim), 1);
    // Console.WriteLine("#1");
    // ConvexPolytop cutted  = ConvexPolytop.SectionByHyperPlaneAndProject(polytop, new HyperPlane(new Vector(new ddouble[]{1,1,1,1}),0.1));
    // Console.WriteLine("#2");
    // cutted.WriteTXT_3D(pathData + "sphere4D");

    // ConvexPolytop polytop = ConvexPolytop.AsVPolytop(Shift(ConvexPolytop.Cube01(3).VRep, Vector.Ones(3)));



    // ConvexPolytop A       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop B       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop polytop = MinkowskiSum.BySandipDas(A, B);
    // ConvexPolytop polytop = ConvexPolytop.AsVPolytop(ConvexPolytop.Cube01(4).VRep.Union(new List<Vector>()
    //   {
    //     new Vector(new ddouble[]{0.5,0.5,0.5,2}), // - в общем-то, это не нужно, чтобы сломать
    //     new Vector(new ddouble[]{0.5,0.5,0.5,-2})
    //   }));

    SimplexMethod sm = new SimplexMethod(polytop.HRep, i => 1);
    (SimplexMethod.SimplexMethodResultStatus a, ddouble b, ddouble[]? c) = sm.Solve(true);
    Console.WriteLine(new Vector(c!));

    // var           g       = ConvexPolytop.HRepToVRep_Geometric(polytop.HRep);
    // Console.WriteLine("polytop == geometricH2V");
    // Console.WriteLine(polytop.VRep.SetEquals(g));
    // Console.WriteLine("polytop \\subset geometricH2V");
    // Console.WriteLine(polytop.VRep.IsSubsetOf(g));
    // Console.WriteLine("geometricH2V \\ polytop");
    // Console.WriteLine(string.Join('\n', g.Except(polytop.VRep)));
    // Console.WriteLine("polytop \\ geometricH2V");
    // Console.WriteLine(string.Join('\n', polytop.VRep.Except(g)));


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
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simpleMotion");
    // SolverLDG solverLdg = new SolverLDG(pathData, "simplestGame_5D");

    // solverLdg.Solve(true);
  }

}
