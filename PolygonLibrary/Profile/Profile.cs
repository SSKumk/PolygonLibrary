using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
// using System.Numerics;
// using System.Numerics;
using System.Runtime.InteropServices;
using CGLibrary;
using DoubleDouble;
using Tests.ToolsTests;
using MultiPrecision;
using Tests;

// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;


namespace Profile;

using System;

class Program {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  //
  // static void Main(string[] args) {
  //   CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
  //
  //
  //   Tools.Eps = 1e-8;
  //
  //   // Vector v1 = new Vector(new double[] { -1.0596541049778718, 1.1515585830401667, 2 });
  //   // Vector v2 = new Vector(new double[] { -1.0596541050169939, 1.151558582963422, 2 });
  //
  //   Vector v1 = new Vector(new double[] { 1.9100000000000001, 0.09999999999999969, 2 });
  //   Vector v2 = new Vector(new double[] { 1.9100000000000001, 0.10000000000000739, 2 });
  //
  //   // Vector v1 = new Vector(new double[] { -1.554574501452087, 0.4747626291950904, 2 });
  //   // Vector v2 = new Vector(new double[] { -1.5545745015129653, 0.4747626289226897, 2 }); // 11.0  11.1  11.2  11.3  11.4  11.5
  //
  //   Console.WriteLine(v1 == v2);
  //
  //   Console.WriteLine($"{v1[0]}: {Tools.GetHashCodeTNum(v1[0])}");
  //   Console.WriteLine($"{v2[0]}: {Tools.GetHashCodeTNum(v2[0])}");
  //   Console.WriteLine($"{v1[1]}: {Tools.GetHashCodeTNum(v1[1])}");
  //   Console.WriteLine($"{v2[1]}: {Tools.GetHashCodeTNum(v2[1])}");
  //   Console.WriteLine($"{v2[1]}: {Tools.GetHashCodeTNum(v2[1] + Tools.Eps)}");
  //
  //   // Console.WriteLine(Math.Floor(2.5));
  //   // Console.WriteLine(Math.Floor(-2.5));
  //
  //   // Console.WriteLine(Tools.GetHashCodeTNum(-0.09999999999999969));
  //   // Console.WriteLine(Tools.GetHashCodeTNum(-0.10000000000000739));
  //   // Console.WriteLine(Tools.EQ(-0.09999999999999969, -0.10000000000000739));
  //
  //   string materialdot_path =
  //     pathData + "Ep_MaterialDot-1-0.9_T[4,7]_P#RectParallel_Q#RectParallel_M#DtnOrigin_Ball_2-T4-P100_-CMax2/";
  //
  //
  //   // double t = 7.0;
  //   // while (t > 5.9) {
  //   //   Console.WriteLine($"t = {t:F1}");
  //   //    ParamReader prFirst  = new ParamReader($"{materialdot_path}/double/Naive/1e-008/{t:F2})materialDot1-0.9-supG.cpolytop");
  //   //   ParamReader prSecond = new ParamReader($"{materialdot_path}/double/Geometric/1e-008/{t:F2})materialDot1-0.9-supG.cpolytop");
  //   //
  //   //   ConvexPolytop first  = ConvexPolytop.AsFLPolytop(prFirst);
  //   //   ConvexPolytop second = ConvexPolytop.AsFLPolytop(prSecond);
  //   //
  //   //   Console.WriteLine(first.VRep.SetEquals(second.VRep));
  //   //   List<Vector> lnaive     = new List<Vector>(first.VRep).Order().ToList();
  //   //   List<Vector> lgeometric = new List<Vector>(second.VRep).Order().ToList();
  //   //
  //   //   List<int> lnaive_Hash     = new List<Vector>(first.VRep).Order().Select(v => v.GetHashCode()).ToList();
  //   //   List<int> lgeometric_Hash = new List<Vector>(second.VRep).Order().Select(v => v.GetHashCode()).ToList();
  //   //
  //   //   int    diff  = 0;
  //   //   double error = 0;
  //   //   for (int i = 0; i < lnaive.Count; i++) {
  //   //     if (lnaive[i] != lgeometric[i]) {
  //   //       Console.WriteLine("el is diff");
  //   //       Console.WriteLine($"{lnaive[i]}");
  //   //       Console.WriteLine($"{lgeometric[i]}");
  //   //       Console.WriteLine();
  //   //
  //   //       diff++;
  //   //     }
  //   //     // else {
  //   //       // Vector vector = lnaive[i] - lgeometric[i];
  //   //       // error += vector.GetAsArray().Sum(double.Abs) / vector.Length;
  //   //     // }
  //   //
  //   //     if (lnaive_Hash[i] != lgeometric_Hash[i]) {
  //   //       Console.WriteLine("hash is diff");
  //   //       Console.WriteLine($"{lnaive[i]}");
  //   //       Console.WriteLine($"{lgeometric[i]}");
  //   //       Console.WriteLine();
  //   //     }
  //   //   }
  //   //   // Console.WriteLine(error / lnaive.Count);
  //   //   Console.WriteLine(diff);
  //   //
  //   //   // break;
  //   //
  //   //   if (!first.Equals(second)) {
  //   //     throw new ArgumentException($"NOT equal at t = {t}");
  //   //   }
  //   //
  //   //   t -= 0.1;
  //   // }
  //   //
  //
  //
  //   // SolverLDG solverLdg = new SolverLDG(pathData, "materialDot1-0.9-supG");
  //
  //   // solverLdg.Solve(false, true, false);
  //   // solverLdg.Solve(true, true, false);
  // }

}
