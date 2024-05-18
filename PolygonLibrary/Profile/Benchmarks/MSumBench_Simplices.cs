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

  // [Benchmark]
  // public void MSumSmSm_SDas() => MinkowskiSum.BySandipDas(P, Q);

  [Benchmark]
  public void MSumSmSm_SDasCutted() => MinkowskiSum.BySandipDas(P, Q, true);

  // [Benchmark]
  // public void MSumSmSm_CH() => MinkowskiSum.ByConvexHull(P, Q);

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
| Method        | dim | Mean      |
|-------------- |---- |----------:|
| MSumSmSm_SDas | 3   |  0.0011 s |
| MSumSmSm_CH   | 3   |  0.0014 s |
| MSumSmSm_SDas | 4   |  0.0072 s |
| MSumSmSm_CH   | 4   |  0.0179 s |
| MSumSmSm_SDas | 5   |  0.0412 s |
| MSumSmSm_CH   | 5   |  0.2547 s |
| MSumSmSm_SDas | 6   |  0.2695 s |
| MSumSmSm_CH   | 6   |  4.9545 s |
| MSumSmSm_SDas | 7   |  1.294 s  |
| MSumSmSm_CH   | 7   | 93.251 s  |
| MSumSmSm_SDasCutted | 3   | 0.0007 s|
| MSumSmSm_SDasCutted | 4   | 0.0040 s|
| MSumSmSm_SDasCutted | 5   | 0.0217 s|
| MSumSmSm_SDasCutted | 6   | 0.1333 s|
| MSumSmSm_SDasCutted | 7   | 0.6503 s|

 */
