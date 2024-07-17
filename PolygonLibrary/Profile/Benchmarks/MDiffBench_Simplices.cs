namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
public class MDiffBench_Simplices
{

  [Params(3, 4, 5, 6,7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop P;
  private ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp()
  {
    P = ConvexPolytop.SimplexRND(dim, true);
    Q = ConvexPolytop.SimplexRND(dim);
  }

  [Benchmark]
  public void MDiffSimplexRND() => MinkowskiDiff.Naive(P, Q);


  // public class Program
  // {
  //
  //   public static void Main(string[] args)
  //   {
  //     var summary = BenchmarkRunner.Run<MDiffBench_Simplices>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)));
  //   }
  //
  // }

}

/*
| Method          | dim | Mean      | Error     | StdDev    |
|---------------- |---- |----------:|----------:|----------:|
| MDiffSimplexRND | 3   | 0.0085 ms | 0.0006 ms | 0.0000 ms |
| MDiffSimplexRND | 4   | 0.0162 ms | 0.0052 ms | 0.0003 ms |
| MDiffSimplexRND | 5   | 0.0281 ms | 0.0056 ms | 0.0003 ms |
| MDiffSimplexRND | 6   | 0.0454 ms | 0.0042 ms | 0.0002 ms |
| MDiffSimplexRND | 7   | 0.0719 ms | 0.0016 ms | 0.0001 ms |

 */