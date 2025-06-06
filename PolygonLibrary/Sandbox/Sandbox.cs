// using static CGLibrary.Geometry<double, Sandbox.DConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Sandbox.DDConvertor>;


namespace Sandbox;

class Sandbox {
  // public static Vector V(params double[] coords) => new Vector(coords);
  // public static Vector V(params ddouble[] coords) => new Vector(coords);

  static void Main(string[] args) {
    // const string ppath = @"F:\Works\IMM\Аспирантура\LDG\Polytopes\";
    const string ppath = @"E:\Work\LDG\Polytopes\";

    ConvexPolytop hex2d   = ConvexPolytop.Sphere(Vector.Zero(2), 1, 6,-1);
    var           hex3d_0 = hex2d.LiftUp(3, -1);
    var           hex3d_1 = hex2d.LiftUp(3, 1);
    hex3d_1.Vrep.UnionWith(hex3d_0.Vrep);
    var p = ConvexPolytop.CreateFromPoints(hex3d_1.Vrep,true);
    
    
    
    string      pname = "7";
    p.WriteIn(ppath, pname, ConvexPolytop.Rep.FLrep);
    

    // Console.WriteLine();

  }

}
