using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiSumHighDimensionalTests {

  [Test]
  public void AlgSumPoints_Cube3D_DeduplicatesRepeatedPairwiseSumsWithoutLosingVertices() {
    ConvexPolytop cube = ConvexPolytop.Cube01_VRep(3);

    SortedSet<Vector> sum = MinkowskiSum.AlgSumPoints(cube.Vrep, cube.Vrep);
    SortedSet<Vector> expected = new SortedSet<Vector>();
    foreach (Vector a in cube.Vrep) {
      foreach (Vector b in cube.Vrep) {
        expected.Add(a + b);
      }
    }

    Assert.That(sum.SetEquals(expected), Is.True);
    Assert.That(sum.Count, Is.EqualTo(27));
  }

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
