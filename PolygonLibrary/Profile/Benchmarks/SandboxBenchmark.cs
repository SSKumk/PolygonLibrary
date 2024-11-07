using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[SimpleJob(iterationCount: 30)]
// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Sandbox {

  private static readonly string pathData =
    // "E:\\Work\\PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_Computations\\";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  // [Params(3, 7)]
  // public int spaceDim;

  // [Params(3, 7)]
  // public int subSpaceDim;

  public LinearBasis? lb = null;

  public AffineBasis? ab = null;

  public Vector? v = null;

  public Matrix? m = null;

  public List<HyperPlane> HPs;

  public ConvexPolytop? p = null;

  [Params("5.10")]
  public string t;


  [GlobalSetup]
  public void SetUp() {
    SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // string    t         = "5.10";
    string eps   = "1e-016";
    ParamReader prR = new ParamReader
      ($"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/ddouble/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");
    p = ConvexPolytop.CreateFromReader(prR);
    var _  = p.Hrep;
    var __ = p.Vrep;

    // m = Matrix.GenMatrix(spaceDim, subSpaceDim,-100,100);
    // string    t         = "5.10";
    // string    eps       = "1e-016";
    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // ParamReader prR = new ParamReader
    //   ($"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/ddouble/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");
    // ConvexPolytop polytop = ConvexPolytop.CreateFromReader(prR);

    // HPs = polytop.Hrep;
  }


  [Benchmark(Baseline = true)]
  public void NearestPoint() => p.NearestPoint(Vector.Ones(3));

  [Benchmark]
  public void NearestPointOpt() => p.NearestPointOpt(Vector.Ones(3));

  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<Sandbox>();

      // int           dim = 5;
      // ConvexPolytop s   = ConvexPolytop.SimplexRND(dim);
      // for (int i = 0; i < 4; i++) {
      //   Console.WriteLine($"i = {i}. |V| = {s.Vrep.Count}. |H| = {s.Hrep.Count}");
      //   ConvexPolytop n = ConvexPolytop.SimplexRND(dim);
      //   s = MinkowskiSum.BySandipDas(s, n);
      // }

    }

  }

}


/*


MassDot, eps = 1e-16
| Method          | t    | Mean      | Error    | StdDev   | Ratio |
|---------------- |----- |----------:|---------:|---------:|------:|
| NearestPoint    | 5.10 | 821.34 ms | 3.256 ms | 4.773 ms |  1.00 |
| NearestPointOpt | 5.10 |  64.21 ms | 0.193 ms | 0.283 ms |  0.08 |

*/
