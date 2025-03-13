using System.Diagnostics;
using System.Globalization;
using Tests.ToolsTests;
using LDG;
using Tests;
using Rationals;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

// using static CGLibrary.Geometry<Rationals.Rational, Tests.RConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    // "E:\\Work\\CGLibrary\\CGLibrary\\Tests\\OtherTests\\LDG_Computations";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations";


  public static Rational RSum(Rational[] toSum, Func<Rational, Rational, Rational> sum) {
    Rational res = Rational.Zero;
    for (int i = 0; i < toSum.Length; i++) {
      res = sum(res, toSum[i]);
    }

    return res;
  }


  static void Main(string[] args) {
    string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";

    GRandomLC rnd   = new GRandomLC(1);
    double    epsD  = 1e-08;
    double    epsDD = 1e-15;


    LDGPathHolder<double, DConvertor> phD = new LDGPathHolder<double, DConvertor>(pathLdg, "Oscillator3D", 1e-08);
    LDGPathHolder<ddouble, DDConvertor> phDD =
      new LDGPathHolder<ddouble, DDConvertor>(pathLdg, "Oscillator3D", ddouble.Parse("1e-08"));
    LDGPathHolder<Rational, RConvertor> phR08 =
      new LDGPathHolder<Rational, RConvertor>(pathLdg, "Oscillator3D", Rational.Parse("1/100000000"));
    LDGPathHolder<Rational, RConvertor> phR16 =
      new LDGPathHolder<Rational, RConvertor>(pathLdg, "Oscillator3D", Rational.Parse("1/10000000000000000"));

    var WsD   = phD.LoadBridges().First();
    var WsDD  = phDD.LoadBridges().First();
    var WsR08 = phR08.LoadBridges().First();
    var WsR16 = phR16.LoadBridges().First();

    double t = 10;
    for (int i = 0; i < 13; i++, t -= 0.1) {
      Console.WriteLine
        (
         $"t = {t:F2} |Vrep| D: {WsD[t].Vrep.Count}\tDD: {WsDD[t].Vrep.Count}\tR08: {WsR08[(Rational)t].Vrep.Count}\tR16: {WsR16[(Rational)t].Vrep.Count}"
        );
    }
  }

}
