namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
public class HRepToVRepBench {

  [Params(3)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  [Params(4, 8, 12)]
  // ReSharper disable once UnassignedField.Global
  public int theta;

  [Params(4, 8, 12)]
  // ReSharper disable once UnassignedField.Global
  public int phi;

  private ConvexPolytop P;

  [GlobalSetup]
  public void SetUp() {
    P = ConvexPolytop.Sphere(dim, theta, phi, Vector.Zero(3), 1);
    Console.WriteLine($"Hrep.Count = {P.Hrep.Count}");
  }

  [Benchmark]
  public void H2V_Naive() => ConvexPolytop.HRepToVRep_Naive(P.Hrep);

  [Benchmark]
  public void H2V_Geometric() => ConvexPolytop.HRepToVRep_Geometric(P.Hrep);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<HRepToVRepBench>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Millisecond)));
  //   }
  //
  // }

}

/*
| Method        | dim | theta | phi | Mean          |
|-------------- |---- |------ |---- |--------------:|
| H2V_Naive     | 3   | 4     | 4   |     0.6977 ms |
| H2V_Geometric | 3   | 4     | 4   |     0.4022 ms |
| H2V_Naive     | 3   | 4     | 8   |     7.7309 ms |
| H2V_Geometric | 3   | 4     | 8   |     1.1483 ms |
| H2V_Naive     | 3   | 4     | 12  |    29.5216 ms |
| H2V_Geometric | 3   | 4     | 12  |     2.2879 ms |
| H2V_Naive     | 3   | 8     | 4   |     6.0210 ms |
| H2V_Geometric | 3   | 8     | 4   |     1.1462 ms |
| H2V_Naive     | 3   | 8     | 8   |    64.7978 ms |
| H2V_Geometric | 3   | 8     | 8   |     3.5726 ms |
| H2V_Naive     | 3   | 8     | 12  |   273.1547 ms |
| H2V_Geometric | 3   | 8     | 12  |     7.2123 ms |
| H2V_Naive     | 3   | 12    | 4   |    21.1797 ms |
| H2V_Geometric | 3   | 12    | 4   |     2.3026 ms |
| H2V_Naive     | 3   | 12    | 8   |   237.8120 ms |
| H2V_Geometric | 3   | 12    | 8   |     7.3941 ms |
| H2V_Naive     | 3   | 12    | 12  | 1,004.9469 ms |
| H2V_Geometric | 3   | 12    | 12  |    15.1339 ms |

 */
