using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class AffineBasisComparisonAndEnumerationTests {

  [Test]
  public void Equals_SameObject() {
    AffineBasis basis = AffineBasis.GenAffineBasis(3, 2);
    Assert.That(basis.Equals(basis), Is.True);
  }

  [Test]
  public void Equals_DifferentObjectsSameSubspace() {
    Vector origin = V(1, 1, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis1 = new AffineBasis(origin, linearBasis);
    AffineBasis basis2 = new AffineBasis(origin, linearBasis);
    AffineBasis basis3 = new AffineBasis(origin, new LinearBasis(linearBasis, false));
    AffineBasis basis4 = new AffineBasis(origin + basis1[0] * 5.0 + basis1[1] * (-3.0), new LinearBasis(linearBasis, false));
    AffineBasis basis5 = new AffineBasis(origin, new LinearBasis(V(0, 1, 0), V(-1, 0, 0)));

    Assert.That(basis1.Equals(basis2));
    Assert.That(basis1.Equals(basis3));
    Assert.That(basis1.Equals(basis4));
    Assert.That(basis1.Equals(basis5));
  }

  [Test]
  public void Equals_DifferentSubspaces() {
    AffineBasis basisXY = new AffineBasis(Vector.Zero(3), new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    AffineBasis basisXZ = new AffineBasis(Vector.Zero(3), new LinearBasis(V(1, 0, 0), V(0, 0, 1)));
    AffineBasis basisX = new AffineBasis(Vector.Zero(3), new LinearBasis(new[] { V(1, 0, 0) }));

    Assert.That(basisXY, Is.Not.EqualTo(basisXZ));
    Assert.That(basisXY, Is.Not.EqualTo(basisX));

    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis shifted1 = new AffineBasis(V(0, 0, 0), linearBasis);
    AffineBasis shifted2 = new AffineBasis(V(0, 0, 1), linearBasis);
    Assert.That(!shifted1.Equals(shifted2));

    AffineBasis basis3D = new AffineBasis(3);
    AffineBasis basis4D = new AffineBasis(4);
    Assert.That(!basis3D.Equals(basis4D));
  }

  [Test]
  public void Equals_NullOrDifferentType() {
    AffineBasis basis = new AffineBasis(3);
    Assert.That(basis, Is.Not.EqualTo(null));
    Assert.That(basis, Is.Not.EqualTo(new object()));
  }

  [Test]
  public void CompareTo_Null_Returns1() {
    AffineBasis basis = new AffineBasis(V(1, 1, 1));
    Assert.That(basis.CompareTo(null), Is.EqualTo(1));
  }

  [Test]
  public void CompareTo_EqualBases_Returns0() {
    AffineBasis basis1 = new AffineBasis(V(0, 0), new LinearBasis(V(1, 1)));
    AffineBasis basis2 = new AffineBasis(V(5, 5), new LinearBasis(V(-1, -1)));

    Assert.That(basis1.CompareTo(basis2), Is.EqualTo(0));
  }

  [Test]
  public void CompareTo_Order_BySubspaceDim() {
    AffineBasis line = new AffineBasis(V(0, 0, 0), new LinearBasis(3, 1));
    AffineBasis plane = new AffineBasis(V(0, 0, 0), new LinearBasis(3, 2));

    Assert.That(line.CompareTo(plane), Is.LessThan(0));
    Assert.That(plane.CompareTo(line), Is.GreaterThan(0));
  }

  [Test]
  public void CompareTo_Order_ByLinearBasis() {
    AffineBasis basisXY = new AffineBasis(V(0, 0, 0), new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    AffineBasis basisXZ = new AffineBasis(V(0, 0, 0), new LinearBasis(V(1, 0, 0), V(0, 0, 1)));

    Assert.That(basisXY.CompareTo(basisXZ), Is.GreaterThan(0));
    Assert.That(basisXZ.CompareTo(basisXY), Is.LessThan(0));
  }

  [Test]
  public void CompareTo_Order_ByCanonicalOrigin() {
    AffineBasis basis1 = new AffineBasis(V(5, 5), new LinearBasis(V(1, 1)));
    AffineBasis basis2 = new AffineBasis(V(2, 3), new LinearBasis(V(1, 1)));

    Assert.That(basis1.CompareTo(basis2), Is.GreaterThan(0));
    Assert.That(basis2.CompareTo(basis1), Is.LessThan(0));
  }

  [Test]
  public void GetHashCode_ThrowsInvalidOperationException() {
    AffineBasis basis = new AffineBasis(3);
    Assert.Throws<InvalidOperationException>(() => basis.GetHashCode());
  }

  [Test]
  public void GetEnumerator_IteratesThroughLinearBasis() {
    AffineBasis basis = new AffineBasis(V(1, 1, 1), new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    List<Vector> vectors = new();

    foreach (Vector v in basis) {
      vectors.Add(v);
    }

    Assert.That(vectors.Count, Is.EqualTo(2));
    AreEqual(vectors[0], basis.LinBasis[0]);
    AreEqual(vectors[1], basis.LinBasis[1]);
  }

}
