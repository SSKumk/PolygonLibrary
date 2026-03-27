using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class FaceLatticeStructureAndTransformTests {

  [Test]
  public void Constructor_PointBuildsSingleVertexLattice() {
    Vector point = FaceLatticeAssert.V(1, 2, 3);

    FaceLattice lattice = new(point);

    Assert.Multiple(() => {
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(0));
      Assert.That(lattice.Lattice, Has.Count.EqualTo(1));
      Assert.That(lattice[0], Has.Count.EqualTo(1));
      Assert.That(lattice.Vertices.SetEquals(new SortedSet<Vector> { point }), Is.True);
      Assert.That(lattice.NumberOfKFaces, Is.EqualTo(1));
      Assert.That(lattice.NumberOfNonZeroKFaces, Is.EqualTo(0));
    });
  }

  [Test]
  public void Constructor_LatticeUsesUniqueTopAndCountsFaces() {
    FaceLattice lattice = FaceLatticeTestData.CreateTriangleLattice();

    Assert.Multiple(() => {
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(2));
      Assert.That(lattice.Lattice, Has.Count.EqualTo(3));
      Assert.That(lattice[0], Has.Count.EqualTo(3));
      Assert.That(lattice[1], Has.Count.EqualTo(3));
      Assert.That(lattice[2], Has.Count.EqualTo(1));
      Assert.That(lattice.Vertices.Count, Is.EqualTo(3));
      Assert.That(lattice.NumberOfKFaces, Is.EqualTo(7));
      Assert.That(lattice.NumberOfNonZeroKFaces, Is.EqualTo(4));
    });
  }

  [Test]
  public void AllKfaces_ExceptTop_ReturnsAllLowerDimensionalFaces() {
    FaceLattice lattice = FaceLatticeTestData.CreateTriangleLattice();

    List<FLNode> facesExceptTop = lattice.AllKfaces_ExceptTop();

    Assert.Multiple(() => {
      Assert.That(facesExceptTop, Has.Count.EqualTo(6));
      Assert.That(facesExceptTop.All(node => node.PolytopDim < lattice.Top.PolytopDim), Is.True);
      Assert.That(facesExceptTop.Count(node => node.PolytopDim == 0), Is.EqualTo(3));
      Assert.That(facesExceptTop.Count(node => node.PolytopDim == 1), Is.EqualTo(3));
    });
  }

  [Test]
  public void VertexTransform_TransformsVerticesAndPreservesTopology() {
    FaceLattice lattice = FaceLatticeTestData.CreateTriangleLattice();

    FaceLattice translated = lattice.VertexTransform(v => v + FaceLatticeAssert.V(10, -1));

    Assert.Multiple(() => {
      Assert.That(translated.Lattice.Count, Is.EqualTo(lattice.Lattice.Count));
      Assert.That(translated[0], Has.Count.EqualTo(lattice[0].Count));
      Assert.That(translated[1], Has.Count.EqualTo(lattice[1].Count));
      Assert.That(translated[2], Has.Count.EqualTo(1));
      Assert.That(translated.Vertices.SetEquals(new SortedSet<Vector> {
        FaceLatticeAssert.V(10, -1),
        FaceLatticeAssert.V(12, -1),
        FaceLatticeAssert.V(11, 0)
      }), Is.True);
      Assert.That(lattice.Vertices.SetEquals(new SortedSet<Vector> {
        FaceLatticeAssert.V(0, 0),
        FaceLatticeAssert.V(2, 0),
        FaceLatticeAssert.V(1, 1)
      }), Is.True);
    });
  }

  [Test]
  public void Equals_UsesAffineStructureOfEachNonTopLevel() {
    FaceLattice lattice = FaceLatticeTestData.CreateTriangleLattice();
    FaceLattice same = FaceLatticeTestData.CreateTriangleLattice();
    FaceLattice translated = lattice.VertexTransform(v => v + FaceLatticeAssert.V(1, 0));
    FaceLattice point = new(FaceLatticeAssert.V(0, 0));

    Assert.Multiple(() => {
      Assert.That(lattice, Is.EqualTo(same));
      Assert.That(lattice, Is.Not.EqualTo(translated));
      Assert.That(lattice, Is.Not.EqualTo(point));
      Assert.That(lattice, Is.Not.EqualTo(null));
      Assert.That(lattice, Is.Not.EqualTo(new object()));
    });
  }

  [Test]
  public void GetHashCode_AlwaysThrowsInvalidOperationException() {
    FaceLattice lattice = FaceLatticeTestData.CreateTriangleLattice();

    Assert.That(() => lattice.GetHashCode(), Throws.InvalidOperationException);
  }

}
