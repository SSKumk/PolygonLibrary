using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;
using PolygonLibrary.Toolkit;


namespace Tests.GW_hD_Tests;

[TestFixture]
public class InitialPlaneTests {

  /// <summary>
  /// Simple 3D-cube test  
  /// </summary>
  [Test]
  public void InitialPlane_1() {
    var Swarm = new List<Point>()
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

    var expectedOrigin = new Point(new double[] { 0, 0, 0 });

    var expectedBasis = new List<Vector>() //todo Как проверять базис?
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    var affineBasis = GiftWrapping.BuildInitialPlane(Swarm);

    Assert.That(affineBasis.Origin == expectedOrigin, $"Origin don't match! Found {affineBasis.Origin} expected {expectedOrigin}");

    Assert.That
      (affineBasis.Basis.All(x => expectedBasis.Any(y => x == y)), $"Origin don't match! Found {affineBasis.Origin} expected {expectedOrigin}");
  }

  /// <summary>
  /// 3D-cube rotated in xOy by 45
  /// </summary>
  [Test]
  public void InitialPlane_2() {
    var Swarm = new List<Point>()
      {
        new Point(new double[] { 0, 0, 0 })
      , new Point(new double[] { 2, 2, 0 })
      , new Point(new double[] { 0, 4, 0 })
      , new Point(new double[] { -2, 2, 0 })
      , new Point(new double[] { 0, 0, 2.83 })
      , new Point(new double[] { 2, 2, 2.83 })
      , new Point(new double[] { 0, 4, 2.83 })
      , new Point(new double[] { -2, 2, 2.83 })
      };

    var expectedOrigin = new Point(new double[] { -2, 2, 0 });

    var expectedBasis = new List<Vector>() //todo Как проверять базис?
      {
        new Vector(new double[] { 2/double.Sqrt(8), 2/double.Sqrt(8), 0 })
      , new Vector(new double[] { 2/double.Sqrt(8), -2/double.Sqrt(8), 0 })
      , new Vector(new double[] { 0, 0, 1 })
      };

    var affineBasis = GiftWrapping.BuildInitialPlane(Swarm);

    Assert.That(affineBasis.Origin == expectedOrigin, $"Origin don't match! Found {affineBasis.Origin} expected {expectedOrigin}");

    Assert.That
      (affineBasis.Basis.All(x => expectedBasis.Any(y => x == y)), $"Origin don't match! Found {affineBasis.Origin} expected {expectedOrigin}");
  }

}
