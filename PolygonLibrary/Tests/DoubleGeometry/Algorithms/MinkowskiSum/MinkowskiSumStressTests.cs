using NUnit.Framework;
using System.Linq;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsPolytopes<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiSumStressTests {

  [Test]
  public void BySandipDas_Cube5DWithItself_EqualsConvexHullResult() {
    ConvexPolytop cube = ConvexPolytop.CreateFromPoints(Cube5D_list);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(cube, cube);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(cube, cube);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_Cube5DAndRandomSimplex5D_EqualsConvexHullResult() {
    ConvexPolytop cube = ConvexPolytop.CreateFromPoints(Cube5D_list);
    ConvexPolytop randomSimplex = ConvexPolytop.CreateFromPoints(SimplexRND5D_list);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(cube, randomSimplex);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(cube, randomSimplex);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_Cube5DAndSimplex5D_PreservesCommutativityAndAffineInvariance() {
    ConvexPolytop cube = ConvexPolytop.CreateFromPoints(Cube5D_list);
    ConvexPolytop simplex = ConvexPolytop.CreateFromPoints(Simplex5D_list);

    ConvexPolytop direct = MinkowskiSum.BySandipDas(cube, simplex);
    ConvexPolytop reverse = MinkowskiSum.BySandipDas(simplex, cube);

    Matrix rotation12 = Tests.TestInfrastructure.TestsBase<double, Tests.DConvertor>.MakeRotationMatrix(5, 1, 2, double.Pi / 4);
    Matrix rotation35 = Tests.TestInfrastructure.TestsBase<double, Tests.DConvertor>.MakeRotationMatrix(5, 3, 5, double.Pi / 4);
    Vector shiftCube = new(new double[] { 1, 2, 3, 4, 5 });
    Vector shiftSimplex = new(new double[] { -2, 1, 0, 3, -1 });

    ConvexPolytop rotated12Direct = MinkowskiSum.BySandipDas(cube.Rotate(rotation12), simplex.Rotate(rotation12));
    ConvexPolytop rotated35Direct = MinkowskiSum.BySandipDas(cube.Rotate(rotation35), simplex.Rotate(rotation35));
    ConvexPolytop shiftedDirect = MinkowskiSum.BySandipDas(cube.Shift(shiftCube), simplex.Shift(shiftSimplex));

    Assert.Multiple(() => {
      Assert.That(direct, Is.EqualTo(reverse));
      Assert.That(rotated12Direct, Is.EqualTo(direct.Rotate(rotation12)));
      Assert.That(rotated35Direct, Is.EqualTo(direct.Rotate(rotation35)));
      Assert.That(shiftedDirect, Is.EqualTo(direct.Shift(shiftCube + shiftSimplex)));
    });
  }

  [Test]
  public void BySandipDas_LiftedCube2DAndCube5D_EqualsConvexHullResult() {
    ConvexPolytop liftedCube2D = ConvexPolytop.CreateFromPoints(Cube2D_list.Select(p => p.LiftUp(5, Tools.Zero)));
    ConvexPolytop cube5D = ConvexPolytop.CreateFromPoints(Cube5D_list);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(liftedCube2D, cube5D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(liftedCube2D, cube5D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_LiftedSimplex3DAndCube5D_EqualsConvexHullResult() {
    ConvexPolytop liftedSimplex3D = ConvexPolytop.CreateFromPoints(Simplex3D_list.Select(p => p.LiftUp(5, Tools.Zero)));
    ConvexPolytop cube5D = ConvexPolytop.CreateFromPoints(Cube5D_list);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(liftedSimplex3D, cube5D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(liftedSimplex3D, cube5D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

}
