using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class FaceLatticeNodeTests {

  [Test]
  public void Constructor_SingleVertex_PropertiesCorrect() {
    Vector p = FaceLatticeAssert.V(1, 2, 3);
    FLNode node = new(p);

    Assert.Multiple(() => {
      Assert.That(node.Vertices.Count, Is.EqualTo(1), "Vertices count should be 1.");
      Assert.That(node.Vertices.First(), Is.EqualTo(p), "Vertices should contain the input point.");
      FaceLatticeAssert.AssertVectorsAreEqual(node.InnerPoint, p, "InnerPoint should be the vertex itself.");
      Assert.That(node.AffBasis.SubSpaceDim, Is.EqualTo(0), "AffBasis dimension should be 0.");
      Assert.That(node.AffBasis.Origin, Is.EqualTo(p), "AffBasis origin should be the vertex.");
      Assert.That(node.PolytopDim, Is.EqualTo(0), "PolytopDim should be 0.");
      Assert.That(node.Sub, Is.Empty, "Sub should be empty for a vertex node.");
      Assert.That(node.Super, Is.Empty, "Super should be empty initially for a vertex node.");
    });
  }

  [Test]
  public void Constructor_FromSubNodes_EdgeFromTwoVertices() {
    FLNode v0 = new(FaceLatticeAssert.V(0, 0));
    FLNode v1 = new(FaceLatticeAssert.V(1, 0));

    FLNode expectedFirstVert = v0.CompareTo(v1) < 0 ? v0 : v1;
    FLNode expectedLastVert = v0.CompareTo(v1) < 0 ? v1 : v0;

    FLNode edge = new([v0, v1]);
    Vector expectedInnerPoint = (expectedFirstVert.InnerPoint + expectedLastVert.InnerPoint) / 2.0;

    Assert.Multiple(() => {
      Assert.That(edge.Sub.Count, Is.EqualTo(2), "Edge should have 2 sub-nodes (vertices).");
      Assert.That(edge.Sub.Contains(v0), Is.True, "Edge.Sub should contain v0.");
      Assert.That(edge.Sub.Contains(v1), Is.True, "Edge.Sub should contain v1.");
      Assert.That(v0.Super.Count, Is.EqualTo(1), "v0.Super should contain the edge.");
      Assert.That(v0.Super.First(), Is.SameAs(edge), "v0.Super should be the new edge.");
      Assert.That(v1.Super.Count, Is.EqualTo(1), "v1.Super should contain the edge.");
      Assert.That(v1.Super.First(), Is.SameAs(edge), "v1.Super should be the new edge.");
      Assert.That(edge.Vertices.Count, Is.EqualTo(2), "Edge vertices count should be 2.");
      Assert.That(edge.Vertices.Contains(FaceLatticeAssert.V(0, 0)), Is.True);
      Assert.That(edge.Vertices.Contains(FaceLatticeAssert.V(1, 0)), Is.True);
      FaceLatticeAssert.AssertVectorsAreEqual(edge.InnerPoint, expectedInnerPoint, "Edge InnerPoint calculation.");
      Assert.That(edge.PolytopDim, Is.EqualTo(1), "Edge PolytopDim should be 1.");
      Assert.That(edge.AffBasis.SubSpaceDim, Is.EqualTo(1), "Edge AffBasis dimension should be 1.");
      Assert.That(edge.AffBasis.Contains(v0.InnerPoint), Is.True, "AffBasis should contain v0.");
      Assert.That(edge.AffBasis.Contains(v1.InnerPoint), Is.True, "AffBasis should contain v1.");
    });
  }

  [Test]
  public void Constructor_FromSubNodes_FaceFromThreeEdges() {
    var (v0, v1, v2, e01, e12, e20, face) = FaceLatticeTestData.CreateTriangleHierarchy();
    Vector expectedFaceInnerPoint = (e20.InnerPoint + e12.InnerPoint) / 2.0;

    Assert.Multiple(() => {
      Assert.That(face.Sub.Count, Is.EqualTo(3), "Face should have 3 sub-nodes (edges).");
      Assert.That(face.Sub.Contains(e01), Is.True);
      Assert.That(face.Sub.Contains(e12), Is.True);
      Assert.That(face.Sub.Contains(e20), Is.True);
      Assert.That(e01.Super.Contains(face), Is.True, "e01.Super should contain the face.");
      Assert.That(e12.Super.Contains(face), Is.True, "e12.Super should contain the face.");
      Assert.That(e20.Super.Contains(face), Is.True, "e20.Super should contain the face.");
      Assert.That(face.Vertices.Count, Is.EqualTo(3), "Face vertices count should be 3.");
      Assert.That(face.Vertices.Contains(v0.Vertices.First()), Is.True);
      Assert.That(face.Vertices.Contains(v1.Vertices.First()), Is.True);
      Assert.That(face.Vertices.Contains(v2.Vertices.First()), Is.True);
      FaceLatticeAssert.AssertVectorsAreEqual(face.InnerPoint, expectedFaceInnerPoint, "Face InnerPoint calculation.");
      Assert.That(face.PolytopDim, Is.EqualTo(2), "Face PolytopDim should be 2.");
      Assert.That(face.AffBasis.SubSpaceDim, Is.EqualTo(2), "Face AffBasis dimension should be 2.");
      Assert.That(face.AffBasis.Contains(v0.InnerPoint), Is.True, "Face AffBasis should contain v0.");
      Assert.That(face.AffBasis.Contains(v1.InnerPoint), Is.True, "Face AffBasis should contain v1.");
      Assert.That(face.AffBasis.Contains(v2.InnerPoint), Is.True, "Face AffBasis should contain v2.");
    });
  }

  [Test]
  public void Constructor_FromSubNodes_WithExplicitAffBasis() {
    FLNode v0 = new(FaceLatticeAssert.V(0, 0));
    FLNode v1 = new(FaceLatticeAssert.V(1, 0));
    AffineBasis explicitBasis = new([FaceLatticeAssert.V(0, 0), FaceLatticeAssert.V(1, 0)]);

    FLNode edge = new([v0, v1], explicitBasis);

    Assert.Multiple(() => {
      Assert.That(edge.AffBasis, Is.EqualTo(explicitBasis), "Edge should use the provided AffineBasis.");
      Assert.That(edge.PolytopDim, Is.EqualTo(explicitBasis.SubSpaceDim), "PolytopDim from explicit basis.");
    });
  }

  [Test]
  public void LevelNodes_And_ConstructLevelNodes_Correctness() {
    var (v0, v1, v2, e01, e12, e20, face) = FaceLatticeTestData.CreateTriangleHierarchy();

    _ = e01.GetAllLevels();

    IEnumerable<FLNode> e01Level0 = e01.GetLevelBelowNonStrict(0);
    IEnumerable<FLNode> e01Level1 = e01.GetLevelBelowNonStrict(1);
    List<SortedSet<FLNode>> faceAllLevels = face.GetAllLevels();

    Assert.Multiple(() => {
      Assert.That(e01Level0.Count(), Is.EqualTo(2));
      Assert.That(e01Level0.Contains(v0) && e01Level0.Contains(v1), Is.True, "e01 level 0 should be v0, v1.");
      Assert.That(e01Level1.Count(), Is.EqualTo(1));
      Assert.That(e01Level1.First(), Is.SameAs(e01), "e01 level 1 should be e01 itself.");
      Assert.That(faceAllLevels.Count, Is.EqualTo(3), "Face should see 3 levels in its hierarchy.");
      Assert.That(faceAllLevels[0].SetEquals(new SortedSet<FLNode> { v0, v1, v2 }), Is.True, "Face level 0.");
      Assert.That(faceAllLevels[1].SetEquals(new SortedSet<FLNode> { e01, e12, e20 }), Is.True, "Face level 1.");
      Assert.That(faceAllLevels[2].SetEquals(new SortedSet<FLNode> { face }), Is.True, "Face level 2.");
    });
  }

  [Test]
  public void AllNonStrictSub_Correctness() {
    var (v0, v1, _, e01, _, _, face) = FaceLatticeTestData.CreateTriangleHierarchy();

    IEnumerable<FLNode> faceSubs = face.AllNonStrictSub;
    IEnumerable<FLNode> e01Subs = e01.AllNonStrictSub;
    IEnumerable<FLNode> v0Subs = v0.AllNonStrictSub;

    Assert.Multiple(() => {
      Assert.That(faceSubs.Count(), Is.EqualTo(7), "Face AllNonStrictSub count.");
      Assert.That(faceSubs.Contains(face) && faceSubs.Contains(e01) && faceSubs.Contains(v0), Is.True);
      Assert.That(e01Subs.Count(), Is.EqualTo(3), "Edge AllNonStrictSub count.");
      Assert.That(e01Subs.Contains(e01) && e01Subs.Contains(v0) && e01Subs.Contains(v1), Is.True);
      Assert.That(v0Subs.Count(), Is.EqualTo(1), "Vertex AllNonStrictSub count.");
      Assert.That(v0Subs.Contains(v0), Is.True);
    });
  }

  [Test]
  public void Equals_BasicCases() {
    FLNode v00 = new(FaceLatticeAssert.V(0, 0));
    FLNode v00Copy = new(FaceLatticeAssert.V(0, 0));
    FLNode v10 = new(FaceLatticeAssert.V(1, 0));
    FLNode edge1 = new([v00, v10]);
    FLNode edge1EquivalentVertices = new([new FLNode(FaceLatticeAssert.V(0, 0)), new FLNode(FaceLatticeAssert.V(1, 0))]);
    FLNode v01 = new(FaceLatticeAssert.V(0, 1));
    FLNode edge2 = new([v00, v01]);

    Assert.Multiple(() => {
      Assert.That(v00, Is.Not.EqualTo(null));
      Assert.That(v00, Is.Not.EqualTo(new object()));
      Assert.That(v00.Equals(v00), Is.True, "Equals self.");
      Assert.That(v00, Is.EqualTo(v00Copy), "Equals another node with same vertex.");
      Assert.That(v00, Is.Not.EqualTo(v10), "Not equals node with different vertex.");
      Assert.That(edge1, Is.EqualTo(edge1EquivalentVertices), "Edges with same vertex sets should be equal.");
      Assert.That(edge1, Is.Not.EqualTo(edge2), "Edges with different vertex sets are not equal.");
    });
  }

  [Test]
  public void CompareTo_BasicCases() {
    FLNode v0 = new(FaceLatticeAssert.V(0, 0));
    FLNode v1 = new(FaceLatticeAssert.V(1, 0));
    FLNode v0Copy = new(FaceLatticeAssert.V(0, 0));
    FLNode edgeV0V1 = new([v0, v1]);
    FLNode edgeV0V0x = new([v0, new FLNode(FaceLatticeAssert.V(0, 1))]);

    Assert.Multiple(() => {
      Assert.That(v0.CompareTo(null), Is.EqualTo(1));
      Assert.That(v0.CompareTo(edgeV0V1), Is.LessThan(0), "Dim 0 node < Dim 1 node.");
      Assert.That(edgeV0V1.CompareTo(v0), Is.GreaterThan(0), "Dim 1 node > Dim 0 node.");
      Assert.That(v0.CompareTo(v0Copy), Is.EqualTo(0), "Nodes with same single vertex.");
      Assert.That(v0.CompareTo(v1), Is.LessThan(0), "V(0,0) node < V(1,0) node.");
      Assert.That(v1.CompareTo(v0), Is.GreaterThan(0), "V(1,0) node > V(0,0) node.");
      Assert.That(edgeV0V0x.CompareTo(edgeV0V1), Is.LessThan(0));
      Assert.That(edgeV0V1.CompareTo(edgeV0V0x), Is.GreaterThan(0));
    });
  }

  [Test]
  public void GetHashCode_AlwaysThrowsInvalidOperationException() {
    FLNode node = new(FaceLatticeAssert.V(0, 0));

    Assert.That(() => node.GetHashCode(), Throws.InvalidOperationException);
  }

}
