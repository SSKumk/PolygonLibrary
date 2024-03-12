using System.Diagnostics;
using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests;

[TestFixture]
public class ConvexPolytopesTests {

  [Test]
  public void MakeSphereTest() {
    for (int i = 2; i < 8; i++) {
      ConvexPolytop sphere   = ConvexPolytop.Sphere(i, 15, 20,1);
      foreach (Vector p in sphere.Vertices) {
        Assert.That(Tools.EQ(p.Length2, 1), "Sphere maker is broken! Do not use it!");
      }
    }
  }

}
