using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[SimpleJob(iterationCount: 10)]
// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Sandbox {

  private static readonly string pathData =
    // "E:\\Work\\PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_Computations\\";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  // [Params(2,3,4)]
  // public int spaceDim;

  // [Params(3)]
  // public int subSpaceDim;

  public LinearBasis? lb = null;

  public AffineBasis? ab = null;

  public Vector? v = null;

  public Matrix? m = null;

  public List<HyperPlane> HPs;


  [GlobalSetup]
  public void SetUp() {

    string    t         = "5.10";
    string    eps       = "1e-016";
    SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    ParamReader prR = new ParamReader
      ($"{solverLdg.WorkDir}{solverLdg.gd.ProblemName}/ddouble/Geometric/{eps}/{t}){solverLdg.fileName}.cpolytop");
    ConvexPolytop polytop = ConvexPolytop.CreateFromReader(prR);
    HPs = polytop.Hrep;
  }


  [Benchmark(Baseline = true)]
  public void FindInitialVertex_Naive_MassPoint_5dot1() => ConvexPolytop.FindInitialVertex_Naive(HPs, 3);

  [Benchmark]
  public void FindInitialVertex_Simplex_MassPoint_5dot1() => ConvexPolytop.FindInitialVertex_Simplex(HPs, out _);


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


---------------------------------------------------------------------------------------------------------------------
| Method                           | spaceDim | Mean         | Error       | StdDev      | Ratio | RatioSD |
|--------------------------------- |--------- |-------------:|------------:|------------:|------:|--------:|
| FindInitialVertex_Naive_Cube01   | 2        |     818.5 ns |    14.92 ns |    13.96 ns |  1.00 |    0.00 |
| FindInitialVertex_Simplex_Cube01 | 2        |   2,983.0 ns |    44.82 ns |    41.92 ns |  3.65 |    0.08 |
|                                  |          |              |             |             |       |         |
| FindInitialVertex_Naive_Cube01   | 3        |   3,683.9 ns |    56.96 ns |    53.28 ns |  1.00 |    0.00 |
| FindInitialVertex_Simplex_Cube01 | 3        |   6,792.6 ns |    80.54 ns |    71.40 ns |  1.84 |    0.03 |
|                                  |          |              |             |             |       |         |
| FindInitialVertex_Naive_Cube01   | 4        |  21,762.1 ns |   250.41 ns |   234.23 ns |  1.00 |    0.00 |
| FindInitialVertex_Simplex_Cube01 | 4        |  14,482.8 ns |   289.10 ns |   557.00 ns |  0.64 |    0.02 |
|                                  |          |              |             |             |       |         |
| FindInitialVertex_Naive_Cube01   | 5        | 141,728.0 ns | 2,788.04 ns | 6,060.98 ns |  1.00 |    0.00 |
| FindInitialVertex_Simplex_Cube01 | 5        |  25,461.6 ns |   509.14 ns | 1,051.47 ns |  0.18 |    0.01 |


-----------------------------------------------------------------------------------------------
| Method                                 | spaceDim | Mean         | Error      | StdDev     | Ratio | RatioSD |
|--------------------------------------- |--------- |-------------:|-----------:|-----------:|------:|--------:|
| FindInitialVertex_Naive_SimplexRNDx2   | 2        |     1.171 us |  0.0088 us |  0.0052 us |  1.00 |    0.00 |
| FindInitialVertex_Simplex_SimplexRNDx2 | 2        |    26.899 us |  0.7964 us |  0.5268 us | 23.00 |    0.45 |
|                                        |          |              |            |            |       |         |
| FindInitialVertex_Naive_SimplexRNDx2   | 3        |     3.889 us |  0.0492 us |  0.0293 us |  1.00 |    0.00 |
| FindInitialVertex_Simplex_SimplexRNDx2 | 3        |   236.132 us |  1.4070 us |  0.9306 us | 60.69 |    0.61 |
|                                        |          |              |            |            |       |         |
| FindInitialVertex_Naive_SimplexRNDx2   | 4        | 5,766.904 us | 15.4712 us |  9.2067 us |  1.00 |    0.00 |
| FindInitialVertex_Simplex_SimplexRNDx2 | 4        | 2,229.222 us | 64.6538 us | 42.7645 us |  0.39 |    0.01 |

| FindInitialVertex_Naive_SimplexRNDx2   | 5        | 3,710.48 ms | 100.022 ms | 66.158 ms |         |   1.000 |
| FindInitialVertex_Simplex_SimplexRNDx2 | 5        |    16.44 ms |   0.284 ms |  0.169 ms |         |   0.004 |


| Method                                    | Mean       | Error     | StdDev   | Ratio |
|------------------------------------------ |-----------:|----------:|---------:|------:|
| FindInitialVertex_Naive_MassPoint_5dot1   | 2,914.6 ms | 139.31 ms | 92.15 ms |  1.00 |
| FindInitialVertex_Simplex_MassPoint_5dot1 |   392.4 ms |  25.93 ms | 17.15 ms |  0.13 |



*/
