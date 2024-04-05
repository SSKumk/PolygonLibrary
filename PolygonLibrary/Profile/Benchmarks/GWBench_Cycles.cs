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
public class GWBench_Cycles
{

  [Params(3, 4, 5, 6)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? cycle;

  // [Params(8, 16, 32, 64, 128)]
  [Params(8, 16, 32, 64, 128)]
  // ReSharper disable once UnassignedField.Global
  public int amountPointsOnMomentCurve;

  [GlobalSetup]
  public void SetUp()
  {
    cycle = ConvexPolytop.Cyclic
      (dim, amountPointsOnMomentCurve, 1.0 / (amountPointsOnMomentCurve));
  }

  /*
    Заворачиваем циклические многогранники. Количество точек было выбрано "из головы".

  */
  [Benchmark]
  public void GWCyclicPolytop() => GiftWrapping.WrapVRep(cycle!.VRep);


  // public class Program
  // {
  //
  //   public static void Main(string[] args)
  //   {
  //     var summary = BenchmarkRunner.Run<GWBench_Cycles>(
  //       DefaultConfig.Instance.WithSummaryStyle(
  //         SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)));
  //   }
  //
  // }
}
