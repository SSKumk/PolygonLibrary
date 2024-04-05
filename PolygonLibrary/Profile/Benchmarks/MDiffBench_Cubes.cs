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
public class MDiffBench_Cubes {

  [Params(3, 4, 5, 6)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop P;
  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cube01(dim).RotateRND(true);
    Q = ConvexPolytop.Cube01(dim).RotateRND();
  }

  [Benchmark]
  public void MDiffCubes() => MinkowskiDiff.Naive(P,Q);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MDiffBench_Cubes>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}

/*

 */