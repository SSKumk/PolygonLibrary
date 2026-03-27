using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using Tests.DoubleGeometry.Polyhedra;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GiftWrappingInitializationTests {

  [OneTimeSetUp]
  public void SetUp() => Tools.Eps = 1e-8;

  [Test]
  public void Constructor_EmptySwarm_ThrowsArgumentException() {
    Assert.That(
      () => new GiftWrapping(Array.Empty<Vector>()),
      Throws.TypeOf<ArgumentException>().With.Message.EqualTo("GW: At least one point must be in Swarm for convexification.")
    );
  }

  [Test]
  public void SinglePointSwarm_ProducesSamePointInVrepAndSingleNodeFaceLattice() {
    Vector point = new(new double[] { 1, 2, 3 });

    SortedSet<Vector> vrep = GiftWrapping.WrapVRep([point]);
    FaceLattice lattice = GiftWrapping.WrapFaceLattice([point]);

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVertexSetEquals(vrep, [point]);
      Assert.That(lattice.NumberOfKFaces, Is.EqualTo(1));
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(0));
      Assert.That(lattice.Vertices.SetEquals(new SortedSet<Vector> { point }), Is.True);
    });
  }

  [Test]
  public void LineSwarmInHigherDimensionalSpace_IsReducedToSegmentEndpoints() {
    SortedSet<Vector> vrep = GiftWrapping.WrapVRep(GiftWrappingTestData.LineIn2DWithInnerPoints);
    FaceLattice lattice = GiftWrapping.WrapFaceLattice(GiftWrappingTestData.LineIn2DWithInnerPoints);

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVertexSetEquals(vrep, [new Vector(new double[] { -1, 2 }), new Vector(new double[] { 3, 2 })]);
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(1));
      Assert.That(lattice[0], Has.Count.EqualTo(2));
      Assert.That(lattice[1], Has.Count.EqualTo(1));
    });
  }

}
