using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsPolytopes<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.TestInfrastructure;

[TestFixture]
public class PolytopesGeneratorsTests {

  [OneTimeSetUp]
  public void SetUp() => Tools.Eps = 1e-8;

  [TestCase(2)]
  [TestCase(3)]
  [TestCase(4)]
  public void Cube01_WithoutInnerPoints_ProducesPureHypercube(int dim) {
    List<Vector> swarm = Cube01(dim, out List<Vector> pureCube);
    SortedSet<Vector> swarmSet = new(swarm);
    SortedSet<Vector> pureSet = new(pureCube);

    Assert.Multiple(() => {
      Assert.That(pureCube, Has.Count.EqualTo(1 << dim));
      Assert.That(swarm.Count, Is.EqualTo(pureCube.Count));
      Assert.That(swarmSet.SetEquals(pureSet), Is.True);
      Assert.That(pureSet.Count, Is.EqualTo(1 << dim));
    });
  }

  [Test]
  public void Cube01_WithInnerPoints_PreservesPureCubeVertices() {
    List<Vector> swarm = Cube01(4, out List<Vector> pureCube, [1, 2, 3, 4], amount: 2);
    SortedSet<Vector> swarmSet = new(swarm);
    SortedSet<Vector> pureSet = new(pureCube);

    Assert.Multiple(() => {
      Assert.That(pureSet.Count, Is.EqualTo(16));
      Assert.That(swarm.Count, Is.GreaterThan(pureCube.Count));
      Assert.That(pureSet.IsSubsetOf(swarmSet), Is.True);
    });
  }

  [TestCase(2)]
  [TestCase(3)]
  [TestCase(4)]
  public void Simplex_ProducesDimPlusOneAffinelyIndependentVertices(int dim) {
    List<Vector> swarm = Simplex(dim, out List<Vector> pureSimplex);
    AffineBasis aff = new(pureSimplex);

    Assert.Multiple(() => {
      Assert.That(swarm.Count, Is.EqualTo(dim + 1));
      Assert.That(pureSimplex.Count, Is.EqualTo(dim + 1));
      Assert.That(aff.FullDim, Is.True);
      Assert.That(aff.SubSpaceDim, Is.EqualTo(dim));
    });
  }

  [TestCase(2, 1202u)]
  [TestCase(3, 1303u)]
  [TestCase(4, 1404u)]
  public void SimplexRND_ProducesFullDimensionalSimplex(int dim, uint seed) {
    GRandomLC rnd = new(seed);
    List<Vector> swarm = SimplexRND(dim, out List<Vector> pureSimplex, rnd: rnd);
    AffineBasis aff = new(pureSimplex);

    Assert.Multiple(() => {
      Assert.That(swarm.Count, Is.EqualTo(dim + 1));
      Assert.That(pureSimplex.Count, Is.EqualTo(dim + 1));
      Assert.That(aff.FullDim, Is.True);
      Assert.That(aff.SubSpaceDim, Is.EqualTo(dim));
    });
  }

  [Test]
  public void CyclicPolytop_ProducesRequestedCountAndDimension() {
    List<Vector> points = CyclicPolytop(4, 7, 0.5);

    Assert.Multiple(() => {
      Assert.That(points, Has.Count.EqualTo(7));
      Assert.That(points.All(p => p.SpaceDim == 4), Is.True);
      Assert.That(new SortedSet<Vector>(points).Count, Is.EqualTo(7));
    });
  }

  [Test]
  public void SphereList_AllPointsLieOnSphereOfRequestedRadius() {
    List<Vector> points = Sphere_list(4, 4, 6, 2.0);

    Assert.Multiple(() => {
      Assert.That(points, Is.Not.Empty);
      Assert.That(points.All(p => p.SpaceDim == 4), Is.True);
      Assert.That(points.All(p => Tools.EQ(p.Length, 2.0)), Is.True);
    });
  }

  [Test]
  public void MakePointsOnSphere3D_AddsRequestedPolesAndPreservesUnitRadius() {
    List<Vector> points = MakePointsOnSphere_3D(3, 4, addUpperPole: true, addBottomPole: true);

    Assert.Multiple(() => {
      Assert.That(points.All(p => p.SpaceDim == 3), Is.True);
      Assert.That(points.Any(p => p == new Vector(new double[] { 0, 0, 1 })), Is.True);
      Assert.That(points.Any(p => p == new Vector(new double[] { 0, 0, -1 })), Is.True);
      Assert.That(points.All(p => Tools.EQ(p.Length, 1.0)), Is.True);
    });
  }

}
