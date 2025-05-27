using NUnit.Framework;
using CGLibrary;
using System.Collections.Generic;
using System.Linq;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;
using System;

namespace Tests.CGLibraryTests {
[TestFixture]
public class FLNodeTests {

  [Test]
  public void Constructor_SingleVertex_PropertiesCorrect() {
    Vector p    = V(1, 2, 3);
    FLNode node = new FLNode(p);

    Assert.That(node.Vertices.Count, Is.EqualTo(1), "Vertices count should be 1.");
    Assert.That(node.Vertices.First(), Is.EqualTo(p), "Vertices should contain the input point.");
    AssertVectorsAreEqual(p, node.InnerPoint, "InnerPoint should be the vertex itself.");
    Assert.That(node.AffBasis.SubSpaceDim, Is.EqualTo(0), "AffBasis dimension should be 0.");
    Assert.That(node.AffBasis.Origin, Is.EqualTo(p), "AffBasis origin should be the vertex.");
    Assert.That(node.PolytopDim, Is.EqualTo(0), "PolytopDim should be 0.");
    Assert.That(node.Sub, Is.Empty, "Sub should be empty for a vertex node.");
    Assert.That(node.Super, Is.Empty, "Super should be empty initially for a vertex node.");
  }

  [Test]
  public void Constructor_FromSubNodes_EdgeFromTwoVertices() {
    FLNode v0 = new FLNode(V(0, 0));
    FLNode v1 = new FLNode(V(1, 0));

    // Determine expected order for Sub.First/Last based on CompareTo
    FLNode expectedFirstVert = v0.CompareTo(v1) < 0 ? v0 : v1;
    FLNode expectedLastVert  = v0.CompareTo(v1) < 0 ? v1 : v0;

    FLNode edge = new FLNode(new[] { v0, v1 });

    Assert.That(edge.Sub.Count, Is.EqualTo(2), "Edge should have 2 sub-nodes (vertices).");
    Assert.That(edge.Sub.Contains(v0), Is.True, "Edge.Sub should contain v0.");
    Assert.That(edge.Sub.Contains(v1), Is.True, "Edge.Sub should contain v1.");

    Assert.That(v0.Super.Count, Is.EqualTo(1), "v0.Super should contain the edge.");
    Assert.That(v0.Super.First(), Is.SameAs(edge), "v0.Super should be the new edge.");
    Assert.That(v1.Super.Count, Is.EqualTo(1), "v1.Super should contain the edge.");
    Assert.That(v1.Super.First(), Is.SameAs(edge), "v1.Super should be the new edge.");

    Assert.That(edge.Vertices.Count, Is.EqualTo(2), "Edge vertices count should be 2.");
    Assert.That(edge.Vertices.Contains(V(0, 0)), Is.True);
    Assert.That(edge.Vertices.Contains(V(1, 0)), Is.True);

    Vector expectedInnerPoint = (expectedFirstVert.InnerPoint + expectedLastVert.InnerPoint) / 2.0;
    AssertVectorsAreEqual(expectedInnerPoint, edge.InnerPoint, "Edge InnerPoint calculation.");

    Assert.That(edge.PolytopDim, Is.EqualTo(1), "Edge PolytopDim should be 1.");
    Assert.That(edge.AffBasis.SubSpaceDim, Is.EqualTo(1), "Edge AffBasis dimension should be 1.");
    // Check if AffBasis correctly spans the two points
    Assert.That(edge.AffBasis.Contains(v0.InnerPoint), Is.True, "AffBasis should contain v0.");
    Assert.That(edge.AffBasis.Contains(v1.InnerPoint), Is.True, "AffBasis should contain v1.");
  }

  private (FLNode v0, FLNode v1, FLNode v2, FLNode e01, FLNode e12, FLNode e20, FLNode f012) CreateTriangleHierarchy() {
    FLNode v0 = new FLNode(V(0, 0)); // Smallest lexicographically
    FLNode v1 = new FLNode(V(2, 0)); // Largest
    FLNode v2 = new FLNode(V(1, 1)); // Middle

    // Sub-nodes for edges
    FLNode e_v0_v1 = new FLNode(new[] { v0, v1 }); // Sub = {v0,v1}
    FLNode e_v1_v2 = new FLNode(new[] { v1, v2 }); // Sub = {v2,v1} (v2<v1 by vector compare)
    FLNode e_v0_v2 = new FLNode(new[] { v0, v2 }); // Sub = {v0,v2}


    FLNode f_triangle = new FLNode(new[] { e_v0_v1, e_v1_v2, e_v0_v2 });

    return (v0, v1, v2, e_v0_v1, e_v1_v2, e_v0_v2, f_triangle);
  }

