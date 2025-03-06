using System.Diagnostics;
using System.Globalization;
using Tests.ToolsTests;
using LDG;
using Tests;
using Rationals;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
// using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static CGLibrary.Geometry<Rationals.Rational, Tests.RConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    // "E:\\Work\\CGLibrary\\CGLibrary\\Tests\\OtherTests\\LDG_Computations";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations";


  public static Rational RSum(Rational[] toSum, Func<Rational,Rational,Rational> sum) {
    Rational res = Rational.Zero;
    for (int i = 0; i < toSum.Length; i++) {
      res = sum(res, toSum[i]);
    }
    return res;
  }


  static void Main(string[] args) {
    string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";

    GRandomLC rnd = new GRandomLC(1);
    double epsD  = 1e-08;
    double epsDD = 1e-15;


    // ConvexPolytop p = ConvexPolytop.SimplexRND(3);
    // var x = p.GetInFLrep();
    // Console.WriteLine($"{string.Join('\n', x.Vrep)}");




    // LDGPathHolder<double, DConvertor> ph =
    // new LDGPathHolder<double, DConvertor>(pathLdg, "Oscillator", "DoubleDouble.ddouble", epsDD);

    // const double                            t      = 6.4;
    // SortedDictionary<double, ConvexPolytop> bridge = new SortedDictionary<double, ConvexPolytop>(Tools.TComp);
    // ph.LoadBridgeSection(bridge, 0, t);
    // var p = bridge[t];
    // Console.WriteLine($"{p.NearestPoint(new Vector(new double[] { 0.2, 0.3, 3 }))}");
    // Console.WriteLine($"{p.NearestPoint(new Vector(new double[] { 1.22, 0, 1 }))}"); // на грани  (1.0592591369594095,-0.0066899114567881834,1.290119495661504)
    // Console.WriteLine($"{p.NearestPoint(new Vector(new double[] { -1.33, 0.1, 1 }))}"); //  на грани  (-1.1195813659540708,0.08769657244251747,1.3269287269958363)

    // p.Vrep.TryGetValue()

    // на пересечении нескольких граней -0.2815753061702822063403420110214,1.290524349044212509360547779272,1.058374937912177279801186291598
    // внутри -0.3 -0.5 1.3
  }
}