using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using Tests.DoubleGeometry.Polyhedra;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GiftWrappingBasicTests {

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

  [Test]
  public void WrapVRep_ForSquareWithInnerPoints_ReturnsOnlyHullVertices() {
    SortedSet<Vector> vrep = GiftWrapping.WrapVRep(GiftWrappingTestData.UnitSquareWithInnerPoints);

    ConvexPolytopAssert.AssertVertexSetEquals(
      vrep,
      [
        new Vector(new double[] { 0, 0 }),
        new Vector(new double[] { 1, 0 }),
        new Vector(new double[] { 1, 1 }),
        new Vector(new double[] { 0, 1 })
      ],
      "The set of vertices must be equal."
    );
  }

  [Test]
  public void WrapVRep_ForTetrahedronFollowsSimplexShortcut() {
    SortedSet<Vector> vrep = GiftWrapping.WrapVRep(GiftWrappingTestData.Tetrahedron3D);

    ConvexPolytopAssert.AssertVertexSetEquals(vrep, GiftWrappingTestData.Tetrahedron3D, "The set of vertices must be equal.");
  }

  [Test]
  public void WrapVRep_ForCubeWithInnerPoints_ReturnsCubeVertices() {
    SortedSet<Vector> vrep = GiftWrapping.WrapVRep(GiftWrappingTestData.Cube3DWithInnerPoints);

    ConvexPolytopAssert.AssertVertexSetEquals(vrep, GiftWrappingTestData.Cube3DVertices, "The set of vertices must be equal.");
  }

  [Test]
  public void WrapVRep_ForSquareIn3DWithInnerPoints_ReducesToPlanarHullVertices() {
    SortedSet<Vector> vrep = GiftWrapping.WrapVRep(GiftWrappingTestData.SquareIn3DWithInnerPoints);

    ConvexPolytopAssert.AssertVertexSetEquals(
      vrep,
      GiftWrappingTestData.SquareIn3DWithInnerPointsVertices,
      "The set of vertices must be equal."
    );
  }

  [Test]
  public void WrapVRep_ForTetrahedronLiftedTo4D_FollowsSimplexShortcutAfterAffineReduction() {
    SortedSet<Vector> vrep = GiftWrapping.WrapVRep(GiftWrappingTestData.Tetrahedron3DLiftedTo4D);

    ConvexPolytopAssert.AssertVertexSetEquals(
      vrep,
      GiftWrappingTestData.Tetrahedron3DLiftedTo4D,
      "The set of vertices must be equal."
    );
  }

  [Test]
  public void WrapFaceLattice_ForSquareWithInnerPoints_BuildsExpectedFaceCounts() {
    FaceLattice lattice = GiftWrapping.WrapFaceLattice(GiftWrappingTestData.UnitSquareWithInnerPoints);

    Assert.Multiple(() => {
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(2));
      Assert.That(lattice[0], Has.Count.EqualTo(4));
      Assert.That(lattice[1], Has.Count.EqualTo(4));
      Assert.That(lattice[2], Has.Count.EqualTo(1));
      Assert.That(lattice.NumberOfKFaces, Is.EqualTo(9));
    });
  }

  [Test]
  public void WrapFaceLattice_ForSquareIn3DWithInnerPoints_BuildsPlanarFaceCountsAfterAffineReduction() {
    FaceLattice lattice = GiftWrapping.WrapFaceLattice(GiftWrappingTestData.SquareIn3DWithInnerPoints);

    Assert.Multiple(() => {
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(2));
      Assert.That(lattice[0], Has.Count.EqualTo(4));
      Assert.That(lattice[1], Has.Count.EqualTo(4));
      Assert.That(lattice[2], Has.Count.EqualTo(1));
      Assert.That(lattice.Vertices.SetEquals(GiftWrappingTestData.SquareIn3DWithInnerPointsVertices), Is.True);
    });
  }

  [Test]
  public void ConstructFL_FromWrappedCube_PreservesCubeVertexSetAndFaceCounts() {
    GiftWrapping gw = new(GiftWrappingTestData.Cube3DWithInnerPoints);

    FaceLattice lattice = gw.ConstructFL();

    Assert.Multiple(() => {
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(3));
      Assert.That(lattice[0], Has.Count.EqualTo(8));
      Assert.That(lattice[1], Has.Count.EqualTo(12));
      Assert.That(lattice[2], Has.Count.EqualTo(6));
      Assert.That(lattice[3], Has.Count.EqualTo(1));
      Assert.That(lattice.Vertices.SetEquals(GiftWrappingTestData.Cube3DVertices), Is.True, "The set of vertices must be equal.");
    });
  }

}
