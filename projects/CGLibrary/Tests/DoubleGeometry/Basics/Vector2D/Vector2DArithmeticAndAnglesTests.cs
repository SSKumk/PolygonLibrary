using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class Vector2DArithmeticAndAnglesTests {

  [Test]
  public void UnaryMinus_AdditionAndSubtraction_WorkCoordinatewise() {
    Vector2D v1 = new Vector2D(3.0, -2.0);
    Vector2D v2 = new Vector2D(-1.0, 4.0);

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(-v1, new Vector2D(-3.0, 2.0));
      Vector2DAssert.AreEqual(v1 + v2, new Vector2D(2.0, 2.0));
      Vector2DAssert.AreEqual(v1 - v2, new Vector2D(4.0, -6.0));
    });
  }

  [Test]
  public void ScalarMultiplicationAndDivision_WorkAsExpected() {
    Vector2D v = new Vector2D(3.0, -2.0);

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(2.0 * v, new Vector2D(6.0, -4.0));
      Vector2DAssert.AreEqual(v * 2.0, new Vector2D(6.0, -4.0));
      Vector2DAssert.AreEqual(v / 2.0, new Vector2D(1.5, -1.0));
    });
  }

  [Test]
  public void DotAndCrossProducts_UseStandardFormulas() {
    Vector2D v1 = new Vector2D(1.0, 2.0);
    Vector2D v2 = new Vector2D(3.0, 4.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(v1 * v2, 11.0), Is.True);
      Assert.That(Tools.EQ(v1 ^ v2, -2.0), Is.True);
    });
  }

  [Test]
  public void Dist2_EqualsSquareOfDist() {
    Vector2D p1 = new Vector2D(1.0, 2.0);
    Vector2D p2 = new Vector2D(4.0, 6.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(Vector2D.Dist2(p1, p2), 25.0), Is.True);
      Assert.That(Tools.EQ(Vector2D.Dist(p1, p2), 5.0), Is.True);
      Assert.That(Tools.EQ(Vector2D.Dist2(p1, p2), Vector2D.Dist(p1, p2) * Vector2D.Dist(p1, p2)), Is.True);
    });
  }

  [Test]
  public void NormalizeAndNormalizeZero_WorkForZeroAndNonZeroVectors() {
    Vector2D nonZero = new Vector2D(3.0, 4.0);
    Vector2D zero = Vector2D.Zero;
    Vector2D normalized = nonZero.Normalize();

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(normalized, new Vector2D(0.6, 0.8));
      Assert.That(Tools.EQ(normalized.Length, 1.0), Is.True);
      Vector2DAssert.AreEqual(zero.NormalizeZero(), zero);
      Vector2DAssert.AreEqual(nonZero.NormalizeZero(), normalized);
    });
  }

  [Test]
  public void TurnCW_TurnCCW_AndTurn_AreConsistent() {
    Vector2D v = new Vector2D(1.0, 0.0);

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(v.TurnCW(), new Vector2D(0.0, -1.0));
      Vector2DAssert.AreEqual(v.TurnCCW(), new Vector2D(0.0, 1.0));
      Vector2DAssert.AreEqual(v.Turn(0.0), v);
      Vector2DAssert.AreEqual(v.Turn(Tools.HalfPI), v.TurnCCW());
      Vector2DAssert.AreEqual(v.Turn(-Tools.HalfPI), v.TurnCW());
    });
  }

  [Test]
  public void Angle_ReturnsPositiveNegativeAndZeroValuesAsExpected() {
    Vector2D e1 = Vector2D.E1;
    Vector2D e2 = Vector2D.E2;
    Vector2D minusE2 = new Vector2D(0.0, -1.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(Vector2D.Angle(e1, e2), Tools.HalfPI), Is.True);
      Assert.That(Tools.EQ(Vector2D.Angle(e1, minusE2), -Tools.HalfPI), Is.True);
      Assert.That(Tools.EQ(Vector2D.Angle(e1, e1)), Is.True);
      Assert.That(Tools.EQ(Vector2D.Angle(Vector2D.Zero, e1)), Is.True);
    });
  }

  [Test]
  public void AngleTest() {
    double sq3 = double.Sqrt(3) / 2.0;
    Vector2D[] v = new Vector2D[] {
        new Vector2D(1, 0), new Vector2D(sq3, 0.5), new Vector2D(0.5, sq3), new Vector2D(0, 1), new Vector2D(-1, 1)
      , new Vector2D(-1, 0), new Vector2D(-0.5, -sq3), new Vector2D(0, -1), new Vector2D(0.5, -sq3)
      };
    int[,] p = new int[,] {
        { 0, 0 }, { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 5 }, { 6, 6 }, { 7, 7 }, { 8, 8 }, { 0, 1 }, { 0, 2 }
      , { 0, 3 }, { 0, 4 }, { 0, 5 }, { 0, 6 }, { 0, 7 }, { 0, 8 }, { 1, 2 }, { 1, 3 }, { 1, 4 }, { 1, 5 }, { 1, 6 }
      , { 1, 7 }, { 1, 8 }, { 3, 4 }, { 3, 5 }, { 3, 6 }, { 3, 7 }, { 3, 8 }
      };
    double[] res = new double[] {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 30, 60, 90, 135, 180, 240, 270, 300, 30, 60, 105, 150, 210, 240, 270, 45, 90, 150, 180
      , 210
      };

    for (int i = 0; i < res.Length; i++) {
      double r = Vector2D.Angle2PI(v[p[i, 0]], v[p[i, 1]]);
      Assert.That(Tools.EQ(r, res[i] * Tools.PI / 180.0),
                  "Direct angle: test #" + i + " has failed");
    }

    for (int i = 9; i < res.Length; i++) {
      double r = Vector2D.Angle2PI(v[p[i, 1]], v[p[i, 0]]);
      Assert.That(Tools.EQ(2 * Tools.PI - r, res[i] * Tools.PI / 180.0),
                  "Back angle: test #" + i + " has failed");
    }
  }

  [Test]
  public void FromPolar_CreatesVectorWithRequestedAngleAndRadius() {
    Vector2D v1 = Vector2D.FromPolar(Tools.HalfPI, 2.0);
    Vector2D v2 = Vector2D.FromPolar(0.0, -3.0);

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(v1, new Vector2D(0.0, 2.0));
      Assert.That(Tools.EQ(v1.Length, 2.0), Is.True);
      Vector2DAssert.AreEqual(v2, new Vector2D(-3.0, 0.0));
      Assert.That(Tools.EQ(v2.Length, 3.0), Is.True);
    });
  }

}
