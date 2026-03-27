using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopTransformsAndOverridesTests {

  [Test]
  public void Shift_And_Rotate_TransformVerticesAsExpected() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareVrep();

    ConvexPolytop shifted = polytope.Shift(ConvexPolytopAssert.V(2, -1));
    ConvexPolytop rotated = polytope.Rotate(ConvexPolytopTestData.Rotate90Counterclockwise);

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVertexSetEquals(
        shifted.Vrep,
        [
          ConvexPolytopAssert.V(2, -1),
          ConvexPolytopAssert.V(3, -1),
          ConvexPolytopAssert.V(3, 0),
          ConvexPolytopAssert.V(2, 0)
        ]
      );
      ConvexPolytopAssert.AssertVertexSetEquals(
        rotated.Vrep,
        [
          ConvexPolytopAssert.V(0, 0),
          ConvexPolytopAssert.V(0, -1),
          ConvexPolytopAssert.V(1, -1),
          ConvexPolytopAssert.V(1, 0)
        ]
      );
    });
  }

  [Test]
  public void LiftUp_AddsNewCoordinateWithGivenValue() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareVrep();

    ConvexPolytop lifted = polytope.LiftUp(3, 5);

    Assert.Multiple(() => {
      Assert.That(lifted.SpaceDim, Is.EqualTo(3));
      Assert.That(lifted.Vrep.All(v => Tools.EQ(v[2], 5)), Is.True);
      Assert.That(lifted.Vrep, Has.Count.EqualTo(4));
    });
  }

  [Test]
  public void SectionByHyperPlane_ForUnitSquare_ReturnsVerticalMidSegment() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareFlrep();
    HyperPlane section = new(Vector.MakeOrth(2, 1), 0.5);

    ConvexPolytop result = polytope.SectionByHyperPlane(section);

    ConvexPolytopAssert.AssertVertexSetEquals(
      result.Vrep,
      [
        ConvexPolytopAssert.V(0.5, 0),
        ConvexPolytopAssert.V(0.5, 1)
      ]
    );
  }

  [Test]
  public void ShiftToOrigin_ReturnsUsedInnerPointAndMovesPolytopeAroundZero() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareVrep();

    ConvexPolytop shifted = polytope.ShiftToOrigin(out Vector innerPoint);

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVectorsAreEqual(innerPoint, ConvexPolytopAssert.V(0.5, 0.5));
      ConvexPolytopAssert.AssertVertexSetEquals(
        shifted.Vrep,
        [
          ConvexPolytopAssert.V(-0.5, -0.5),
          ConvexPolytopAssert.V(0.5, -0.5),
          ConvexPolytopAssert.V(0.5, 0.5),
          ConvexPolytopAssert.V(-0.5, 0.5)
        ]
      );
      Assert.That(shifted.GetInHrep().ContainsStrict(Vector.Zero(2)), Is.True);
    });
  }

  [Test]
  public void Scale_FromOrigin_ScalesAllVertices_AndNegativeScaleIsRejected() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareVrep();

    ConvexPolytop scaled = polytope.Scale(2, Vector.Zero(2));

    Assert.Multiple(() => {
      ConvexPolytopAssert.AssertVertexSetEquals(
        scaled.Vrep,
        [
          ConvexPolytopAssert.V(0, 0),
          ConvexPolytopAssert.V(2, 0),
          ConvexPolytopAssert.V(2, 2),
          ConvexPolytopAssert.V(0, 2)
        ]
      );
      Assert.That(() => polytope.Scale(-1, Vector.Zero(2)), Throws.TypeOf<NotImplementedException>());
    });
  }

  [Test]
  public void WhichRepToString_EqualsAndGetHashCode_FollowCurrentRepresentationContracts() {
    ConvexPolytop vrep = ConvexPolytopTestData.CreateUnitSquareVrep();
    ConvexPolytop hrep = ConvexPolytopTestData.CreateUnitSquareHrepOnly();
    ConvexPolytop flrep = ConvexPolytopTestData.CreateUnitSquareFlrep();

    Assert.Multiple(() => {
      Assert.That(vrep.WhichRepToString(), Is.EqualTo("Vrep"));
      Assert.That(hrep.WhichRepToString(), Is.EqualTo("Hrep"));
      Assert.That(flrep.WhichRepToString(), Is.EqualTo("FLrep"));
      Assert.That(vrep.Equals(hrep), Is.True);
      Assert.That(hrep.Equals(flrep), Is.True);
      Assert.That(() => vrep.GetHashCode(), Throws.InvalidOperationException);
    });
  }

}
