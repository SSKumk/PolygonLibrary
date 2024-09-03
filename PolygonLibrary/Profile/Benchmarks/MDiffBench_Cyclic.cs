namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
public class MDiffBench_Cyclic
{

  [Params(3, 4, 5)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  [Params(8, 12, 14)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  private ConvexPolytop P = null!;
  private ConvexPolytop Q = null!;

  [GlobalSetup]
  public void SetUp()
  {
    P = ConvexPolytop.Cyclic(dim, amount, 1.123, true);
    Q = ConvexPolytop.Cyclic(dim, amount, 1.457);
  }


  [Benchmark]
  public void MDiffCyclic() => MinkowskiDiff.Naive(P, Q);


  // public class Program
  // {
  //
  //   public static void Main(string[] args)
  //   {
  //     var summary = BenchmarkRunner.Run<MDiffBench_Cyclic>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}

/*
| Method      | dim | amount | Mean       | Error        | StdDev    |
|------------ |---- |------- |-----------:|-------------:|----------:|
| MDiffCyclic | 3   | 8      |   0.0002 s |     0.0002 s |  0.0000 s |
| MDiffCyclic | 3   | 12     |   0.0011 s |     0.0002 s |  0.0000 s |
| MDiffCyclic | 3   | 14     |   0.0021 s |     0.0035 s |  0.0002 s |
| MDiffCyclic | 4   | 8      |   0.0084 s |     0.0059 s |  0.0003 s |
| MDiffCyclic | 4   | 12     |   0.5166 s |     0.1587 s |  0.0087 s |
| MDiffCyclic | 4   | 14     |   2.2492 s |     0.0611 s |  0.0034 s |
| MDiffCyclic | 5   | 8      |   0.0405 s |     0.0110 s |  0.0006 s |
| MDiffCyclic | 5   | 12     |  36.2913 s |     1.2559 s |  0.0688 s |
 */
