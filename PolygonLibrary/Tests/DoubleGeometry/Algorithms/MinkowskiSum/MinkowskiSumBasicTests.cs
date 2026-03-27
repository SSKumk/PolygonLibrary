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
  public void BySandipDas_ShiftedSegments_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.ShiftedHorizontalSegment, MinkowskiSumTestData.ShiftedVerticalSegment);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.ShiftedHorizontalSegment, MinkowskiSumTestData.ShiftedVerticalSegment)));
  }

  [Test]
  public void BySandipDas_SquareAndDiagonalSegment_EqualsConvexHullResult() {
    ConvexPolytop sum = MinkowskiSum.BySandipDas(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.Square);

    Assert.That(sum, Is.EqualTo(MinkowskiSum.ByConvexHull(MinkowskiSumTestData.DiagonalSegment, MinkowskiSumTestData.Square)));
  }

}
