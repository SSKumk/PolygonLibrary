using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.LinearBasisAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class LinearBasisMutationTests {

  [Test]
  public void AddVector_ToEmpty_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(3, 0);
    bool added = basis.AddVector(V(5, 0, 0));

    Assert.That(added, Is.True);
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SpanSameSpace(new LinearBasis(3, 1)));
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVector_Independent_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0));
    bool added = basis.AddVector(V(0, 2, 0));

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.SpanSameSpace(new LinearBasis(3, 2)));
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVector_IndependentNonOrthogonal_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0));
    bool added = basis.AddVector(V(1, 2, 0));

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.SpanSameSpace(new LinearBasis(3, 2)));
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVector_Dependent_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0), V(0, 1, 0));
    bool added = basis.AddVector(V(3, 4, 0));

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVector_Zero_IsIgnored() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0));
    bool added = basis.AddVector(Vector.Zero(3));

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVector_ToFullBasis_IsIgnored() {
    LinearBasisMutable basis = new LinearBasisMutable(2);
    bool added = basis.AddVector(V(1, 1));

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.FullDim, Is.True);
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVectors_AddsMultiple() {
    LinearBasisMutable basis = new LinearBasisMutable(4, 0);
    List<Vector> vectors = new() {
      V(1, 0, 0, 0),
      V(1, 1, 0, 0),
      V(0, 0, 1, 0),
      V(0, 0, 0, 5)
    };

    basis.AddVectors(vectors);

    Assert.That(basis.SubSpaceDim, Is.EqualTo(4), "Should find 4 LI vectors from the set.");
    Assert.That(basis.FullDim, Is.True);
    IsOrthonormal(basis);
  }

  [Test]
  public void AddVectorRND() {
    GRandomLC rnd = new GRandomLC(2345);
    LinearBasisMutable basis = LinearBasisMutable.GenLinearBasis(5, 3, rnd);
    Vector v = Vector.GenVector(5, rnd);

    basis.AddVector(v);

    List<Vector> basisList = new() {
      V(-0.5469069029145192, 0.7375266873774018, -0.06093629951375099, -0.04428435868483195, -0.3889381543741992),
      V(-0.5437514327000299, -0.5605588954339749, -0.5887103066895489, -0.06269356730321592, -0.19899194858787822),
      V(0.35056986554378083, 0.14703950888480366, -0.46820894124701984, 0.7644356160825109, -0.22781292412948903)
    };

    List<Vector> exp1 = new(basisList);
    List<Vector> exp2 = new(basisList);
    Vector ov = V(-0.00709854200521387, -0.3363850420894928, 0.5556769114749364, 0.18883245131301185, -0.7364510774957459);
    exp1.Add(ov);
    exp2.Add(-ov);

    Assert.That(basis.SequenceEqual(exp1) || basis.SequenceEqual(exp2), "Add vector to the basis");
  }

}
