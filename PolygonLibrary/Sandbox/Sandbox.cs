// using static CGLibrary.Geometry<double, Sandbox.DConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Sandbox.DDConvertor>;


namespace Sandbox;

class Sandbox {
  // public static Vector V(params double[] coords) => new Vector(coords);
  // public static Vector V(params ddouble[] coords) => new Vector(coords);

  static void Main(string[] args) {
    const string ppath = @"F:\Works\IMM\Аспирантура\LDG\Polytopes\";

    string      pname = "6";
    ConvexPolytop p = ConvexPolytop.DistanceToPointBall_2(Vector.Zero(2), 6, -1, 1);
    p.WriteIn(ppath, pname, ConvexPolytop.Rep.FLrep);




  }

}