  [Test]
  public void Constructor_FromSubNodes_FaceFromThreeEdges() {
    var (v0, v1, v2, e01, e12, e20, face) = CreateTriangleHierarchy();

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

    Vector expectedFaceInnerPoint = (e20.InnerPoint + e12.InnerPoint) / 2.0; // ((0.5,0.5) + (1.5,0.5))/2 = (1.0, 0.5)
    AssertVectorsAreEqual(expectedFaceInnerPoint, face.InnerPoint, "Face InnerPoint calculation.");

    Assert.That(face.PolytopDim, Is.EqualTo(2), "Face PolytopDim should be 2.");
    Assert.That(face.AffBasis.SubSpaceDim, Is.EqualTo(2), "Face AffBasis dimension should be 2.");
    Assert.That(face.AffBasis.Contains(v0.InnerPoint), Is.True, "Face AffBasis should contain v0.");
    Assert.That(face.AffBasis.Contains(v1.InnerPoint), Is.True, "Face AffBasis should contain v1.");
    Assert.That(face.AffBasis.Contains(v2.InnerPoint), Is.True, "Face AffBasis should contain v2.");
  }

  [Test]
  public void Constructor_FromSubNodes_WithExplicitAffBasis() {
    FLNode      v0            = new FLNode(V(0, 0));
    FLNode      v1            = new FLNode(V(1, 0));
    AffineBasis explicitBasis = new AffineBasis(new[] { V(0, 0), V(1, 0) }); // A 1D basis

    FLNode edge = new FLNode(new[] { v0, v1 }, explicitBasis);

    Assert.That(edge.AffBasis, Is.EqualTo(explicitBasis), "Edge should use the provided AffineBasis.");
    Assert.That(edge.PolytopDim, Is.EqualTo(explicitBasis.SubSpaceDim), "PolytopDim from explicit basis.");
  }

  [Test]
  public void LevelNodes_And_ConstructLevelNodes_Correctness() {
    var (v0, v1, v2, e01, e12, e20, face) = CreateTriangleHierarchy();

    _ = e01.GetAllLevels(); // Triggers construction for e01 and its connected components

    IEnumerable<FLNode> e01_level0 = e01.GetLevelBelowNonStrict(0);
    Assert.That(e01_level0.Count(), Is.EqualTo(2));
    Assert.That(e01_level0.Contains(v0) && e01_level0.Contains(v1), Is.True, "e01 level 0 should be v0, v1.");

    IEnumerable<FLNode> e01_level1 = e01.GetLevelBelowNonStrict(1);
    Assert.That(e01_level1.Count(), Is.EqualTo(1));
    Assert.That(e01_level1.First(), Is.SameAs(e01), "e01 level 1 should be e01 itself.");

    List<SortedSet<FLNode>> face_allLevels = face.GetAllLevels();
    Assert.That(face_allLevels.Count, Is.EqualTo(3), "Face should see 3 levels in its hierarchy.");
    Assert.That(face_allLevels[0].SetEquals(new SortedSet<FLNode> { v0, v1, v2 }), Is.True, "Face level 0.");
    Assert.That(face_allLevels[1].SetEquals(new SortedSet<FLNode> { e01, e12, e20 }), Is.True, "Face level 1.");
    Assert.That(face_allLevels[2].SetEquals(new SortedSet<FLNode> { face }), Is.True, "Face level 2.");
  }

