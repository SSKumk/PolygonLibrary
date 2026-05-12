using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

public static class HyperPlaneAssert {

  public static void IsConsistent(HyperPlane plane) {
#if DEBUG
    Assert.DoesNotThrow(() => HyperPlane.CheckCorrectness(plane));
#endif
    Vector normal = plane.Normal;
    Assert.That(Tools.EQ(normal.Length, 1.0), Is.True, "Normal should be unit length.");

    double constant = plane.ConstantTerm;
    Assert.That(Tools.EQ(normal * plane.Origin, constant), Is.True, "Origin should satisfy N*Origin = C");

    AffineBasis affineBasis = plane.AffBasis;
    foreach (Vector basisVector in affineBasis) {
      Assert.That(Tools.EQ(normal * basisVector), Is.True, $"Normal should be orthogonal to basis vector {basisVector}");
    }
  }

}
