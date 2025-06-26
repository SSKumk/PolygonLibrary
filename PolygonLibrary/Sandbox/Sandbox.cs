using Graphics;
using LDG;
using static CGLibrary.Geometry<double, Sandbox.DConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Sandbox.DDConvertor>;


namespace Sandbox;

class Sandbox {
  // public static Vector V(params double[] coords) => new Vector(coords);
  // public static Vector V(params ddouble[] coords) => new Vector(coords);

  static void Main(string[] args) {
    // const string ppath = @"F:\Works\IMM\Аспирантура\LDG\";
    const string ppath = @"E:\Work\LDG\";

    const string eps   = "1E-05";
    const string ntype = "System.Double";

    const string gameName = "Oscillator-cone6";
    const string br       = $@"{ppath}_Out\{gameName}\Br\0\{ntype}\{eps}\";
    const string vecp     = $@"{ppath}_Out\{gameName}\Ps\{ntype}\{eps}\";

    const string temp = $@"{ppath}Visualization\Temp";

    ParamReader   prw = new ParamReader($"{br}9.20.cpolytope");
    ParamReader   prp = new ParamReader($"{vecp}9.10.cpolytope");
    ConvexPolytop w   = ConvexPolytop.CreateFromReader(prw);
    ConvexPolytop p   = ConvexPolytop.CreateFromReader(prp);


    var sumCut = MinkowskiSum.BySandipDas(w, p, true);
    var sum    = MinkowskiSum.BySandipDas(w, p, false);
    sumCut.WriteIn(temp, "w9.2+p9.1-SDasCut", ConvexPolytop.Rep.Hrep);
    sum.WriteIn(temp, "w9.2+p9.1-SDas", ConvexPolytop.Rep.Hrep);

    Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, "w9.2+p9.1-SDasCut");
    Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, "w9.2+p9.1-SDas");



    // string      pname = "7";
    // p.WriteIn(ppath, pname, ConvexPolytop.Rep.FLrep);


    // Console.WriteLine();

  }

}
