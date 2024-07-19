using System.Globalization;
// using System.Numerics;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;


    SortedSet<Vector> cube4d = new SortedSet<Vector>()
      {
        new Vector(new ddouble[] { 0, 0, 0, 0 })
      , new Vector(new ddouble[] { 0, 0, 0, 1 })
      , new Vector(new ddouble[] { 0, 0, 1, 0 })
      , new Vector(new ddouble[] { 0, 0, 1, 1 })
      , new Vector(new ddouble[] { 0, 1, 0, 0 })
      , new Vector(new ddouble[] { 0, 1, 0, 1 })
      , new Vector(new ddouble[] { 0, 1, 1, 0 })
      , new Vector(new ddouble[] { 0, 1, 1, 1 })
      , new Vector(new ddouble[] { 1, 0, 0, 0 })
      , new Vector(new ddouble[] { 1, 0, 0, 1 })
      , new Vector(new ddouble[] { 1, 0, 1, 0 })
      , new Vector(new ddouble[] { 1, 0, 1, 1 })
      , new Vector(new ddouble[] { 1, 1, 0, 0 })
      , new Vector(new ddouble[] { 1, 1, 0, 1 })
      , new Vector(new ddouble[] { 1, 1, 1, 0 })
      , new Vector(new ddouble[] { 1, 1, 1, 1 })
      };


    GiftWrapping gw = new GiftWrapping(cube4d);
    // GiftWrapping gw = new GiftWrapping(Simplex3D_list);
    var x = gw.FaceLattice;


    Tools.Eps = 1e-8;


    string materialdot_path =
      pathData + "Ep_MaterialDot-1-0.9_T[4,7]_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2/";


    // double t = 7.0;
    // while (t > 5.9) {
    //   Console.WriteLine($"t = {t:F1}");
    //    ParamReader prFirst  = new ParamReader($"{materialdot_path}/double/Naive/1e-008/{t:F2})materialDot1-0.9-supG.cpolytop");
    //   ParamReader prSecond = new ParamReader($"{materialdot_path}/double/Geometric/1e-008/{t:F2})materialDot1-0.9-supG.cpolytop");
    //
    //   ConvexPolytop first  = ConvexPolytop.AsFLPolytop(prFirst);
    //   ConvexPolytop second = ConvexPolytop.AsFLPolytop(prSecond);
    //
    //   Console.WriteLine(first.VRep.SetEquals(second.VRep));
    //   List<Vector> lnaive     = new List<Vector>(first.VRep).Order().ToList();
    //   List<Vector> lgeometric = new List<Vector>(second.VRep).Order().ToList();
    //
    //   List<int> lnaive_Hash     = new List<Vector>(first.VRep).Order().Select(v => v.GetHashCode()).ToList();
    //   List<int> lgeometric_Hash = new List<Vector>(second.VRep).Order().Select(v => v.GetHashCode()).ToList();
    //
    //   int    diff  = 0;
    //   double error = 0;
    //   for (int i = 0; i < lnaive.Count; i++) {
    //     if (lnaive[i] != lgeometric[i]) {
    //       Console.WriteLine("el is diff");
    //       Console.WriteLine($"{lnaive[i]}");
    //       Console.WriteLine($"{lgeometric[i]}");
    //       Console.WriteLine();
    //
    //       diff++;
    //     }
    //     // else {
    //       // Vector vector = lnaive[i] - lgeometric[i];
    //       // error += vector.GetAsArray().Sum(double.Abs) / vector.Length;
    //     // }
    //
    //     if (lnaive_Hash[i] != lgeometric_Hash[i]) {
    //       Console.WriteLine("hash is diff");
    //       Console.WriteLine($"{lnaive[i]}");
    //       Console.WriteLine($"{lgeometric[i]}");
    //       Console.WriteLine();
    //     }
    //   }
    //   // Console.WriteLine(error / lnaive.Count);
    //   Console.WriteLine(diff);
    //
    //   // break;
    //
    //   if (!first.Equals(second)) {
    //     throw new ArgumentException($"NOT equal at t = {t}");
    //   }
    //
    //   t -= 0.1;
    // }
    //


    // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");

    // solverLdg.Solve(false, true, false);
    // solverLdg.Solve(true, true, false);
  }

}
