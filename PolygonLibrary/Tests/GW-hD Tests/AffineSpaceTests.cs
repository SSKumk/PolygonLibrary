using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace Tests.GW_hD_Tests;

[TestFixture]
public class AffineSpaceTests {

  /// <summary>
  /// Simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_1() {
    var origin = new Point(new double[] { 0, 0 });

    var basis = new List<Vector>
      {
        new Vector(new double[] { 1, 0 })
      , new Vector(new double[] { 0, 1 })
      };

    var points = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 2, 3 })
      , new Point(new double[] { -1, 4 })
      };

    var expected = points;

    var result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  /// <summary>
  /// Reflect simple test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_2() {
    var origin = new Point(new double[] { 0, 0 });

    var basis = new List<Vector>
      {
        new Vector(new double[] { -1, 0 })
      , new Vector(new double[] { 0, -1 })
      };

    var points = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 2, 3 })
      , new Point(new double[] { -1, 4 })
      };

    var expected = new HashSet<Point>
      {
        new Point(new double[] { -1, -1 })
      , new Point(new double[] { -2, -3 })
      , new Point(new double[] { 1, -4 })
      };

    var result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  /// <summary>
  /// Reflect and shift simple test 
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_3() {
    var origin = new Point(new double[] { 2, 2 });

    var basis = new List<Vector>
      {
        new Vector(new double[] { -1, 0 })
      , new Vector(new double[] { 0, -1 })
      };

    var points = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 2, 4 })
      , new Point(new double[] { -4, 4 })
      };

    var expected = new HashSet<Point>
      {
        new Point(new double[] { 1, 1 })
      , new Point(new double[] { 0, -2 })
      , new Point(new double[] { 6, -2 })
      };

    var result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

  /// <summary>
  /// 4D-test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_4() {
    var origin = new Point(new double[] { 0, 0, 0, 0 });

    var basis = new List<Vector>
      {
        new Vector(new double[] { 1, 0, 0, 0 })
      , new Vector(new double[] { 0, 1, 0, 0 })
      };

    var points = new HashSet<Point>
      {
        new Point(new double[] { 0, 0, 0, 0})
      , new Point(new double[] { 1, 0, 0, 0})
      , new Point(new double[] { 0, 1, 0, 0})
      , new Point(new double[] { 1, 1, 1, 1})
      };

    var expected = new HashSet<Point>
      {
        new Point(new double[] { 0, 0, 0, 0})
      , new Point(new double[] { 1, 0, 0, 0})
      , new Point(new double[] { 0, 1, 0, 0})
      , new Point(new double[] { 1, 1, 0, 0})
      };

    var result = Tools.ProjectToAffineSpace(origin, basis, points);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

}
