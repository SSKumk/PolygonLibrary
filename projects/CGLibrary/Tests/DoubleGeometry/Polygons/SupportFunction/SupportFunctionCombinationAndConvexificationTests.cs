using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class SupportFunctionCombinationAndConvexificationTests {

  [Test]
  public void CombineFunctions_PreservesLegacyReferenceResults() {
    SupportFunction
      sf1 = new SupportFunction(SupportFunctionTestData.CreateGps1True()),
      sf2 = new SupportFunction(SupportFunctionTestData.CreateGps2True()),
      sf11 = SupportFunction.CombineFunctions(sf1, sf1, 1, 1),
      sf22 = SupportFunction.CombineFunctions(sf2, sf2, 1, 1),
      sf12 = SupportFunction.CombineFunctions(sf1, sf2, 1, 1);

    int i;

    Assert.Multiple(() => {
      for (i = 0; i < sf11.Count; i++) {
        Assert.That(sf11[i].Normal, Is.EqualTo(sf1[i].Normal), "sf11: " + i + "th normal is wrong");
        Assert.That(Tools.EQ(sf11[i].Value, 2 * sf1[i].Value), "sf11: " + i + "th value is wrong");
      }

      for (i = 0; i < sf22.Count; i++) {
        Assert.That(sf22[i].Normal, Is.EqualTo(sf2[i].Normal), "sf22: " + i + "th normal is wrong");
        Assert.That(Tools.EQ(sf22[i].Value, 2 * sf2[i].Value), "sf22: " + i + "th value is wrong");
      }

      for (i = 0; i < sf12.Count; i++) {
        if (i % 2 == 0) {
          Assert.That(sf12[i].Normal, Is.EqualTo(sf1[i / 2].Normal), "sf12: " + i + "th normal is wrong");
          Assert.That(Tools.EQ(sf12[i].Value, 3 * sf1[i / 2].Value), "sf12: " + i + "th value is wrong");
        }
        else {
          Assert.That(sf12[i].Normal, Is.EqualTo(sf2[i / 2].Normal), "sf12: " + i + "th normal is wrong");
          Assert.That(Tools.EQ(sf12[i].Value, 2 * sf2[i / 2].Value), "sf12: " + i + "th value is wrong");
        }
      }
    });
  }

  [Test]
  public void CombineFunctions_PopulatesSuspiciousIndicesForSecondFunctionNormals() {
    SupportFunction sf1 = new SupportFunction(SupportFunctionTestData.CreateGps1True());
    SupportFunction sf2 = new SupportFunction(SupportFunctionTestData.CreateGps2True());
    List<int> suspiciousIndices = new();
    List<Vector2D> suspiciousVectors = new();

    SupportFunction combined = SupportFunction.CombineFunctions(sf1, sf2, 1, 1, suspiciousIndices, suspiciousVectors);

    Assert.Multiple(() => {
      Assert.That(combined, Has.Count.EqualTo(8));
      Assert.That(suspiciousIndices, Is.EqualTo(new List<int> { 1, 3, 5, 7 }));
      Assert.That(suspiciousVectors, Is.EqualTo(sf2.Select(pair => pair.Normal).ToList()));
    });
  }

  [Test]
  public void CheckTripleDirect_UsesIntersectionAndOppositeNormalsRules() {
    GammaPair pm = new GammaPair(new Vector2D(1.0, 0.0), 1.0);
    GammaPair pc = new GammaPair(new Vector2D(0.0, 1.0), 1.0);
    GammaPair ppValid = new GammaPair(new Vector2D(-1.0, -1.0), -1.5);
    GammaPair ppInvalid = new GammaPair(new Vector2D(-1.0, -1.0), -3.0);
    GammaPair oppositeValid = new GammaPair(new Vector2D(0.0, 1.0), 2.0);
    GammaPair oppositeCurrent = new GammaPair(new Vector2D(1.0, 1.0), 1.0);
    GammaPair oppositeCounter = new GammaPair(new Vector2D(0.0, -1.0), -2.0);
    GammaPair oppositeInvalid = new GammaPair(new Vector2D(0.0, -1.0), -3.0);

    Assert.Multiple(() => {
      Assert.That(SupportFunctionProbe.CheckTripleDirect(pm, pc, ppValid), Is.True);
      Assert.That(SupportFunctionProbe.CheckTripleDirect(pm, pc, ppInvalid), Is.False);
      Assert.That(SupportFunctionProbe.CheckTripleDirect(oppositeValid, oppositeCurrent, oppositeCounter), Is.True);
      Assert.That(SupportFunctionProbe.CheckTripleDirect(oppositeValid, oppositeCurrent, oppositeInvalid), Is.False);
    });
  }

  [Test]
  public void ConvexifyFunctionWithInfo_WithEmptySuspiciousList_ReturnsSameInstance() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps2True());
    SupportFunction? result = supportFunction.ConvexifyFunctionWithInfo(new List<int>());

    Assert.That(result, Is.SameAs(supportFunction));
  }

  [Test]
  public void ConvexifyFunctionWithInfo_RemovesLocalNonConvexPairAndReturnsConvexResult() {
    SupportFunction supportFunction =
      new SupportFunction(
        new[] {
          new GammaPair(new Vector2D(0.0, -1.0), 1.0),
          new GammaPair(new Vector2D(1.0, 0.0), 1.0),
          new GammaPair(new Vector2D(1.0, 1.0), 3.0, true),
          new GammaPair(new Vector2D(0.0, 1.0), 1.0),
          new GammaPair(new Vector2D(-1.0, 0.0), 1.0)
        }
      );

    SupportFunction? result = supportFunction.ConvexifyFunctionWithInfo(new List<int> { 2 });
    GammaPair[] expected =
      new[] {
        new GammaPair(new Vector2D(0.0, -1.0), 1.0),
        new GammaPair(new Vector2D(1.0, 0.0), 1.0),
        new GammaPair(new Vector2D(0.0, 1.0), 1.0),
        new GammaPair(new Vector2D(-1.0, 0.0), 1.0)
      };

    Assert.Multiple(() => {
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Has.Count.EqualTo(expected.Length));
      for (int i = 0; i < expected.Length; i++) {
        Assert.That(result![i], Is.EqualTo(expected[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void ConvexifyFunctionWithInfo_ReturnsNullWhenRemovalCreatesAngleGapOfPiOrMore() {
    SupportFunction supportFunction =
      new SupportFunction(
        new[] {
          new GammaPair(new Vector2D(1.0, 0.0), 1.0),
          new GammaPair(new Vector2D(1.0, 1.0), 3.0, true),
          new GammaPair(new Vector2D(-1.0, 0.0), -2.0)
        }
      );

    SupportFunction? result = supportFunction.ConvexifyFunctionWithInfo(new List<int> { 1 });

    Assert.That(result, Is.Null);
  }

}
