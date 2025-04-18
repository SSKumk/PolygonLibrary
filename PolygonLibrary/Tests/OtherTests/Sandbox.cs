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
// using static CGLibrary.Geometry<Rationals.Rational, Tests.RConvertor>;


namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations/Other/";

  [Test]
  public void Sandboxx() {
    int       dim              = 5;
    // GRandomLC rnd        = new GRandomLC(0);

    ConvexPolytop Q = ConvexPolytop.SimplexRND(dim);
    // var      Q = ConvexPolytop.SimplexRND(2, false, rnd).LiftUp(4, 0);
    // ConvexPolytop Q = ConvexPolytop.RectAxisParallel(Vector.Zero(dim), Vector.Ones(dim)).RotateRND().Shift(Vector.GenVector(dim, -10,10));

    // var       P       = ConvexPolytop.SimplexRND(2, false, rnd).LiftUp(3, 0).RotateRND(true, rnd);
    // ConvexPolytop P = ConvexPolytop.SimplexRND(dim);
    ConvexPolytop P = ConvexPolytop.RectAxisParallel(Vector.Zero(dim), Vector.Ones(dim)).RotateRND();

    ConvexPolytop polytop = MinkowskiSum.BySandipDas(P, Q);
    // ConvexPolytop polytopCH = MinkowskiSum.ByConvexHull(P, Q);



    // polytop.WriteIn(new ParamWriter("F:/Temp/polytope.polytope"), ConvexPolytop.Rep.FLrep);

    var flrep         = HrepToFLrep.HrepToFLrep_Geometric(polytop.Hrep, dim);
    var polytopeflrep = ConvexPolytop.CreateFromFaceLattice(flrep);
    // polytopeflrep.WriteIn(new ParamWriter("F:/Temp/polytopeflrep.polytope"), ConvexPolytop.Rep.FLrep);


    Console.WriteLine($"Polytope");
    Console.WriteLine($"Number 3: {polytop.FLrep[^2].Count(node => node.Sub.Count == 3)}");
    Console.WriteLine($"Number 4: {polytop.FLrep[^2].Count(node => node.Sub.Count == 4)}");
    Console.WriteLine($"Number > 4: {polytop.FLrep[^2].Count(node => node.Sub.Count > 4)}");

    // Console.WriteLine($"");
    //
    // Console.WriteLine($"PolytopeCH");
    // Console.WriteLine($"Number 3: {polytopCH.FLrep[^2].Count(node => node.Sub.Count == 3)}");
    // Console.WriteLine($"Number 4: {polytopCH.FLrep[^2].Count(node => node.Sub.Count == 4)}");
    // Console.WriteLine($"Number > 4: {polytopCH.FLrep[^2].Count(node => node.Sub.Count > 4)}");

    Console.WriteLine($"");

    Console.WriteLine($"V2FL");
    Console.WriteLine($"Number 3: {polytopeflrep.FLrep[^2].Count(node => node.Sub.Count == 3)}");
    Console.WriteLine($"Number 4: {polytopeflrep.FLrep[^2].Count(node => node.Sub.Count == 4)}");
    Console.WriteLine($"Number > 4: {polytopeflrep.FLrep[^2].Count(node => node.Sub.Count > 4)}");

    // Assert.That(polytop.FLrep.Equals(polytopCH));
    Assert.That(polytop.FLrep.Equals(flrep));

  }

}
