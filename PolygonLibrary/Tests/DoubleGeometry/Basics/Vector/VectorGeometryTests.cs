using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class VectorGeometryTests {

  [Test]
  public void LengthLength2AndIsZero_AreConsistent() {
    Vector zero = V(0, 0, 0);
    Vector v1 = V(3, 4, 0);
    Vector v2 = V(1, 1, 1, 1);
    Vector v3 = V(0, 0, -2);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(zero.Length2, 0.0), Is.True);
      Assert.That(Tools.EQ(zero.Length, 0.0), Is.True);
      Assert.That(zero.IsZero, Is.True);
      Assert.That(Tools.EQ(v1.Length2, 25.0), Is.True);
      Assert.That(Tools.EQ(v1.Length, 5.0), Is.True);
      Assert.That(v1.IsZero, Is.False);
      Assert.That(Tools.EQ(v2.Length2, 4.0), Is.True);
      Assert.That(Tools.EQ(v2.Length, 2.0), Is.True);
      Assert.That(Tools.EQ(v3.Length2, 4.0), Is.True);
      Assert.That(Tools.EQ(v3.Length, 2.0), Is.True);
    });
  }

  [Test]
  public void NormalizeAndNormalizeZero_FollowCurrentContract() {
    Vector v = V(3, 4, 0);
    Vector normalizedV = v.Normalize();
    Vector expected = V(0.6, 0.8, 0);

    AreEqual(normalizedV, expected);
    Assert.That(Tools.EQ(normalizedV.Length, 1.0), Is.True, "Normalized vector length should be 1.");

    Vector alreadyNormalized = V(0, 1, 0);
    AreEqual(alreadyNormalized.Normalize(), alreadyNormalized);
    AreEqual(v.NormalizeZero(), expected);
    AreEqual(Vector.Zero(3).NormalizeZero(), Vector.Zero(3), "NormalizeZero() on zero vector should return zero vector.");
  }

  [Test]
  public void ProjectTo2DAffineSpace_ProjectsToGivenAffinePlane() {
    Vector p = V(3, 4, 5);
    Vector o = Vector.Zero(3);
    Vector u1 = V(1, 0, 0);
    Vector u2 = V(0, 1, 0);

    Vector projected = p.ProjectTo2DAffineSpace(o, u1, u2);
    AreEqual(projected, V(3, 4, 0));

    o = V(1, 1, 1);
    u1 = V(1, 0, 0);
    u2 = V(0, 0, 1);
    projected = p.ProjectTo2DAffineSpace(o, u1, u2);
    AreEqual(projected, V(2, 0, 4));
  }

  [Test]
  public void LiftUp_ExtendsVectorWithGivenValue() {
    Vector v2 = V(1, 2);
    Vector v4 = v2.LiftUp(4, 5.0);
    AreEqual(v4, V(1, 2, 5, 5));
  }

  [Test]
  public void CosAngleAndAngle_HandleCanonicalDirectionsAndClampNearBounds() {
    Vector v1 = V(1, 0);
    Vector v2 = V(1, 1);
    Vector v3 = V(0, 1);
    Vector v4 = V(-1, 0);
    Vector v5 = V(2, 0);
    Vector v6 = V(1, -1);
    Vector zero = Vector.Zero(2);

    double cos12 = Vector.CosAngle(v1, v2);
    double cos13 = Vector.CosAngle(v1, v3);
    double cos14 = Vector.CosAngle(v1, v4);
    double cos15 = Vector.CosAngle(v1, v5);
    double cos10 = Vector.CosAngle(v1, zero);
    double cos00 = Vector.CosAngle(zero, zero);

    Assert.Multiple(() => {
      Assert.That(Tools.EQ(cos12, 1.0 / Math.Sqrt(2.0)), Is.True);
      Assert.That(Tools.EQ(cos13, 0.0), Is.True);
      Assert.That(Tools.EQ(cos14, -1.0), Is.True);
      Assert.That(Tools.EQ(cos15, 1.0), Is.True);
      Assert.That(Tools.EQ(cos10, 1.0), Is.True, "CosAngle with zero vector should be 1.");
      Assert.That(Tools.EQ(cos00, 1.0), Is.True, "CosAngle of zero vectors should be 1.");
      Assert.That(Tools.EQ(Vector.Angle(v1, v2), Math.PI / 4.0), Is.True);
      Assert.That(Tools.EQ(Vector.Angle(v1, v3), Math.PI / 2.0), Is.True);
      Assert.That(Tools.EQ(Vector.Angle(v1, v4), Math.PI), Is.True);
      Assert.That(Tools.EQ(Vector.Angle(v1, v5), 0.0), Is.True);
      Assert.That(Tools.EQ(Vector.Angle(v1, v6), Math.PI / 4.0), Is.True);
    });

    Vector vAlmostMinusOne = (-1.0 - Tools.Eps * 0.5) * v1;
    Vector vAlmostOne = (1.0 + Tools.Eps * 0.5) * v1;
    Assert.That(Tools.EQ(Vector.Angle(v1, vAlmostMinusOne), Math.PI), Is.True, "Angle should clamp near -1");
    Assert.That(Tools.EQ(Vector.Angle(v1, vAlmostOne), 0.0), Is.True, "Angle should clamp near 1");
  }

  [Test]
  public void OuterProduct_BuildsExpectedMatrix() {
    Vector v1 = V(1, 2);
    Vector v2 = V(3, 4, 5);
    Matrix outer = v1.OuterProduct(v2);

    Assert.That(outer.Rows, Is.EqualTo(2));
    Assert.That(outer.Cols, Is.EqualTo(3));
    Assert.That(Tools.EQ(outer[0, 0], 3.0));
    Assert.That(Tools.EQ(outer[0, 1], 4.0));
    Assert.That(Tools.EQ(outer[0, 2], 5.0));
    Assert.That(Tools.EQ(outer[1, 0], 6.0));
    Assert.That(Tools.EQ(outer[1, 1], 8.0));
    Assert.That(Tools.EQ(outer[1, 2], 10.0));
  }

  [Test]
  public void SubVector_ReturnsContinuousCoordinateSlice() {
    Vector v = V(0, 1, 2, 3, 4, 5);

    AreEqual(v.SubVector(2, 4), V(2, 3, 4));
    AreEqual(v.SubVector(0, 0), V(0));
    AreEqual(v.SubVector(5, 5), V(5));
  }

  [Test]
  public void ParallelismAndOrthogonalityPredicates_FollowCurrentContract() {
    Vector v1 = V(1, 2, -1);
    Vector v2 = V(2, 4, -2);
    Vector v3 = V(-1, -2, 1);
    Vector v4 = V(1, 0, 0);
    Vector orth1 = V(1, 2, 0);
    Vector orth2 = V(2, -1, 5);
    Vector nonOrth = V(1, 1, 1);
    Vector zero = Vector.Zero(3);

    Assert.Multiple(() => {
      Assert.That(Vector.AreParallel(v1, v2), Is.True);
      Assert.That(Vector.AreParallel(v1, v3), Is.True);
      Assert.That(Vector.AreParallel(v1, v4), Is.False);
      Assert.That(Vector.AreParallel(v1, zero), Is.True);
      Assert.That(Vector.AreParallel(zero, zero), Is.True);
      Assert.That(Vector.AreCodirected(v1, v2), Is.True);
      Assert.That(Vector.AreCodirected(v1, v3), Is.False);
      Assert.That(Vector.AreCodirected(v1, v4), Is.False);
      Assert.That(Vector.AreCodirected(v1, zero), Is.True);
      Assert.That(Vector.AreCodirected(zero, zero), Is.True);
      Assert.That(Vector.AreCounterdirected(v1, v2), Is.False);
      Assert.That(Vector.AreCounterdirected(v1, v3), Is.True);
      Assert.That(Vector.AreCounterdirected(v1, v4), Is.False);
      Assert.That(Vector.AreCounterdirected(v1, zero), Is.True);
      Assert.That(Vector.AreCounterdirected(zero, zero), Is.True);
      Assert.That(Vector.AreOrthogonal(orth1, orth2), Is.True);
      Assert.That(Vector.AreOrthogonal(orth1, nonOrth), Is.False);
      Assert.That(Vector.AreOrthogonal(orth1, zero), Is.True);
      Assert.That(Vector.AreOrthogonal(zero, zero), Is.True);
    });
  }

}
