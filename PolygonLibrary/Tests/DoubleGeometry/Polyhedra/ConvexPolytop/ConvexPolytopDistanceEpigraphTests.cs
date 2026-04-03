using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

[TestFixture]
public class ConvexPolytopDistanceEpigraphTests {

  [Test]
  public void BuildDistanceEpigraph_ForUnitBallAndPoint_BuildsConeOverLiftedBase() {
    ConvexPolytop unitBall = ConvexPolytop.Ball_1(Vector.Zero(2), 1);
    Vector point = ConvexPolytopAssert.V(1, 2);

    ConvexPolytop epigraph = ConvexPolytop.BuildDistanceEpigraph(unitBall, point, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      epigraph.Vrep,
      [
        new Vector(new double[] { 1, 2, 0 }),
        new Vector(new double[] { 3, 2, 2 }),
        new Vector(new double[] { -1, 2, 2 }),
        new Vector(new double[] { 1, 4, 2 }),
        new Vector(new double[] { 1, 0, 2 })
      ]
    );
  }

  [Test]
  public void BuildDistanceEpigraphL1_ForPoint_BuildsDiamondCone() {
    Vector point = ConvexPolytopAssert.V(1, 2);

    ConvexPolytop epigraph = ConvexPolytop.BuildDistanceEpigraph_L1(point, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      epigraph.Vrep,
      [
        new Vector(new double[] { 1, 2, 0 }),
        new Vector(new double[] { 3, 2, 2 }),
        new Vector(new double[] { -1, 2, 2 }),
        new Vector(new double[] { 1, 4, 2 }),
        new Vector(new double[] { 1, 0, 2 })
      ]
    );
  }

  [Test]
  public void BuildDistanceEpigraphLinf_ForPoint_BuildsSquareCone() {
    Vector point = ConvexPolytopAssert.V(1, 2);

    ConvexPolytop epigraph = ConvexPolytop.BuildDistanceEpigraph_Linf(point, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(
      epigraph.Vrep,
      [
        new Vector(new double[] { 1, 2, 0 }),
        new Vector(new double[] { -1, 0, 2 }),
        new Vector(new double[] { 3, 0, 2 }),
        new Vector(new double[] { 3, 4, 2 }),
        new Vector(new double[] { -1, 4, 2 })
      ]
    );
  }

  [Test]
  public void BuildDistanceEpigraphL2_In2DMatchesSphereBasedCone() {
    Vector point = ConvexPolytopAssert.V(1, 2);

    ConvexPolytop viaL2 = ConvexPolytop.BuildDistanceEpigraph_L2(point, 4, 7, 2);
    ConvexPolytop viaSphere = ConvexPolytop.BuildDistanceEpigraph(ConvexPolytop.Sphere(Vector.Zero(2), 1, 4, 1), point, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(viaL2.Vrep, viaSphere.Vrep);
  }

  [Test]
  public void BuildDistanceEpigraphToPolytopeL1_ForSingletonPolytope_MatchesPointVersion() {
    Vector point = ConvexPolytopAssert.V(1, 2);
    ConvexPolytop singleton = ConvexPolytop.CreateFromPoints([point], true);

    ConvexPolytop viaPolytope = ConvexPolytop.BuildDistanceEpigraph_ToPolytope_L1(singleton, 2);
    ConvexPolytop viaPoint = ConvexPolytop.BuildDistanceEpigraph_L1(point, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(viaPolytope.Vrep, viaPoint.Vrep);
  }

  [Test]
  public void BuildDistanceEpigraphToPolytopeLinf_ForSingletonPolytope_MatchesPointVersion() {
    Vector point = ConvexPolytopAssert.V(1, 2);
    ConvexPolytop singleton = ConvexPolytop.CreateFromPoints([point], true);

    ConvexPolytop viaPolytope = ConvexPolytop.BuildDistanceEpigraph_ToPolytope_Linf(singleton, 2);
    ConvexPolytop viaPoint = ConvexPolytop.BuildDistanceEpigraph_Linf(point, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(viaPolytope.Vrep, viaPoint.Vrep);
  }

  [Test]
  public void BuildDistanceEpigraphToPolytopeL2_ForSingletonPolytope_MatchesPointVersion() {
    Vector point = ConvexPolytopAssert.V(1, 2);
    ConvexPolytop singleton = ConvexPolytop.CreateFromPoints([point], true);

    ConvexPolytop viaPolytope = ConvexPolytop.BuildDistanceEpigraph_ToPolytope_L2(singleton, 2, 4, 7);
    ConvexPolytop viaPoint = ConvexPolytop.BuildDistanceEpigraph_L2(point, 4, 7, 2);

    ConvexPolytopAssert.AssertVertexSetEquals(viaPolytope.Vrep, viaPoint.Vrep);
  }

}
