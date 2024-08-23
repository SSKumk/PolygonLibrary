using System.Globalization;
using Tests.ToolsTests;
// using System.Numerics;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";


  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    Tools.Eps = 1e-16;

    int           dim      = 4;
    ConvexPolytop cube     = ConvexPolytop.Cube01_VRep(dim);
    string        filePath = $"{pathData}Temp/cube3d.cpolytop";
    ParamWriter   pw       = new ParamWriter(filePath);
    cube.WriteIn(pw, ConvexPolytop.Rep.FLrep);
    pw.Close();
    ParamReader   pr       = new ParamReader(filePath);
    ConvexPolytop cube_read = ConvexPolytop.FromReader(pr);
    Console.WriteLine(cube_read.FLrep.Lattice[^2].Max.InnerPoint);


    // Console.WriteLine(cube.SectionByHyperPlane(new HyperPlane(Vector.MakeOrth(dim,1),0.5)).PolytopDim);





    //
    // string materialdot_path =
    //   pathData + "Ep_MaterialDot-1-0.9_T[4,7]_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2/";
    // string        filePath = $"{materialdot_path}/1.00)materialDot1-0.9-supG";
    // ParamReader   pr       = new ParamReader($"{filePath}.cpolytop");
    // ParamWriter   pw       = new ParamWriter(filePath);
    // ConvexPolytop P        = ConvexPolytop.CreateFromFaceLattice(pr);
    // P.WriteIn(pw,ConvexPolytop.Rep.FLrep);


    // ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);
    // var cutted = cube.SectionByHyperPlane(new HyperPlane(Vector.Ones(3), 0.5));

    // cutted.WriteIn(new ParamWriter($"F:\\Works\\IMM\\Аспирантура\\_PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_computations\\Other\\cube3dcut.cpolytop"), ConvexPolytop.Rep.FLrep);



    // double t = 7.0;
    // while (t > 5.9) {
    //   Console.WriteLine($"t = {t:F1}");
    //    ParamReader prFirst  = new ParamReader($"{materialdot_path}/double/Naive/1e-008/{t:F2})materialDot1-0.9-supG.cpolytop");
    //   ParamReader prSecond = new ParamReader($"{materialdot_path}/double/Geometric/1e-008/{t:F2})materialDot1-0.9-supG.cpolytop");
    //
    //   ConvexPolytop first  = ConvexPolytop.CreateFromFaceLattice(prFirst);
    //   ConvexPolytop second = ConvexPolytop.CreateFromFaceLattice(prSecond);
    //
    //   Console.WriteLine(first.Vrep.SetEquals(second.Vrep));
    //   List<Vector> lnaive     = new List<Vector>(first.Vrep).Order().ToList();
    //   List<Vector> lgeometric = new List<Vector>(second.Vrep).Order().ToList();
    //
    //   List<int> lnaive_Hash     = new List<Vector>(first.Vrep).Order().Select(v => v.GetHashCode()).ToList();
    //   List<int> lgeometric_Hash = new List<Vector>(second.Vrep).Order().Select(v => v.GetHashCode()).ToList();
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
