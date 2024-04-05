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

  [Params(8, 16, 32)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  private ConvexPolytop P;

  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cyclic(dim,amount, 1,true);
    Q = ConvexPolytop.Cyclic(dim,amount, 1,true);
  }

  [Benchmark]
  public void MSumClCl_SDas() => MinkowskiSum.BySandipDas(P, Q);

  [Benchmark]
  public void MSumClCl_CH() => MinkowskiSum.ByConvexHull(P, Q);

  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<MSumBench_Cyclic>
        (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
    }

  }

 /*

 */

}
