using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopConstructionAndRepresentationTests {

  [Test]
  public void CreateFromPoints_WithoutConvexification_KeepsAllGivenPointsInVrep() {
    ConvexPolytop polytope = ConvexPolytop.CreateFromPoints(ConvexPolytopTestData.UnitSquareWithInnerPoints);

    Assert.Multiple(() => {
      Assert.That(polytope.SpaceDim, Is.EqualTo(2));
      Assert.That(polytope.IsVrep, Is.True);
      Assert.That(polytope.IsFLrep, Is.False);
      Assert.That(polytope.IsHrep, Is.False);
      Assert.That(polytope.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Vrep));
      ConvexPolytopAssert.AssertVertexSetEquals(polytope.Vrep, ConvexPolytopTestData.UnitSquareWithInnerPoints);
      ConvexPolytopAssert.AssertVectorsAreEqual(polytope.InnerPoint, ConvexPolytopAssert.V(0.45, 0.5333333333333333));
    });
  }

  [Test]
  public void CreateFromPoints_WithConvexification_RemovesInnerPointsAndBuildsFaceLattice() {
    ConvexPolytop polytope = ConvexPolytop.CreateFromPoints(ConvexPolytopTestData.UnitSquareWithInnerPoints, true);

    Assert.Multiple(() => {
      Assert.That(polytope.IsFLrep, Is.True);
      Assert.That(polytope.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      ConvexPolytopAssert.AssertVertexSetEquals(polytope.Vrep, ConvexPolytopTestData.UnitSquareVertices, "The set of vertices must be equal.");
      Assert.That(polytope.Hrep, Has.Count.EqualTo(4));
      Assert.That(polytope.FLrep.NumberOfKFaces, Is.EqualTo(9));
      Assert.That(polytope.fVector, Is.EqualTo(new Vector(new double[] { 4, 4, 1 })));
    });
  }

  [Test]
  public void CreateFromHalfSpaces_BuildsHrepAndSupportsContainment() {
    ConvexPolytop polytope = ConvexPolytop.CreateFromHalfSpaces(ConvexPolytopTestData.UnitSquareHrep);

    Assert.Multiple(() => {
      Assert.That(polytope.SpaceDim, Is.EqualTo(2));
      Assert.That(polytope.IsHrep, Is.True);
      Assert.That(polytope.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Hrep));
      Assert.That(polytope.Hrep, Has.Count.EqualTo(4));
      Assert.That(polytope.ContainsStrict(ConvexPolytopAssert.V(0.5, 0.5)), Is.True);
      Assert.That(polytope.ContainsOnBorder(ConvexPolytopAssert.V(1, 0.5)), Is.True);
      Assert.That(polytope.ContainsComplement(ConvexPolytopAssert.V(1.5, 0.5)), Is.True);
      Assert.That(polytope.ContainsStrict(polytope.InnerPoint), Is.True);
    });
  }

  [Test]
  public void CreateFromFaceLattice_UsesGivenLatticeAndExposesFVector() {
    FaceLattice lattice = FaceLatticeTestData.CreateTriangleLattice();

    ConvexPolytop polytope = ConvexPolytop.CreateFromFaceLattice(lattice);

    Assert.Multiple(() => {
      Assert.That(polytope.IsFLrep, Is.True);
      Assert.That(polytope.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      Assert.That(ReferenceEquals(polytope.FLrep, lattice), Is.True);
      Assert.That(polytope.SpaceDim, Is.EqualTo(2));
      Assert.That(polytope.PolytopDim, Is.EqualTo(2));
      Assert.That(polytope.fVector, Is.EqualTo(new Vector(new double[] { 3, 3, 1 })));
    });
  }

  [Test]
  public void InnerPoint_ForFlrep_UsesTopNodeInnerPoint() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareFlrep();

    ConvexPolytopAssert.AssertVectorsAreEqual(polytope.InnerPoint, polytope.FLrep.Top.InnerPoint);
  }

  [Test]
  public void GetInRepresentations_ConstructEquivalentPolytopesWithRequestedPriority() {
    ConvexPolytop vrep = ConvexPolytopTestData.CreateUnitSquareVrep();
    ConvexPolytop hrep = ConvexPolytopTestData.CreateUnitSquareHrepOnly();

    ConvexPolytop asFlrep = vrep.GetInFLrep();
    ConvexPolytop asHrep = vrep.GetInHrep();
    ConvexPolytop asVrep = hrep.GetInVrep();

    Assert.Multiple(() => {
      Assert.That(asFlrep.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      Assert.That(asHrep.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Hrep));
      Assert.That(asVrep.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Vrep));
      Assert.That(asFlrep.Equals(asHrep), Is.True);
      Assert.That(asHrep.Equals(asVrep), Is.True);
      Assert.That(asVrep.Equals(asFlrep), Is.True);
    });
  }

}
