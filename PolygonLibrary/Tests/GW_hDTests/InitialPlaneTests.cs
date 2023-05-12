using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;


namespace Tests.GW_hDTests;


[TestFixture]
public class InitialPlaneTests {

  /// <summary>
  /// Aux method to check basis of Initial Plane
  /// </summary>
  /// <param name="Swarm"></param>
  private static void AssertInitialPlaneBasis(IEnumerable<Point> Swarm) {
    AffineBasis aBasis = GiftWrapping.BuildInitialPlane(Swarm);
    AffineBasis.CheckCorrectness(aBasis);
    HyperPlane         hp         = new HyperPlane(aBasis);
    IEnumerable<Point> notInPlane = hp.FilterNotIn(Swarm);
    Assert.That(hp.AllAtOneSide(notInPlane).Item1, Is.True);
  }

  [Test]
  public void ThreeIndependentPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, 1, 0 })
      , new Point(new double[] { 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void ManyPointsInPlaneTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 2, 4, 0 })
      , new Point(new double[] { -3, 1, 0 })
      , new Point(new double[] { -9, -2, 0 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void TetrahedronSinglePointTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 4 })
      , new Point(new double[] { 3, 0, 0 })
      , new Point(new double[] { 1, 2, 0 })
      , new Point(new double[] { 5, 5, 5 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void TetrahedronEdgeTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { -1, 0, 0 })
      , new Point(new double[] { -3, 0, 0 })
      , new Point(new double[] { 0, -3, 0 })
      , new Point(new double[] { -1, -1, 5 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void TetrahedronThreeIndependentPointTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { -2, 0, 0 })
      , new Point(new double[] { 0, 0, 3 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, -5, 0 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void TetrahedronManyEdgePointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { -2, 0, 0 })
      , new Point(new double[] { -1, 0, 3 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, -5, 0 })
      , new Point(new double[] { 0.5, 0, 0 })
      , new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { -1, 0, 0 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void TetrahedronManyFacePointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { -2, 0, 0 })
      , new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 0, 0, 0.5 })
      , new Point(new double[] { 0.5, 0, 0.5 })
      , new Point(new double[] { 0.5, 0, 0.5 })
      , new Point(new double[] { -1, 0, 1.5 })
      , new Point(new double[] { 0, 0, 3 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, -5, 0 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void TetrahedronEdgeNeighborsPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { -2, 0, 0 })
      , new Point(new double[] { 0, -1, 3 })
      , new Point(new double[] { 0, -5, 0 })
      , new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 0, -1, 0 })
      , new Point(new double[] { 0, -3, 0 })
      , new Point(new double[] { 0, -0.2685115568, 0.8055346704 })
      , new Point(new double[] { -1.724137931, -0.6896551724, 0 })
      , new Point(new double[] { -0.7682913285, -3.0792716788, 0 })
      , new Point(new double[] { 0, -0.6719742173, 2.0159226519 })
      , new Point(new double[] { 0, -0.2685115568, 0.8055346704 })
      , new Point(new double[] { -1.3021563037, -0.3489218481, 1.0467655444 })
      , new Point(new double[] { -0.6464806022, -0.6767596989, 2.0302790966 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void QuadrangleSinglePointTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 1, 2, 0 })
      , new Point(new double[] { 2, 1, 0 })
      , new Point(new double[] { 3, 0.5, 1 })
      , new Point(new double[] { 0.5, 3, 1 })
      , new Point(new double[] { 1, 6, 0 })
      , new Point(new double[] { 6, 1, 0 })
      , new Point(new double[] { 6, 0.5, 1 })
      , new Point(new double[] { 0.5, 6, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void CubeEdgeTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 2, 2, 0 })
      , new Point(new double[] { 0, 4, 0 })
      , new Point(new double[] { -2, 2, 0 })
      , new Point(new double[] { 0, 0, 2.8284271247 })
      , new Point(new double[] { 2, 2, 2.8284271247 })
      , new Point(new double[] { 0, 4, 2.8284271247 })
      , new Point(new double[] { -2, 2, 2.8284271247 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void CubeFaceTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, 1, 0 })
      , new Point(new double[] { 0, 0, 1 })
      , new Point(new double[] { 1, 1, 0 })
      , new Point(new double[] { 0, 1, 1 })
      , new Point(new double[] { 1, 0, 1 })
      , new Point(new double[] { 1, 1, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void CubeEdgeWithPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 0, 0, 1 })
      , new Point(new double[] { 0, 0, 2 })
      , new Point(new double[] { 2, 2, 0 })
      , new Point(new double[] { 0, 4, 0 })
      , new Point(new double[] { -2, 2, 0 })
      , new Point(new double[] { 0, 0, 2.8284271247 })
      , new Point(new double[] { 2, 2, 2.8284271247 })
      , new Point(new double[] { 0, 4, 2.8284271247 })
      , new Point(new double[] { -2, 2, 2.8284271247 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void CubeFaceWithPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0 })
      , new Point(new double[] { 0, 1, 0 })
      , new Point(new double[] { 0, 0, 1 })
      , new Point(new double[] { 1, 1, 0 })
      , new Point(new double[] { 0, 1, 1 })
      , new Point(new double[] { 1, 0, 1 })
      , new Point(new double[] { 1, 1, 1 })
      , new Point(new double[] { 0.5, 0, 0.5 })
      , new Point(new double[] { 0.6, 0, 0.4 })
      , new Point(new double[] { 0.1, 0, 0.9 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void LowerDim4D_ThreeIndependentPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void LowerDim4D_ManyPointsIn2DPlaneTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      , new Point(new double[] { 2, 0, 0, 7 })
      , new Point(new double[] { 3, 0, 0, 5 })
      , new Point(new double[] { 4, 0, 0, 8 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void LowerDim4D_ManyPointsIn3DPlaneTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 0, 0, 1, 0 })
      , new Point(new double[] { 2, 113, double.Pi, 0 })
      , new Point(new double[] { -5, 23, -1, 0 })
      , new Point(new double[] { 77, 25, 1.454, 0 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void Simplex4D_SinglePointTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0.1, 1, 0, 0 })
      , new Point(new double[] { 0.1, 0, 1, 0 })
      , new Point(new double[] { 0.1, 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void Simplex4D_1DEdgeTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0.1, 1, 0, 0 })
      , new Point(new double[] { 0.1, 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void Simplex4D_1DEdgeWithPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0.1, 1, 0, 0 })
      , new Point(new double[] { 0.1, 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      , new Point(new double[] { 0, 0, 0, 0.2 })
      , new Point(new double[] { 0, 0, 0, 0.5 })
      , new Point(new double[] { 0, 0, 0, 0.8 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void Simplex4D_2DEdgeTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0.1, 1, 0, 0 })
      , new Point(new double[] { 0, 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void Simplex4D_2DEdgeWithPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0.1, 1, 0, 0 })
      , new Point(new double[] { 0, 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      , new Point(new double[] { 0, 0, 2, 1 })
      , new Point(new double[] { 0, 0, 2, -1 })
      , new Point(new double[] { 0, 0, -2, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void Simplex4D_3DFaceTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 0, 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void Simplex4D_3DFaceWithPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 0, 0, 1, 0 })
      , new Point(new double[] { 0, 0, 0, 1 })
      , new Point(new double[] { 0, 5, 8, 2 })
      , new Point(new double[] { 0, -9, 4, 3 })
      };

    AssertInitialPlaneBasis(Swarm);
  }


  [Test]
  public void Simplex4D_1DEdge_2DNeighborsPointsTest() {
    Point p1 = new Point(new double[] { 1, 0, 0, 0 });
    Point p2 = new Point(new double[] { 0, 1, 0, 0 });

    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 0.1, 0, 1, 0 })
      , new Point(new double[] { 0.1, 0, 0, 1 })
      , Point.LinearCombination(p1, 0.3, p2, 0.2)
      , Point.LinearCombination(p1, 0.4, p2, 0.1)
      };

    AssertInitialPlaneBasis(Swarm);
  }

  [Test]
  public void Simplex4D_1DEdge_3DNeighborsPointsTest() {
    Point p1 = new Point(new double[] { 1, 0, 0, 0 });
    Point p2 = new Point(new double[] { 0, 1, 0, 0 });
    Point p3 = new Point(new double[] { 0.1, 0, 1, 0 });

    List<Point> ps = new List<Point>()
      {
        p1
      , p2
      , p3
      };

    List<double> ws = new List<double>()
      {
        0.1
      , 0.2
      , 0.3
      };

    List<Point> Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0, 0 })
      , new Point(new double[] { 1, 0, 0, 0 })
      , new Point(new double[] { 0, 1, 0, 0 })
      , new Point(new double[] { 0.1, 0, 1, 0 })
      , new Point(new double[] { 0.1, 0, 0, 1 })
      , Point.LinearCombination(ps, ws)
      };

    AssertInitialPlaneBasis(Swarm);
  }

}