  [Test]
  public void AllNonStrictSub_Correctness() {
    var (v0, v1, v2, e01, e12, e20, face) = CreateTriangleHierarchy();

    IEnumerable<FLNode> faceSubs = face.AllNonStrictSub;
    Assert.That(faceSubs.Count(), Is.EqualTo(1 + 3 + 3), "Face AllNonStrictSub count."); // self + 3 edges + 3 vertices
    Assert.That(faceSubs.Contains(face) && faceSubs.Contains(e01) && faceSubs.Contains(v0), Is.True);

    IEnumerable<FLNode> e01Subs = e01.AllNonStrictSub;
    Assert.That(e01Subs.Count(), Is.EqualTo(1 + 2), "Edge AllNonStrictSub count."); // self + 2 vertices
    Assert.That(e01Subs.Contains(e01) && e01Subs.Contains(v0) && e01Subs.Contains(v1), Is.True);

    IEnumerable<FLNode> v0Subs = v0.AllNonStrictSub;
    Assert.That(v0Subs.Count(), Is.EqualTo(1), "Vertex AllNonStrictSub count."); // self
    Assert.That(v0Subs.Contains(v0), Is.True);
  }

  [Test]
  public void Equals_BasicCases() {
    FLNode v_0_0                = new FLNode(V(0, 0));
    FLNode v_0_0_copy           = new FLNode(V(0, 0));
    FLNode v_1_0                = new FLNode(V(1, 0));
    FLNode edge1                = new FLNode(new[] { v_0_0, v_1_0 });
    FLNode edge1_equiv_vertices = new FLNode(new[] { new FLNode(V(0, 0)), new FLNode(V(1, 0)) });


    Assert.That(v_0_0.Equals(null), Is.False);
    Assert.That(v_0_0.Equals(new object()), Is.False);
    Assert.That(v_0_0.Equals(v_0_0), Is.True, "Equals self.");
    Assert.That(v_0_0.Equals(v_0_0_copy), Is.True, "Equals another node with same vertex.");
    Assert.That(v_0_0.Equals(v_1_0), Is.False, "Not equals node with different vertex.");

    Assert.That(edge1.Equals(edge1_equiv_vertices), Is.True, "Edges with same vertex sets should be equal.");

    FLNode v_0_1 = new FLNode(V(0, 1));
    FLNode edge2 = new FLNode(new[] { v_0_0, v_0_1 });
    Assert.That(edge1.Equals(edge2), Is.False, "Edges with different vertex sets are not equal.");
  }

  [Test]
  public void CompareTo_BasicCases() {
    FLNode v0      = new FLNode(V(0, 0)); // dim 0
    FLNode v1      = new FLNode(V(1, 0)); // dim 0
    FLNode v0_copy = new FLNode(V(0, 0)); // dim 0

    FLNode edge_v0_v1  = new FLNode(new[] { v0, v1 });                  // dim 1, 2 vertices {V(0,0), V(1,0)}
    FLNode edge_v0_v0x = new FLNode(new[] { v0, new FLNode(V(0, 1)) }); // dim 1, 2 vertices {V(0,0), V(0,1)}


    Assert.That(v0.CompareTo(null), Is.EqualTo(1));

    // Compare by dimension
    Assert.That(v0.CompareTo(edge_v0_v1), Is.LessThan(0), "Dim 0 node < Dim 1 node.");
    Assert.That(edge_v0_v1.CompareTo(v0), Is.GreaterThan(0), "Dim 1 node > Dim 0 node.");

    // Compare same dimension, same vertex
    Assert.That(v0.CompareTo(v0_copy), Is.EqualTo(0), "Nodes with same single vertex.");

    // Compare same dimension (0), different vertices
    // V(0,0).CompareTo(V(1,0)) is < 0
    Assert.That(v0.CompareTo(v1), Is.LessThan(0), "V(0,0) node < V(1,0) node.");
    Assert.That(v1.CompareTo(v0), Is.GreaterThan(0), "V(1,0) node > V(0,0) node.");

    // Compare same dimension (1), same vertex count (2)
    // edge_v0_v1.Vertices = {V(0,0), V(1,0)}
    // edge_v0_v0x.Vertices = {V(0,0), V(0,1)}
    // Lexicographical: V(0,0)==V(0,0), then V(1,0) vs V(0,1). V(0,1) < V(1,0).
    // So edge_v0_v0x < edge_v0_v1
    Assert.That(edge_v0_v0x.CompareTo(edge_v0_v1), Is.LessThan(0));
    Assert.That(edge_v0_v1.CompareTo(edge_v0_v0x), Is.GreaterThan(0));
  }

}
}
