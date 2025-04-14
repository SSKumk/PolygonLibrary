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
    int       dim              = 3;
    GRandomLC rnd        = new GRandomLC(1);
    var       P       = ConvexPolytop.SimplexRND(dim, true, rnd);
    var      Q = ConvexPolytop.RectAxisParallel(Vector.Zero(dim), Vector.Ones(dim));

    ConvexPolytop polytop = MinkowskiSum.BySandipDas(P, Q);

    var res = ConvexPolytop.HrepToVrep_Geometric(polytop.Hrep);
    Console.WriteLine($"polytop.Vrep == h2v: {polytop.Vrep.SetEquals(res)}");
    Console.WriteLine($"polytope: {polytop.Vrep.Count}");
    Console.WriteLine($"res: {res.Count}");







    // var       P       = ConvexPolytop.SimplexRND(2, false, rnd).LiftUp(3, 0).RotateRND(true, rnd);
    // var      Q = ConvexPolytop.RectAxisParallel(Vector.Zero(2), Vector.Ones(2)).LiftUp(3, 0);

    // ConvexPolytop Q = ConvexPolytop.RectAxisParallel(Vector.Zero(dim), Vector.Ones(dim));
    // ConvexPolytop Q = ConvexPolytop.SimplexRND(dim);
    //
    // polytop.WriteIn(new ParamWriter("F:/Temp/polytope.polytope"), ConvexPolytop.Rep.FLrep);
    //
    // var flrep         = HrepToFLrep.HrepToFLrep_Geometric(polytop.Hrep, dim);
    // var polytopeflrep = ConvexPolytop.CreateFromFaceLattice(flrep);
    // polytopeflrep.WriteIn(new ParamWriter("F:/Temp/polytopeflrep.polytope"), ConvexPolytop.Rep.FLrep);
    //
    // Console.WriteLine($"pFL == v2fl: {polytop.FLrep.Equals(flrep)}");
    // Console.WriteLine($"pVrep == h2v: {polytop.Vrep.SetEquals(ConvexPolytop.HrepToVrep_Geometric(polytop.Hrep)!)}");
    //
    // Console.WriteLine($"Polytope");
    // Console.WriteLine($"Number 3: {polytop.FLrep[^2].Count(node => node.Sub.Count == 3)}");
    // Console.WriteLine($"Number 4: {polytop.FLrep[^2].Count(node => node.Sub.Count == 4)}");
    // Console.WriteLine($"Number > 4: {polytop.FLrep[^2].Count(node => node.Sub.Count > 4)}");
    //
    // Console.WriteLine($"");
    //
    // Console.WriteLine($"V2FL");
    // Console.WriteLine($"Number 3: {polytopeflrep.FLrep[^2].Count(node => node.Sub.Count == 3)}");
    // Console.WriteLine($"Number 4: {polytopeflrep.FLrep[^2].Count(node => node.Sub.Count == 4)}");
    // Console.WriteLine($"Number > 4: {polytopeflrep.FLrep[^2].Count(node => node.Sub.Count > 4)}");
  }

}
