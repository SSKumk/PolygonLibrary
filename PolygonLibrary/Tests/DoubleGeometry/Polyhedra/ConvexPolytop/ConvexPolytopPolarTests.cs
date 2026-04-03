using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopPolarTests {

  [Test]
  public void Polar_ForCenteredInfinityBallInVrep_ReturnsOneBallAsDual() {
    ConvexPolytop square = ConvexPolytop.Ball_oo(Vector.Zero(2), 2);
    ConvexPolytop expectedDual = ConvexPolytop.Ball_1(Vector.Zero(2), 0.5);

    ConvexPolytop dual = square.Polar();

    Assert.Multiple(() => {
      Assert.That(dual.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Hrep));
      ConvexPolytopAssert.AssertVertexSetEquals(dual.Vrep, expectedDual.Vrep);
    });
  }

  [Test]
  public void Polar_ForCenteredOneBallInHrep_ReturnsInfinityBallAsDual() {
    ConvexPolytop diamond = ConvexPolytop.Ball_1(Vector.Zero(2), 2).GetInHrep();
    ConvexPolytop expectedDual = ConvexPolytop.Ball_oo(Vector.Zero(2), 0.5);

    ConvexPolytop dual = diamond.Polar();

    Assert.Multiple(() => {
      Assert.That(dual.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Vrep));
      ConvexPolytopAssert.AssertVertexSetEquals(dual.Vrep, expectedDual.Vrep);
    });
  }

  [Test]
  public void Polar_Twice_ForCenteredInfinityBall_RestoresOriginalGeometry() {
    ConvexPolytop square = ConvexPolytop.Ball_oo(Vector.Zero(2), 2);

    ConvexPolytop restored = square.Polar().Polar();

    ConvexPolytopAssert.AssertVertexSetEquals(restored.Vrep, square.Vrep);
  }

  [Test]
  public void PolarOutShift_ForTranslatedSquare_ReturnsCenterShiftAndCenteredDual() {
    ConvexPolytop translatedSquare =
      ConvexPolytop.RectAxisParallel(
        ConvexPolytopAssert.V(1, 2),
        ConvexPolytopAssert.V(3, 4)
      );

    ConvexPolytop dual = translatedSquare.Polar(out Vector shiftToOrigin);
    ConvexPolytop expectedDual = ConvexPolytop.Ball_1(Vector.Zero(2), 1);

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVectorsAreEqual(shiftToOrigin, ConvexPolytopAssert.V(2, 3));
      ConvexPolytopAssert.AssertVertexSetEquals(dual.Vrep, expectedDual.Vrep);
    });
  }

  [Test]
  public void Polar_ForCenteredInfinityBallInFlrep_ReturnsEquivalentOneBallInFlrep() {
    ConvexPolytop square = ConvexPolytop.Ball_oo(Vector.Zero(2), 2).GetInFLrep();
    ConvexPolytop expectedDual = ConvexPolytop.Ball_1(Vector.Zero(2), 0.5);

    ConvexPolytop dual = square.Polar();

    Assert.Multiple(() => {
      Assert.That(dual.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      ConvexPolytopAssert.AssertVertexSetEquals(dual.Vrep, expectedDual.Vrep);
      Assert.That(dual.fVector, Is.EqualTo(new Vector(new double[] { 4, 4, 1 })));
    });
  }

  [Test]
  public void Polar_IsConsistentAcrossVrepHrepAndFlrep_ForCenteredSquare() {
    ConvexPolytop vrep = ConvexPolytop.Ball_oo(Vector.Zero(2), 2);
    ConvexPolytop hrep = vrep.GetInHrep();
    ConvexPolytop flrep = vrep.GetInFLrep();

    ConvexPolytop dualFromVrep = vrep.Polar();
    ConvexPolytop dualFromHrep = hrep.Polar();
    ConvexPolytop dualFromFlrep = flrep.Polar();

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVertexSetEquals(dualFromVrep.Vrep, dualFromHrep.Vrep);
      ConvexPolytopAssert.AssertVertexSetEquals(dualFromHrep.Vrep, dualFromFlrep.Vrep);
      ConvexPolytopAssert.AssertVertexSetEquals(dualFromFlrep.Vrep, dualFromVrep.Vrep);
    });
  }

  [Test]
  public void PolarTrue_InHrepBranch_RemovesRedundantDualPointsButPreservesGeometry() {
    List<HyperPlane> redundantSquareHrep =
      [
        new HyperPlane(ConvexPolytopAssert.V(1, 0), 2),
        new HyperPlane(ConvexPolytopAssert.V(-1, 0), 2),
        new HyperPlane(ConvexPolytopAssert.V(0, 1), 2),
        new HyperPlane(ConvexPolytopAssert.V(0, -1), 2),
        new HyperPlane(ConvexPolytopAssert.V(1, 0), 3),
        new HyperPlane(ConvexPolytopAssert.V(-1, 0), 3)
      ];
    ConvexPolytop squareWithRedundantFacets = ConvexPolytop.CreateFromHalfSpaces(redundantSquareHrep);

    ConvexPolytop dualWithoutCleanup = squareWithRedundantFacets.Polar();
    ConvexPolytop dualWithCleanup = squareWithRedundantFacets.Polar(true);
    ConvexPolytop expectedDual = ConvexPolytop.Ball_1(Vector.Zero(2), 0.5);

    Assert.Multiple(() => {
      Assert.That(dualWithoutCleanup.Vrep, Has.Count.EqualTo(6));
      Assert.That(dualWithCleanup.Vrep, Has.Count.EqualTo(4));
      ConvexPolytopAssert.AssertVertexSetEquals(dualWithCleanup.Vrep, expectedDual.Vrep);
      Assert.That(dualWithoutCleanup.GetInHrep().Equals(dualWithCleanup.GetInHrep()), Is.True);
    });
  }

  [Test]
  public void Polar_ForCenteredInfinityBallInFlrep_PreservesExpectedIncidenceStructure() {
    ConvexPolytop square = ConvexPolytop.Ball_oo(Vector.Zero(2), 2).GetInFLrep();

    FaceLattice dualLattice = square.Polar().FLrep;

    Assert.Multiple(() => {
      Assert.That(dualLattice[0], Has.Count.EqualTo(4));
      Assert.That(dualLattice[1], Has.Count.EqualTo(4));
      Assert.That(dualLattice.Top.Sub.Count, Is.EqualTo(4));

      foreach (FLNode vertex in dualLattice[0]) {
        Assert.That(vertex.Sub, Is.Empty);
        Assert.That(vertex.Super.Count, Is.EqualTo(2), "Each vertex of a dual 2D polygon should belong to exactly two edges.");
      }

      foreach (FLNode edge in dualLattice[1]) {
        Assert.That(edge.Sub.Count, Is.EqualTo(2), "Each edge should have two incident vertices.");
        Assert.That(edge.Super.Count, Is.EqualTo(1), "Each edge should belong only to the top face.");
        Assert.That(edge.Super.First(), Is.SameAs(dualLattice.Top));
      }
    });
  }

  [Test]
  public void PolarTwice_ForCenteredFlrepSquare_RestoresFaceLatticeStructure() {
    ConvexPolytop square = ConvexPolytop.Ball_oo(Vector.Zero(2), 2).GetInFLrep();

    ConvexPolytop restored = square.Polar().Polar();

    Assert.Multiple(() => {
      Assert.That(restored.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      Assert.That(restored.FLrep.Equals(square.FLrep), Is.True);
      Assert.That(restored.fVector, Is.EqualTo(square.fVector));
    });
  }

  [Test]
  public void PolarTwice_ForCenteredFlrepTriangle_RestoresFaceLatticeStructure() {
    ConvexPolytop triangle =
      ConvexPolytop.CreateFromPoints(
        [
          ConvexPolytopAssert.V(1, 0),
          ConvexPolytopAssert.V(0, 1),
          ConvexPolytopAssert.V(-1, -1)
        ],
        true
      );

    ConvexPolytop restored = triangle.Polar().Polar();

    Assert.Multiple(() => {
      Assert.That(triangle.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      Assert.That(restored.WhichRep, Is.EqualTo(ConvexPolytop.Rep.FLrep));
      Assert.That(restored.FLrep.Equals(triangle.FLrep), Is.True);
      Assert.That(restored.fVector, Is.EqualTo(triangle.fVector));
      ConvexPolytopAssert.AssertVertexSetEquals(restored.Vrep, triangle.Vrep);
    });
  }

}
