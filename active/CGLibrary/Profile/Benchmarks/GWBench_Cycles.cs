namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
[IterationCount(1)]
public class GWBench_Cycles {

  [Params(6)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? cycle;

  [Params(32)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  [GlobalSetup]
  public void SetUp() { cycle = ConvexPolytop.Cyclic(dim, amount, 0.1); }

  [Benchmark]
  public void GWCyclicPolytop() => GiftWrapping.WrapVRep(cycle!.Vrep);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<GWBench_Cycles>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}

/*
  Заворачиваем циклические многогранники. Количество точек было выбрано "из головы".
| Method          | dim | amount | Mean      | Error    | StdDev   |
|---------------- |---- |------- |----------:|---------:|---------:|
| GWCyclicPolytop | 3   | 8      |  0.0006 s | 0.0002 s | 0.0000 s |
| GWCyclicPolytop | 3   | 16     |  0.0018 s | 0.0009 s | 0.0000 s |
| GWCyclicPolytop | 3   | 32     |  0.0058 s | 0.0008 s | 0.0000 s |
| GWCyclicPolytop | 3   | 64     |  0.0192 s | 0.0043 s | 0.0002 s |
| GWCyclicPolytop | 4   | 8      |  0.0053 s | 0.0014 s | 0.0001 s |
| GWCyclicPolytop | 4   | 16     |  0.0293 s | 0.0132 s | 0.0007 s |
| GWCyclicPolytop | 4   | 32     |  0.1479 s | 0.0183 s | 0.0010 s |
| GWCyclicPolytop | 4   | 64     |  0.8745 s | 0.2203 s | 0.0121 s |
| GWCyclicPolytop | 5   | 8      |  0.0233 s | 0.0101 s | 0.0006 s |
| GWCyclicPolytop | 5   | 16     |  0.1947 s | 0.2284 s | 0.0125 s |
| GWCyclicPolytop | 5   | 32     |  1.1047 s | 0.3241 s | 0.0178 s |
| GWCyclicPolytop | 5   | 64     |  4.9086 s | 0.7759 s | 0.0425 s |
| GWCyclicPolytop | 6   | 8      |  0.1009 s | 0.0301 s | 0.0017 s |
| GWCyclicPolytop | 6   | 16     |  2.3233 s | 1.4533 s | 0.0797 s |
| GWCyclicPolytop | 6   | 32     | 28.6002 s | 0.9271 s | 0.0508 s |
| GWCyclicPolytop | 6   | 64     | 249.4 s   | 23.85 s  | 1.31 s   |


*/
