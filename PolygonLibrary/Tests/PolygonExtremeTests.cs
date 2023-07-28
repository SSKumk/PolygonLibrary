using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons;
using PolygonLibrary.Polygons.ConvexPolygons;

namespace Tests;

[TestFixture]
public class PolygonExtremeTests {
  [Test]
  public void PolygonExtremeTest1() {
    // Polygon has no normal codirected with E1
    ConvexPolygon cp = PolygonTools.Circle(0, 0, 1, 10);
    double[] testAnglesDeg =
      {
        0, 18, 36, 54, 72
      , 342, 350
      };
    Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
    Point2D?[,] res = new Point2D[testAnglesDeg.Length, 2];
    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Math.PI / 180;
      testDirs[i] = new Vector2D(5 * Math.Cos(alpha), 5 * Math.Sin(alpha));
      cp.GetExtremeElements(testDirs[i], out res[i, 0], out res[i, 1]);
    }

    Assert.Multiple(() => {
      Assert.That(ReferenceEquals(res[0, 1], null), "0");
      Assert.That(ReferenceEquals(res[2, 1], null), "2");
      Assert.That(ReferenceEquals(res[4, 1], null), "4");

      Assert.That(ReferenceEquals(res[1, 1], null), Is.False, "1");
      Assert.That(ReferenceEquals(res[3, 1], null), Is.False, "3");
      Assert.That(ReferenceEquals(res[5, 1], null), Is.False, "5");
    });
  }

  [Test]
  public void PolygonExtremeTest2() {
    // Polygon has a normal codirected with E1
    ConvexPolygon cp = PolygonTools.Circle(0, 0, 1, 10, Math.PI / 10);
    double[] testAnglesDeg = new double[]
      {
        0, 18, 36, 54, 72
      , 342
      };
    Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
    Point2D?[,] res = new Point2D?[testAnglesDeg.Length, 2];
    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Math.PI / 180;
      testDirs[i] = new Vector2D(5 * Math.Cos(alpha), 5 * Math.Sin(alpha));
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

  [Test]
  public void PolygonExtremeTest3() {
    // Polygon is created from a randomly ordered points
    List<Point2D> ps = new List<Point2D>()
      {
        new Point2D(-2, -1), new Point2D(2, -1), new Point2D(0, 3), new Point2D(0, -3), new Point2D(-2, 1)
      , new Point2D(2, 1)
      };
    ConvexPolygon cp = ConvexPolygon.CreateConvexPolygonFromSwarm(ps);
    double[] testAnglesDeg =
      {
        0, 18, 45, 85, 135
      , 180, 270, 315, 320
      };

    int[,] ansInds =
      {
        { 2, 3 }, { 3, -1 }, { 3, 4 }, { 4, -1 }, { 4, 5 }
      , { 5, 0 }, { 1, -1 }, { 1, 2 }, { 2, -1 }
      };

    Vector2D[] testDirs = new Vector2D[testAnglesDeg.Length];
    Point2D?[,] res = new Point2D?[testAnglesDeg.Length, 2];
    for (int i = 0; i < testAnglesDeg.Length; i++) {
      double alpha = testAnglesDeg[i] * Math.PI / 180;
      testDirs[i] = new Vector2D(5 * Math.Cos(alpha), 5 * Math.Sin(alpha));
      cp.GetExtremeElements(testDirs[i], out res[i, 0], out res[i, 1]);
    }

    Assert.Multiple(() => {
      Assert.That(ReferenceEquals(res[0, 1], null), Is.False, $"0 - angle = {testAnglesDeg[0]} (null check)");
      Assert.That(ReferenceEquals(res[2, 1], null), Is.False, $"2 - angle = {testAnglesDeg[2]} (null check)");
      Assert.That(ReferenceEquals(res[4, 1], null), Is.False, $"4 - angle = {testAnglesDeg[4]} (null check)");
      Assert.That(ReferenceEquals(res[5, 1], null), Is.False, $"5 - angle = {testAnglesDeg[5]} (null check)");
      Assert.That(ReferenceEquals(res[7, 1], null), Is.False, $"7 - angle = {testAnglesDeg[7]} (null check)");

      Assert.That(ReferenceEquals(res[1, 1], null), $"1 - angle = {testAnglesDeg[1]} (null check)");
      Assert.That(ReferenceEquals(res[3, 1], null), $"3 - angle = {testAnglesDeg[3]} (null check)");
      Assert.That(ReferenceEquals(res[6, 1], null), $"6 - angle = {testAnglesDeg[6]} (null check)");
      Assert.That(ReferenceEquals(res[8, 1], null), $"8 - angle = {testAnglesDeg[8]} (null check)");
    });

    Assert.Multiple(() => {
      for (int i = 0; i < testAnglesDeg.Length; i++) {
        Console.WriteLine(i);
        Assert.That(res[i, 0]!.Equals(cp.Vertices[ansInds[i, 0]]), $"{i}/0");
        if (ansInds[i, 1] != -1) {
          Assert.That(res[i, 1]!.Equals(cp.Vertices[ansInds[i, 1]]), $"{i}/1");
        }
      }
    });
  }
}