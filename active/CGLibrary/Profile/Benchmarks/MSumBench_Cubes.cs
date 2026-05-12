namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
[IterationCount(2)]
public class MSumBench_Cubes {

  [Params(4,5,6,7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop P = null!;
  private ConvexPolytop Q = null!;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cube01_VRep(dim).RotateRND(true);
    Q = ConvexPolytop.Cube01_VRep(dim).RotateRND(true);
  }

  [Benchmark]
  public void MSumCuCu_SDas() => MinkowskiSum.BySandipDas(P, Q);
  [Benchmark]
  public void MSumCuCu_SDasCutted() => MinkowskiSum.BySandipDas(P, Q,true);

  // [Benchmark]
  // public void MSumCuCu_CH() => MinkowskiSum.ByConvexHull(P, Q);

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MSumBench_Cubes>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

 /*
| Method        | dim | Mean     |
|-------------- |---- |---------:|
| MSumCuCu_SDas | 3   | 0.0032 s |
| MSumCuCu_CH   | 3   | 0.0057 s |
| MSumCuCu_SDas | 4   | 0.0417 s |
| MSumCuCu_CH   | 4   | 0.1499 s |
| MSumCuCu_SDas | 5   | 0.5187 s |
| MSumCuCu_CH   | 5   | 4.1384 s |
| MSumCuCu_SDas | 6   | 6.047 s  |
| MSumCuCu_CH   | 6   | 152.1 s  |
| MSumCuCu_SDas | 7   | 68.64 s  |
| MSumCuCu_SDasCutted | 4   |  0.0236 s |
| MSumCuCu_SDasCutted | 5   |  0.2626 s |
| MSumCuCu_SDasCutted | 6   |  3.2353 s |
| MSumCuCu_SDasCutted | 7   | 31.3387 s |

 */

}
