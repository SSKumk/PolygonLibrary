using Graphics;
using LDG;
using static CGLibrary.Geometry<double, Sandbox.DConvertor>;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Sandbox.DDConvertor>;


namespace Sandbox;

class Sandbox {
  // public static Vector V(params double[] coords) => new Vector(coords);
  // public static Vector V(params ddouble[] coords) => new Vector(coords);

  static void Main(string[] args) {
    const string ppath = @"F:\Works\IMM\Аспирантура\LDG\";
    // const string ppath = @"E:\Work\LDG\";

    const string eps   = "1E-05";
    const string ntype = "System.Double";

    const string gameName = "Oscillator-cone6";
    const string br       = $@"{ppath}_Out\{gameName}\Br\0\{ntype}\{eps}\";
    const string vecp     = $@"{ppath}_Out\{gameName}\Ps\{ntype}\{eps}\";
    const string vecq     = $@"{ppath}_Out\{gameName}\Qs\{ntype}\{eps}\";

    const string temp = $@"{ppath}Visualization\Temp";

    const string brT = "9.40";
    const string pqT = "9.30";

    ParamReader   prw = new ParamReader($"{br}{brT}.wsection");
    ParamReader   prp = new ParamReader($"{vecp}{pqT}.psection");
    ParamReader   prq = new ParamReader($"{vecq}{pqT}.qsection");
    ConvexPolytop w   = ConvexPolytop.CreateFromReader(prw);
    ConvexPolytop p   = ConvexPolytop.CreateFromReader(prp);
    ConvexPolytop q   = ConvexPolytop.CreateFromReader(prq);


    // Hrep
    var sumCut = MinkowskiSum.BySandipDas(w, p, true);
    string sumSD_cut_f = $"{eps} w{brT}+p{pqT}-SDasCut";
    sumCut.WriteIn(temp, sumSD_cut_f, ConvexPolytop.Rep.Hrep);
    Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_cut_f);


    // var sumSD  = MinkowskiSum.BySandipDas(w, p, false);
    // var sumCH  = MinkowskiSum.ByConvexHull(w, p);


    // string sumSD_Hrep_f  = $"{eps} w{brT}+p{pqT}-SDas_Hrep";
    // string sumSD_FLrep_f = $"{eps} w{brT}+p{pqT}-SDas_FLrep";
    // string sumSD_Vrep_f  = $"{eps} w{brT}+p{pqT}-SDas_Vrep";
    // sumSD.WriteIn(temp, sumSD_Hrep_f, ConvexPolytop.Rep.Hrep);
    // sumSD.WriteIn(temp, sumSD_FLrep_f, ConvexPolytop.Rep.FLrep);
    // sumSD.WriteIn(temp, sumSD_Vrep_f, ConvexPolytop.Rep.Vrep);

    // string sumCH_f = $"{eps} w{brT}+p{pqT}-CH";
    // sumCH.WriteIn(temp, sumCH_f, ConvexPolytop.Rep.FLrep);

    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_cut_f);

    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_Vrep_f);
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_FLrep_f);

    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumCH_f);


    // var    diff   = MinkowskiDiff.Geometric(sumCH, q);
    // string diff_f = $"({sumCH_f})-q";
    // diff.WriteIn(temp, diff_f, ConvexPolytop.Rep.FLrep);
    //
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, diff_f);


  }

}
