using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsBase<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsPolytopes<double, Tests.DConvertor>;
using Tests.DoubleGeometry.Polyhedra;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GiftWrappingStressTests {

  private static IEnumerable<TestCaseData> CubeStressCases() {
    yield return new TestCaseData(3, new[] { 1, 2, 3 }, 10, 6, 27, 3001u).SetName("CreateFromPoints_Stress_Cube3D_AllInnerLevels");
    yield return new TestCaseData(4, new[] { 1, 2, 4 }, 4, 8, 81, 4001u).SetName("CreateFromPoints_Stress_Cube4D_RepresentativeInnerLevels");
    yield return new TestCaseData(5, new[] { 1, 3, 5 }, 2, 10, 243, 5001u).SetName("CreateFromPoints_Stress_Cube5D_RepresentativeInnerLevels");
    yield return new TestCaseData(6, new[] { 1, 6 }, 1, 12, 729, 6001u).SetName("CreateFromPoints_Stress_Cube6D_EdgesAndInterior");
  }

  private static IEnumerable<TestCaseData> SimplexStressCases() {
    yield return new TestCaseData(3, new[] { 1, 2, 3 }, 10, 4, 15, 3101u).SetName("CreateFromPoints_Stress_Simplex3D_AllInnerLevels");
    yield return new TestCaseData(4, new[] { 1, 2, 3, 4 }, 8, 5, 31, 4101u).SetName("CreateFromPoints_Stress_Simplex4D_AllInnerLevels");
    yield return new TestCaseData(5, new[] { 1, 3, 5 }, 6, 6, 63, 5101u).SetName("CreateFromPoints_Stress_Simplex5D_RepresentativeInnerLevels");
    yield return new TestCaseData(6, new[] { 1, 6 }, 2, 7, 127, 6101u).SetName("CreateFromPoints_Stress_Simplex6D_EdgesAndInterior");
    yield return new TestCaseData(7, new[] { 1, 7 }, 1, 8, 255, 7101u).SetName("CreateFromPoints_Stress_Simplex7D_EdgesAndInterior");
  }

  private static IEnumerable<TestCaseData> RandomSimplexStressCases() {
    yield return new TestCaseData(3, new[] { 1, 2, 3 }, 8, 4, 15, 2337035596u).SetName("CreateFromPoints_Stress_RandomSimplex3D");
    yield return new TestCaseData(4, new[] { 1, 2, 4 }, 6, 5, 31, 42424242u).SetName("CreateFromPoints_Stress_RandomSimplex4D");
    yield return new TestCaseData(5, new[] { 1, 3, 5 }, 4, 6, 63, 987654321u).SetName("CreateFromPoints_Stress_RandomSimplex5D");
  }

  [OneTimeSetUp]
  public void SetUp() => Tools.Eps = 1e-8;

  private static void AssertStressCase(
      List<Vector> swarm
    , List<Vector> expectedVertices
    , int          dim
    , int          expectedHrepCount
    , int          expectedFaceCount
    , uint         seed
    ) {
    GRandomLC transformRandom = new(seed);
    ShiftAndRotate(dim, ref expectedVertices, ref swarm, transformRandom);
    swarm.Shuffle(new RandomLC(seed ^ 0xA5A5A5A5u));

    ConvexPolytop polytop = ConvexPolytop.CreateFromPoints(swarm, true);

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVertexSetEquals(polytop.Vrep, expectedVertices, $"Seed: {seed}");
      Assert.That(polytop.Hrep, Has.Count.EqualTo(expectedHrepCount), $"Seed: {seed}");
      Assert.That(polytop.FLrep.NumberOfKFaces, Is.EqualTo(expectedFaceCount), $"Seed: {seed}");
    });
  }

  [TestCaseSource(nameof(CubeStressCases))]
  public void CreateFromPoints_RepresentativeCubeStressCases_PreserveVerticesAndCombinatorics(
      int   dim
    , int[] faceDims
    , int   amount
    , int   expectedHrepCount
    , int   expectedFaceCount
    , uint  seed
    ) {
    List<Vector> swarm = Cube01(dim, out List<Vector> cube, faceDims, amount, new GRandomLC(seed));

    AssertStressCase(swarm, cube, dim, expectedHrepCount, expectedFaceCount, seed);
  }

  [TestCaseSource(nameof(SimplexStressCases))]
  public void CreateFromPoints_RepresentativeAxisSimplexStressCases_PreserveVerticesAndCombinatorics(
      int   dim
    , int[] faceDims
    , int   amount
    , int   expectedHrepCount
    , int   expectedFaceCount
    , uint  seed
    ) {
    List<Vector> swarm = Simplex(dim, out List<Vector> simplex, faceDims, amount, new GRandomLC(seed));

    AssertStressCase(swarm, simplex, dim, expectedHrepCount, expectedFaceCount, seed);
  }

  [TestCaseSource(nameof(RandomSimplexStressCases))]
  public void CreateFromPoints_RepresentativeRandomSimplexStressCases_PreserveVerticesAndCombinatorics(
      int   dim
    , int[] faceDims
    , int   amount
    , int   expectedHrepCount
    , int   expectedFaceCount
    , uint  seed
    ) {
    List<Vector> swarm = SimplexRND(dim, out List<Vector> simplex, faceDims, amount, new GRandomLC(seed));

    AssertStressCase(swarm, simplex, dim, expectedHrepCount, expectedFaceCount, seed);
  }

}
