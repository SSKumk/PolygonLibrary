using System.Diagnostics;
using static CGLibrary.Geometry<double, Sandbox.DConvertor>;


namespace Sandbox;

class Sandbox {

  static void Main(string[] args) {
    GRandomLC   rnd   = new GRandomLC(1);

    int dim = 5;

    
    ConvexPolytop polytop1 = ConvexPolytop.SimplexRND(dim);
    //ConvexPolytop polytop2 = ConvexPolytop.SimplexRND(dim);
    
    ConvexPolytop polytop2 = ConvexPolytop.Cube01_VRep(dim);

    ConvexPolytop res = MinkowskiSum.BySandipDas(polytop1, polytop2);

    _     = res.Hrep;
    Stopwatch timer = new Stopwatch();
    
    timer.Restart();
    // previous approach
    var x = ConvexPolytop.HrepToVrep_Geometric(res.Hrep);
    var y = ConvexPolytop.CreateFromPoints(x!);
    _ = y.FLrep;
    timer.Stop();
    Console.WriteLine($"old: {timer.Elapsed.Seconds}");
    Console.WriteLine($"old f-vec: {string.Join(", ",y.fVector)}");
    
    
    timer.Restart();
    // new approach
    ConvexPolytop nwe = ConvexPolytop.CreateFromFaceLattice(HrepToFLrep.HrepToFLrep_Geometric(res.Hrep, 5)!);

    timer.Stop();
    Console.WriteLine($"new: {timer.Elapsed.Seconds}");
    Console.WriteLine($"new f-vec: {string.Join(", ",nwe.fVector)}");
    
    Console.WriteLine($"res = {string.Join(", ",res.fVector)}");
    Console.WriteLine($"{y.FLrep.Equals(nwe.FLrep)}");

    // Matrix m = new Matrix(new double[,]{ { 2.0, 3, 4 }});
    // Console.WriteLine($"{m.ToRREF()}");
    // LinearBasis basis = LinearBasis.GenLinearBasis(3, 2);
    //
    // Vector n = basis.FindOrthonormalVector();
    //
    // HyperPlane hp = new HyperPlane(n, Vector.Zero(3));
    //
    //
    // Console.WriteLine($"{basis}");
    // LinearBasis linBasis = hp.AffBasis.LinBasis;
    // Console.WriteLine($"{linBasis}");
    // Console.WriteLine($"same space? {linBasis.SpanSameSpace(basis)}\n");
    // Console.WriteLine($"{basis.Basis.ToRREF()}\n");
    // Console.WriteLine($"{linBasis.Basis.ToRREF()}");
  }

}
