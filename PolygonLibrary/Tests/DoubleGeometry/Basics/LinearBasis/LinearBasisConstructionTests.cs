using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.LinearBasisAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class LinearBasisConstructionTests {

  [Test]
  public void Constructor_Empty() {
    LinearBasis basis = new LinearBasis(3, 0);
    Assert.That(basis.Empty, Is.True);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(0));
    Assert.That(basis.FullDim, Is.False);
    Assert.Throws<ArgumentException>(
      () => {
        var b = basis.Basis;
      },
      "Accessing Basis property of empty basis should throw."
    );
    Assert.Throws<ArgumentException>(
      () => {
        var p = basis.ProjMatrix;
      },
      "Accessing ProjMatrix property of empty basis should throw."
    );
  }

  [Test]
  public void Constructor_SingleVector() {
    Vector v = V(3, 4, 0);
    LinearBasis basis = new LinearBasis(v);

    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    Assert.That(basis.FullDim, Is.False);
    AreEqual(basis[0], V(0.6, 0.8, 0), "Single vector constructor should normalize.");
    IsOrthonormal(basis);
  }

  [Test]
  public void Constructor_SingleZeroVector_Throws() {
    Assert.Throws<ArgumentException>(() => new LinearBasis(Vector.Zero(3)), "Constructor should always throw ArgumentException for a zero vector.");
  }

  [Test]
  public void Constructor_StandardBasis() {
    LinearBasis basis = new LinearBasis(3);
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    Assert.That(basis.FullDim, Is.True);
    AreEqual(basis[0], V(1, 0, 0));
    AreEqual(basis[1], V(0, 1, 0));
    AreEqual(basis[2], V(0, 0, 1));
    IsOrthonormal(basis);
  }

  [Test]
  public void Constructor_PartialStandardBasis() {
    LinearBasis basis = new LinearBasis(4, 2);
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SpaceDim, Is.EqualTo(4));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.FullDim, Is.False);
    AreEqual(basis[0], V(1, 0, 0, 0));
    AreEqual(basis[1], V(0, 1, 0, 0));
    IsOrthonormal(basis);
  }

  [Test]
  public void Constructor_WithVectors_Orthogonalize() {
    List<Vector> vectors = new() {
      V(2, 0, 0),
      V(1, 3, 0),
      V(0, 0, 4),
      V(-1, 1, 5.7412)
    };

    LinearBasis basis = new LinearBasis(vectors);

    Assert.That(basis.SpaceDim, Is.EqualTo(3), "SpaceDim should be determined from vectors.");
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3), "Should find 3 linearly independent vectors.");
    Assert.That(basis.FullDim, Is.True);
    Assert.That(basis.Empty, Is.False);
    IsOrthonormal(basis);

    Assert.That(basis.Contains(V(1, 0, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 1, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 0, 1)), Is.True);
  }

  [Test]
  public void Constructor_Copy() {
    LinearBasisMutable basis1 = new LinearBasisMutable(V(1, 2, 0), V(0, 0, 3));
    LinearBasis basis2 = new LinearBasis(basis1, true);

    Assert.That(basis2.SpaceDim, Is.EqualTo(basis1.SpaceDim));
    Assert.That(basis2.SubSpaceDim, Is.EqualTo(basis1.SubSpaceDim));
    Assert.That(basis2.FullDim, Is.EqualTo(basis1.FullDim));
    Assert.That(basis2.Empty, Is.EqualTo(basis1.Empty));
    Assert.That(basis1, Is.EqualTo(basis2));

    basis1.AddVector(V(2, -1, 0));
    Assert.That(basis2.SubSpaceDim, Is.EqualTo(2), "Copy should not be affected by changes to original.");
  }

  [Test]
  public void Constructor_Merge() {
    LinearBasis lb1 = new LinearBasis(V(1, 0, 0, 0));
    LinearBasis lb2 = new LinearBasis(V(0, 1, 0, 0));
    LinearBasis lb3 = new LinearBasis(V(0, 0, 1, 0));
    LinearBasis lb12 = new LinearBasis(lb1, lb2);
    LinearBasis lb11 = new LinearBasis(lb1, lb1);
    LinearBasis lb123 = new LinearBasis(lb12, lb3);
    LinearBasis lbEmpty = new LinearBasis(4, 0);
    LinearBasis lbEmpty1 = new LinearBasis(lbEmpty, lb1);

    Assert.That(lb12.SubSpaceDim, Is.EqualTo(2));
    IsOrthonormal(lb12);
    Assert.That(lb12.Contains(V(1, 0, 0, 0)), Is.True);
    Assert.That(lb12.Contains(V(0, 1, 0, 0)), Is.True);

    Assert.That(lb11.SubSpaceDim, Is.EqualTo(1));
    IsOrthonormal(lb11);
    Assert.That(lb11, Is.EqualTo(lb1));

    Assert.That(lb123.SubSpaceDim, Is.EqualTo(3));
    IsOrthonormal(lb123);
    Assert.That(lb123.Contains(V(1, 0, 0, 0)), Is.True);
    Assert.That(lb123.Contains(V(0, 1, 0, 0)), Is.True);
    Assert.That(lb123.Contains(V(0, 0, 1, 0)), Is.True);

    Assert.That(lbEmpty1.SubSpaceDim, Is.EqualTo(1));
    IsOrthonormal(lbEmpty1);
    Assert.That(lbEmpty1, Is.EqualTo(lb1));

    Assert.That(new LinearBasis(lb1, lbEmpty), Is.EqualTo(lb1));
    Assert.That(new LinearBasis(lbEmpty, lbEmpty).Empty);
  }

  [Test]
  public void Constructor_WithVectors_NoOrthogonalize_ValidInput() {
    List<Vector> vectors = new() { V(1, 0, 0), V(0, 1, 0), V(0, 0, 1) };

    LinearBasis basis = null!;
    Assert.DoesNotThrow(() => basis = new LinearBasis(vectors));

    Assert.That(basis, Is.Not.Null);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    AreEqual(basis[0], V(1, 0, 0));
    AreEqual(basis[1], V(0, 1, 0));
    AreEqual(basis[2], V(0, 0, 1));
    IsOrthonormal(basis);
  }

}
