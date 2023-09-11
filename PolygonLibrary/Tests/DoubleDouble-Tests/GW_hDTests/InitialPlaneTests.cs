using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Convertors.DDConvertor>;


namespace DoubleDoubleTests;

[TestFixture]
public class InitialPlaneTests {

  /// <summary>
  /// Aux method to check basis of Initial Plane
  /// </summary>
  /// <param name="Swarm">The swarm for which the plane is constructed</param>
  /// <param name="planeDim">The desired dimension of the plane</param>
  private static void AssertInitialPlaneBasis(IEnumerable<Point> Swarm, int planeDim) {
    { //Проверка нашего алгоритма
      AffineBasis aBasisUs = GiftWrapping.BuildInitialPlaneUs
        (new HashSet<SubPoint>(Swarm.Select(s => new SubPoint(s, null, s))), out Vector? nUs);
      AffineBasis.CheckCorrectness(aBasisUs);
      Assert.That
        (
         aBasisUs.SpaceDim
       , Is.EqualTo(planeDim)
       , $"The expected dimension of initial basis is {planeDim}. Found {aBasisUs.SpaceDim}."
        );

      if (nUs is not null) {
        HyperPlane hp = new HyperPlane(aBasisUs.Origin, nUs);
        Assert.That(hp.AllAtOneSide(Swarm).atOneSide, "BIP_Us: Some points outside the initial plane!");
        IEnumerable<Point> inThePlane = hp.FilterIn(Swarm);
        Assert.That(inThePlane.Count(), Is.GreaterThanOrEqualTo(hp.Dim), "BIP_Us:In initial plane must be at least 3 points!");
      }
    }


    { // Проверка Сварта
      AffineBasis aBasisSw = GiftWrapping.BuildInitialPlaneSwart
        (new HashSet<SubPoint>(Swarm.Select(s => new SubPoint(s, null, s))), out Vector nSwart);

      AffineBasis.CheckCorrectness(aBasisSw);
      Assert.That
        (
         aBasisSw.SpaceDim
       , Is.EqualTo(planeDim)
       , $"The expected dimension of initial basis is {planeDim}. Found {aBasisSw.SpaceDim}."
        );

      HyperPlane hp = new HyperPlane(aBasisSw.Origin, nSwart);
      Assert.That(hp.AllAtOneSide(Swarm).atOneSide, "BIP_Swart: Some points outside the initial plane!");
      IEnumerable<Point> inThePlane = hp.FilterIn(Swarm);
      Assert.That(inThePlane.Count(), Is.GreaterThanOrEqualTo(hp.Dim), "BIP_Swart:In initial plane must be at least 3 points!");
    }
  }

#region TODO Lower Dimension Swarms  //todo Научиться работать с неполными размерностями исходного роя
    // [Test]
    // public void LineInInitialPlaneTest() {
    //   Point p0 = new Point(new ddouble[] { 0, 0, 0 });
    //   Point p1 = new Point(new ddouble[] { 0, 1, 0 });
    //
    //   List<Point> Swarm = new List<Point>()
    //     {
    //       p0
    //     , p1
    //     , Point.LinearCombination(p0, 1, p1, 2)
    //     , Point.LinearCombination(p0, 1, p1, 3)
    //     , Point.LinearCombination(p0, 1, p1, 0.5)
    //     };
    //
    //   AssertInitialPlaneBasis(Swarm, 1);
    // }
    //
    // [Test]
    // public void LineNotInInitialPlaneTest() {
    //   Point p0 = new Point(new ddouble[] { 0, 0, 0 });
    //   Point p1 = new Point(new ddouble[] { 1, 1, 0 });
    //
    //   List<Point> Swarm = new List<Point>()
    //     {
    //       p0
    //     , p1
    //     , Point.LinearCombination(p0, 1, p1, 2)
    //     , Point.LinearCombination(p0, 1, p1, 3)
    //     , Point.LinearCombination(p0, 1, p1, 0.5)
    //     };
    //
    //   AssertInitialPlaneBasis(Swarm, 1);
    // }

