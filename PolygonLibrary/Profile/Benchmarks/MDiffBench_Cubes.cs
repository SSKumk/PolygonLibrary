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
[WarmupCount(1)]
public class MDiffBench_Cubes {

  [Params(3, 4, 5, 6, 7)]
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
  public void MDiffCubes() => MinkowskiDiff.Naive(P, Q);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MDiffBench_Cubes>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)));
  //   }
  //
  // }

}

/*
| Method     | dim | Mean       | Error     | StdDev    |
|----------- |---- |-----------:|----------:|----------:|
| MDiffCubes | 3   |  0.0246 ms | 0.0052 ms | 0.0003 ms |
| MDiffCubes | 4   |  0.1191 ms | 0.0043 ms | 0.0002 ms |
| MDiffCubes | 5   |  0.6516 ms | 0.2482 ms | 0.0136 ms |
| MDiffCubes | 6   |  3.3697 ms | 0.0780 ms | 0.0043 ms |
| MDiffCubes | 7   | 18.0803 ms | 7.8944 ms | 0.4327 ms |
 */
