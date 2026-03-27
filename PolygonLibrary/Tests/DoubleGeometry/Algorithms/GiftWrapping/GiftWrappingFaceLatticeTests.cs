using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GiftWrappingFaceLatticeTests {

  [OneTimeSetUp]
  public void SetUp() => Tools.Eps = 1e-8;

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
