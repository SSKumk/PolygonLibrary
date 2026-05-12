using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class ConvexPolygonExtremeAndAreaTests {

  [Test]
  public void GetExtremeElements_ReturnsSingleVertexOrEdgeDependingOnDirectionWithoutCodirectedNormal() {
    ConvexPolygon polygon = PolygonTools.Circle(0, 0, 1, 10);
    double[] testAnglesDeg = new[] { 0.0, 18.0, 36.0, 54.0, 72.0, 342.0, 350.0 };
    Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
    Vector2D?[,] result = new Vector2D[testAnglesDeg.Length, 2];

    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Tools.PI / 180.0;
      testDirs[i] = new Vector2D(5 * double.Cos(alpha), 5 * double.Sin(alpha));
      polygon.GetExtremeElements(testDirs[i], out result[i, 0], out result[i, 1]);
    }

    Assert.Multiple(() => {
      Assert.That(ReferenceEquals(result[0, 1], null), "0");
      Assert.That(ReferenceEquals(result[2, 1], null), "2");
      Assert.That(ReferenceEquals(result[4, 1], null), "4");
      Assert.That(ReferenceEquals(result[4, 1], null), "4");
      Assert.That(ReferenceEquals(result[1, 1], null), Is.False, "1");
      Assert.That(ReferenceEquals(result[3, 1], null), Is.False, "3");
      Assert.That(ReferenceEquals(result[5, 1], null), Is.False, "5");
    });
  }

  [Test]
  public void GetExtremeElements_ReturnsEdgeWhenPolygonHasCodirectedNormal() {
    ConvexPolygon polygon = PolygonTools.Circle(0, 0, 1, 10, Tools.PI / 10);
    double[] testAnglesDeg = new[] { 0.0, 18.0, 36.0, 54.0, 72.0, 342.0 };
    Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
    Vector2D?[,] result = new Vector2D?[testAnglesDeg.Length, 2];

    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Tools.PI / 180.0;
      testDirs[i] = new Vector2D(5 * double.Cos(alpha), 5 * double.Sin(alpha));
      polygon.GetExtremeElements(testDirs[i], out result[i, 0], out result[i, 1]);
    }

    Assert.Multiple(() => {
      Assert.That(ReferenceEquals(result[1, 1], null), "0");
      Assert.That(ReferenceEquals(result[3, 1], null), "2");
      Assert.That(ReferenceEquals(result[5, 1], null), "4");
      Assert.That(ReferenceEquals(result[0, 1], null), Is.False, "1");
      Assert.That(ReferenceEquals(result[2, 1], null), Is.False, "3");
      Assert.That(ReferenceEquals(result[4, 1], null), Is.False, "5");
    });
  }

  [Test]
  public void Square_ComputesExpectedAreaForSquareAndOctagon() {
    ConvexPolygon square = new ConvexPolygon(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight, true);
    ConvexPolygon octagon = new ConvexPolygon(ConvexPolygonTestData.CreateOctagonVertices(), true);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(square.Square, 4.0), Is.True, "WeightTest1: wrong square");
      Assert.That(Tools.EQ(octagon.Square, 14.0), Is.True, "WeightTest1: wrong square");
    });
  }

  [Test]
  public void NearestPoint_CurrentlyThrowsNotImplementedException() {
    ConvexPolygon polygon = new ConvexPolygon(ConvexPolygonTestData.SquareVerticesClockwiseStartingTopRight, true);

    Assert.That(() => polygon.NearestPoint(Vector2D.Zero), Throws.TypeOf<NotImplementedException>());
  }

}
