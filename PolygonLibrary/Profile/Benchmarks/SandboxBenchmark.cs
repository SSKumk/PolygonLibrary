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

  [Params(3,7)]
  public int spaceDim;

  [Params(3,7)]
  public int subSpaceDim;

  public LinearBasis? lb = null;

  public AffineBasis? ab = null;

  public Vector? v = null;

  public Matrix? m = null;

  public List<HyperPlane> HPs;


  [GlobalSetup]
  public void SetUp() {

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
  public void FindInitialVertex_Naive_SimplexRNDx2() => ConvexPolytop.FindInitialVertex_Naive(HPs, 3);

  [Benchmark]
  public void FindInitialVertex_Simplex_SimplexRNDx2() => ConvexPolytop.FindInitialVertex_Simplex(HPs, out _);

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<Sandbox>();
  //
  //     // int           dim = 5;
  //     // ConvexPolytop s   = ConvexPolytop.SimplexRND(dim);
  //     // for (int i = 0; i < 4; i++) {
  //     //   Console.WriteLine($"i = {i}. |V| = {s.Vrep.Count}. |H| = {s.Hrep.Count}");
  //     //   ConvexPolytop n = ConvexPolytop.SimplexRND(dim);
  //     //   s = MinkowskiSum.BySandipDas(s, n);
  //     // }
  //
  //   }
  //
  // }

}


/*



*/
