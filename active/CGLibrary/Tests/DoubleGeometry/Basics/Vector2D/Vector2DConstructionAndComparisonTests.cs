using System;
using System.Globalization;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class Vector2DConstructionAndComparisonTests {

  [Test]
  public void Constructor_Default_CreatesZeroVector() {
    Vector2D v = new Vector2D();

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(v.x), Is.True);
      Assert.That(Tools.EQ(v.y), Is.True);
      Assert.That(Tools.EQ(v.Length), Is.True);
      Assert.That(Tools.EQ(v.Abs), Is.True);
      Assert.That(v.IsZero, Is.True);
    });
  }

  [Test]
  public void Constructors_CoordinateAndCopy_PreserveCoordinates() {
    Vector2D original = new Vector2D(1.5, -2.5);
    Vector2D copy = new Vector2D(original);

    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(original, new Vector2D(1.5, -2.5));
      Vector2DAssert.AreEqual(copy, original);
    });
  }

  [Test]
  public void Constants_Zero_E1_E2_HaveExpectedCoordinates() {
    Assert.Multiple(() => {
      Vector2DAssert.AreEqual(Vector2D.Zero, new Vector2D(0.0, 0.0));
      Vector2DAssert.AreEqual(Vector2D.E1, new Vector2D(1.0, 0.0));
      Vector2DAssert.AreEqual(Vector2D.E2, new Vector2D(0.0, 1.0));
    });
  }

  [Test]
  public void Indexer_ReturnsCoordinatesByIndex() {
    Vector2D v = new Vector2D(4.0, -3.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(v[0], 4.0), Is.True);
      Assert.That(Tools.EQ(v[1], -3.0), Is.True);
      Assert.That(() => _ = v[2], Throws.TypeOf<IndexOutOfRangeException>());
    });
  }

  [Test]
  public void Length_And_Abs_AreEqualAsVectorNorm() {
    Vector2D v = new Vector2D(3.0, 4.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(v.Length, 5.0), Is.True);
      Assert.That(Tools.EQ(v.Abs, 5.0), Is.True);
      Assert.That(Tools.EQ(v.Length, v.Abs), Is.True);
      Assert.That(v.IsZero, Is.False);
    });
  }

  [Test]
  public void CompareToNoEps_UsesExactLexicographicComparison() {
    Vector2D v1 = new Vector2D(1.0, 2.0);
    Vector2D v2 = new Vector2D(1.0, 3.0);
    Vector2D v3 = new Vector2D(2.0, -5.0);

    Assert.Multiple(() => {
      Assert.That(Vector2D.CompareToNoEps(v1, v1), Is.EqualTo(0));
      Assert.That(Vector2D.CompareToNoEps(v1, v2), Is.LessThan(0));
      Assert.That(Vector2D.CompareToNoEps(v3, v2), Is.GreaterThan(0));
    });
  }

  [Test]
  public void CompareTo_UsesEpsAndTreatsNullAsSmaller() {
    Vector2D reference = new Vector2D(1.0, 2.0);
    Vector2D withinEps = new Vector2D(1.0 + Tools.Eps / 2.0, 2.0 - Tools.Eps / 2.0);
    Vector2D outsideEps = new Vector2D(1.0, 2.0 + 2.0 * Tools.Eps);

    Assert.Multiple(() => {
      Assert.That(reference.CompareTo(null), Is.EqualTo(1));
      Assert.That(reference.CompareTo(withinEps), Is.EqualTo(0));
      Assert.That(reference.CompareTo(outsideEps), Is.LessThan(0));
      Assert.That(outsideEps.CompareTo(reference), Is.GreaterThan(0));
    });
  }

  [Test]
  public void EqualityAndInequalityOperators_AreConsistentWithEpsComparison() {
    Vector2D reference = new Vector2D(1.0, 2.0);
    Vector2D withinEps = new Vector2D(1.0 + Tools.Eps / 2.0, 2.0 - Tools.Eps / 2.0);
    Vector2D outsideEps = new Vector2D(1.0 + 2.0 * Tools.Eps, 2.0);

    Assert.Multiple(() => {
      Assert.That(reference == withinEps, Is.True);
      Assert.That(reference != withinEps, Is.False);
      Assert.That(reference == outsideEps, Is.False);
      Assert.That(reference != outsideEps, Is.True);
    });
  }

  [Test]
  public void RelationalOperators_AreConsistentWithCompareTo() {
    Vector2D smaller = new Vector2D(1.0, 2.0);
    Vector2D equalWithinEps = new Vector2D(1.0 + Tools.Eps / 2.0, 2.0);
    Vector2D larger = new Vector2D(1.0, 2.0 + 2.0 * Tools.Eps);

    Assert.Multiple(() => {
      Assert.That(smaller < larger, Is.True);
      Assert.That(smaller <= larger, Is.True);
      Assert.That(larger > smaller, Is.True);
      Assert.That(larger >= smaller, Is.True);
      Assert.That(smaller <= equalWithinEps, Is.True);
      Assert.That(smaller >= equalWithinEps, Is.True);
    });
  }

  [Test]
  public void ToString_UsesInvariantCultureFormat() {
    Vector2D v = new Vector2D(1.5, -2.25);

    Assert.That(v.ToString(), Is.EqualTo(string.Create(CultureInfo.InvariantCulture, $"({1.5};{-2.25})")));
  }

  [Test]
  public void PolarAngle_ReturnsZeroForZeroVectorAndExpectedAxisAngles() {
    Vector2D zero = Vector2D.Zero;
    Vector2D e1 = Vector2D.E1;
    Vector2D e2 = Vector2D.E2;
    Vector2D minusE1 = new Vector2D(-1.0, 0.0);
    Vector2D minusE2 = new Vector2D(0.0, -1.0);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(zero.PolarAngle), Is.True);
      Assert.That(Tools.EQ(e1.PolarAngle), Is.True);
      Assert.That(Tools.EQ(e2.PolarAngle, Tools.HalfPI), Is.True);
      Assert.That(Tools.EQ(minusE1.PolarAngle, Tools.PI), Is.True);
      Assert.That(Tools.EQ(minusE2.PolarAngle, -Tools.HalfPI), Is.True);
    });
  }

}
