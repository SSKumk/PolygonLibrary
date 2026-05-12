using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiDiffBasicTests {

  [Test]
  public void FindExtrInCPOnVectorNaive_ReturnsVertexWithMaximumDotProduct() {
    SortedSet<Vector> square =
      [
        new(new double[] { 0, 0 }),
        new(new double[] { 1, 0 }),
        new(new double[] { 0, 1 }),
        new(new double[] { 1, 1 })
      ];

    Vector extr = MinkowskiDiff.FindExtrInCPOnVector_Naive(square, new Vector(new double[] { 2, 1 }));

    Assert.That(extr, Is.EqualTo(new Vector(new double[] { 1, 1 })));
  }

  [Test]
  public void DoSubtract_ShiftsHyperPlaneConstantByExtremePointProjection() {
    HyperPlane hp = new(Vector.MakeOrth(2, 1), 3);

    HyperPlane shifted = MinkowskiDiff.doSubtract(hp, new Vector(new double[] { 2, 5 }));

    Assert.Multiple(() => {
      Assert.That(shifted.Normal, Is.EqualTo(hp.Normal));
      Assert.That(shifted.ConstantTerm, Is.EqualTo(1));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForCubeMinusZeroSegment_ReturnOriginalCube() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);
    ConvexPolytop zeroSegment = ConvexPolytop.CreateFromPoints([Vector.Zero(3), Vector.Zero(3)]);

    ConvexPolytop? naive = MinkowskiDiff.Naive(cube, zeroSegment);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(cube, zeroSegment);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive!.FLrep, Is.EqualTo(cube.FLrep));
      Assert.That(geometric, Is.EqualTo(naive));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForCubeMinusLongSegment_ReturnNull() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([Vector.Zero(3), new Vector(new double[] { 0, 0, 1.5 })]);

    Assert.Multiple(() => {
      Assert.That(MinkowskiDiff.Naive(cube, segment), Is.Null);
      Assert.That(MinkowskiDiff.Geometric(cube, segment), Is.Null);
    });
  }

  [Test]
  public void Naive_And_Geometric_ForCubeMinusUnitSegment_ReturnNonEmptyDifference() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([Vector.Zero(3), new Vector(new double[] { 0, 0, 1.0 })]);

    ConvexPolytop? naive = MinkowskiDiff.Naive(cube, segment);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(cube, segment);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive!.PolytopDim, Is.EqualTo(2));
      Assert.That(geometric, Is.EqualTo(naive));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForSquareMinusPoint_ShiftSquareByNegativePoint() {
    ConvexPolytop square = ConvexPolytop.Cube01_VRep(2);
    Vector pointVector = new(new double[] { 0.25, 0.75 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = square.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(square, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(square, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForSquareMinusShortHorizontalSegment_ReturnExpectedRectangle() {
    ConvexPolytop square = ConvexPolytop.Cube01_VRep(2);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([
      Vector.Zero(2),
      new Vector(new double[] { 0.5, 0.0 })
    ]);
    ConvexPolytop expected = ConvexPolytop.RectAxisParallel(
      Vector.Zero(2),
      new Vector(new double[] { 0.5, 1.0 })
    );

    ConvexPolytop? naive = MinkowskiDiff.Naive(square, segment);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(square, segment);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForSquareMinusUnitHorizontalSegment_ReturnNullByCurrentContract() {
    ConvexPolytop square = ConvexPolytop.Cube01_VRep(2);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([
      Vector.Zero(2),
      new Vector(new double[] { 1.0, 0.0 })
    ]);

    Assert.Multiple(() => {
      Assert.That(MinkowskiDiff.Naive(square, segment), Is.Null);
      Assert.That(MinkowskiDiff.Geometric(square, segment), Is.Null);
    });
  }

  [Test]
  public void Naive_And_Geometric_ForCubeMinusPoint_ShiftCubeByNegativePoint() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);
    Vector pointVector = new(new double[] { 0.25, 0.5, 0.75 });
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([pointVector]);
    ConvexPolytop expected = cube.Shift(-pointVector);

    ConvexPolytop? naive = MinkowskiDiff.Naive(cube, point);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(cube, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

  [Test]
  public void Naive_And_Geometric_ForCubeMinusShortAxisSegment_ReturnExpectedBox() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([
      Vector.Zero(3),
      new Vector(new double[] { 0.0, 0.0, 0.5 })
    ]);
    ConvexPolytop expected = ConvexPolytop.RectAxisParallel(
      Vector.Zero(3),
      new Vector(new double[] { 1.0, 1.0, 0.5 })
    );

    ConvexPolytop? naive = MinkowskiDiff.Naive(cube, segment);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(cube, segment);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(naive, Is.EqualTo(expected));
      Assert.That(geometric, Is.EqualTo(expected));
    });
  }

}
