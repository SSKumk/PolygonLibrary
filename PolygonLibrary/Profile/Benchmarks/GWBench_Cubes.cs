namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
public class GWBenchCubes {

  // [Params(3, 4, 5, 6)]
  [Params(7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? polytop;

  [Params(0,10,100,1000)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  [GlobalSetup]
  public void SetUp() {
    polytop = ConvexPolytop.AsVPolytop(Cube01(dim, out _, new int[]{dim}, amount).ToHashSet());

  }

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
| GWCube | 7   | 0      | 13.96 s  | 5.849 s  | 0.321 s  |
| GWCube | 7   | 10     | 15.05 s  | 2.924 s  | 0.160 s  |
| GWCube | 7   | 100    | 13.53 s  | 3.277 s  | 0.180 s  |
| GWCube | 7   | 1000   | 15.32 s  | 3.632 s  | 0.199 s  |

 */