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
[WarmupCount(1)]
public class MDiffBench_Simplices
{

  [Params(3, 4, 5, 6)]
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

  //   public static void Main(string[] args)
  //   {
  //     var summary = BenchmarkRunner.Run<MDiffBench_Simplices>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)));
  //   }

  // }

}

/*
| Method          | dim | Mean      | Error     | StdDev    |
|---------------- |---- |----------:|----------:|----------:|
| MDiffSimplexRND | 3   | 0.0069 ms | 0.0027 ms | 0.0001 ms |
| MDiffSimplexRND | 4   | 0.0125 ms | 0.0026 ms | 0.0001 ms |
| MDiffSimplexRND | 5   | 0.0221 ms | 0.0044 ms | 0.0002 ms |
| MDiffSimplexRND | 6   | 0.0359 ms | 0.0020 ms | 0.0001 ms |
 */