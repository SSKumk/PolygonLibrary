using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopBallFactoriesTests {

  [Test]
  public void BallInfinity_WithNonZeroCenter_ShiftsSquareAroundCenter() {
    ConvexPolytop ball = ConvexPolytop.Ball_oo(ConvexPolytopAssert.V(1, 1), 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      ball.Vrep,
      [
        ConvexPolytopAssert.V(-1, -1),
        ConvexPolytopAssert.V(3, -1),
        ConvexPolytopAssert.V(3, 3),
        ConvexPolytopAssert.V(-1, 3)
      ]
    );
  }

  [Test]
  public void Sphere_In1D_ReturnsShiftedSegment() {
    ConvexPolytop sphere = ConvexPolytop.Sphere(new Vector(new double[] { 3 }), 2, 4, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      sphere.Vrep,
      [
        new Vector(new double[] { 1 }),
        new Vector(new double[] { 5 })
      ]
    );
  }

  [Test]
  public void Sphere_In2D_WithFourAzimuthDivisions_ReturnsCardinalPointsAroundCenter() {
    ConvexPolytop sphere = ConvexPolytop.Sphere(ConvexPolytopAssert.V(1, 2), 2, 4, 1);

    ConvexPolytopAssert.AssertVertexSetEquals(
      sphere.Vrep,
      [
        ConvexPolytopAssert.V(3, 2),
        ConvexPolytopAssert.V(-1, 2),
        ConvexPolytopAssert.V(1, 4),
        ConvexPolytopAssert.V(1, 0)
      ]
    );
  }

  [Test]
  public void Sphere_In2D_IgnoresPolarDivision() {
    ConvexPolytop sphereLowPolar = ConvexPolytop.Sphere(ConvexPolytopAssert.V(1, 2), 2, 4, 1);
    ConvexPolytop sphereHighPolar = ConvexPolytop.Sphere(ConvexPolytopAssert.V(1, 2), 2, 4, 7);

    ConvexPolytopAssert.AssertVertexSetEquals(sphereLowPolar.Vrep, sphereHighPolar.Vrep);
  }

  [Test]
  public void Sphere_In3D_WithFourAzimuthsAndTwoPolarDivisions_ReturnsAxisOctahedron() {
    ConvexPolytop sphere = ConvexPolytop.Sphere(Vector.Zero(3), 2, 4, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      sphere.Vrep,
      [
        new Vector(new double[] {  2,  0,  0 }),
        new Vector(new double[] { -2,  0,  0 }),
        new Vector(new double[] {  0,  2,  0 }),
        new Vector(new double[] {  0, -2,  0 }),
        new Vector(new double[] {  0,  0,  2 }),
        new Vector(new double[] {  0,  0, -2 })
      ]
    );
  }

  [Test]
  public void Ellipsoid_In1D_ReturnsShiftedSegmentWithSemiAxisLength() {
    ConvexPolytop ellipsoid =
      ConvexPolytop.Ellipsoid(
        4,
        2,
        new Vector(new double[] { 3 }),
        new Vector(new double[] { 5 })
      );

    ConvexPolytopAssert.AssertVertexSetEquals(
      ellipsoid.Vrep,
      [
        new Vector(new double[] { -2 }),
        new Vector(new double[] {  8 })
      ]
    );
  }

  [Test]
  public void Ellipsoid_In2D_WithFourAzimuthDivisions_ReturnsAxisVerticesScaledBySemiAxes() {
    ConvexPolytop ellipsoid =
      ConvexPolytop.Ellipsoid(
        4,
        1,
        ConvexPolytopAssert.V(1, 2),
        ConvexPolytopAssert.V(3, 5)
      );

    ConvexPolytopAssert.AssertVertexSetEquals(
      ellipsoid.Vrep,
      [
        ConvexPolytopAssert.V(4, 2),
        ConvexPolytopAssert.V(-2, 2),
        ConvexPolytopAssert.V(1, 7),
        ConvexPolytopAssert.V(1, -3)
      ]
    );
  }

  [Test]
  public void Ellipsoid_In2D_IgnoresPolarDivision() {
    ConvexPolytop ellipsoidLowPolar =
      ConvexPolytop.Ellipsoid(
        4,
        1,
        ConvexPolytopAssert.V(1, 2),
        ConvexPolytopAssert.V(3, 5)
      );
    ConvexPolytop ellipsoidHighPolar =
      ConvexPolytop.Ellipsoid(
        4,
        7,
        ConvexPolytopAssert.V(1, 2),
        ConvexPolytopAssert.V(3, 5)
      );

    ConvexPolytopAssert.AssertVertexSetEquals(ellipsoidLowPolar.Vrep, ellipsoidHighPolar.Vrep);
  }

  [Test]
  public void Ball2FuncCreator_ReturnsSameFactoryAsDirectSphereCall() {
    Func<Vector, double, ConvexPolytop> factory = ConvexPolytop.Ball_2FuncCreator(4, 1);
    ConvexPolytop viaFactory = factory(ConvexPolytopAssert.V(1, 2), 2);
    ConvexPolytop direct = ConvexPolytop.Sphere(ConvexPolytopAssert.V(1, 2), 2, 4, 1);

    ConvexPolytopAssert.AssertVertexSetEquals(viaFactory.Vrep, direct.Vrep);
  }

}
