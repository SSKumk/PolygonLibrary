using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopHRedundancyTests {

  [Test]
  public void HRedundancyByGW_ForCenteredSquare_RemovesOuterConstraintsAndPreservesGeometry() {
    List<HyperPlane> redundantSquareHrep =
      [
        new HyperPlane(ConvexPolytopAssert.V(1, 0), 2),
        new HyperPlane(ConvexPolytopAssert.V(-1, 0), 2),
        new HyperPlane(ConvexPolytopAssert.V(0, 1), 2),
        new HyperPlane(ConvexPolytopAssert.V(0, -1), 2),
        new HyperPlane(ConvexPolytopAssert.V(1, 0), 3),
        new HyperPlane(ConvexPolytopAssert.V(-1, 0), 3),
        new HyperPlane(ConvexPolytopAssert.V(0, 1), 3),
        new HyperPlane(ConvexPolytopAssert.V(0, -1), 3)
      ];

    List<HyperPlane> reduced = ConvexPolytop.HRedundancyByGW(redundantSquareHrep);
    ConvexPolytop reducedPolytope = ConvexPolytop.CreateFromHalfSpaces(reduced);
    ConvexPolytop expected = ConvexPolytop.Ball_oo(Vector.Zero(2), 2);

    Assert.Multiple(() => {
      Assert.That(reduced, Has.Count.EqualTo(4));
      Assert.That(reducedPolytope.Equals(expected), Is.True);
      ConvexPolytopAssert.AssertVertexSetEquals(reducedPolytope.Vrep, expected.Vrep);
    });
  }

  [Test]
  public void CreateFromHalfSpaces_WithHRedundancy_RemovesRedundantFacetsForTranslatedSquare() {
    List<HyperPlane> redundantTranslatedSquareHrep =
      [
        new HyperPlane(ConvexPolytopAssert.V(1, 0), 3),
        new HyperPlane(ConvexPolytopAssert.V(-1, 0), -1),
        new HyperPlane(ConvexPolytopAssert.V(0, 1), 4),
        new HyperPlane(ConvexPolytopAssert.V(0, -1), -2),
        new HyperPlane(ConvexPolytopAssert.V(1, 0), 4),
        new HyperPlane(ConvexPolytopAssert.V(-1, 0), 0),
        new HyperPlane(ConvexPolytopAssert.V(0, 1), 5),
        new HyperPlane(ConvexPolytopAssert.V(0, -1), -1)
      ];

    ConvexPolytop reducedViaCtor = ConvexPolytop.CreateFromHalfSpaces(redundantTranslatedSquareHrep, true);
    ConvexPolytop expected =
      ConvexPolytop.RectAxisParallel(
        ConvexPolytopAssert.V(1, 2),
        ConvexPolytopAssert.V(3, 4)
      );

    Assert.Multiple(() => {
      Assert.That(reducedViaCtor.Hrep, Has.Count.EqualTo(4));
      Assert.That(reducedViaCtor.Equals(expected), Is.True);
      ConvexPolytopAssert.AssertVertexSetEquals(reducedViaCtor.Vrep, expected.Vrep);
    });
  }

}
