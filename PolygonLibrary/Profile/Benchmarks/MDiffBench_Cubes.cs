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
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)));
  //   }
  //
  // }

}

/*
| Method     | dim | Mean      | Error     | StdDev    |
|----------- |---- |----------:|----------:|----------:|
| MDiffCubes | 3   | 0.0239 ms | 0.0019 ms | 0.0001 ms |
| MDiffCubes | 4   | 0.1175 ms | 0.0111 ms | 0.0006 ms |
| MDiffCubes | 5   | 0.6303 ms | 0.0636 ms | 0.0035 ms |
| MDiffCubes | 6   | 3.3543 ms | 0.2918 ms | 0.0160 ms |

 */