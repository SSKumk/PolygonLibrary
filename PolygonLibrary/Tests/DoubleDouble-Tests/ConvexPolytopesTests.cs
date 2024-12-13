using System.Diagnostics;
using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests;

[TestFixture]
public class ConvexPolytopesTests {

  [Test]
  public void MakeSphereZeroCeneterTest() {
    for (int i = 2; i < 7; i++) {
      ConvexPolytop sphere   = ConvexPolytop.Sphere(Vector.Zero(i),1, 15, 20);
      foreach (Vector p in sphere.Vrep) {
        Assert.That(Tools.EQ(p.Length2, 1), "Sphere maker is broken! Do not use it!");
      }
    }
  }
  [Test]
  public void MakeSphereCeneterTest() {
    for (int i = 2; i < 7; i++) {
      Vector        center = Vector.GenVector(i);
      ConvexPolytop sphere = ConvexPolytop.Sphere(center,1, 15, 20);
      foreach (Vector p in sphere.Vrep) {
        Assert.That(Tools.EQ((p - center).Length2, 1), "Sphere maker is broken! Do not use it!");
      }
    }
  }

}
