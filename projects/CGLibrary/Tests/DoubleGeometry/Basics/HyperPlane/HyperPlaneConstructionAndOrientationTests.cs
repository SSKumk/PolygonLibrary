using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.HyperPlaneAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class HyperPlaneConstructionAndOrientationTests {

  [Test]
  public void Constructor_NormalOrigin_NormalizeTrue() {
    Vector normal = V(0, 0, 2);
    Vector origin = V(1, 2, 5);
    HyperPlane plane = new HyperPlane(normal, origin, needNormalize: true);

    Assert.That(plane.SpaceDim, Is.EqualTo(3));
    AreEqual(plane.Origin, origin);
    AreEqual(plane.Normal, V(0, 0, 1), "Normal should be normalized.");
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0), "Constant term should be N_norm * Origin.");
    IsConsistent(plane);
  }

  [Test]
  public void Constructor_NormalOrigin_NormalizeFalse() {
    Vector normal = V(0, 0, 1);
    Vector origin = V(1, 2, 5);
    HyperPlane plane = new HyperPlane(normal, origin, needNormalize: false);

    Assert.That(plane.SpaceDim, Is.EqualTo(3));
    AreEqual(plane.Origin, origin);
    AreEqual(plane.Normal, normal, "Normal should be used as is.");
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0));
    IsConsistent(plane);
  }

  [Test]
  public void Constructor_NormalConstant_CorrectOriginCalculation() {
    Vector normal = V(0, 3, 0);
    double constant = 6.0;
    HyperPlane plane = new HyperPlane(normal, constant);

    Assert.That(plane.SpaceDim, Is.EqualTo(3));
    AreEqual(plane.Normal, V(0, 1, 0), "Normal should be normalized.");
    Assert.That(Tools.EQ(plane.ConstantTerm, 2.0), "ConstantTerm should be normalized.");
    AreEqual(plane.Origin, V(0, 2, 0), "Calculated Origin is incorrect.");
    IsConsistent(plane);
  }

  [Test]
  public void Constructor_AffineBasis_CorrectDerivation() {
    Vector origin = V(0, 0, 1);
    AffineBasis affineBasis = new AffineBasis(origin, new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    HyperPlane plane = new HyperPlane(affineBasis);

    Assert.That(plane.SpaceDim, Is.EqualTo(3));
    AreEqual(plane.Origin, origin);
    Vector normal = plane.Normal;
    Assert.That(Tools.EQ(normal.Length, 1.0), Is.True);
    Assert.That(Vector.AreParallel(normal, V(0, 0, 1)), Is.True, "Normal should be parallel to Z-axis.");
    Assert.That(Tools.EQ(Math.Abs(plane.ConstantTerm), 1.0), Is.True);
    Assert.That(Tools.EQ(plane.ConstantTerm, plane.Normal * plane.Origin), Is.True);
    IsConsistent(plane);
    Assert.That(plane.AffBasis, Is.SameAs(affineBasis));
  }

  [Test]
  public void Constructor_AffineBasis_WithOrientation() {
    Vector origin = V(0, 0, 0);
    AffineBasis affineBasis = new AffineBasis(origin, new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    Vector pointPositive = V(0, 0, 5);
    Vector pointNegative = V(0, 0, -5);

    HyperPlane hpPos = new HyperPlane(affineBasis, toOrient: (pointPositive, true));
    AreEqual(hpPos.Normal, V(0, 0, 1), "Normal should point towards positive point.");
    Assert.That(Tools.EQ(hpPos.ConstantTerm, 0.0));
    IsConsistent(hpPos);

    HyperPlane hpNeg = new HyperPlane(affineBasis, toOrient: (pointNegative, false));
    AreEqual(hpNeg.Normal, V(0, 0, 1), "Normal should point towards the 'positive' side, even if orientation point is negative.");
    Assert.That(Tools.EQ(hpNeg.ConstantTerm, 0.0));
    IsConsistent(hpNeg);

    HyperPlane hpFlip = new HyperPlane(affineBasis, toOrient: (pointPositive, false));
    AreEqual(hpFlip.Normal, V(0, 0, -1), "Normal should flip if orientation is reversed.");
    Assert.That(Tools.EQ(hpFlip.ConstantTerm, 0.0));
    IsConsistent(hpFlip);
  }

  [Test]
  public void Constructor_FromPoints_Plane() {
    Vector p1 = V(3, 0, 0);
    Vector p2 = V(0, 3, 0);
    Vector p3 = V(0, 0, 3);
    Vector p4 = V(1, 1, 1);
    HyperPlane plane = new HyperPlane(new[] { p1, p2, p3, p4 });

    Vector expectedNormal = V(1, 1, 1).Normalize();
    double expectedConstant = expectedNormal * p1;

    Assert.That(plane.SpaceDim, Is.EqualTo(3));
    Assert.That(Vector.AreParallel(plane.Normal, expectedNormal), Is.True);
    Assert.That(Tools.EQ(Math.Abs(plane.ConstantTerm), Math.Abs(expectedConstant)), Is.True);
    Assert.That(plane.Contains(p1), Is.True);
    Assert.That(plane.Contains(p2), Is.True);
    Assert.That(plane.Contains(p3), Is.True);
    Assert.That(plane.Contains(p4), Is.True);
    IsConsistent(plane);
  }

  [Test]
  public void Method_OrientNormal_FlipsCorrectly() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), V(1, 1, 5));
    Vector pointAbove = V(0, 0, 10);
    Vector pointBelow = V(0, 0, 0);

    AreEqual(plane.Normal, V(0, 0, 1));
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0));

    plane.OrientNormal(pointBelow, false);
    AreEqual(plane.Normal, V(0, 0, 1), "Orientation 1 failed.");
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0), "Orientation 1 C failed.");

    plane.OrientNormal(pointAbove, true);
    AreEqual(plane.Normal, V(0, 0, 1), "Orientation 2 failed.");
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0), "Orientation 2 C failed.");

    plane.OrientNormal(pointBelow, true);
    AreEqual(plane.Normal, V(0, 0, -1), "Orientation 3 failed.");
    Assert.That(Tools.EQ(plane.ConstantTerm, -5.0), "Orientation 3 C failed.");

    plane.OrientNormal(pointAbove, true);
    AreEqual(plane.Normal, V(0, 0, 1), "Orientation 4 failed.");
    Assert.That(Tools.EQ(plane.ConstantTerm, 5.0), "Orientation 4 C failed.");
  }

  [Test]
  public void LazyInitialization_NormalFirst() {
    Vector origin = V(1, 1, 1);
    AffineBasis affineBasis = new AffineBasis(origin, new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    HyperPlane plane = new HyperPlane(affineBasis);

    Vector normal = plane.Normal;
    AreEqual(normal, V(0, 0, 1));

    AffineBasis basis = plane.AffBasis;
    Assert.That(basis, Is.SameAs(affineBasis));

    double constant = plane.ConstantTerm;
    double expectedConstant = normal * origin;
    Assert.That(Tools.EQ(constant, expectedConstant), Is.True);

    IsConsistent(plane);
  }

  [Test]
  public void LazyInitialization_AffBasisFirst() {
    Vector normal = V(0, 0, 1);
    Vector origin = V(1, 1, 5);
    HyperPlane plane = new HyperPlane(normal, origin, needNormalize: false);

    AffineBasis basis = plane.AffBasis;
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AreEqual(basis.Origin, origin);
    Assert.That(Tools.EQ(basis[0] * normal), Is.True);
    Assert.That(Tools.EQ(basis[1] * normal), Is.True);

    Vector restoredNormal = plane.Normal;
    AreEqual(restoredNormal, normal);

    double constant = plane.ConstantTerm;
    Assert.That(Tools.EQ(constant, 5.0), Is.True);

    IsConsistent(plane);
  }

}
