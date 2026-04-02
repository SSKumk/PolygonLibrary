using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class SupportFunctionInitializationTests {

  [Test]
  public void Constructor_GammaPairs_NormalizesSortsAndDeduplicatesLegacyDataSet1() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps1());
    GammaPair[] answer = SupportFunctionTestData.CreateGps1True();
    Array.Sort(answer);

    Assert.Multiple(() => {
      Assert.That(supportFunction, Has.Count.EqualTo(4), "Wrong number of resultant vectors");

      for (int i = 0; i < 4; i++) {
        Assert.That(supportFunction[i], Is.EqualTo(answer[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void Constructor_GammaPairs_NormalizesSortsAndDeduplicatesLegacyDataSet2() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps2());
    GammaPair[] answer = SupportFunctionTestData.CreateGps2True();
    Array.Sort(answer);

    Assert.Multiple(() => {
      Assert.That(supportFunction, Has.Count.EqualTo(4), "Wrong number of resultant vectors");

      for (int i = 0; i < 4; i++) {
        Assert.That(supportFunction[i], Is.EqualTo(answer[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void Constructor_Points_BuildsSupportFunctionForConvexPolygon() {
    SupportFunction supportFunction = new SupportFunction(PolylineTestData.CreateUnitSquareVertices(), false);
    GammaPair[] expected =
      new[] {
        new GammaPair(new Vector2D(0.0, -1.0), 0.0),
        new GammaPair(new Vector2D(1.0, 0.0), 1.0),
        new GammaPair(new Vector2D(0.0, 1.0), 1.0),
        new GammaPair(new Vector2D(-1.0, 0.0), 0.0)
      };

    Assert.Multiple(() => {
      Assert.That(supportFunction, Has.Count.EqualTo(4));
      for (int i = 0; i < expected.Length; i++) {
        Assert.That(supportFunction[i], Is.EqualTo(expected[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void Constructor_Points_BuildsSpecialSupportFunctionForSegment() {
    SupportFunction supportFunction =
      new SupportFunction(new List<Vector2D> { new Vector2D(1.0, 0.0), new Vector2D(3.0, 0.0) }, false);
    GammaPair[] expected =
      new[] {
        new GammaPair(new Vector2D(0.0, -1.0), 0.0),
        new GammaPair(new Vector2D(1.0, 0.0), 3.0),
        new GammaPair(new Vector2D(0.0, 1.0), 0.0),
        new GammaPair(new Vector2D(-1.0, 0.0), -1.0)
      };

    Assert.Multiple(() => {
      Assert.That(supportFunction, Has.Count.EqualTo(4));
      for (int i = 0; i < expected.Length; i++) {
        Assert.That(supportFunction[i], Is.EqualTo(expected[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void Constructor_Points_BuildsSpecialSupportFunctionForSinglePoint() {
    Vector2D point = new Vector2D(2.0, 3.0);
    SupportFunction supportFunction = new SupportFunction(new List<Vector2D> { point }, false);
    GammaPair[] expected =
      new[] {
        new GammaPair(new Vector2D(0.0, -1.0), -3.0),
        new GammaPair(new Vector2D(1.0, 0.0), 2.0),
        new GammaPair(new Vector2D(0.0, 1.0), 3.0),
        new GammaPair(new Vector2D(-1.0, 0.0), -2.0)
      };

    Assert.Multiple(() => {
      Assert.That(supportFunction, Has.Count.EqualTo(4));
      for (int i = 0; i < expected.Length; i++) {
        Assert.That(supportFunction[i], Is.EqualTo(expected[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void Constructor_Points_WithConvexify_UsesConvexHullOfPointSet() {
    List<Vector2D> points =
      new() {
        new Vector2D(0.0, 0.0),
        new Vector2D(1.0, 0.0),
        new Vector2D(1.0, 1.0),
        new Vector2D(0.0, 1.0),
        new Vector2D(0.5, 0.5),
        new Vector2D(0.0, 0.0)
      };
    SupportFunction directHull = new SupportFunction(PolylineTestData.CreateUnitSquareVertices(), false);
    SupportFunction supportFunction = new SupportFunction(points, true);

    Assert.Multiple(() => {
      Assert.That(supportFunction, Has.Count.EqualTo(directHull.Count));
      for (int i = 0; i < directHull.Count; i++) {
        Assert.That(supportFunction[i], Is.EqualTo(directHull[i]), "i = " + i);
      }
    });
  }

}
