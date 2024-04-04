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
public class GWBenchCubes {

  // [Params(3, 4, 5, 6)]
  [Params(3, 4)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? polytop;

  [Params(0,10,100,1000)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  [GlobalSetup]
  public void SetUp() {
    polytop = ConvexPolytop.AsVPolytop(Cube(dim, out _, new int[]{dim}, amount).ToHashSet());

  }

/*
Заворачиваем кубы с дополнительными точками
| Method | dim | amount | Mean     | Error    | StdDev   |
|------- |---- |------- |---------:|---------:|---------:|
| GWCube | 3   | 0      | 0.0003 s | 0.0000 s | 0.0000 s |
| GWCube | 3   | 10     | 0.0005 s | 0.0003 s | 0.0000 s |
| GWCube | 3   | 100    | 0.0015 s | 0.0006 s | 0.0000 s |
| GWCube | 3   | 1000   | 0.0117 s | 0.0028 s | 0.0002 s |
| GWCube | 4   | 0      | 0.0039 s | 0.0003 s | 0.0000 s |
| GWCube | 4   | 10     | 0.0039 s | 0.0005 s | 0.0000 s |
| GWCube | 4   | 100    | 0.0055 s | 0.0016 s | 0.0001 s |
| GWCube | 4   | 1000   | 0.0224 s | 0.0066 s | 0.0004 s |
| GWCube | 5   | 0      | 0.0554 s | 0.0088 s | 0.0005 s |
| GWCube | 5   | 10     | 0.0547 s | 0.0183 s | 0.0010 s |
| GWCube | 5   | 100    | 0.0602 s | 0.0023 s | 0.0001 s |
| GWCube | 5   | 1000   | 0.0797 s | 0.0410 s | 0.0022 s |
| GWCube | 6   | 0      | 0.7600 s | 0.1162 s | 0.0064 s |
| GWCube | 6   | 10     | 0.9271 s | 0.2008 s | 0.0110 s |
| GWCube | 6   | 100    | 0.8453 s | 0.2616 s | 0.0143 s |
| GWCube | 6   | 1000   | 0.8339 s | 0.1995 s | 0.0109 s |

 */

  [Benchmark]
  public void GWCube() => GiftWrapping.WrapVRep(polytop!.VRep);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<GWBenchCubes>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}
