using System.Globalization;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class VectorComparisonAndFormattingTests {

  [Test]
  public void CompareTo_UsesToleranceLexicographicOrderAndNullConvention() {
    Vector almostEqualLeft = V(1.0, 2.0, 3.0);
    Vector almostEqualRight = V(1.0 + Tools.Eps * 0.5, 2.0, 3.0);
    Vector differsInLast = V(1.0, 2.0, 4.0);
    Vector differsInMiddle = V(1.0, 3.0, 0.0);

    Assert.Multiple(() => {
      Assert.That(almostEqualLeft.CompareTo(almostEqualRight), Is.EqualTo(0));
      Assert.That(almostEqualLeft.CompareTo(differsInLast), Is.LessThan(0));
      Assert.That(differsInLast.CompareTo(almostEqualLeft), Is.GreaterThan(0));
      Assert.That(almostEqualLeft.CompareTo(differsInMiddle), Is.LessThan(0));
      Assert.That(differsInMiddle.CompareTo(almostEqualLeft), Is.GreaterThan(0));
      Assert.That(almostEqualLeft.CompareTo(null), Is.EqualTo(1));
    });
  }

  [Test]
  public void ComparisonOperators_AreConsistentWithCompareTo() {
    Vector v1 = V(1, 2, 3);
    Vector v2 = V(1, 2, 4);
    Vector v1Copy = new Vector(v1);

    Assert.Multiple(() => {
      Assert.That(v1, Is.EqualTo(v1Copy));
      Assert.That(v1, Is.Not.EqualTo(v2));
      Assert.That(v1 < v2, Is.True);
      Assert.That(v1 <= v2, Is.True);
      Assert.That(v1 <= v1Copy, Is.True);
      Assert.That(v1 > v2, Is.False);
      Assert.That(v1 >= v2, Is.False);
      Assert.That(v1 >= v1Copy, Is.True);
      Assert.That(v2 > v1, Is.True);
      Assert.That(v2 >= v1, Is.True);
      Assert.That(v2 < v1, Is.False);
      Assert.That(v2 <= v1, Is.False);
    });
  }

  [Test]
  public void EqualsOverride_ReturnsExpectedResultsForSupportedAndForeignObjects() {
    Vector v1 = V(1, 2, 3);
    Vector v1Copy = new Vector(v1);
    Vector v2 = V(1, 2, 4);
    object v1Obj = v1Copy;
    object otherObj = new object();

    Assert.Multiple(() => {
      Assert.That(v1, Is.EqualTo(v1Copy));
      Assert.That(v1, Is.EqualTo(v1Obj));
      Assert.That(v1, Is.Not.EqualTo(v2));
      Assert.That(v1, Is.Not.EqualTo(null));
      Assert.That(v1, Is.Not.EqualTo(otherObj));
    });
  }

  [Test]
  public void GetHashCode_Throws() {
    Vector v = V(1, 2);
    Assert.Throws<InvalidOperationException>(() => v.GetHashCode());
  }

  [Test]
  public void ToString_UsesExpectedDefaultAndCustomFormat() {
    Vector v = V(1.2, -3.45, 0.0);

    string s = v.ToString();
    string expected = "(1.2,-3.45,0)";
    Assert.That(s, Is.EqualTo(expected));

    string sCustom = v.ToStringBraceAndDelim('[', ']', ';');
    string expectedCustom = "[1.2;-3.45;0]";
    Assert.That(sCustom, Is.EqualTo(expectedCustom));

    string sNoBrace = v.ToStringBraceAndDelim(null, null, ',');
    string expectedNoBrace = "1.2,-3.45,0";
    Assert.That(sNoBrace, Is.EqualTo(expectedNoBrace));
  }

  [Test]
  public void ToString_UsesInvariantCultureIndependentlyOfCurrentCulture() {
    Vector v = V(1.2, -3.45);
    CultureInfo savedCulture = CultureInfo.CurrentCulture;

    try {
      CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
      Assert.That(v.ToString(), Is.EqualTo("(1.2,-3.45)"));
    }
    finally {
      CultureInfo.CurrentCulture = savedCulture;
    }
  }

}
