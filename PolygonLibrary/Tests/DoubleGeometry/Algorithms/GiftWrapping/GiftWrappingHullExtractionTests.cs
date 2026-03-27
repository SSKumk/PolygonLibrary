using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using Tests.DoubleGeometry.Polyhedra;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class GiftWrappingHullExtractionTests {

  [OneTimeSetUp]
  public void SetUp() => Tools.Eps = 1e-8;

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

}
