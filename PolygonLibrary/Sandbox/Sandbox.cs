using static CGLibrary.Geometry<double, Sandbox.DConvertor>;


namespace Sandbox;

class Sandbox {

  static void Main(string[] args) {
    GRandomLC   rnd   = new GRandomLC(1);

    Matrix m = new Matrix(new double[,]{ { 2.0, 3, 4 }});
    Console.WriteLine($"{m.ToRREF()}");
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
