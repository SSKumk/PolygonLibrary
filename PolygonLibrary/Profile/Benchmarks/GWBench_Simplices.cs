namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<ddouble, Tests.DDConvertor>;

[ShortRunJob]
public class GWBench_Simplices {

  [Params(7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? simplex;

  [Params(0,10,100,1000)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  [GlobalSetup]
  public void SetUp() {
    // simplex = ConvexPolytop.SimplexRND(dim);
    simplex = ConvexPolytop.AsVPolytop(Simplex(dim, out _, new int[]{dim}, amount).ToHashSet());

  }



  [Benchmark]
  public void GWSimplexRND() => GiftWrapping.WrapVRep(simplex!.VRep);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<GWBench_Simplices>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}
/*
Заворачиваем случайные симплексы с дополнительными точками
| Method       | dim | amount | Mean     | Error    | StdDev   |
|------------- |---- |------- |---------:|---------:|---------:|
| GWSimplexRND | 3   | 0      | 0.0001 s | 0.0000 s | 0.0000 s |
| GWSimplexRND | 3   | 10     | 0.0002 s | 0.0001 s | 0.0000 s |
| GWSimplexRND | 3   | 100    | 0.0009 s | 0.0000 s | 0.0000 s |
| GWSimplexRND | 3   | 1000   | 0.0076 s | 0.0004 s | 0.0000 s |
| GWSimplexRND | 4   | 0      | 0.0009 s | 0.0001 s | 0.0000 s |
| GWSimplexRND | 4   | 10     | 0.0011 s | 0.0002 s | 0.0000 s |
| GWSimplexRND | 4   | 100    | 0.0021 s | 0.0001 s | 0.0000 s |
| GWSimplexRND | 4   | 1000   | 0.0120 s | 0.0013 s | 0.0001 s |
| GWSimplexRND | 5   | 0      | 0.0054 s | 0.0001 s | 0.0000 s |
| GWSimplexRND | 5   | 10     | 0.0057 s | 0.0002 s | 0.0000 s |
| GWSimplexRND | 5   | 100    | 0.0078 s | 0.0003 s | 0.0000 s |
| GWSimplexRND | 5   | 1000   | 0.0208 s | 0.0008 s | 0.0000 s |
| GWSimplexRND | 6   | 0      | 0.0378 s | 0.0081 s | 0.0004 s |
| GWSimplexRND | 6   | 10     | 0.0357 s | 0.0133 s | 0.0007 s |
| GWSimplexRND | 6   | 100    | 0.0371 s | 0.0016 s | 0.0001 s |
| GWSimplexRND | 6   | 1000   | 0.0577 s | 0.0476 s | 0.0026 s |
| GWSimplexRND | 7   | 0      | 0.2474 s | 0.0310 s | 0.0017 s |
| GWSimplexRND | 7   | 10     | 0.2768 s | 0.0070 s | 0.0004 s |
| GWSimplexRND | 7   | 100    | 0.2470 s | 0.0181 s | 0.0010 s |
| GWSimplexRND | 7   | 1000   | 0.2756 s | 0.0159 s | 0.0009 s |

 */