    // [Test]
    // public void LowerDim4D_ThreeIndependentPointsTest() {
    //   List<Point> Swarm = new List<Point>()
    //     {
    //       new Point(new ddouble[] { 0, 0, 0, 0 })
    //     , new Point(new ddouble[] { 1, 0, 0, 0 })
    //     , new Point(new ddouble[] { 0, 1, 0, 0 })
    //     };
    //
    //   AssertInitialPlaneBasis(Swarm, 2);
    // }
    // [Test]
    // public void LowerDim4D_ManyPointsIn2DPlaneTest() {
    //   List<Point> Swarm = new List<Point>()
    //     {
    //       new Point(new ddouble[] { 0, 0, 0, 0 })
    //     , new Point(new ddouble[] { 1, 0, 0, 0 })
    //     , new Point(new ddouble[] { 0, 0, 0, 1 })
    //     , new Point(new ddouble[] { 2, 0, 0, 7 })
    //     , new Point(new ddouble[] { 3, 0, 0, 5 })
    //     , new Point(new ddouble[] { 4, 0, 0, 8 })
    //     };
    //
    //   AssertInitialPlaneBasis(Swarm, 2);
    // }
    //
    // [Test]
    // public void LowerDim4D_ManyPointsIn3DPlaneTest() {
    //   List<Point> Swarm = new List<Point>()
    //     {
    //       new Point(new ddouble[] { 0, 0, 0, 0 })
    //     , new Point(new ddouble[] { 1, 0, 0, 0 })
    //     , new Point(new ddouble[] { 0, 1, 0, 0 })
    //     , new Point(new ddouble[] { 0, 0, 1, 0 })
    //     , new Point(new ddouble[] { 2, 113, ddouble.PI, 0 })
    //     , new Point(new ddouble[] { -5, 23, -1, 0 })
    //     , new Point(new ddouble[] { 77, 25, 1.454, 0 })
    //     };
    //
    //   AssertInitialPlaneBasis(Swarm, 3);
    // }
#endregion


