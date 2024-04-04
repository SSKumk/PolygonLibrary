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
public class GWBenchSpheres {

  // [Params(3, 4, 5, 6)]
  [Params(3, 4)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? polytop;

  // [Params(0,10,100,1000)]
  // // ReSharper disable once UnassignedField.Global
  // public int amount;

  [Params( 10, 18, 36)]
  // ReSharper disable once UnassignedField.Global
  public int thetaPartition;

  [Params(10, 16, 32)]
  // ReSharper disable once UnassignedField.Global
  public int phiPartition;

  [GlobalSetup]
  public void SetUp() {
    var sphere = Sphere_list(dim, thetaPartition, phiPartition, 3);
    polytop = ConvexPolytop.AsVPolytop(sphere.ToHashSet());
  }

/*
Заворачиваем сферы с дополнительными точками


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
