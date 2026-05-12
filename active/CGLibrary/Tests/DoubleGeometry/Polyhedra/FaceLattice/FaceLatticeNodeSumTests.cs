using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class FaceLatticeNodeSumTests {

  private static (FLNodeSum v0, FLNodeSum v1, FLNodeSum v2, FLNodeSum e01, FLNodeSum e12, FLNodeSum e20, FLNodeSum face)
    CreateTriangleHierarchy() {
    Vector p0 = FaceLatticeAssert.V(0, 0);
    Vector p1 = FaceLatticeAssert.V(2, 0);
    Vector p2 = FaceLatticeAssert.V(1, 1);

    FLNodeSum v0 = new(p0, new AffineBasis(p0));
    FLNodeSum v1 = new(p1, new AffineBasis(p1));
    FLNodeSum v2 = new(p2, new AffineBasis(p2));

    FLNodeSum e01 = new((p0 + p1) / 2.0, new AffineBasis([p0, p1]));
    FLNodeSum e12 = new((p1 + p2) / 2.0, new AffineBasis([p1, p2]));
    FLNodeSum e20 = new((p0 + p2) / 2.0, new AffineBasis([p0, p2]));

    e01.AddSub(v0);
    e01.AddSub(v1);
    e12.AddSub(v1);
    e12.AddSub(v2);
    e20.AddSub(v0);
    e20.AddSub(v2);

    v0.AddSuper(e01);
    v0.AddSuper(e20);
    v1.AddSuper(e01);
    v1.AddSuper(e12);
    v2.AddSuper(e12);
    v2.AddSuper(e20);

    FLNodeSum face = new((p0 + p1 + p2) / 3.0, new AffineBasis([p0, p1, p2]));
    face.AddSub(e01);
    face.AddSub(e12);
    face.AddSub(e20);

    e01.AddSuper(face);
    e12.AddSuper(face);
    e20.AddSuper(face);

    return (v0, v1, v2, e01, e12, e20, face);
  }

  [Test]
  public void Constructor_SetsInnerPointAffBasisAndEmptyNeighbourSets() {
    Vector point = FaceLatticeAssert.V(2, -1);
    FLNodeSum node = new(point, new AffineBasis(point));

    Assert.Multiple(() => {
      FaceLatticeAssert.AssertVectorsAreEqual(node.InnerPoint, point);
      Assert.That(node.PolytopDim, Is.EqualTo(0));
      Assert.That(node.Sub, Is.Empty);
      Assert.That(node.Super, Is.Empty);
      Assert.That(node.AffBasis.Origin.Equals(point), Is.True);
    });
  }

  [Test]
  public void LevelsAndNeighbourSets_AreBuiltCorrectlyForTriangleHierarchy() {
    var (v0, v1, v2, e01, _, e20, face) = CreateTriangleHierarchy();
    List<SortedSet<FLNodeSum>> levels = face.GetAllLevels();

    Assert.Multiple(() => {
      Assert.That(e01.Sub.SetEquals([v0, v1]), Is.True);
      Assert.That(v0.Super.SetEquals([e01, e20]), Is.True);
      Assert.That(levels, Has.Count.EqualTo(3));
      Assert.That(levels[0], Has.Count.EqualTo(3));
      Assert.That(levels[1], Has.Count.EqualTo(3));
      Assert.That(levels[2], Has.Count.EqualTo(1));
      Assert.That(new SortedSet<FLNodeSum>(face.GetLevelBelowNonStrict(0)), Is.EqualTo(levels[0]));
      Assert.That(new SortedSet<FLNodeSum>(face.GetLevelBelowNonStrict(1)), Is.EqualTo(levels[1]));
      Assert.That(new SortedSet<FLNodeSum>(face.GetLevelBelowNonStrict(2)), Is.EqualTo(levels[2]));
      Assert.That(face.GetLevelBelowNonStrict(3), Is.Empty);
      Assert.That(v2.GetLevelBelowNonStrict(1), Is.Empty);
    });
  }

  [Test]
  public void AllNonStrictSub_ReturnsNodeAndAllItsSubfaces() {
    var (_, _, _, _, _, _, face) = CreateTriangleHierarchy();
    SortedSet<FLNodeSum> all = new(face.AllNonStrictSub);

    Assert.Multiple(() => {
      Assert.That(all, Has.Count.EqualTo(7));
      Assert.That(all.Contains(face), Is.True);
      Assert.That(all.Count(node => node.PolytopDim == 0), Is.EqualTo(3));
      Assert.That(all.Count(node => node.PolytopDim == 1), Is.EqualTo(3));
      Assert.That(all.Count(node => node.PolytopDim == 2), Is.EqualTo(1));
    });
  }

  [Test]
  public void EqualsAndCompareTo_FollowAffineBasisContract() {
    Vector p0 = FaceLatticeAssert.V(0, 0);
    Vector p1 = FaceLatticeAssert.V(2, 0);
    Vector p2 = FaceLatticeAssert.V(1, 1);

    FLNodeSum edgeA = new((p0 + p1) / 2.0, new AffineBasis([p0, p1]));
    FLNodeSum edgeB = new((p1 + p0) / 2.0, new AffineBasis([p1, p0]));
    FLNodeSum edgeC = new((p0 + p2) / 2.0, new AffineBasis([p0, p2]));
    FLNodeSum vertex = new(p0, new AffineBasis(p0));
    int compareToNull = edgeA.CompareTo((FLNodeSum?)null);

    Assert.Multiple(() => {
      Assert.That(edgeA.Equals(edgeB), Is.True);
      Assert.That(edgeA.CompareTo(edgeB), Is.Zero);
      Assert.That(edgeA.Equals(edgeC), Is.False);
      Assert.That(edgeA.CompareTo(edgeC), Is.Not.Zero);
      Assert.That(vertex.CompareTo(edgeA), Is.LessThan(0));
      Assert.That(edgeA.CompareTo(vertex), Is.GreaterThan(0));
      Assert.That(edgeA.Equals(null), Is.False);
      Assert.That(compareToNull, Is.EqualTo(1));
    });
  }

  [Test]
  public void GetHashCode_IsForbidden() {
    FLNodeSum node = new(FaceLatticeAssert.V(0, 0), new AffineBasis(FaceLatticeAssert.V(0, 0)));

    Assert.That(node.GetHashCode, Throws.InvalidOperationException);
  }

}
