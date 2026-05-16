using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class HyperPlaneEvaluationAndContainmentTests {

  [Test]
  public void Method_Eval() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), V(0, 0, 5));
    Vector pOn = V(1, 1, 5);
    Vector pAbove = V(1, 1, 6);
    Vector pBelow = V(1, 1, 4);

    Assert.That(Tools.EQ(plane.Eval(pOn)), Is.True);
    Assert.That(Tools.EQ(plane.Eval(pAbove), 1.0), Is.True);
    Assert.That(Tools.EQ(plane.Eval(pBelow), -1.0), Is.True);
  }

  [Test]
  public void Methods_Contains_Positive_Negative() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), V(0, 0, 5));
    Vector pOn = V(1, 1, 5);
    Vector pAbove = V(1, 1, 6);
    Vector pBelow = V(1, 1, 4);

    Assert.That(plane.Contains(pOn), Is.True);
    Assert.That(plane.Contains(pAbove), Is.False);
    Assert.That(plane.Contains(pBelow), Is.False);

    Assert.That(plane.ContainsPositive(pOn), Is.False);
    Assert.That(plane.ContainsPositive(pAbove), Is.True);
    Assert.That(plane.ContainsPositive(pBelow), Is.False);

    Assert.That(plane.ContainsNegative(pOn), Is.False);
    Assert.That(plane.ContainsNegative(pAbove), Is.False);
    Assert.That(plane.ContainsNegative(pBelow), Is.True);

    Assert.That(plane.ContainsNegativeNonStrict(pOn), Is.True);
    Assert.That(plane.ContainsNegativeNonStrict(pAbove), Is.False);
    Assert.That(plane.ContainsNegativeNonStrict(pBelow), Is.True);
  }

  [Test]
  public void Methods_Filter() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), V(0, 0, 5));
    Vector pOn1 = V(1, 1, 5);
    Vector pOn2 = V(2, 2, 5);
    Vector pAbove = V(1, 1, 6);
    Vector pBelow = V(1, 1, 4);
    List<Vector> swarm = new() { pOn1, pAbove, pOn2, pBelow };

    List<Vector> filteredIn = plane.FilterIn(swarm).ToList();
    List<Vector> filteredNotIn = plane.FilterNotIn(swarm).ToList();

    Assert.That(filteredIn.Count, Is.EqualTo(2));
    Assert.That(filteredIn, Does.Contain(pOn1));
    Assert.That(filteredIn, Does.Contain(pOn2));

    Assert.That(filteredNotIn.Count, Is.EqualTo(2));
    Assert.That(filteredNotIn, Does.Contain(pAbove));
    Assert.That(filteredNotIn, Does.Contain(pBelow));
  }

  [Test]
  public void Method_AllAtOneSide() {
    HyperPlane plane = new HyperPlane(V(0, 0, 1), V(0, 0, 5));
    Vector pOn1 = V(1, 1, 5);
    Vector pOn2 = V(2, 2, 5);
    Vector pAbove1 = V(1, 1, 6);
    Vector pAbove2 = V(0, 0, 10);
    Vector pBelow1 = V(1, 1, 4);
    Vector pBelow2 = V(0, 0, 0);

    var res1 = plane.AllAtOneSide(new[] { pOn1, pOn2 });
    Assert.That(res1.atOneSide, Is.True);
    Assert.That(res1.where, Is.EqualTo(0));

    var res2 = plane.AllAtOneSide(new[] { pAbove1, pAbove2 });
    Assert.That(res2.atOneSide, Is.True);
    Assert.That(res2.where, Is.EqualTo(1));

    var res3 = plane.AllAtOneSide(new[] { pBelow1, pBelow2 });
    Assert.That(res3.atOneSide, Is.True);
    Assert.That(res3.where, Is.EqualTo(-1));

    var res4 = plane.AllAtOneSide(new[] { pAbove1, pBelow1 });
    Assert.That(res4.atOneSide, Is.False);

    var res5 = plane.AllAtOneSide(new[] { pOn1, pAbove1 });
    Assert.That(res5.atOneSide, Is.False);

    var res6 = plane.AllAtOneSide(new[] { pOn1, pBelow1 });
    Assert.That(res6.atOneSide, Is.False);

    var res7 = plane.AllAtOneSide(new[] { pOn1, pAbove1, pBelow1 });
    Assert.That(res7.atOneSide, Is.False);
  }

  [Test]
  public void ContainsTest() {
    Vector origin = new Vector(new double[] { 0, 0, 0 });
    Vector v1 = new Vector(new double[] { 1, 1, 1 });
    Vector v2 = new Vector(new double[] { 1, -1, 1 });
    Vector v3 = new Vector(new double[] { 0, 0, 1 });

    AffineBasis affineBasis = AffineBasis.FromVectors(origin, new List<Vector>() { v1, v2 });
    HyperPlane plane = new HyperPlane(affineBasis);

    Vector p1 = Vector.LinearCombination(v1, 3, v2, 5);
    Vector p2 = Vector.LinearCombination(v1, -3, v2, 5);
    Vector p3 = Vector.LinearCombination(v1, -3, v2, -5);
    Vector p4 = Vector.LinearCombination(v1, 3, v2, -5);

    Assert.That(plane.Contains(p1), Is.True);
    Assert.That(plane.Contains(p2), Is.True);
    Assert.That(plane.Contains(p3), Is.True);
    Assert.That(plane.Contains(p4), Is.True);

    Assert.That(plane.Contains(p1 + (v3 - origin)), Is.False);
    Assert.That(plane.Contains(p1 - (v3 - origin)), Is.False);
  }

  [Test]
  public void TestFilter() {
    Vector origin = new Vector(new double[] { 0, 0, 0 });
    Vector e1 = new Vector(new double[] { 1, 0, 0 });
    Vector e2 = new Vector(new double[] { 0, 1, 0 });
    Vector e3 = new Vector(new double[] { 0, 0, 1 });

    AffineBasis affineBasis = AffineBasis.FromVectors(origin, new Vector[] { e1, e2 });
    HyperPlane plane = new HyperPlane(affineBasis);

    List<Vector> swarm = new() {
      Vector.LinearCombination(e1, 3, e2, 5),
      Vector.LinearCombination(e1, -3, e2, 5),
      Vector.LinearCombination(e1, -3, e2, -5),
      Vector.LinearCombination(e1, 3, e2, -5),
      Vector.LinearCombination(e1, 3, e3, 5),
      Vector.LinearCombination(e1, -3, e3, 5),
      Vector.LinearCombination(e1, -3, e3, -5),
      Vector.LinearCombination(e1, 3, e3, -5),
      Vector.LinearCombination(e1, 3, e3, 4)
    };

    Assert.That(plane.AllAtOneSide(swarm).Item1, Is.False);

    IEnumerable<Vector> inPlane = plane.FilterIn(swarm);
    IEnumerable<Vector> notInPlane = plane.FilterNotIn(swarm);

    Assert.That(plane.AllAtOneSide(inPlane), Is.EqualTo((true, 0)));
    Assert.That(inPlane.Count(), Is.EqualTo(4));
    Assert.That(notInPlane.Count(), Is.EqualTo(5));
  }

}
