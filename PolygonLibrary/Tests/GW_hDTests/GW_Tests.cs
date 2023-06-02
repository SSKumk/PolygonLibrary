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
      Console.WriteLine(x.PolyhedronDim);
  }



}
