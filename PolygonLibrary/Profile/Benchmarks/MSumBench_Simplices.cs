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
public class MSumBench_Simplices {

  // config.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.<WhateverYouWant>))
  [Params(3, 4, 5, 6, 7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  public ConvexPolytop P;

  public ConvexPolytop Q;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.SimplexRND(dim, true);
    Q = ConvexPolytop.SimplexRND(dim, true);
  }

  public void MSumSmSm_SDas() => MinkowskiSum.BySandipDas(P, Q);

  public void MSumSmSm_CH() => MinkowskiSum.ByConvexHull(P, Q);

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<MSumBench_Simplices>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }
}
