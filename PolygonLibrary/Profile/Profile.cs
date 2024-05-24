using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using CGLibrary;
// using DoubleDouble;
// using Tests.ToolsTests;
using MultiPrecision;
// using Tests;

// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<MultiPrecision.MultiPrecision<Tests.MultiPrecision.Pow2.N8>, Tests.MPConvertor>;
using static CGLibrary.Geometry<MultiPrecision.MultiPrecision<MultiPrecision.Pow2.N8>, Profile.MPConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Profile;



/// <summary>
/// Interface for multi-precision conversions.
/// </summary>
public class MPConvertor : INumConvertor<MultiPrecision<Pow2.N8>> {

  public static double             ToDouble(MultiPrecision<Pow2.N8> from) => (double)from;
  public static MultiPrecision<Pow2.N8> FromDouble(double           from) => from;
  public static int                ToInt(MultiPrecision<Pow2.N8>    from) => (int)from;
  public static MultiPrecision<Pow2.N8> FromInt(int                 from) => from;
  public static uint               ToUInt(MultiPrecision<Pow2.N8>   from) => (uint)from;
  public static MultiPrecision<Pow2.N8> FromUInt(uint               from) => from;

}

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    // MultiPrecision<MultiPrecision.Pow2.N8> x = MultiPrecision<MultiPrecision.Pow2.N8>.Pow2(2);
    // //
    // var y  = x + x;
    // Console.WriteLine(y);

    // Vector a = Vector.Zero(3);
    // Vector b = Vector.Ones(3);

    // Console.WriteLine(a + b);

    // int dim     = 3;
    // ConvexPolytop    polytop = ConvexPolytop.Sphere(dim, 2, 4, Vector.Zero(dim), 1);


    // string      matDot = "Ep_MaterialDot-1-0.9_T[4,7]_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2";
    // string      t      = "5.20)";
    // ParamReader pr     = new ParamReader($"{pathData}{matDot}/{t}materialDot1-0.9-supG.tsection");
    // pr.ReadNumber<ddouble>("t");
    // ConvexPolytop A = ConvexPolytop.AsFLPolytop(pr);
    // Console.WriteLine(A.VRep.Count);
    //
    // pr = new ParamReader($"{pathData}{matDot}/{t} P materialDot1-0.9-supG.tsection");
    // pr.ReadNumber<ddouble>("t");
    // ConvexPolytop P = ConvexPolytop.AsFLPolytop(pr);
    // Console.WriteLine(P.VRep.Count);
    //
    // pr = new ParamReader($"{pathData}{matDot}/{t} Q materialDot1-0.9-supG.tsection");
    // pr.ReadNumber<ddouble>("t");
    // ConvexPolytop Q = ConvexPolytop.AsVPolytop(pr);
    // Console.WriteLine(Q.VRep.Count);
    //
    // ConvexPolytop  x = MinkowskiSum.BySandipDas(A, Q);
    // ConvexPolytop? y = MinkowskiDiff.H2VGeometric(x, Q);
    //
    // Console.WriteLine(y!.VRep.Count);


    // ConvexPolytop A       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop B       = ConvexPolytop.SimplexRND(4).RotateRND();
    // ConvexPolytop pp      = MinkowskiSum.BySandipDas(A, B);
    // ConvexPolytop ppc      = MinkowskiSum.BySandipDas(A, B, true);
    // Console.WriteLine(pp.HRep.Count);
    // Console.WriteLine(ppc.HRep.Count);

    // ConvexPolytop polytop = ConvexPolytop.AsVPolytop(ConvexPolytop.Cube01(4).VRep.Union(new List<Vector>()
    //   {
    //     new Vector(new ddouble[]{0.5,0.5,0.5,2}), // - в общем-то, это не нужно, чтобы сломать
    //     new Vector(new ddouble[]{0.5,0.5,0.5,-2})
    //   }));


    // var           g       = ConvexPolytop.HRepToVRep_Geometric(polytop.HRep);
    // Console.WriteLine("polytop == geometricH2V");
    // Console.WriteLine(polytop.VRep.SetEquals(g));
    // Console.WriteLine("polytop \\subset geometricH2V");
    // Console.WriteLine(polytop.VRep.IsSubsetOf(g));
    // Console.WriteLine("geometricH2V \\ polytop");
    // Console.WriteLine(string.Join('\n', g.Except(polytop.VRep)));
    // Console.WriteLine("polytop \\ geometricH2V");
    // Console.WriteLine(string.Join('\n', polytop.VRep.Except(g)));


    // SolverLDG.WriteSimplestTask_TerminalSet_GameItself(5, pathData);

    SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
    // SolverLDG solverLdg = new SolverLDG(pathData, "oscillator1-0.9-supG");

    solverLdg.Solve(true);
  }

}
