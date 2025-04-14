using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using System.IO;
using DoubleDouble;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;


namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations/Other/";

  [Test]
  public void Sandboxx() {
    int       dim = 3;
    GRandomLC rnd = new GRandomLC(1);
    var       P   = ConvexPolytop.SimplexRND(dim, true, rnd);
    var       Q   = ConvexPolytop.RectAxisParallel(Vector.Zero(dim), Vector.Ones(dim));

    ConvexPolytop polytop = MinkowskiSum.BySandipDas(P, Q);

    var res = ConvexPolytop.HrepToVrep_Geometric(polytop.Hrep);
    Console.WriteLine($"polytop.Vrep == h2v: {polytop.Vrep.SetEquals(res)}");
    Console.WriteLine($"polytope: {polytop.Vrep.Count}");
    Console.WriteLine($"res: {res.Count}");

    //
    // int dim = 5;
    // var x   = Cube01(dim, out List<Vector> cube);
    // var y   = SimplexRND(dim, out _);
    // ShiftAndRotate(dim, ref cube, ref x);
    //
    // var xP = ConvexPolytop.CreateFromPoints(x);
    // var yP = ConvexPolytop.CreateFromPoints(y);
    // var xy = MinkowskiSum.BySandipDas(xP, yP);
    //
    // var h  = xy.Hrep;
    //
    // var hG = ConvexPolytop.CreateFromPoints(ConvexPolytop.HrepToVrep_Geometric(h));
    //
    // Console.WriteLine($"{xy.Vrep.SetEquals(hG.Vrep)}");
  }


}
