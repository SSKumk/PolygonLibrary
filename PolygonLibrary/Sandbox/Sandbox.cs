using static CGLibrary.Geometry<double, Sandbox.DConvertor>;


namespace Sandbox;

class Sandbox {

  static void Main(string[] args) {

    GRandomLC rnd = new GRandomLC(1);
    Matrix    m   = Matrix.GenMatrixInt(3,4, -10,10);

    Console.WriteLine($"{m}");

    Vector v = Vector.GenVectorInt(4,6,7,rnd);

    Console.WriteLine($"{v}");

    Console.WriteLine($"{Matrix.vcat(m,v)}");



  }


}
