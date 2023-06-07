using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;



namespace Tests.GW_hDTests; 

[TestFixture]
public class GW_Tests {

  [Test]
  public void Cube3D() {
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

      var x = GiftWrapping.ToConvex(Swarm);
      foreach (Point point in x.Vertices)
      {
        Console.WriteLine(point);
      }
  }
  
  [Test]
  public void Simplex3D() {
      List<Point> Swarm = new List<Point>()
        {
          new Point(new double[] { 0, 0, 0 })
        , new Point(new double[] { 1, 0, 0 })
        , new Point(new double[] { 0, 1, 0 })
        , new Point(new double[] { 0, 0, 1 })
        };

      var x = GiftWrapping.ToConvex(Swarm);
      foreach (Point point in x.Vertices)
      {
        Console.WriteLine(point);
      }
  }


  [Test]
  public void Cube3D_withInnerPoints() {
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
      , new Point(new double[] { 0.5, 0.6, 0.7 })
      , new Point(new double[] { 0.5, 0.5, 0.7 })
      , new Point(new double[] { 0.7, 0.6, 0.7 })
    };

    var x = GiftWrapping.ToConvex(Swarm);
    foreach (Point point in x.Vertices)
    {
      Console.WriteLine(point);
    }
  }


  [Test]
  public void Simplex4D_1DEdge_2DNeighborsPointsTest() {
    Point p0 = new Point(new double[] { 0, 0, 0, 0 });
    Point p1 = new Point(new double[] { 1, 0, 0, 0 });
    Point p2 = new Point(new double[] { 0, 1, 0, 0 });
    Point p3 = new Point(new double[] { 0.1, 0, 1, 0 });
    Point p4 = new Point(new double[] { 0.1, 0, 0, 1 });

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

    var x = GiftWrapping.ToConvex(Swarm);
    foreach (Point point in x.Vertices)
    {
      Console.WriteLine(point);
    }    
  }

}
