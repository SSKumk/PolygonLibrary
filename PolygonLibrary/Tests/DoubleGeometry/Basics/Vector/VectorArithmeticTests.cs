using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class VectorArithmeticTests {

  [Test]
  public void Operator_UnaryMinus() {
    Vector v = V(1, -2, 0);
    Vector negV = -v;
    AreEqual(negV, V(-1, 2, 0));
    AreEqual(-Vector.Zero(3), Vector.Zero(3));
  }

  [Test]
  public void Operator_AdditionAndSubtraction() {
    Vector v1 = V(1, 2, 3);
    Vector v2 = V(4, -1, 0);

    AreEqual(v1 + v2, V(5, 1, 3));
    AreEqual(v1 - v2, V(-3, 3, 3));
    AreEqual(v1 + Vector.Zero(3), v1);
    AreEqual(Vector.Zero(3) + v1, v1);
  }

  [Test]
  public void Operator_ScalarMultiplicationAndDivision() {
    Vector v = V(1, -2, 3);
    double scalar = 2.0;

    AreEqual(scalar * v, V(2, -4, 6));
    AreEqual(v * scalar, V(2, -4, 6));
    AreEqual(0.0 * v, Vector.Zero(3));
    AreEqual(1.0 * v, v);
    AreEqual(-1.0 * v, -v);
    AreEqual(V(2, -4, 6) / scalar, V(1, -2, 3));
  }

  [Test]
  public void Operator_DotProduct() {
    Vector v1 = V(1, 2, 3);
    Vector v2 = V(4, -1, 2);
    Vector v3 = V(-2, 1, 0);
    double dot12 = v1 * v2;
    double dot13 = v1 * v3;

    Assert.That(Tools.EQ(dot12, 8.0), Is.True);
    Assert.That(Tools.EQ(dot13, 0.0), Is.True, "Dot product of orthogonal vectors should be zero.");
    Assert.That(Tools.EQ(v1 * Vector.Zero(3), 0.0), Is.True, "Dot product with zero vector should be zero.");
    Assert.That(Tools.EQ(v1 * v1, v1.Length2), Is.True, "Dot product with self should be Length2.");
  }

  [Test]
  public void Sum_AccumulatesSequenceOfVectors() {
    Vector sum = Vector.Sum(new[] { V(1, 0, 0), V(0, 2, 0), V(0, 0, 3) });

    AreEqual(sum, V(1, 2, 3));
  }

  [Test]
  public void LinearCombination_TwoVectors() {
    Vector v1 = V(1, 0);
    Vector v2 = V(0, 1);
    double w1 = 2.0;
    double w2 = 3.0;
    Vector lc = Vector.LinearCombination(v1, w1, v2, w2);
    AreEqual(lc, V(2, 3));
  }

  [Test]
  public void LinearCombination_List() {
    List<Vector> vs = new() { V(1, 0, 0), V(0, 1, 0), V(0, 0, 1) };
    List<double> ws = new() { 1.0, 2.0, 3.0 };
    Vector lc = Vector.LinearCombination(vs, ws);
    AreEqual(lc, V(1, 2, 3));
  }

  [Test]
  public void MulByNumAndAdd_IsEquivalentToScaledVectorPlusAnotherVector() {
    Vector v1 = V(1, 2, 3);
    Vector v2 = V(10, 0, -10);
    double a = 3.0;
    Vector result = Vector.MulByNumAndAdd(v1, a, v2);
    AreEqual(result, V(13, 6, -1));
  }

  [Test]
  public void AffMul_ComputesAffineDotProduct() {
    Vector v1 = V(3, 4, 5);
    Vector origin = V(1, 1, 1);
    Vector v2 = V(1, 0, 2);
    double affMul = Vector.AffMul(v1, origin, v2);
    Assert.That(Tools.EQ(affMul, 10.0), Is.True);
  }

}
