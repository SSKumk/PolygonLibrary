using NUnit.Framework;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace Tests;

[TestFixture]
public class HyperPlaneTests {

  [Test]
  public void ConstructorWithPointAndNormalTest() {
    Point      origin     = new Point(new double[] { 0, 0, 0 });
    Vector     normal     = new Vector(new double[] { 1, 1, 1 });
    HyperPlane hyperplane = new HyperPlane(origin, normal);

    AffineBasis.CheckCorrectness(hyperplane.AffineBasis);
  }

  [Test]
  public void ConstructorWithAffineBasisTest() {
    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      };

    AffineBasis affineBasis = new AffineBasis(new Point(new double[] { 1, 1, 1 }), vectors);
    HyperPlane  hyperplane  = new HyperPlane(affineBasis);

    AffineBasis.CheckCorrectness(hyperplane.AffineBasis);
  }

  [Test]
  public void ContainsTest() {
    Point origin = new Point(new double[] { 0, 0, 0 });
    Point v1     = new Point(new double[] { 1, 1, 1 });
    Point v2     = new Point(new double[] { 1, -1, 1 });
    Point v3     = new Point(new double[] { 0, 0, 1 });

    AffineBasis aBasis = new AffineBasis
      (
       origin
     , new List<Point>()
         {
           v1
         , v2
         }
      );

    HyperPlane hp = new HyperPlane(aBasis);

    Point p1 = Point.LinearCombination(v1, 3, v2, 5);
    Point p2 = Point.LinearCombination(v1, -3, v2, 5);
    Point p3 = Point.LinearCombination(v1, -3, v2, -5);
    Point p4 = Point.LinearCombination(v1, 3, v2, -5);

    Assert.That(hp.Contains(p1), Is.True);
    Assert.That(hp.Contains(p2), Is.True);
    Assert.That(hp.Contains(p3), Is.True);
    Assert.That(hp.Contains(p4), Is.True);

    Assert.That(hp.Contains(p1 + (v3 - origin)), Is.False);
    Assert.That(hp.Contains(p1 - (v3 - origin)), Is.False);
  }

  [Test]
  public void TestFilter() {
    Point origin = new Point(new double[] { 0, 0, 0 });
    Point e1     = new Point(new double[] { 1, 0, 0 });
    Point e2     = new Point(new double[] { 0, 1, 0 });
    Point e3     = new Point(new double[] { 0, 0, 1 });

    AffineBasis aBasis = new AffineBasis(origin, new Point[] { e1, e2 });

    HyperPlane hp = new HyperPlane(aBasis);

    List<Point> Swarm = new List<Point>()
      {
        Point.LinearCombination(e1, 3, e2, 5)
      , Point.LinearCombination(e1, -3, e2, 5)
      , Point.LinearCombination(e1, -3, e2, -5)
      , Point.LinearCombination(e1, 3, e2, -5)
      , Point.LinearCombination(e1, 3, e3, 5)
      , Point.LinearCombination(e1, -3, e3, 5)
      , Point.LinearCombination(e1, -3, e3, -5)
      , Point.LinearCombination(e1, 3, e3, -5)
      , Point.LinearCombination(e1, 3, e3, 4)
      };

    Assert.That(hp.AllAtOneSide(Swarm).Item1, Is.False);

    IEnumerable<Point> inPlane = hp.FilterIn(Swarm); 
    IEnumerable<Point> notInPlane = hp.FilterNotIn(Swarm);

    Assert.That(hp.AllAtOneSide(inPlane), Is.EqualTo((true,0)));
    Assert.That(inPlane.Count(), Is.EqualTo(4));
    Assert.That(notInPlane.Count(), Is.EqualTo(5));
  }

}
