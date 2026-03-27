using NUnit.Framework;
using System.Globalization;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.HyperPlaneAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class HyperPlaneComparisonOverridesAndFactoryTests {

  [Test]
  public void Factory_Make3D_xyParallel() {
    HyperPlane plane = HyperPlane.Make3D_xyParallel(5.0);
    AreEqual(plane.Normal, V(0, 0, 1));
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0));
    AreEqual(plane.Origin, V(0, 0, 5));
    IsConsistent(plane);
  }

  [Test]
  public void Override_ToString() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), 5.0);
    string expected = "0,0,1 5";
    Assert.That(plane.ToString(), Is.EqualTo(expected));

    HyperPlane plane2 = new HyperPlane(V(1, 2, 3).Normalize(), 10.0);
    string s2 = plane2.ToString();
    string cStr = plane2.ConstantTerm.ToString(null, CultureInfo.InvariantCulture);
    string nStr = plane2.Normal.ToStringBraceAndDelim(null, null, ',');
    Assert.That(s2, Does.StartWith(nStr));
    Assert.That(s2, Does.EndWith(cStr));
    Assert.That(s2.Contains(" "), Is.True);
  }

  [Test]
  public void Override_Equals() {
    HyperPlane plane1 = new HyperPlane(V(0, 0, 1), 5.0);
    HyperPlane plane1Copy = new HyperPlane(V(0, 0, 1), 5.0);
    HyperPlane plane1Flipped = new HyperPlane(V(0, 0, -1), -5.0);
    HyperPlane plane2Parallel = new HyperPlane(V(0, 0, 1), 6.0);
    HyperPlane plane3Other = new HyperPlane(V(0, 1, 0), 5.0);

    Assert.That(plane1, Is.EqualTo(plane1Copy), "Equals: Same object representation");
    Assert.That(plane1, Is.Not.EqualTo(plane1Flipped), "Equals: Same plane, flipped representation");
    Assert.That(plane1, Is.Not.EqualTo(plane2Parallel), "Equals: Parallel planes");
    Assert.That(plane1, Is.Not.EqualTo(plane3Other), "Equals: Different planes");
    Assert.That(plane1, Is.Not.EqualTo(null), "Equals: Null");
    Assert.That(plane1, Is.Not.EqualTo(new object()), "Equals: Different type");
  }

  [Test]
  public void GetHashCode_ThrowsInvalidOperationException() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), 5.0);
    Assert.Throws<InvalidOperationException>(() => plane.GetHashCode());
  }

  [Test]
  public void CompareTo_Null_ReturnsOne() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), 5.0);
    Assert.That(plane.CompareTo(null), Is.EqualTo(1));
  }

  [Test]
  public void CompareTo_IdenticalPlanes_ReturnsZero() {
    HyperPlane plane1 = new HyperPlane(V(0, 0, 1), 5.0);
    HyperPlane plane2 = new HyperPlane(V(0, 0, 1), 5.0);
    Assert.That(plane1.CompareTo(plane2), Is.EqualTo(0));
  }

  [Test]
  public void CompareTo_DifferentNormals_ComparesNormalsFirst() {
    HyperPlane smallerNormal = new HyperPlane(V(0, 0, 1), 10.0);
    HyperPlane largerNormal = new HyperPlane(V(0, 1, 0), 5.0);

    Assert.That(smallerNormal.CompareTo(largerNormal), Is.LessThan(0), "Plane with smaller normal should come first.");
    Assert.That(largerNormal.CompareTo(smallerNormal), Is.GreaterThan(0), "Plane with larger normal should come second.");
  }

  [Test]
  public void CompareTo_SameNormalDifferentConstants_ComparesConstants() {
    HyperPlane plane1 = new HyperPlane(V(0, 0, 1), 5.0);
    HyperPlane plane2 = new HyperPlane(V(0, 0, 1), 6.0);

    Assert.That(plane1.CompareTo(plane2), Is.LessThan(0), "Plane with smaller constant should come first.");
    Assert.That(plane2.CompareTo(plane1), Is.GreaterThan(0), "Plane with larger constant should come second.");
  }

}
