using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiDiffRegressionTests {

  [TestCase(0.5)]
  [TestCase(1.0)]
  [TestCase(1.5)]
  [TestCase(2.0)]
  [TestCase(2.5)]
  public void Naive_And_Geometric_ForSphereMinusAxisSegment_Agree(double length) {
    ConvexPolytop sphere = ConvexPolytop.Sphere(Vector.Zero(3), 2, 20, 10);
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([Vector.Zero(3), new Vector(new[] { 0.0, 0.0, length })]);

    ConvexPolytop? naive = MinkowskiDiff.Naive(sphere, segment);
    ConvexPolytop? geometric = MinkowskiDiff.Geometric(sphere, segment);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.EqualTo(naive));
    });
  }

}
