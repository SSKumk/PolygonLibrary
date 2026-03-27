using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopContainmentAndNearestPointTests {

  [Test]
  public void Contains_ReturnsExpectedPositionCodesForFlrepPolytope() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareFlrep();

    Assert.Multiple(() => {
      Assert.That(polytope.Contains(ConvexPolytopAssert.V(0.5, 0.5)), Is.EqualTo(-1));
      Assert.That(polytope.Contains(ConvexPolytopAssert.V(1, 0.5)), Is.EqualTo(0));
      Assert.That(polytope.Contains(ConvexPolytopAssert.V(1.5, 0.5)), Is.EqualTo(1));
      Assert.That(polytope.ContainsNonStrict(ConvexPolytopAssert.V(1, 0.5)), Is.True);
      Assert.That(polytope.ContainsStrict(ConvexPolytopAssert.V(0.5, 0.5)), Is.True);
      Assert.That(polytope.ContainsComplement(ConvexPolytopAssert.V(1.5, 0.5)), Is.True);
    });
  }

  [Test]
  public void Contains_VrepOnly_ThrowsNotImplementedException() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareVrep();

    Assert.That(() => polytope.Contains(ConvexPolytopAssert.V(0.5, 0.5)), Throws.TypeOf<NotImplementedException>());
  }

  [Test]
  public void NearestPoint_ForStrictlyInteriorPointInFlrep_ReturnsNearestBoundaryProjection() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareFlrep();
    Vector point = ConvexPolytopAssert.V(0.1, 0.5);

    Vector nearest = polytope.NearestPoint(point, out int position);

    Assert.Multiple(() => {
      Assert.That(position, Is.EqualTo(-1));
      ConvexPolytopAssert.AssertVectorsAreEqual(nearest, ConvexPolytopAssert.V(0, 0.5));
    });
  }

  [Test]
  public void NearestPoint_ForExteriorPointInFlrep_ReturnsVisibleBoundaryProjection() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareFlrep();
    Vector point = ConvexPolytopAssert.V(1.5, 0.25);

    Vector nearest = polytope.NearestPoint(point, out int position);

    Assert.Multiple(() => {
      Assert.That(position, Is.EqualTo(1));
      ConvexPolytopAssert.AssertVectorsAreEqual(nearest, ConvexPolytopAssert.V(1, 0.25));
    });
  }

  [Test]
  public void NearestPoint_OnBorder_ReturnsSamePointAndZeroPosition() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareFlrep();
    Vector point = ConvexPolytopAssert.V(0, 0.5);

    Vector nearest = polytope.NearestPoint(point, out int position);

    Assert.Multiple(() => {
      Assert.That(position, Is.EqualTo(0));
      ConvexPolytopAssert.AssertVectorsAreEqual(nearest, point);
    });
  }

  [Test]
  public void NearestPoint_ForExteriorPointInHrepOnly_IsNotImplementedYet() {
    ConvexPolytop polytope = ConvexPolytopTestData.CreateUnitSquareHrepOnly();

    Assert.That(
      () => polytope.NearestPoint(ConvexPolytopAssert.V(1.5, 0.5)),
      Throws.TypeOf<NotImplementedException>()
    );
  }

}
