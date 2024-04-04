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
// [WarmupCount(1)]
// [IterationCount(1)]
public class GWBenchSpheres {

  // [Params(3, 4, 5, 6)]
  [Params(3, 4)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? polytop;

  // [Params(0,10,100,1000)]
  // // ReSharper disable once UnassignedField.Global
  // public int amount;

  [Params( 10, 18)]
  // ReSharper disable once UnassignedField.Global
  public int thetaPartition;

  [Params(10, 16)]
  // ReSharper disable once UnassignedField.Global
  public int phiPartition;

  [GlobalSetup]
  public void SetUp() {
    var sphere = Sphere_list(dim, thetaPartition, phiPartition, 3);
    polytop = ConvexPolytop.AsVPolytop(sphere.ToHashSet());
  }

/*
Заворачиваем сферы с дополнительными точками

| Method   | dim | thetaPartition | phiPartition | Mean       | Error |
|--------- |---- |--------------- |------------- |-----------:|------:|
| GWSphere | 3   | 10             | 10           |   0.0222 s |    NA |
| GWSphere | 3   | 10             | 16           |   0.0526 s |    NA |
| GWSphere | 3   | 18             | 10           |   0.0671 s |    NA |
| GWSphere | 3   | 18             | 16           |   0.1601 s |    NA |
| GWSphere | 3   | 36             | 10           |   0.2446 s |    NA |
| GWSphere | 3   | 36             | 16           |   0.6793 s |    NA |
| GWSphere | 4   | 10             | 10           |   2.5352 s |    NA |
| GWSphere | 4   | 10             | 16           |   6.1830 s |    NA |
| GWSphere | 4   | 18             | 10           |  26.8099 s |    NA |
| GWSphere | 4   | 18             | 16           |  57.5789 s |    NA |
| GWSphere | 4   | 36             | 10           | 361.7732 s |    NA |
| GWSphere | 4   | 36             | 16           | 924.0218 s |    NA |

 */

  [Benchmark]
  public void GWSphere() => GiftWrapping.WrapVRep(polytop!.VRep);


  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<GWBenchSpheres>
        (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
    }

  }

}
