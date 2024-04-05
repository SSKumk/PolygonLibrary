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

  [Params(8, 16, 32)]
  // ReSharper disable once UnassignedField.Global
  public int amountPointsOnMomentCurve;

  [GlobalSetup]
  public void SetUp()
  {
    cycle = ConvexPolytop.Cyclic
      (dim, amountPointsOnMomentCurve, 1);
  }

  [Benchmark]
  public void GWCyclicPolytop() => GiftWrapping.WrapVRep(cycle!.VRep);


  public class Program
  {

    public static void Main(string[] args)
    {
      var summary = BenchmarkRunner.Run<GWBench_Cycles>(
        DefaultConfig.Instance.WithSummaryStyle(
          SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
    }

  }
}

/*
  Заворачиваем циклические многогранники. Количество точек было выбрано "из головы".
| Method          | dim | amountPointsOnMomentCurve | Mean      | Error    | StdDev   |
|---------------- |---- |-------------------------- |----------:|---------:|---------:|
| GWCyclicPolytop | 3   | 8                         |  0.0005 s | 0.0000 s | 0.0000 s |
| GWCyclicPolytop | 3   | 16                        |  0.0016 s | 0.0002 s | 0.0000 s |
| GWCyclicPolytop | 3   | 32                        |  0.0048 s | 0.0004 s | 0.0000 s |
| GWCyclicPolytop | 4   | 8                         |  0.0040 s | 0.0012 s | 0.0001 s |
| GWCyclicPolytop | 4   | 16                        |  0.0237 s | 0.0085 s | 0.0005 s |
| GWCyclicPolytop | 4   | 32                        |  0.1298 s | 0.2208 s | 0.0121 s |
| GWCyclicPolytop | 5   | 8                         |  0.0183 s | 0.0004 s | 0.0000 s |
| GWCyclicPolytop | 5   | 16                        |  0.1488 s | 0.0053 s | 0.0003 s |
| GWCyclicPolytop | 5   | 32                        |  0.8125 s | 0.0104 s | 0.0006 s |
| GWCyclicPolytop | 6   | 8                         |  0.0799 s | 0.0036 s | 0.0002 s |
| GWCyclicPolytop | 6   | 16                        |  1.8076 s | 0.1091 s | 0.0060 s |
| GWCyclicPolytop | 6   | 32                        | 21.0714 s | 8.2077 s | 0.4499 s |

*/