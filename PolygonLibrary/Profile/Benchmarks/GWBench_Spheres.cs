namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
public class GWBenchSpheres {

  // [Params(3, 4, 5, 6)]
  [Params(3, 4)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? polytop;

  [Params(0, 10, 100, 1000)]
  // ReSharper disable once UnassignedField.Global
  public int amount;

  [Params(10, 18)]
  // ReSharper disable once UnassignedField.Global
  public int thetaPartition;

  [Params(10, 16)]
  // ReSharper disable once UnassignedField.Global
  public int phiPartition;

  [GlobalSetup]
  public void SetUp() {
    var sphere = Sphere_list(dim, thetaPartition, phiPartition, 3);
    var S      = sphere.Union(SimplexRND(dim, out _, new int[] { dim }, amount - dim - 1));
    polytop = ConvexPolytop.AsVPolytop(S.ToHashSet());
  }


  [Benchmark]
  public void GWSphere() => GiftWrapping.WrapVRep(polytop!.VRep);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<GWBenchSpheres>
  //       (DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Second)));
  //   }
  //
  // }

}

/*
Заворачиваем сферы с дополнительными точками
| Method   | dim | amount | thetaPartition | phiPartition | Mean      | Error     | StdDev   |
|--------- |---- |------- |--------------- |------------- |----------:|----------:|---------:|
| GWSphere | 3   | 0      | 10             | 10           |  0.0164 s |  0.0014 s | 0.0001 s |
| GWSphere | 3   | 0      | 10             | 16           |  0.0359 s |  0.0088 s | 0.0005 s |
| GWSphere | 3   | 0      | 18             | 10           |  0.0449 s |  0.0053 s | 0.0003 s |
| GWSphere | 3   | 0      | 18             | 16           |  0.1027 s |  0.0045 s | 0.0002 s |
| GWSphere | 3   | 10     | 10             | 10           |  0.0170 s |  0.0019 s | 0.0001 s |
| GWSphere | 3   | 10     | 10             | 16           |  0.0363 s |  0.0036 s | 0.0002 s |
| GWSphere | 3   | 10     | 18             | 10           |  0.0462 s |  0.0085 s | 0.0005 s |
| GWSphere | 3   | 10     | 18             | 16           |  0.1038 s |  0.0251 s | 0.0014 s |
| GWSphere | 3   | 100    | 10             | 10           |  0.0293 s |  0.0017 s | 0.0001 s |
| GWSphere | 3   | 100    | 10             | 16           |  0.0555 s |  0.0034 s | 0.0002 s |
| GWSphere | 3   | 100    | 18             | 10           |  0.0666 s |  0.0055 s | 0.0003 s |
| GWSphere | 3   | 100    | 18             | 16           |  0.1317 s |  0.0144 s | 0.0008 s |
| GWSphere | 3   | 1000   | 10             | 10           |  0.1502 s |  0.0054 s | 0.0003 s |
| GWSphere | 3   | 1000   | 10             | 16           |  0.2356 s |  0.0633 s | 0.0035 s |
| GWSphere | 3   | 1000   | 18             | 10           |  0.2645 s |  0.0183 s | 0.0010 s |
| GWSphere | 3   | 1000   | 18             | 16           |  0.4290 s |  0.0612 s | 0.0034 s |
| GWSphere | 4   | 0      | 10             | 10           |  2.0000 s |  0.5791 s | 0.0317 s |
| GWSphere | 4   | 0      | 10             | 16           |  4.0037 s |  1.5619 s | 0.0856 s |
| GWSphere | 4   | 0      | 18             | 10           | 14.9503 s |  2.3061 s | 0.1264 s |
| GWSphere | 4   | 0      | 18             | 16           | 35.6670 s |  3.4093 s | 0.1869 s |
| GWSphere | 4   | 10     | 10             | 10           |  1.9472 s |  0.1312 s | 0.0072 s |
| GWSphere | 4   | 10     | 10             | 16           |  3.9371 s |  0.5301 s | 0.0291 s |
| GWSphere | 4   | 10     | 18             | 10           | 15.0854 s |  1.0379 s | 0.0569 s |
| GWSphere | 4   | 10     | 18             | 16           | 36.4134 s |  3.9148 s | 0.2146 s |
| GWSphere | 4   | 100    | 10             | 10           |  2.5002 s |  3.5168 s | 0.1928 s |
| GWSphere | 4   | 100    | 10             | 16           |  4.3221 s |  5.2738 s | 0.2891 s |
| GWSphere | 4   | 100    | 18             | 10           | 16.4536 s | 16.7765 s | 0.9196 s |
| GWSphere | 4   | 100    | 18             | 16           | 36.1814 s | 16.3318 s | 0.8952 s |
| GWSphere | 4   | 1000   | 10             | 10           |  4.0765 s |  3.2863 s | 0.1801 s |
| GWSphere | 4   | 1000   | 10             | 16           |  6.5171 s |  0.5906 s | 0.0324 s |
| GWSphere | 4   | 1000   | 18             | 10           | 20.7711 s | 16.2601 s | 0.8913 s |
| GWSphere | 4   | 1000   | 18             | 16           | 44.4061 s | 47.0076 s | 2.5766 s |


 */