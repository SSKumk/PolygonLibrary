using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiSumHighDimensionalTests {

  [Test]
  public void BySandipDas_Cube3DWithItself_EqualsConvexHullResult() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(cube, cube);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(cube, cube);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_OnlyHrep_ProducesEquivalentPolytope() {
    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(MinkowskiSumTestData.Square, MinkowskiSumTestData.ShiftedSquare);
    ConvexPolytop sumOnlyHrep = MinkowskiSum.BySandipDas(MinkowskiSumTestData.Square, MinkowskiSumTestData.ShiftedSquare, onlyHrep: true);

    Assert.Multiple(() => {
      Assert.That(sumOnlyHrep.WhichRep, Is.EqualTo(ConvexPolytop.Rep.Hrep));
      Assert.That(sumOnlyHrep, Is.EqualTo(sumConvexHull));
    });
  }

}
