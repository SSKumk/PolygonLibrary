using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopFactoriesAndMetricsTests {

  [Test]
  public void Zero_ReturnsSinglePointAtOrigin() {
    ConvexPolytop zero = ConvexPolytop.Zero();

    Assert.Multiple(() => {
      Assert.That(zero.SpaceDim, Is.EqualTo(1));
      Assert.That(zero.Vrep, Has.Count.EqualTo(1));
      ConvexPolytopAssert.AssertVertexSetEquals(zero.Vrep, [Vector.Zero(1)]);
    });
  }

  [Test]
  public void Cube01Factories_ProduceEquivalentCubes() {
    ConvexPolytop vrep = ConvexPolytop.Cube01_VRep(3);
    ConvexPolytop hrep = ConvexPolytop.Cube01_HRep(3);

    Assert.Multiple(() => {
      Assert.That(vrep.Vrep, Has.Count.EqualTo(8));
      Assert.That(hrep.Hrep, Has.Count.EqualTo(6));
      Assert.That(vrep.Equals(hrep), Is.True);
      Assert.That(hrep.ContainsStrict(new Vector(new double[] { 0.5, 0.5, 0.5 })), Is.True);
    });
  }

  [Test]
  public void RectAxisParallel_ReturnsAllRectangleCorners() {
    ConvexPolytop rect = ConvexPolytop.RectAxisParallel(ConvexPolytopAssert.V(-1, 2), ConvexPolytopAssert.V(3, 4));

    ConvexPolytopAssert.AssertVertexSetEquals(
      rect.Vrep,
      [
        ConvexPolytopAssert.V(-1, 2),
        ConvexPolytopAssert.V(3, 2),
        ConvexPolytopAssert.V(-1, 4),
        ConvexPolytopAssert.V(3, 4)
      ]
    );
  }

  [Test]
  public void Ball1_In2D_ReturnsDiamondVertices() {
    ConvexPolytop ball = ConvexPolytop.Ball_1(Vector.Zero(2), 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      ball.Vrep,
      [
        ConvexPolytopAssert.V(2, 0),
        ConvexPolytopAssert.V(-2, 0),
        ConvexPolytopAssert.V(0, 2),
        ConvexPolytopAssert.V(0, -2)
      ]
    );
  }

  [Test]
  public void BallInfinity_In2D_ReturnsAxisParallelSquare() {
    ConvexPolytop ball = ConvexPolytop.Ball_oo(Vector.Zero(2), 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      ball.Vrep,
      [
        ConvexPolytopAssert.V(-2, -2),
        ConvexPolytopAssert.V(2, -2),
        ConvexPolytopAssert.V(2, 2),
        ConvexPolytopAssert.V(-2, 2)
      ]
    );
  }

  [Test]
  public void MinimalDiameter_And_MinDistBtwVs_ReturnShortestVertexDistance() {
    SortedSet<Vector> points =
      [
        ConvexPolytopAssert.V(0, 0),
        ConvexPolytopAssert.V(3, 0),
        ConvexPolytopAssert.V(0, 4),
        ConvexPolytopAssert.V(0, 1)
      ];

    ConvexPolytop rect = ConvexPolytop.RectAxisParallel(Vector.Zero(2), ConvexPolytopAssert.V(2, 1));

    Assert.Multiple(() => {
      Assert.That(ConvexPolytop.MinimalDiameter(points), Is.EqualTo(1).Within(Tools.Eps));
      Assert.That(rect.MinDistBtwVs(), Is.EqualTo(1).Within(Tools.Eps));
    });
  }

}
