using static CGLibrary.Geometry<double, Sandbox.DConvertor>;


namespace Sandbox;

class Sandbox {

  static void Main(string[] args) {
    GRandomLC   rnd   = new GRandomLC(4567);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);

    Vector v        = Vector.GenVector(5, rnd);

    Vector prv = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    Console.WriteLine($"{prv}");
  }

}
