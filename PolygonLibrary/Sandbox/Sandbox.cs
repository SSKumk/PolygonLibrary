using Graphics;
using LDG;
using static CGLibrary.Geometry<double, Sandbox.DConvertor>;

// using static CGLibrary.Geometry<DoubleDouble.ddouble, Sandbox.DDConvertor>;


namespace Sandbox;

class Sandbox {

  public static Vector V(params double[] coords) => new Vector(coords);
  // public static Vector V(params ddouble[] coords) => new Vector(coords);

  static void Main(string[] args) {
    Tools.Eps = 1e-8;

    const string ppath = @"F:\Works\IMM\Аспирантура\LDG\";
    // const string ppath = @"E:\Work\LDG\";

    const string eps = "0.001";
    // const string eps   = "1E-08";
    const string ntype = "System.Double";

    const string gameName = "Oscillator-triangle";
    const string br       = $@"{ppath}_Out\{gameName}\Br\0\{ntype}\{eps}\";
    const string vecp     = $@"{ppath}_Out\{gameName}\Ps\{ntype}\{eps}\";
    const string vecq     = $@"{ppath}_Out\{gameName}\Qs\{ntype}\{eps}\";

    const string temp = $@"{ppath}Visualization\Temp";

    const string brT = "9.70";
    const string pqT = "9.60";

    ParamReader   prw = new ParamReader($"{br}{brT}.wsection");
    ParamReader   prp = new ParamReader($"{vecp}{pqT}.psection");
    ParamReader   prq = new ParamReader($"{vecq}{pqT}.qsection");
    ConvexPolytop w   = ConvexPolytop.CreateFromReader(prw);
    ConvexPolytop p   = ConvexPolytop.CreateFromReader(prp);
    // ConvexPolytop q   = ConvexPolytop.CreateFromReader(prq);

    // ConvexPolytop w = MinkowskiSum.BySandipDas(ConvexPolytop.Cube01_VRep(3).RotateRND(), ConvexPolytop.SimplexRND(3));


    Tools.Eps = 1e-8;
    var sumCut18 = MinkowskiSum.BySandipDas(w, p, true);
    var sum18    = MinkowskiSum.BySandipDas(w, p);
    var sum18ch  = MinkowskiSum.ByConvexHull(w, p);
    var vrep18g  = ConvexPolytop.HrepToVrep_Geometric(sumCut18.Hrep);
    var vrep18n  = ConvexPolytop.HrepToVrep_Naive(sumCut18.Hrep);
    Tools.Eps = 1e-3;
    var sumCut13 = MinkowskiSum.BySandipDas(w, p, true);
    var sum13    = MinkowskiSum.BySandipDas(w, p);
    var sum13ch  = MinkowskiSum.ByConvexHull(w, p);
    var vrep13g  = ConvexPolytop.HrepToVrep_Geometric(sumCut13.Hrep);
    var vrep13n  = ConvexPolytop.HrepToVrep_Naive(sumCut13.Hrep);

    Tools.Eps = 1e-8;
    Console.WriteLine($"sC18 == sC13: {sumCut13.Equals(sumCut18)}");
    Console.WriteLine($"s18 == s13: {sum13.Equals(sum18)}");
    Console.WriteLine($"sC18vg == sC13vg: {vrep13g.SetEquals(vrep18g)}");
    Console.WriteLine($"sC18vn == sC13vn: {vrep13n.SetEquals(vrep18n)}");
    Console.WriteLine($"sC13vg == sC13vn: {vrep13g.SetEquals(vrep13n)}");
    Console.WriteLine($"sC18vg == sC18vn: {vrep18g.SetEquals(vrep18n)}");
    Console.WriteLine($"s18 == sC18vn: {sum18.Vrep.SetEquals(vrep18n)}");
    Console.WriteLine($"s18 == sC18vg: {sum18.Vrep.SetEquals(vrep18g)}");
    Console.WriteLine($"s13 == sC13vn: {sum13.Vrep.SetEquals(vrep13n)}");
    Console.WriteLine($"s13 == sC13vg: {sum13.Vrep.SetEquals(vrep13g)}");
    Console.WriteLine($"s18ch == s13ch: {sum18ch.Vrep.SetEquals(sum13ch.Vrep)}");

    Console.WriteLine($"");


    // SDasCut
    // var sumCut = MinkowskiSum.BySandipDas(w, p, true);
    // string sumSD_cut_f = $"Eps = {Tools.Eps}, eps = {eps} w{brT}+p{pqT}-SDasCut";
    // sumCut.WriteIn(temp, sumSD_cut_f, ConvexPolytop.Rep.Hrep);
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_cut_f);


    // var sumSD  = MinkowskiSum.BySandipDas(w, p, false);
    // var sumCH  = MinkowskiSum.ByConvexHull(w, p);


    // string sumSD_Hrep_f  = $"{eps} w{brT}+p{pqT}-SDas_Hrep";
    // sumSD.WriteIn(temp, sumSD_Hrep_f, ConvexPolytop.Rep.Hrep);
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_Hrep_f);
    // string sumSD_FLrep_f = $"{eps} w{brT}+p{pqT}-SDas_FLrep";
    // sumSD.WriteIn(temp, sumSD_FLrep_f, ConvexPolytop.Rep.FLrep);
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_FLrep_f);
    // string sumSD_Vrep_f  = $"{eps} w{brT}+p{pqT}-SDas_Vrep";
    // sumSD.WriteIn(temp, sumSD_Vrep_f, ConvexPolytop.Rep.Vrep);
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumSD_Vrep_f);

    // string sumCH_f = $"{eps} w{brT}+p{pqT}-CH";
    // sumCH.WriteIn(temp, sumCH_f, ConvexPolytop.Rep.FLrep);
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, sumCH_f);


    // var    diff   = MinkowskiDiff.Geometric(sumCut, q);
    // string diff_f = $"({sumCut})-q";
    // diff.WriteIn(temp, diff_f, ConvexPolytop.Rep.FLrep);
    //
    // Visualization<double, DConvertor>.ReadAndDrawPolytopePLY(temp, diff_f);
  }

}
