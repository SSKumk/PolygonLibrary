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
public class MSumBench_Cyclic {

  // Дальше 4 размерности вычисляется не пойми что
  [Params(3, 4)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  [Params(8, 12, 16)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  private ConvexPolytop P;

  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cyclic(dim,amount, 0.0123,true);
    Q = ConvexPolytop.Cyclic(dim,amount, 0.0457,true);
  }

  [Benchmark]
  public void MSumClCl_SDas() => MinkowskiSum.BySandipDas(P, Q);

  [Benchmark]
  public void MSumClCl_SDasCutted() => MinkowskiSum.BySandipDas(P, Q, true);

  [Benchmark]
  public void MSumClCl_CH() => MinkowskiSum.ByConvexHull(P, Q);

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MSumBench_Cyclic>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

 /*
| Method              | dim | amount | Mean     |
|-------------------- |---- |------- |---------:|
| MSumClCl_SDas       | 3   | 8      | 0.0063 s |
| MSumClCl_SDasCutted | 3   | 8      | 0.0049 s |
| MSumClCl_CH         | 3   | 8      | 0.0077 s |
| MSumClCl_SDas       | 3   | 12     | 0.0157 s |
| MSumClCl_SDasCutted | 3   | 12     | 0.0130 s |
| MSumClCl_CH         | 3   | 12     | 0.0235 s |
| MSumClCl_SDas       | 3   | 16     | 0.0280 s |
| MSumClCl_SDasCutted | 3   | 16     | 0.0244 s |
| MSumClCl_CH         | 3   | 16     | 0.0522 s |
| MSumClCl_SDas       | 4   | 8      | 0.0596 s |
| MSumClCl_SDasCutted | 4   | 8      | 0.0447 s |
| MSumClCl_CH         | 4   | 8      | 0.0817 s |
| MSumClCl_SDas       | 4   | 12     | 0.3348 s |
| MSumClCl_SDasCutted | 4   | 12     | 0.3005 s |
| MSumClCl_CH         | 4   | 12     | 0.2830 s |
| MSumClCl_SDas       | 4   | 16     | 1.1404 s |
| MSumClCl_SDasCutted | 4   | 16     | 1.0879 s |
| MSumClCl_CH         | 4   | 16     | 0.7089 s |
| MSumClCl_SDas       | 5   | 8      | 0.2312 s |
| MSumClCl_SDasCutted | 5   | 8      | 0.1629 s |
| MSumClCl_CH         | 5   | 8      | 0.7972 s |
| MSumClCl_SDas       | 5   | 12     | 2.0184 s |
| MSumClCl_SDasCutted | 5   | 12     | 1.7010 s |
| MSumClCl_CH         | 5   | 12     | 2.6561 s |
| MSumClCl_SDas       | 5   | 16     | 8.4358 s |
| MSumClCl_SDasCutted | 5   | 16     | 8.0215 s |
| MSumClCl_CH         | 5   | 16     | 5.8697 s |




 */

}