  [Test]
  public void ThreeIndependentPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 1, 0, 0 }), new Point(new ddouble[] { 0, 1, 0 }), new Point(new ddouble[] { 0, 0, 1 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void SeveralPointsInPlaneTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 1, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 0 })
      , new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { 2, 4, 0 })
      , new Point(new ddouble[] { -3, 1, 0 })
      , new Point(new ddouble[] { -9, -2, 0 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void Simplex3D_Test() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 4 })
      , new Point(new ddouble[] { 3, 0, 0 })
      , new Point(new ddouble[] { 1, 2, 0 })
      , new Point(new ddouble[] { 5, 5, 5 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void Simplex3D_1DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { -2, 0, 0 })
      , new Point(new ddouble[] { -1, 0, 3 })
      , new Point(new ddouble[] { 1, 0, 0 })
      , new Point(new ddouble[] { 0, -5, 0 })
      , new Point(new ddouble[] { 0.5, 0, 0 })
      , new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { -1, 0, 0 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void Simplex3D_EdgeNeighborsPointsTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { -2, 0, 0 })
      , new Point(new ddouble[] { 0, -1, 3 })
      , new Point(new ddouble[] { 0, -5, 0 })
      , new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { 0, -1, 0 })
      , new Point(new ddouble[] { 0, -3, 0 })
      , new Point(new ddouble[] { 0, -0.2685115568, 0.8055346704 })
      , new Point(new ddouble[] { -1.724137931, -0.6896551724, 0 })
      , new Point(new ddouble[] { -0.7682913285, -3.0792716788, 0 })
      , new Point(new ddouble[] { 0, -0.6719742173, 2.0159226519 })
      , new Point(new ddouble[] { 0, -0.2685115568, 0.8055346704 })
      , new Point(new ddouble[] { -1.3021563037, -0.3489218481, 1.0467655444 })
      , new Point(new ddouble[] { -0.6464806022, -0.6767596989, 2.0302790966 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }


  [Test]
  public void QuadrangleSinglePointTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { 1, 2, 0 })
      , new Point(new ddouble[] { 2, 1, 0 })
      , new Point(new ddouble[] { 3, 0.5, 1 })
      , new Point(new ddouble[] { 0.5, 3, 1 })
      , new Point(new ddouble[] { 1, 6, 0 })
      , new Point(new ddouble[] { 6, 1, 0 })
      , new Point(new ddouble[] { 6, 0.5, 1 })
      , new Point(new ddouble[] { 0.5, 6, 1 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }


  [Test]
  public void Cube3D_1DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { 2, 2, 0 })
      , new Point(new ddouble[] { 0, 4, 0 })
      , new Point(new ddouble[] { -2, 2, 0 })
      , new Point(new ddouble[] { 0, 0, 2.8284271247 })
      , new Point(new ddouble[] { 2, 2, 2.8284271247 })
      , new Point(new ddouble[] { 0, 4, 2.8284271247 })
      , new Point(new ddouble[] { -2, 2, 2.8284271247 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void Cube4DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 1, 1 })
      , new Point(new ddouble[] { 1, 0, 0, 1 })
      , new Point(new ddouble[] { 0, 0, 0, 1 })
      , new Point(new ddouble[] { 1, 1, 0, 0 })
      , new Point(new ddouble[] { 1, 1, 1, 0 })
      , new Point(new ddouble[] { 1, 0, 1, 0 })
      , new Point(new ddouble[] { 1, 1, 0, 1 })
      , new Point(new ddouble[] { 0, 1, 0, 1 })
      , new Point(new ddouble[] { 0, 0, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 1, 1 })
      , new Point(new ddouble[] { 1, 0, 1, 1 })
      , new Point(new ddouble[] { 0, 0, 1, 0 })
      , new Point(new ddouble[] { 0, 1, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 1, 0 })
      , new Point(new ddouble[] { 1, 1, 1, 1 })
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }

  [Test]
  public void Simplex4DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 2, 0, 0 })
      , new Point(new ddouble[] { 0, 0, 2, 0 })
      , new Point(new ddouble[] { 0, 0, 0, 2 })
      , new Point(new ddouble[] { 0, 0, 0, 0 })
      , new Point(new ddouble[] { 2, 0, 0, 0 })
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }

  [Test]
  public void Cube3DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 0 })
      , new Point(new ddouble[] { 0, 0, 1 })
      , new Point(new ddouble[] { 1, 1, 0 })
      , new Point(new ddouble[] { 0, 1, 1 })
      , new Point(new ddouble[] { 1, 0, 1 })
      , new Point(new ddouble[] { 1, 1, 1 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void Cube3D_2DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 0 })
      , new Point(new ddouble[] { 0, 0, 1 })
      , new Point(new ddouble[] { 1, 1, 0 })
      , new Point(new ddouble[] { 0, 1, 1 })
      , new Point(new ddouble[] { 1, 0, 1 })
      , new Point(new ddouble[] { 1, 1, 1 })
      , new Point(new ddouble[] { 0.5, 0, 0.5 })
      , new Point(new ddouble[] { 0.6, 0, 0.4 })
      , new Point(new ddouble[] { 0.1, 0, 0.9 })
      };

    AssertInitialPlaneBasis(Swarm, 2);
  }

  [Test]
  public void Simplex4D_1DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0, 0 })
      , new Point(new ddouble[] { 0.1, 1, 0, 0 })
      , new Point(new ddouble[] { 0.1, 0, 1, 0 })
      , new Point(new ddouble[] { 0, 0, 0, 1 })
      , new Point(new ddouble[] { 0, 0, 0, 0.2 })
      , new Point(new ddouble[] { 0, 0, 0, 0.5 })
      , new Point(new ddouble[] { 0, 0, 0, 0.8 })
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }

  [Test]
  public void Simplex4D_2DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0, 0 })
      , new Point(new ddouble[] { 0.1, 1, 0, 0 })
      , new Point(new ddouble[] { 0, 0, 1, 0 })
      , new Point(new ddouble[] { 0, 0, 0, 1 })
      , new Point(new ddouble[] { 0, 0, 2, 1 })
      , new Point(new ddouble[] { 0, 0, 2, -1 })
      , new Point(new ddouble[] { 0, 0, -2, 1 })
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }

  [Test]
  public void Simplex4D_3DTest() {
    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 0, 0 })
      , new Point(new ddouble[] { 0, 0, 1, 0 })
      , new Point(new ddouble[] { 0, 0, 0, 1 })
      , new Point(new ddouble[] { 0, 5, 8, 2 })
      , new Point(new ddouble[] { 0, -9, 4, 3 })
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }


  [Test]
  public void Simplex4D_1DEdge_2DNeighborsPointsTest() {
    Point p0 = new Point(new ddouble[] { 0, 0, 0, 0 });
    Point p1 = new Point(new ddouble[] { 1, 0, 0, 0 });
    Point p2 = new Point(new ddouble[] { 0, 1, 0, 0 });
    Point p3 = new Point(new ddouble[] { 0.1, 0, 1, 0 });
    Point p4 = new Point(new ddouble[] { 0.1, 0, 0, 1 });

    List<Point> Swarm = new List<Point>()
      {
        p0
      , p1
      , p2
      , p3
      , p4
      , Point.LinearCombination(p1, 0.3, p2, 0.2)
      , Point.LinearCombination(p1, 0.4, p2, 0.1)
      , Point.LinearCombination(p1, 0.4, p3, 0.1)
      , Point.LinearCombination(p1, 0.4, p3, 0.1)
      , Point.LinearCombination(p1, 0.4, p4, 0.1)
      , Point.LinearCombination(p1, 0.4, p4, 0.1)
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }

  [Test]
  public void Simplex4D_1DEdge_3DNeighborsPointsTest() {
    Point p1 = new Point(new ddouble[] { 1, 0, 0, 0 });
    Point p2 = new Point(new ddouble[] { 0, 1, 0, 0 });
    Point p3 = new Point(new ddouble[] { 0.1, 0, 1, 0 });

    List<Point> ps = new List<Point>() { p1, p2, p3 };

    List<ddouble> ws = new List<ddouble>() { 0.1, 0.2, 0.3 };

    List<Point> Swarm = new List<Point>()
      {
        new Point(new ddouble[] { 0, 0, 0, 0 })
      , new Point(new ddouble[] { 1, 0, 0, 0 })
      , new Point(new ddouble[] { 0, 1, 0, 0 })
      , new Point(new ddouble[] { 0.1, 0, 1, 0 })
      , new Point(new ddouble[] { 0.1, 0, 0, 1 })
      , Point.LinearCombination(ps, ws)
      };

    AssertInitialPlaneBasis(Swarm, 3);
  }

}
