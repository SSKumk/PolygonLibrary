using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class ToolsMathAndUtilitiesTests {

  private double _savedEps;

  [SetUp]
  public void SetUp() {
    _savedEps = Tools.Eps;
  }

  [TearDown]
  public void TearDown() {
    Tools.Eps = _savedEps;
  }

  [Test]
  public void Constants_HaveExpectedValuesForDouble() {
    Assert.Multiple(() => {
      Assert.That(Tools.Zero, Is.EqualTo(0.0));
      Assert.That(Tools.HalfOne, Is.EqualTo(0.5));
      Assert.That(Tools.One, Is.EqualTo(1.0));
      Assert.That(Tools.MinusOne, Is.EqualTo(-1.0));
      Assert.That(Tools.Two, Is.EqualTo(2.0));
      Assert.That(Tools.Six, Is.EqualTo(6.0));
      Assert.That(Tools.EQ(Tools.HalfPI * Tools.Two, Tools.PI), Is.True);
      Assert.That(Tools.EQ(Tools.PI * Tools.Two, Tools.PI2), Is.True);
    });
  }

  [Test]
  public void InitArrays_CreateRequestedShapeFilledWithZeros() {
    double[] array1D = Tools.InitTNumArray(3);
    double[,] array2D = Tools.InitTNum2DArray(2, 3);

    Assert.Multiple(() => {
      Assert.That(array1D, Has.Length.EqualTo(3));
      Assert.That(array1D.All(Tools.EQ), Is.True);
      Assert.That(array2D.GetLength(0), Is.EqualTo(2));
      Assert.That(array2D.GetLength(1), Is.EqualTo(3));
      Assert.That(array2D.Cast<double>().All(Tools.EQ), Is.True);
    });
  }

  [Test]
  public void Swap_SwapsValuesForValueAndReferenceTypes() {
    int left = 1;
    int right = 2;
    string first = "alpha";
    string second = "beta";

    Tools.Swap(ref left, ref right);
    Tools.Swap(ref first, ref second);

    Assert.Multiple(() => {
      Assert.That(left, Is.EqualTo(2));
      Assert.That(right, Is.EqualTo(1));
      Assert.That(first, Is.EqualTo("beta"));
      Assert.That(second, Is.EqualTo("alpha"));
    });
  }

  [Test]
  public void Atan2_ReturnsExpectedAnglesForOriginAxesAndQuadrants() {
    Assert.Multiple(() => {
      Assert.That(Tools.EQ(Tools.Atan2(0.0, 0.0), 0.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(1.0, 1.0), Math.PI / 4.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(1.0, -1.0), 3.0 * Math.PI / 4.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(-1.0, -1.0), -3.0 * Math.PI / 4.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(-1.0, 1.0), -Math.PI / 4.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(0.0, 2.0), 0.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(0.0, -2.0), Math.PI), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(2.0, 0.0), Math.PI / 2.0), Is.True);
      Assert.That(Tools.EQ(Tools.Atan2(-2.0, 0.0), -Math.PI / 2.0), Is.True);
    });
  }

  [Test]
  public void Abs_UsesToleranceNearZeroAndAbsoluteValueOutsideTolerance() {
    Tools.Eps = 1e-6;

    Assert.Multiple(() => {
      Assert.That(Tools.Abs(0.5e-6), Is.EqualTo(0.0));
      Assert.That(Tools.Abs(-0.5e-6), Is.EqualTo(0.0));
      Assert.That(Tools.Abs(-3.0), Is.EqualTo(3.0));
      Assert.That(Tools.Abs(3.0), Is.EqualTo(3.0));
    });
  }

  [Test]
  public void GetCombinations_EnumeratesLexicographicOrderAndEdgeCases() {
    List<int[]> combinations42 = Tools.GetCombinations(4, 2).Select(c => c.ToArray()).ToList();
    List<int[]> combinations31 = Tools.GetCombinations(3, 1).Select(c => c.ToArray()).ToList();
    List<int[]> combinations33 = Tools.GetCombinations(3, 3).Select(c => c.ToArray()).ToList();

    int[][] expected42 = {
      new[] { 0, 1 },
      new[] { 0, 2 },
      new[] { 0, 3 },
      new[] { 1, 2 },
      new[] { 1, 3 },
      new[] { 2, 3 }
    };
    int[][] expected31 = {
      new[] { 0 },
      new[] { 1 },
      new[] { 2 }
    };
    int[][] expected33 = {
      new[] { 0, 1, 2 }
    };

    Assert.Multiple(() => {
      Assert.That(combinations42, Has.Count.EqualTo(expected42.Length));
      Assert.That(combinations31, Has.Count.EqualTo(expected31.Length));
      Assert.That(combinations33, Has.Count.EqualTo(expected33.Length));
      for (int i = 0; i < expected42.Length; i++) {
        Assert.That(combinations42[i], Is.EqualTo(expected42[i]));
      }
      for (int i = 0; i < expected31.Length; i++) {
        Assert.That(combinations31[i], Is.EqualTo(expected31[i]));
      }
      Assert.That(combinations33[0], Is.EqualTo(expected33[0]));
    });
  }

}
