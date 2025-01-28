using System.Diagnostics;
using System.Globalization;
using Tests.ToolsTests;
using LDG;
using Tests;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

// using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
// using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Profile;

class Program {

  private static readonly string pathData =
    // "E:\\Work\\CGLibrary\\CGLibrary\\Tests\\OtherTests\\LDG_Computations";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/CGLibrary/Tests/OtherTests/LDG_computations";

  static void Main(string[] args) {
    string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";

    double epsD  = 1e-08;
    double epsDD = 1e-15;

    LDGPathHolder<double, DConvertor> ph =
      new LDGPathHolder<double, DConvertor>(pathLdg, "Oscillator", "DoubleDouble.ddouble", epsDD);

    const double                            t      = 6.4;
    SortedDictionary<double, ConvexPolytop> bridge = new SortedDictionary<double, ConvexPolytop>(Tools.TComp);
    ph.LoadBridgeSection(bridge, 0, t);
    var p = bridge[t];
    // Console.WriteLine($"{p.NearestPoint(new Vector(new double[] { 0.2, 0.3, 3 }))}");
    // Console.WriteLine($"{p.NearestPoint(new Vector(new double[] { 1.22, 0, 1 }))}"); // на грани  (1.0592591369594095,-0.0066899114567881834,1.290119495661504)
    // Console.WriteLine($"{p.NearestPoint(new Vector(new double[] { -1.33, 0.1, 1 }))}"); //  на грани  (-1.1195813659540708,0.08769657244251747,1.3269287269958363)

    // p.Vrep.TryGetValue()

   // на пересечении нескольких граней -0.2815753061702822063403420110214,1.290524349044212509360547779272,1.058374937912177279801186291598
   // внутри -0.3 -0.5 1.3
  }

}


// string      t   = "3.10";
// ParamReader prP = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{t}) P {solverLdg.FileName}.cpolytop");
// ParamReader prQ = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{t}) Q {solverLdg.FileName}.cpolytop");
// ParamReader prW = new ParamReader( $"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/{ftype}/Geometric/{eps}/{t}){solverLdg.FileName}.cpolytop");
//
// ConvexPolytop P = ConvexPolytop.CreateFromReader(prP);
// ConvexPolytop Q = ConvexPolytop.CreateFromReader(prQ);
// ConvexPolytop W = ConvexPolytop.CreateFromReader(prW);
//
// var         x  = solverLdg.DoNextSection(W, P, Q);
// string tNext = "3.00)";
// ParamWriter pr = new ParamWriter($"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/ddouble/Geometric/1e-016/{tNext}{solverLdg.FileName}.cpolytop");
// x.WriteIn(pr);
