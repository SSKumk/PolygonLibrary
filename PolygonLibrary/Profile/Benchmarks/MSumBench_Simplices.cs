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
public class MSumBench_Simplices {

  [Params(3, 4, 5, 6, 7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  public ConvexPolytop P;

  public ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.SimplexRND(dim, true);
    Q = ConvexPolytop.SimplexRND(dim, true);
  }

  [Benchmark]
  public void MSumSmSm_SDas() => MinkowskiSum.BySandipDas(P, Q);

  [Benchmark]
  public void MSumSmSm_CH() => MinkowskiSum.ByConvexHull(P, Q);

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MSumBench_Simplices>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }
}

/*
| Method        | dim | Mean      | Error     | StdDev   |
|-------------- |---- |----------:|----------:|---------:|
| MSumSmSm_SDas | 3   |  0.0011 s |  0.0001 s | 0.0000 s |
| MSumSmSm_CH   | 3   |  0.0014 s |  0.0001 s | 0.0000 s |
| MSumSmSm_SDas | 4   |  0.0072 s |  0.0009 s | 0.0000 s |
| MSumSmSm_CH   | 4   |  0.0179 s |  0.0009 s | 0.0000 s |
| MSumSmSm_SDas | 5   |  0.0412 s |  0.0065 s | 0.0004 s |
| MSumSmSm_CH   | 5   |  0.2547 s |  0.0235 s | 0.0013 s |
| MSumSmSm_SDas | 6   |  0.2695 s |  0.1675 s | 0.0092 s |
| MSumSmSm_CH   | 6   |  4.9545 s |  1.4820 s | 0.0812 s |
| MSumSmSm_SDas | 7   |  1.3786 s |  0.8522 s | 0.0467 s |
| MSumSmSm_CH   | 7   | 99.8668 s | 41.4882 s | 2.2741 s |
 */
