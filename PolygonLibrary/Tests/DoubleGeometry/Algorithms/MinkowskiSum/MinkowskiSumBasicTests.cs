using NUnit.Framework;
using Tests.DoubleGeometry.Polyhedra;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiSumBasicTests {

  [Test]
  public void AlgSumPoints_ProducesAllPairwiseSums() {
    SortedSet<Vector> sum = MinkowskiSum.AlgSumPoints(
      [MinkowskiSumTestData.U00, MinkowskiSumTestData.U10],
      [MinkowskiSumTestData.U00, MinkowskiSumTestData.U01]
    );

    ConvexPolytopAssert.AssertVertexSetEquals(sum, [MinkowskiSumTestData.U00, MinkowskiSumTestData.U10, MinkowskiSumTestData.U01, MinkowskiSumTestData.U11]);
  }

  [Test]
  public void BySandipDas_PointPlusPoint_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.Point11, MinkowskiSumTestData.Point11);

    Assert.That(sum, Is.EqualTo(ConvexPolytop.CreateFromPoints([MinkowskiSumTestData.U11 + MinkowskiSumTestData.U11])));
  }

  [Test]
  public void BySandipDas_TwoOrthogonalSegments_ProducesUnitSquare() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.OrthSegment, MinkowskiSumTestData.VerticalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSumTestData.Square));
  }

  [Test]
  public void BySandipDas_PointAndDiagonalSegment_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.Point11, MinkowskiSumTestData.DiagonalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.Point11, MinkowskiSumTestData.DiagonalSegment)));
  }

  [Test]
  public void BySandipDas_DiagonalSegmentWithItself_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.DiagonalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.DiagonalSegment)));
  }

  [Test]
  public void BySandipDas_CollinearShiftedDiagonalSegments_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.ShiftedDiagonalSegment, MinkowskiSumTestData.TripleShiftedDiagonalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.ShiftedDiagonalSegment, MinkowskiSumTestData.TripleShiftedDiagonalSegment)));
  }

  [Test]
  public void BySandipDas_ShiftedSegments_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.ShiftedHorizontalSegment, MinkowskiSumTestData.ShiftedVerticalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.ShiftedHorizontalSegment, MinkowskiSumTestData.ShiftedVerticalSegment)));
  }

  [Test]
  public void BySandipDas_SquareAndDiagonalSegment_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.Square);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.Square)));
  }

  [Test]
  public void BySandipDas_TwoNonOrthogonalSegments_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.ShiftedVerticalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.ShiftedVerticalSegment)));
  }

  [Test]
  public void BySandipDas_AxisSegmentAndSquare_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.OrthSegment, MinkowskiSumTestData.Square);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.OrthSegment, MinkowskiSumTestData.Square)));
  }

  [Test]
  public void BySandipDas_SquareWithItself_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.Square, MinkowskiSumTestData.Square);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.Square, MinkowskiSumTestData.Square)));
  }

  [Test]
  public void BySandipDas_RotatedSquareAndShiftedSquare_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.RotatedSquare, MinkowskiSumTestData.ShiftedSquare);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.RotatedSquare, MinkowskiSumTestData.ShiftedSquare)));
  }

}
