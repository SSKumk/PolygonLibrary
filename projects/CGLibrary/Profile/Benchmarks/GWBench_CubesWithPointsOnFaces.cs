namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;
using static Tests.TestInfrastructure.TestsBase<ddouble, Tests.DDConvertor>;
using static Tests.TestInfrastructure.TestsPolytopes<ddouble, Tests.DDConvertor>;

[ShortRunJob]
[WarmupCount(1)]
public class GWBenchCubesWithPointsOnFaces {

  [Params(3, 4, 5, 6)]
  public int dim;

  [Params(1, 10, 100)]
  public int pointsPerFace;

  private List<Vector> points = null!;

  [GlobalSetup]
  public void SetUp() {
    GRandomLC random = new GRandomLC(255);
    List<int> fIDs   = Enumerable.Range(1, dim).ToList();
    points = RotateRND(Cube01(dim, out _, fIDs, pointsPerFace, random, true), random);
  }

  [Benchmark]
  public GiftWrapping GWCubesWithPointsOnFaces() => new GiftWrapping(points);

}
