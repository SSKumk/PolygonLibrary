using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class FaceLatticeInternalConstructionTests {

  [Test]
  public void ConstructFromFLNodeSum_BuildsEquivalentTriangleLattice() {
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

    FaceLattice lattice =
      FaceLattice.ConstructFromFLNodeSum(
        [
          [v0, v1, v2],
          [e01, e12, e20],
          [face]
        ]
      );
    FaceLattice expected = FaceLatticeTestData.CreateTriangleLattice();

    Assert.Multiple(() => {
      Assert.That(lattice.Equals(expected), Is.True);
      Assert.That(lattice.NumberOfKFaces, Is.EqualTo(7));
      Assert.That(lattice[0], Has.Count.EqualTo(3));
      Assert.That(lattice[1], Has.Count.EqualTo(3));
      Assert.That(lattice[2], Has.Count.EqualTo(1));
    });
  }

  [Test]
  public void ConstructFromBaseSubCP_BuildsEquivalentTriangleLattice() {
    SubPoint s0 = new(FaceLatticeAssert.V(0, 0), null);
    SubPoint s1 = new(FaceLatticeAssert.V(2, 0), null);
    SubPoint s2 = new(FaceLatticeAssert.V(1, 1), null);

    SubTwoDimensional triangle = new(new SortedSet<SubPoint> { s0, s1, s2 });
    SortedSet<BaseSubCP> level0 = new SortedSet<BaseSubCP>(triangle.Faces!.SelectMany(face => face.Faces!));
    SortedSet<BaseSubCP> level1 = new SortedSet<BaseSubCP>(triangle.Faces!);
    SortedSet<BaseSubCP> level2 = new SortedSet<BaseSubCP> { triangle };

    List<SortedSet<BaseSubCP>> levels =
      [
        level0,
        level1,
        level2
      ];

    FaceLattice lattice = FaceLattice.ConstructFromBaseSubCP(levels);
    FaceLattice expected = FaceLatticeTestData.CreateTriangleLattice();

    Assert.Multiple(() => {
      Assert.That(lattice.Equals(expected), Is.True);
      Assert.That(lattice.NumberOfKFaces, Is.EqualTo(7));
      Assert.That(lattice.Top.PolytopDim, Is.EqualTo(2));
      Assert.That(lattice.Vertices.SetEquals(expected.Vertices), Is.True);
    });
  }

}
