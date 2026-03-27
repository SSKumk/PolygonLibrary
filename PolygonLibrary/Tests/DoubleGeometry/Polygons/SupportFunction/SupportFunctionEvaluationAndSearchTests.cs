using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class SupportFunctionEvaluationAndSearchTests {

  [Test]
  public void FuncVal_ComputesLegacyReferenceValues() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps1());
    double v = SupportFunctionTestData.DiagonalCoord;

    Vector2D[] vectors =
      new[] {
        new Vector2D(1, 0),
        new Vector2D(1, 0.5),
        new Vector2D(v, v),
        new Vector2D(1, 1),
        new Vector2D(0, 1),
        new Vector2D(v, -v),
        new Vector2D(-1, -1),
        new Vector2D(1, -0.5)
      };
    double[] values = new[] { 1.0, 1.0, v, 1.0, 1.0, v, 1.0, 1.0 };

    Assert.Multiple(() => {
      for (int i = 0; i < vectors.Length; i++) {
        Assert.That(Tools.EQ(supportFunction.FuncVal(vectors[i]), values[i]), "i = " + i);
      }
    });
  }

  [Test]
  public void FindCone_LocatesLegacyReferenceConesForOctagon() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateFindConeOctagon());
    List<double> testAngles = new() { 0.0, 30.0, 45.0, 60.0, 90.0, 120.0, 135.0, 150.0, 180.0, 210.0, 270.0, 300.0, 315.0, 330.0 };
    int[,] expected = {
      { 3, 4 }, { 3, 4 }, { 4, 5 }, { 4, 5 }, { 5, 6 }, { 5, 6 }, { 6, 7 }, { 6, 7 }, { 7, 0 }, { 7, 0 }, { 1, 2 }, { 1, 2 }, { 2, 3 }, { 2, 3 }
    };
    List<Vector2D> vectors = testAngles.Select(
      angle => {
        double radians = angle * Tools.PI / 180.0;
        return new Vector2D(double.Cos(radians), double.Sin(radians));
      }
    ).ToList();

    Assert.Multiple(() => {
      for (int k = 0; k < vectors.Count; k++) {
        supportFunction.FindCone(vectors[k], out int i, out int j);
        Assert.That(i == expected[k, 0] && j == expected[k, 1], "FindCone1: test #" + k + " failed, angle = " + testAngles[k]);
      }
    });
  }

  [Test]
  public void FindCone_LocatesLegacyReferenceConesForHexagon() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateFindConeHexagon());
    List<double> testAngles = new() { -30.0, -15.0, 0.0, 15.0, 30.0, 60.0, 90.0, 150.0, 165.0, 180.0, 195.0, 210.0, 255.0, 270.0, 300.0 };
    int[,] expected = {
      { 2, 3 }, { 2, 3 }, { 2, 3 }, { 2, 3 }, { 3, 4 }, { 3, 4 }, { 4, 5 }, { 5, 0 }, { 5, 0 }, { 5, 0 }, { 5, 0 }, { 0, 1 }, { 0, 1 }, { 1, 2 }, { 1, 2 }
    };
    List<Vector2D> vectors = testAngles.Select(
      angle => {
        double radians = angle * Tools.PI / 180.0;
        return new Vector2D(double.Cos(radians), double.Sin(radians));
      }
    ).ToList();

    Assert.Multiple(() => {
      for (int k = 0; k < vectors.Count; k++) {
        supportFunction.FindCone(vectors[k], out int i, out int j);
        Assert.That(i == expected[k, 0] && j == expected[k, 1], "FindCone2: test #" + k + " failed, angle = " + testAngles[k]);
      }
    });
  }

  [Test]
  public void FindCone_ReturnsSpecialBoundaryConesForFirstAndLastNormals() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps2());

    supportFunction.FindCone(supportFunction[0].Normal, out int firstI, out int firstJ);
    supportFunction.FindCone(supportFunction[^1].Normal, out int lastI, out int lastJ);

    Assert.Multiple(() => {
      Assert.That(firstI, Is.EqualTo(0));
      Assert.That(firstJ, Is.EqualTo(1));
      Assert.That(lastI, Is.EqualTo(supportFunction.Count - 1));
      Assert.That(lastJ, Is.EqualTo(0));
    });
  }

  [Test]
  public void ConicCombination_DecomposesVectorInConeBasis() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps2());
    Vector2D vector = new Vector2D(1.0, 1.0);

    supportFunction.ConicCombination(vector, 1, 2, out double a, out double b);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(a, 1.0), Is.True);
      Assert.That(Tools.EQ(b, 1.0), Is.True);
      Assert.That(supportFunction[1].Normal * a + supportFunction[2].Normal * b, Is.EqualTo(vector));
    });
  }

  [Test]
  public void FuncVal_WithExplicitConeIndices_MatchesAutomaticSearch() {
    SupportFunction supportFunction = new SupportFunction(SupportFunctionTestData.CreateGps2());
    Vector2D vector = new Vector2D(1.0, 1.0);

    supportFunction.FindCone(vector, out int i, out int j);
    double explicitValue = supportFunction.FuncVal(vector, i, j);
    double implicitValue = supportFunction.FuncVal(vector);

    Assert.That(Tools.EQ(explicitValue, implicitValue), Is.True);
  }

}
