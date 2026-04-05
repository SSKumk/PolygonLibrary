using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsBase<double, Tests.DConvertor>;
using static Tests.TestInfrastructure.TestsPolytopes<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Algorithms;

[TestFixture]
public class MinkowskiSumRegressionTests {

  [Test]
  public void BySandipDas_Simplex3DWithItself_EqualsConvexHullResult() {
    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(Simplex3D, Simplex3D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(Simplex3D, Simplex3D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_Cube4DWithItself_EqualsConvexHullResult() {
    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(Cube4D, Cube4D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(Cube4D, Cube4D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_Cube3DAndRotatedOctahedron_ArticleExample_EqualsConvexHullResult() {
    ConvexPolytop p = ConvexPolytop.CreateFromPoints(Cube3D_list);
    ConvexPolytop q = ConvexPolytop.CreateFromPoints(Rotate(Octahedron3D_list, rotate3D_45XY));

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(p, q);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(p, q);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_IsCommutative_ForSimplex3DAndCube3D() {
    ConvexPolytop pq = MinkowskiSum.BySandipDas(Simplex3D, Cube3D);
    ConvexPolytop qp = MinkowskiSum.BySandipDas(Cube3D, Simplex3D);

    Assert.That(pq, Is.EqualTo(qp));
  }

  [Test]
  public void BySandipDas_WorstCase3D_ArticleExample_EqualsConvexHullResult() {
    const int theta = 2;
    List<Vector> p = MakePointsOnSphere_3D(theta, 8, true, true);
    p = Rotate(p, MakeRotationMatrix(3, 2, 3, -double.Pi / 18));
    List<Vector> q = MakePointsOnSphere_3D(theta, 8, true, true);
    q = Rotate(q, MakeRotationMatrix(3, 1, 3, double.Pi / 2));

    ConvexPolytop P = ConvexPolytop.CreateFromPoints(p);
    ConvexPolytop Q = ConvexPolytop.CreateFromPoints(q);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(P, Q);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(P, Q);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_Cube3DAndRotatedCube_EqualsConvexHullResult() {
    ConvexPolytop p = ConvexPolytop.CreateFromPoints(Cube3D_list);
    ConvexPolytop q = ConvexPolytop.CreateFromPoints(Rotate(Cube3D_list, rotate3D_45XY));

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(p, q);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(p, q);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_TwoIndependentSquaresIn4D_EqualsConvexHullResult() {
    Vector p0 = new(new double[] { 0, 0, 0, 0 });
    Vector p1 = new(new double[] { 1, 0, 0, 0 });
    Vector p2 = new(new double[] { 0, 1, 0, 0 });
    Vector p3 = new(new double[] { 1, 1, 0, 0 });

    Vector u0 = new(new double[] { 0, 0, 1, 1 });
    Vector u1 = new(new double[] { 1, 0, 1, 1 });
    Vector u2 = new(new double[] { 0, 1, 1, 1 });
    Vector u3 = new(new double[] { 1, 1, 1, 1 });

    ConvexPolytop P = ConvexPolytop.CreateFromPoints([p0, p1, p2, p3]);
    ConvexPolytop Q = ConvexPolytop.CreateFromPoints([u0, u1, u2, u3]);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(P, Q);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(P, Q);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_PointIn3DAndCube3D_EqualsConvexHullResult() {
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([new Vector(new double[] { 1, 1, 1 })]);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(point, Cube3D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(point, Cube3D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_SegmentIn3DAndCube3D_EqualsConvexHullResult() {
    ConvexPolytop segment = ConvexPolytop.CreateFromPoints([
      new Vector(new double[] { 0, 0, 0 }),
      new Vector(new double[] { 1, 1, 1 })
    ]);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(segment, Cube3D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(segment, Cube3D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

  [Test]
  public void BySandipDas_SquareIn3DAndCube3D_EqualsConvexHullResult() {
    ConvexPolytop square = ConvexPolytop.CreateFromPoints([
      new Vector(new double[] { 0, 0, 0 }),
      new Vector(new double[] { 1, 0, 0 }),
      new Vector(new double[] { 0, 1, 0 }),
      new Vector(new double[] { 1, 1, 0 })
    ]);

    ConvexPolytop sumConvexHull = MinkowskiSum.ByConvexHull(square, Cube3D);
    ConvexPolytop sumSandipDas = MinkowskiSum.BySandipDas(square, Cube3D);

    Assert.That(sumSandipDas, Is.EqualTo(sumConvexHull));
  }

}
