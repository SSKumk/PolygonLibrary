using DoubleDouble;
using NUnit.Framework;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests;

[TestFixture]
public class HyperPlaneTests {

  [Test]
  public void ConstructorWithPointAndNormalTest() {
    Vector     origin     = new Vector(new ddouble[] { 0, 0, 0 });
    Vector     normal     = new Vector(new ddouble[] { 1, 1, 1 });
    HyperPlane hyperplane = new HyperPlane(normal, origin);

    AffineBasis.CheckCorrectness(hyperplane.AffBasis);
  }

  [Test]
  public void ConstructorWithAffineBasisTest() {
    List<Vector> vectors = new List<Vector>() { new Vector(new ddouble[] { 1, 0, 0 }), new Vector(new ddouble[] { 0, 1, 0 }) };

    AffineBasis affineBasis = AffineBasis.FromVectors(new Vector(new ddouble[] { 1, 1, 1 }), vectors);
    HyperPlane  hyperplane  = new HyperPlane(affineBasis);

    Assert.That
      (
       hyperplane.AffBasis.LinBasis.Equals(new LinearBasis(vectors, false))
     , "ConstructorWithAffineBasisTest: The linear basis must be the same!"
      );
    AffineBasis.CheckCorrectness(hyperplane.AffBasis);
  }

  [Test]
  public void ContainsTest() {
    Vector origin = new Vector(new ddouble[] { 0, 0, 0 });
    Vector v1     = new Vector(new ddouble[] { 1, 1, 1 });
    Vector v2     = new Vector(new ddouble[] { 1, -1, 1 });
    Vector v3     = new Vector(new ddouble[] { 0, 0, 1 });

    AffineBasis aBasis = AffineBasis.FromVectors(origin, new List<Vector>() { v1, v2 });

    HyperPlane hp = new HyperPlane(aBasis);

    Vector p1 = Vector.LinearCombination(v1, 3, v2, 5);
    Vector p2 = Vector.LinearCombination(v1, -3, v2, 5);
    Vector p3 = Vector.LinearCombination(v1, -3, v2, -5);
    Vector p4 = Vector.LinearCombination(v1, 3, v2, -5);

    Assert.That(hp.Contains(p1), Is.True);
    Assert.That(hp.Contains(p2), Is.True);
    Assert.That(hp.Contains(p3), Is.True);
    Assert.That(hp.Contains(p4), Is.True);

    Assert.That(hp.Contains(p1 + (v3 - origin)), Is.False);
    Assert.That(hp.Contains(p1 - (v3 - origin)), Is.False);
  }

  [Test]
  public void TestFilter() {
    Vector origin = new Vector(new ddouble[] { 0, 0, 0 });
    Vector e1     = new Vector(new ddouble[] { 1, 0, 0 });
    Vector e2     = new Vector(new ddouble[] { 0, 1, 0 });
    Vector e3     = new Vector(new ddouble[] { 0, 0, 1 });

    AffineBasis aBasis = AffineBasis.FromVectors(origin, new Vector[] { e1, e2 });

    HyperPlane hp = new HyperPlane(aBasis);

    List<Vector> Swarm = new List<Vector>()
      {
        Vector.LinearCombination(e1, 3, e2, 5)
      , Vector.LinearCombination(e1, -3, e2, 5)
      , Vector.LinearCombination(e1, -3, e2, -5)
      , Vector.LinearCombination(e1, 3, e2, -5)
      , Vector.LinearCombination(e1, 3, e3, 5)
      , Vector.LinearCombination(e1, -3, e3, 5)
      , Vector.LinearCombination(e1, -3, e3, -5)
      , Vector.LinearCombination(e1, 3, e3, -5)
      , Vector.LinearCombination(e1, 3, e3, 4)
      };

    Assert.That(hp.AllAtOneSide(Swarm).Item1, Is.False);

    IEnumerable<Vector> inPlane    = hp.FilterIn(Swarm);
    IEnumerable<Vector> notInPlane = hp.FilterNotIn(Swarm);

    Assert.That(hp.AllAtOneSide(inPlane), Is.EqualTo((true, 0)));
    Assert.That(inPlane.Count(), Is.EqualTo(4));
    Assert.That(notInPlane.Count(), Is.EqualTo(5));
  }

}
