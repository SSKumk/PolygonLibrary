using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsBase<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiDiffRegressionTests {

  [Test]
  public void Naive_And_Geometric_ForTriangleMinusPoint_ShiftTriangleByNegativePoint() {
    ConvexPolytop triangle = ConvexPolytop.CreateFromPoints([
      new Vector(new double[] { 0.0, 0.0 }),
      new Vector(new double[] { 2.0, 0.0 }),
      new Vector(new double[] { 0.0, 1.0 })
    ]);
    Vector pointVector = new(new double[] { 0.5, 0.25 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = triangle.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(triangle, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(triangle, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForRotatedCubeMinusPoint_ShiftRotatedCubeByNegativePoint() {
    Matrix rotation = MakeRotationMatrix(3, 1, 2, double.Pi / 4);
    ConvexPolytop rotatedCube = ConvexPolytop.Cube01_VRep(3).Rotate(rotation);
    Vector pointVector = new(new double[] { 0.1, 0.2, 0.3 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = rotatedCube.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(rotatedCube, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(rotatedCube, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void TodoSignal_Naive_And_Geometric_ForTetrahedronMinusPoint_ShouldShiftTetrahedronByNegativePoint() {
    // TODO high priority:
    // Geometric currently fails here because FindInitialVertex_Simplex receives
    // a simplex optimum on an optimal face instead of a true vertex.
    ConvexPolytop tetrahedron = ConvexPolytop.CreateFromPoints([
      Vector.Zero(3),
      new Vector(new double[] { 1.0, 0.0, 0.0 }),
      new Vector(new double[] { 0.0, 1.0, 0.0 }),
      new Vector(new double[] { 0.0, 0.0, 1.0 })
    ]);
    Vector pointVector = new(new double[] { 0.1, 0.2, 0.3 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = tetrahedron.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(tetrahedron, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(tetrahedron, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void TodoSignal_Naive_And_Geometric_ForOctahedronMinusPoint_ShouldShiftOctahedronByNegativePoint() {
    // TODO high priority:
    // This reveals the same vertex-selection problem as tetrahedron - point.
    ConvexPolytop octahedron = ConvexPolytop.Ball_1(Vector.Zero(3), 1);
    Vector pointVector = new(new double[] { 0.1, 0.2, 0.3 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = octahedron.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(octahedron, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(octahedron, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForSphereMinusPoint_ShiftSphereByNegativePoint() {
    ConvexPolytop sphere = ConvexPolytop.Sphere(Vector.Zero(3), 2, 20, 10);
    Vector pointVector = new(new double[] { 0.5, -0.25, 0.75 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = sphere.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(sphere, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(sphere, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [TestCase(0.5)]
  [TestCase(1.0)]
  [TestCase(1.5)]
  [TestCase(2.0)]
  [TestCase(2.5)]
  public void Naive_And_Geometric_ForSphereMinusAxisSegment_Agree(double length) {
    ConvexPolytop sphere = ConvexPolytop.Sphere(Vector.Zero(3), 2, 20, 10);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([Vector.Zero(3), new Vector(new[] { 0.0, 0.0, length })]);

    ConvexPolytop? naive = MinkowskiDiff.Naive(sphere, segment);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(sphere, segment);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.EqualTo(naive));
    });
  }

}
