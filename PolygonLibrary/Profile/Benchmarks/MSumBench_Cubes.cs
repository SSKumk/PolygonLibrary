using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;
using Perfolizer.Horology;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
[IterationCount(1)]
public class MSumBench_Cubes {

  [Params(6)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop P;

  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cube01(dim).RotateRND(true);
    Q = ConvexPolytop.Cube01(dim).RotateRND(true);
  }

  // [Benchmark]
  // public void MSumCuCu_SDas() => MinkowskiSum.BySandipDas(P, Q);

  [Benchmark]
  public void MSumCuCu_CH() => MinkowskiSum.ByConvexHull(P, Q);

  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<MSumBench_Cubes>
        (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
    }

  }

 /*
| Method        | dim | Mean     |
|-------------- |---- |---------:|
| MSumCuCu_SDas | 3   | 0.0032 s |
| MSumCuCu_CH   | 3   | 0.0057 s |
| MSumCuCu_SDas | 4   | 0.0417 s |
| MSumCuCu_CH   | 4   | 0.1499 s |
| MSumCuCu_SDas | 5   | 0.5187 s |
| MSumCuCu_CH   | 5   | 4.1384 s |
| MSumCuCu_SDas | 6   | 68.64 s  |
| MSumCuCu_CH   | 6   | 152.1 s  |
| MSumCuCu_SDas | 7   | 6.047 s  |
 */

}
