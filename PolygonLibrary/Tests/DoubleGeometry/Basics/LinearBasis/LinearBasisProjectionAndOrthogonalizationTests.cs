using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.LinearBasisAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class LinearBasisProjectionAndOrthogonalizationTests {

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_Simple() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector v = V(3, 4, 5);
    Vector projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    AreEqual(projected, V(3, 4, 0));
  }

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_VectorInSubspace() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector v = V(3, 4, 0);
    Vector projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    AreEqual(projected, v, "Projection of vector already in subspace should be the vector itself.");
  }

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_VectorOrthogonalToSubspace() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector v = V(0, 0, 5);
    Vector projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    AreEqual(projected, V(0, 0, 0), "Projection of vector orthogonal to subspace should be zero vector.");
  }

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_FullDimBasis() {
    LinearBasis basis = new LinearBasis(3);
    Vector v = V(3, 4, 5);
    Vector projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    AreEqual(projected, v, "Projection onto full dimension basis should be the vector itself.");
  }

  [Test]
  public void Contains_VectorInSubspace() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector vIn = V(5, -2, 0);
    Vector vOut = V(1, 1, 1);

    Assert.That(basis.Contains(vIn), Is.True);
    Assert.That(basis.Contains(vOut), Is.False);
  }

  [Test]
  public void Contains_FullDimBasis() {
    LinearBasis basis = new LinearBasis(3);
    Assert.That(basis.Contains(V(1, 2, 3)), Is.True, "Full dimension basis should contain any vector.");
  }

  [Test]
  public void Contains_EmptyBasis_HandlesCorrectly() {
    LinearBasis basis = new LinearBasis(3, 0);
    Vector zeroVec = Vector.Zero(3);
    Vector nonZeroVec = V(1, 2, 3);

    Assert.That(basis.Contains(zeroVec), Is.True, "Empty basis should contain the zero vector.");
    Assert.That(basis.Contains(nonZeroVec), Is.False, "Empty basis should not contain non-zero vectors.");
  }

  [Test]
  public void ProjectVectorToSubSpace_Simple() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector v = V(3, 4, 5);
    Vector projectedCoords = basis.ProjectVectorToSubSpace(v);

    AreEqual(projectedCoords, V(3, 4));
    Assert.That(projectedCoords.SpaceDim, Is.EqualTo(basis.SubSpaceDim));
  }

  [Test]
  public void ProjectVectorToSubSpace_NonStandardBasis() {
    Vector b1 = V(1, 1, 0).Normalize();
    Vector b2 = V(0, 0, 1);
    LinearBasis basis = new LinearBasis(b1, b2);

    Vector v = V(2, 2, 3);
    Vector projectedCoords = basis.ProjectVectorToSubSpace(v);
    Vector expectedCoords = V(v * b1, v * b2);

    Assert.That(projectedCoords.SpaceDim, Is.EqualTo(2));
    Assert.That(projectedCoords[0], Is.EqualTo(expectedCoords[0]).Within(Tools.Eps));
    Assert.That(projectedCoords[1], Is.EqualTo(expectedCoords[1]).Within(Tools.Eps));
  }

  [Test]
  public void ProjectVectorsToSubSpace_Multiple() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    List<Vector> vectors = new() { V(1, 2, 3), V(4, 5, 6), V(0, 0, 1) };
    List<Vector> projected = basis.ProjectVectorsToSubSpace(vectors).ToList();

    Assert.That(projected.Count, Is.EqualTo(3));
    AreEqual(projected[0], V(1, 2));
    AreEqual(projected[1], V(4, 5));
    AreEqual(projected[2], V(0, 0));
  }

  [Test]
  public void Orthonormalize_VectorOrthogonal() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0));
    Assert.Throws<NotImplementedException>(() => basis.Orthonormalize(V(0, 5, 0)));
  }

  [Test]
  public void Orthonormalize_VectorInSubspace() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Assert.Throws<NotImplementedException>(() => basis.Orthonormalize(V(3, 4, 0)));
  }

  [Test]
  public void Orthonormalize_GeneralVector() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0));
    Assert.Throws<NotImplementedException>(() => basis.Orthonormalize(V(3, 4, 0)));
  }

  [Test]
  public void Orthonormalize_AgainstEmptyBasis() {
    LinearBasis basis = new LinearBasis(3, 0);
    Assert.Throws<NotImplementedException>(() => basis.Orthonormalize(V(3, 4, 0)));
  }

  [Test]
  public void Orthonormalize_AgainstFullBasis() {
    LinearBasis basis = new LinearBasis(3);
    Assert.Throws<NotImplementedException>(() => basis.Orthonormalize(V(1, 2, 3)));
  }

  [Test]
  public void FindOrthogonalComplement_PartialBasis() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0, 0), V(0, 1, 0, 0));
    LinearBasis complement = basis.OrthogonalComplement();

    Assert.That(complement, Is.Not.Null);
    Assert.That(complement.SpaceDim, Is.EqualTo(4));
    Assert.That(complement.SubSpaceDim, Is.EqualTo(2));
    IsOrthonormal(complement);

    Assert.That(complement.Contains(V(0, 0, 1, 0)), Is.True);
    Assert.That(complement.Contains(V(0, 0, 0, 1)), Is.True);
    Assert.That(complement.Contains(V(1, 0, 0, 0)), Is.False);
  }

  [Test]
  public void FindOrthogonalComplement_FullBasis() {
    LinearBasis basis = new LinearBasis(3);
    LinearBasis complement = basis.OrthogonalComplement();

    Assert.That(complement.Empty, Is.True, "Orthogonal complement of a full basis should be an empty basis.");
    Assert.That(complement.SubSpaceDim, Is.EqualTo(0));
    Assert.That(complement.SpaceDim, Is.EqualTo(3));
  }

  [Test]
  public void FindOrthogonalComplement_EmptyBasis() {
    LinearBasis basis = new LinearBasis(4, 0);
    LinearBasis complement = basis.OrthogonalComplement();
    LinearBasis expectedFull = new LinearBasis(4);

    Assert.That(complement.SpaceDim, Is.EqualTo(4));
    Assert.That(complement.SubSpaceDim, Is.EqualTo(4));
    Assert.That(complement.FullDim, Is.True);
    Assert.That(complement, Is.EqualTo(expectedFull));
  }

  [Test]
  public void FindOrthonormalVector_PartialBasis() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0, 0));
    Vector ortho = basis.OrthonormalVector();

    Assert.That(ortho.IsZero, Is.False);
    Assert.That(ortho.Length, Is.EqualTo(1.0).Within(Tools.Eps));
    Assert.That(ortho * basis[0], Is.EqualTo(0.0).Within(Tools.Eps), "Found vector should be orthogonal to basis");
  }

  [Test]
  public void FindOrthonormalVector_FullBasis_ReturnsZero() {
    LinearBasis basis = new LinearBasis(3);
    Vector ortho = basis.OrthonormalVector();
    Assert.That(ortho.IsZero, Is.True, "OrthonormalVector for a full basis should return a zero vector.");
  }

  [Test]
  public void OrthonormalizeRND() {
    GRandomLC rnd = new GRandomLC(1234);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);
    Vector v = Vector.GenVector(5, rnd);

    Assert.Throws<NotImplementedException>(() => basis.Orthonormalize(v));
  }

  [Test]
  public void FindOrthogonalComplement() {
    GRandomLC rnd = new GRandomLC(3456);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 2, rnd);
    LinearBasis complement = basis.OrthogonalComplement();

    LinearBasis expected = new LinearBasis(
      V(0.49828681673115194, 0.5043344719202018, 0.6554103491752181, 0.01653950264222742, 0.25984747016523546),
      V(-0.516489212458386, 0.1974367874017911, 0.23895366498487525, 0.796883632509154, -0.046208555743365354),
      V(-0.03138801268197455, -0.5352593332246187, 0.10435700671977013, 0.12897224902012222, 0.8276400262112715)
    );

    Assert.That(complement.SpanSameSpace(expected), Is.True);
  }

  [Test]
  public void ProjectVectorToSubspace() {
    GRandomLC rnd = new GRandomLC(4567);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);
    Vector v = Vector.GenVector(5, rnd);
    Vector expected = V(0.14525705641968203, -0.2535536397279014, -0.036500911569481743);

    Vector projected = basis.ProjectVectorToSubSpace(v);
    Assert.That(projected, Is.EqualTo(expected), "Proj vector to some subspace.");
  }

  [Test]
  public void ProjectVectorToSS_inOrig() {
    GRandomLC rnd = new GRandomLC(4567);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);
    Vector v = Vector.GenVector(5, rnd);
    Vector expected = V(-0.09590126543490077, -0.1165857398613803, -0.014731846409157014, -0.07624884049551936, -0.240626633558726);

    Vector projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);
    Assert.That(projected, Is.EqualTo(expected), "Proj vector to some subspace.");
  }

}
