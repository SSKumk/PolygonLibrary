using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Convertors.DConvertor>;
namespace DoubleTests;

[TestFixture]
public class PolygonExtremeTests {
  [Test]
  public void PolygonExtremeTest1() {
    // Polygon has no normal codirected with E1
    ConvexPolygon cp = PolygonTools.Circle(0, 0, 1, 10);
    double[] testAnglesDeg = new double[] { 0, 18, 36, 54, 72, 342, 350 };
    Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
    Point2D?[,] res = new Point2D[testAnglesDeg.Length, 2];
    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Tools.PI / 180;
      testDirs[i] = new Vector2D(5 * double.Cos(alpha), 5 * double.Sin(alpha));
      cp.GetExtremeElements(testDirs[i], out res[i, 0], out res[i, 1]);
    }

    Assert.Multiple(() => {
      Assert.That(ReferenceEquals(res[0, 1], null), "0");
      Assert.That(ReferenceEquals(res[2, 1], null), "2");
      Assert.That(ReferenceEquals(res[4, 1], null), "4");
      Assert.That(ReferenceEquals(res[4, 1], null), "4");

      Assert.That(ReferenceEquals(res[1, 1], null), Is.False, "1");
      Assert.That(ReferenceEquals(res[3, 1], null), Is.False, "3");
      Assert.That(ReferenceEquals(res[5, 1], null), Is.False, "5");
    });
  }

  [Test]
  public void PolygonExtremeTest2() {
    // Polygon has a normal codirected with E1
    ConvexPolygon cp            = PolygonTools.Circle(0, 0, 1, 10, Tools.PI / 10);
    double[]   testAnglesDeg = new double[] { 0, 18, 36, 54, 72, 342 };
    Vector2D[] testDirs      = new Vector2D[testAnglesDeg.Length];
    Point2D?[,]res           = new Point2D?[testAnglesDeg.Length, 2];
    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Tools.PI / 180;
      testDirs[i] = new Vector2D(5 * double.Cos(alpha), 5 * double.Sin(alpha));
      cp.GetExtremeElements(testDirs[i], out res[i, 0], out res[i, 1]);
    }

    Assert.Multiple(() => {
      Assert.That(ReferenceEquals(res[1, 1], null), "0");
      Assert.That(ReferenceEquals(res[3, 1], null), "2");
      Assert.That(ReferenceEquals(res[5, 1], null), "4");

      Assert.That(ReferenceEquals(res[0, 1], null), Is.False, "1");
      Assert.That(ReferenceEquals(res[2, 1], null), Is.False, "3");
      Assert.That(ReferenceEquals(res[4, 1], null), Is.False, "5");
    });
  }
}