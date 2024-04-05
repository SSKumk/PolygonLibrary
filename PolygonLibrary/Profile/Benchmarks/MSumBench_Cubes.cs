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
public class MSumBench_Cubes {

  [Params(3, 4, 5)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop P;

  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Cube01(dim).RotateRND();
    Q = ConvexPolytop.Cube01(dim).RotateRND();
  }

  public void MSumCuCu_SDas() => MinkowskiSum.BySandipDas(P, Q);

  public void MSumCuCu_CH() => MinkowskiSum.ByConvexHull(P, Q);

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MSumBench_Cubes>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }
}
