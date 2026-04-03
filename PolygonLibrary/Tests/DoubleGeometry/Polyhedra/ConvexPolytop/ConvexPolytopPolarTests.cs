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

}
