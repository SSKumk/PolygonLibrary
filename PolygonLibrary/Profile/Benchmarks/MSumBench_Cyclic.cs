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

  [Params(3, 4, 5)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  [Params(8, 12, 16)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  private ConvexPolytop P;

  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cyclic(dim,amount, 1.123,true);
    Q = ConvexPolytop.Cyclic(dim,amount, 1.457,true);
  }

  [Benchmark]
  public void MSumClCl_SDas() => MinkowskiSum.BySandipDas(P, Q);

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
| Method        | dim | amount | Mean     | Error    | StdDev   |
|-------------- |---- |------- |---------:|---------:|---------:|
| MSumClCl_SDas | 3   | 8      | 0.0061 s | 0.0005 s | 0.0000 s |
| MSumClCl_CH   | 3   | 8      | 0.0077 s | 0.0003 s | 0.0000 s |
| MSumClCl_SDas | 3   | 12     | 0.0154 s | 0.0018 s | 0.0001 s |
| MSumClCl_CH   | 3   | 12     | 0.0235 s | 0.0004 s | 0.0000 s |
| MSumClCl_SDas | 3   | 16     | 0.0279 s | 0.0017 s | 0.0001 s |
| MSumClCl_CH   | 3   | 16     | 0.0531 s | 0.0049 s | 0.0003 s |
| MSumClCl_SDas | 4   | 8      | 0.0593 s | 0.0006 s | 0.0000 s |
| MSumClCl_CH   | 4   | 8      | 0.0769 s | 0.0032 s | 0.0002 s |
| MSumClCl_SDas | 4   | 12     | 0.3276 s | 0.0544 s | 0.0030 s |
| MSumClCl_CH   | 4   | 12     | 0.2884 s | 0.0586 s | 0.0032 s |
| MSumClCl_SDas | 4   | 16     | 1.1388 s | 0.1298 s | 0.0071 s |
| MSumClCl_CH   | 4   | 16     | 0.6901 s | 0.0910 s | 0.0050 s |
| MSumClCl_SDas | 5   | 8      | 0.2303 s | 0.0066 s | 0.0004 s |
| MSumClCl_CH   | 5   | 8      | 0.8722 s | 0.0621 s | 0.0034 s |
| MSumClCl_SDas | 5   | 12     | 1.9211 s | 0.1915 s | 0.0105 s |
| MSumClCl_CH   | 5   | 12     | 2.8804 s | 0.4738 s | 0.0260 s |
| MSumClCl_SDas | 5   | 16     | 8.1156 s | 0.9638 s | 0.0528 s |
| MSumClCl_CH   | 5   | 16     | 5.9135 s | 0.7072 s | 0.0388 s |

 */

}
