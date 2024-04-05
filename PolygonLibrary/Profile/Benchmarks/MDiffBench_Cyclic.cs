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
using static Tests.ToolsTests.TestsPolytopes<ddouble, Tests.DDConvertor>;

[ShortRunJob]
public class MDiffBench_Cyclic {

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
    P = ConvexPolytop.Cyclic(dim, amount, 1, true);
    Q = ConvexPolytop.Cyclic(dim, amount, 1);
  }


  [Benchmark]
  public void MDiffCyclic() => MinkowskiDiff.Naive(P, Q);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MDiffBench_Cyclic>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}

/*

 */
