using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class HrepToFLrepCurrentContractTests {

  [Test]
  public void HrepToFLrepGeometric_ForBoundedUnitSquare_CurrentlyThrowsNotImplementedException() {
    Assert.That(
      () => HrepToFLrep.HrepToFLrep_Geometric(ConvexPolytopTestData.UnitSquareHrep, 2),
      Throws.TypeOf<NotImplementedException>()
        .With.Message.EqualTo("Теперь тут надо сначала собрать все подузлы, а уже затем собирать узел размерности больше")
    );
  }

  [Test]
  public void HrepToFLrepGeometric_ForUnboundedStripWithoutVertex_ReturnsNull() {
    List<HyperPlane> strip =
      [
        new HyperPlane(-Vector.MakeOrth(2, 1), Tools.Zero),
        new HyperPlane(Vector.MakeOrth(2, 1), Tools.One)
      ];

    FaceLattice? lattice = HrepToFLrep.HrepToFLrep_Geometric(strip, 2);

    Assert.That(lattice, Is.Null);
  }

}
