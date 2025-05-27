using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using static Tests.SharedTests.StaticHelpers;

namespace Tests.SharedTests;

[TestFixture]
public class LinearBasisTests {

  // Helper to check orthonormality (useful for debugging tests)
  private void AssertBasisOrthonormal(LinearBasis lb) {
    if (lb.Empty)
      return;

    for (int i = 0; i < lb.SubSpaceDim; i++) {
      Assert.That(lb[i].Length, Is.EqualTo(1.0).Within(Tools.Eps), $"Basis vector {i} is not normalized.");
      for (int j = i + 1; j < lb.SubSpaceDim; j++) {
        Assert.That(lb[i] * lb[j], Is.EqualTo(0.0).Within(Tools.Eps), $"Basis vectors {i} and {j} are not orthogonal.");
      }
    }

#if DEBUG
    Assert.DoesNotThrow(lb.CheckCorrectness, "CheckCorrectness failed.");
#endif
  }

#region Constructor Tests
  [Test]
  public void Constructor_Empty() {
    LinearBasis basis = new LinearBasis(3, 0);
    Assert.That(basis.Empty, Is.True);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(0));
    Assert.That(basis.FullDim, Is.False);
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var b = basis.Basis;
       }
     , "Accessing Basis property of empty basis should throw."
      );
    Assert.Throws<ArgumentException>
      (
       ()
         => {
         var p = basis.ProjMatrix;
       }
     , "Accessing ProjMatrix property of empty basis should throw."
      );
  }

  [Test]
  public void Constructor_SingleVector() {
    Vector      v     = V(3, 4, 0);
    LinearBasis basis = new LinearBasis(v);

    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    Assert.That(basis.FullDim, Is.False);
    AssertVectorsAreEqual(V(0.6, 0.8, 0), basis[0], "Single vector constructor should normalize.");
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_SingleZeroVector_Throws() {
    Assert.Throws<ArgumentException>
      (() => new LinearBasis(Vector.Zero(3)), "Constructor should always throw ArgumentException for a zero vector.");
  }

  [Test]
  public void Constructor_StandardBasis() {
    LinearBasis basis = new LinearBasis(3); // Should create standard 3D basis
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    Assert.That(basis.FullDim, Is.True);
    AssertVectorsAreEqual(V(1, 0, 0), basis[0]);
    AssertVectorsAreEqual(V(0, 1, 0), basis[1]);
    AssertVectorsAreEqual(V(0, 0, 1), basis[2]);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_PartialStandardBasis() {
    LinearBasis basis = new LinearBasis(4, 2); // Should create e1, e2 in 4D
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SpaceDim, Is.EqualTo(4));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.FullDim, Is.False);
    AssertVectorsAreEqual(V(1, 0, 0, 0), basis[0]);
    AssertVectorsAreEqual(V(0, 1, 0, 0), basis[1]);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_WithVectors_Orthogonalize() {
    List<Vector> vectors =
      new List<Vector>()
        {
          V(2, 0, 0)
        , V(1, 3, 0)
        , V(0, 0, 4)
        , V(-1, 1, 5.7412)
        };

    LinearBasis basis = new LinearBasis(vectors);

    Assert.That(basis.SpaceDim, Is.EqualTo(3), "SpaceDim should be determined from vectors.");
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3), "Should find 3 linearly independent vectors.");
    Assert.That(basis.FullDim, Is.True);
    Assert.That(basis.Empty, Is.False);
    AssertBasisOrthonormal(basis);

    // Check if the space spanned is correct (e.g., contains original independent vectors)
    Assert.That(basis.Contains(V(1, 0, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 1, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 0, 1)), Is.True);
  }

  [Test]
  public void Constructor_Copy() {
    LinearBasisMutable basis1 = new LinearBasisMutable(new[] { V(1, 2, 0), V(0, 0, 3) });
    LinearBasis        basis2 = new LinearBasis(basis1, true);

    Assert.That(basis2.SpaceDim, Is.EqualTo(basis1.SpaceDim));
    Assert.That(basis2.SubSpaceDim, Is.EqualTo(basis1.SubSpaceDim));
    Assert.That(basis2.FullDim, Is.EqualTo(basis1.FullDim));
    Assert.That(basis2.Empty, Is.EqualTo(basis1.Empty));
    Assert.That(basis1.Equals(basis2), Is.True);

    // Ensure it's a deep copy of the matrix data (modify basis1, basis2 shouldn't change)
    basis1.AddVector(V(2, -1, 0)); // Add orthogonal vector
    Assert.That(basis2.SubSpaceDim, Is.EqualTo(2), "Copy should not be affected by changes to original.");
  }

  [Test]
  public void Constructor_Merge() {
    LinearBasis lb1        = new LinearBasis(V(1, 0, 0, 0));
    LinearBasis lb2        = new LinearBasis(V(0, 1, 0, 0));
    LinearBasis lb3        = new LinearBasis(V(0, 0, 1, 0));
    LinearBasis lb_12      = new LinearBasis(lb1, lb2);
    LinearBasis lb_11      = new LinearBasis(lb1, lb1);
    LinearBasis lb_123     = new LinearBasis(lb_12, lb3);
    LinearBasis lb_empty   = new LinearBasis(4, 0);
    LinearBasis lb_empty_1 = new LinearBasis(lb_empty, lb1);


    Assert.That(lb_12.SubSpaceDim, Is.EqualTo(2));
    AssertBasisOrthonormal(lb_12);
    Assert.That(lb_12.Contains(V(1, 0, 0, 0)), Is.True);
    Assert.That(lb_12.Contains(V(0, 1, 0, 0)), Is.True);

    Assert.That(lb_11.SubSpaceDim, Is.EqualTo(1)); // Adding same vector shouldn't increase dim
    AssertBasisOrthonormal(lb_11);
    Assert.That(lb_11.Equals(lb1));

    Assert.That(lb_123.SubSpaceDim, Is.EqualTo(3));
    AssertBasisOrthonormal(lb_123);
    Assert.That(lb_123.Contains(V(1, 0, 0, 0)), Is.True);
    Assert.That(lb_123.Contains(V(0, 1, 0, 0)), Is.True);
    Assert.That(lb_123.Contains(V(0, 0, 1, 0)), Is.True);

    Assert.That(lb_empty_1.SubSpaceDim, Is.EqualTo(1));
    AssertBasisOrthonormal(lb_empty_1);
    Assert.That(lb_empty_1.Equals(lb1));

    Assert.That(new LinearBasis(lb1, lb_empty).Equals(lb1));
    Assert.That(new LinearBasis(lb_empty, lb_empty).Empty);
  }

  [Test]
  public void Constructor_WithVectors_NoOrthogonalize_ValidInput() {
    List<Vector> vectors = new List<Vector>() { V(1, 0, 0), V(0, 1, 0), V(0, 0, 1) };

    LinearBasis basis = null!;
    Assert.DoesNotThrow(() => basis = new LinearBasis(vectors));

    Assert.That(basis, Is.Not.Null);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(V(1, 0, 0), basis[0]);
    AssertVectorsAreEqual(V(0, 1, 0), basis[1]);
    AssertVectorsAreEqual(V(0, 0, 1), basis[2]);
    AssertBasisOrthonormal(basis);
  }
#endregion

#region AddVector Tests
  [Test]
  public void AddVector_ToEmpty_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(3, 0);
    bool               added = basis.AddVector(V(5, 0, 0));

    Assert.That(added, Is.True);
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SpanSameSpace(new LinearBasis(3, 1)));

    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_Independent_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0));
    bool               added = basis.AddVector(V(0, 2, 0));

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.SpanSameSpace(new LinearBasis(3, 2)));
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_IndependentNonOrthogonal_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0));
    bool               added = basis.AddVector(V(1, 2, 0)); // Non-orthogonal to basis[0]

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.SpanSameSpace(new LinearBasis(3, 2)));

    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_Dependent_Orthogonalize() {
    LinearBasisMutable basis = new LinearBasisMutable(new[] { V(1, 0, 0), V(0, 1, 0) });
    bool               added = basis.AddVector(V(3, 4, 0)); // Lies in the span of the basis

    Assert.That(added, Is.False);                  // Should not add a dependent vector
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2)); // Dimension should not change
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_Zero_IsIgnored() {
    LinearBasisMutable basis = new LinearBasisMutable(V(1, 0, 0));
    bool               added = basis.AddVector(Vector.Zero(3));

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_ToFullBasis_IsIgnored() {
    LinearBasisMutable basis = new LinearBasisMutable(2); // Full 2D basis
    bool               added = basis.AddVector(V(1, 1));

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.FullDim, Is.True);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVectors_AddsMultiple() {
    LinearBasisMutable basis = new LinearBasisMutable(4, 0);
    var vectors =
      new List<Vector>
        {
          V(1, 0, 0, 0)
        , V(1, 1, 0, 0)
        , V(0, 0, 1, 0)
        , V(0, 0, 0, 5)
        };
    basis.AddVectors(vectors);

    Assert.That(basis.SubSpaceDim, Is.EqualTo(4), "Should find 4 LI vectors from the set.");
    Assert.That(basis.FullDim, Is.True);
    AssertBasisOrthonormal(basis);
  }
#endregion

#region Projection and Contains Tests
  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_Simple() {
    LinearBasis basis     = new LinearBasis(V(1, 0, 0), V(0, 1, 0)); // XY plane in 3D
    Vector      v         = V(3, 4, 5);
    Vector      projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);
    Vector      expected  = V(3, 4, 0);

    AssertVectorsAreEqual(expected, projected);
  }

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_VectorInSubspace() {
    LinearBasis basis     = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector      v         = V(3, 4, 0); // Already in subspace
    Vector      projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    AssertVectorsAreEqual(v, projected, "Projection of vector already in subspace should be the vector itself.");
  }

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_VectorOrthogonalToSubspace() {
    LinearBasis basis     = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector      v         = V(0, 0, 5); // Orthogonal to subspace
    Vector      projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);
    Vector      expected  = V(0, 0, 0);

    AssertVectorsAreEqual(expected, projected, "Projection of vector orthogonal to subspace should be zero vector.");
  }

  [Test]
  public void ProjectVectorToSubSpace_in_OrigSpace_FullDimBasis() {
    LinearBasis basis     = new LinearBasis(3); // Full 3D basis
    Vector      v         = V(3, 4, 5);
    Vector      projected = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    AssertVectorsAreEqual(v, projected, "Projection onto full dimension basis should be the vector itself.");
  }

  [Test]
  public void Contains_VectorInSubspace() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0)); // XY plane
    Vector      vIn   = V(5, -2, 0);
    Vector      vOut  = V(1, 1, 1);

    Assert.That(basis.Contains(vIn), Is.True);
    Assert.That(basis.Contains(vOut), Is.False);
  }

  [Test]
  public void Contains_FullDimBasis() {
    LinearBasis basis = new LinearBasis(3); // Full 3D basis
    Vector      v     = V(1, 2, 3);
    Assert.That(basis.Contains(v), Is.True, "Full dimension basis should contain any vector.");
  }

  [Test]
  public void Contains_EmptyBasis_HandlesCorrectly() {
    LinearBasis basis      = new LinearBasis(3, 0);
    Vector      zeroVec    = Vector.Zero(3);
    Vector      nonZeroVec = V(1, 2, 3);

    Assert.That(basis.Contains(zeroVec), Is.True, "Empty basis should contain the zero vector.");
    Assert.That(basis.Contains(nonZeroVec), Is.False, "Empty basis should not contain non-zero vectors.");
  }


  [Test]
  public void ProjectVectorToSubSpace_Simple() {
    LinearBasis basis           = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector      v               = V(3, 4, 5);
    Vector      projectedCoords = basis.ProjectVectorToSubSpace(v); // Should be (v . b1, v . b2)
    Vector      expectedCoords  = V(3, 4);

    AssertVectorsAreEqual(expectedCoords, projectedCoords);
    Assert.That(projectedCoords.SpaceDim, Is.EqualTo(basis.SubSpaceDim));
  }

  [Test]
  public void ProjectVectorToSubSpace_NonStandardBasis() {
    Vector      b1    = V(1, 1, 0).Normalize();
    Vector      b2    = V(0, 0, 1);
    LinearBasis basis = new LinearBasis(b1, b2);

    Vector v               = V(2, 2, 3);
    Vector projectedCoords = basis.ProjectVectorToSubSpace(v);
    Vector expectedCoords  = V(v * b1, v * b2);

    Assert.That(projectedCoords.SpaceDim, Is.EqualTo(2));
    Assert.That(projectedCoords[0], Is.EqualTo(expectedCoords[0]).Within(Tools.Eps));
    Assert.That(projectedCoords[1], Is.EqualTo(expectedCoords[1]).Within(Tools.Eps));
  }

  [Test]
  public void ProjectVectorsToSubSpace_Multiple() {
    LinearBasis basis     = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    var         vectors   = new List<Vector> { V(1, 2, 3), V(4, 5, 6), V(0, 0, 1) };
    var         projected = basis.ProjectVectorsToSubSpace(vectors).ToList();

    Assert.That(projected.Count, Is.EqualTo(3));
    AssertVectorsAreEqual(V(1, 2), projected[0]);
    AssertVectorsAreEqual(V(4, 5), projected[1]);
    AssertVectorsAreEqual(V(0, 0), projected[2]);
  }
#endregion

#region Orthogonal Complement and Orthonormalization Tests
  [Test]
  public void Orthonormalize_VectorOrthogonal() {
    LinearBasis basis    = new LinearBasis(V(1, 0, 0));
    Vector      v        = V(0, 5, 0);
    Vector      ortho    = basis.Orthonormalize(v);
    Vector      expected = V(0, 1, 0);

    AssertVectorsAreEqual(expected, ortho);
  }

  [Test]
  public void Orthonormalize_VectorInSubspace() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    Vector      v     = V(3, 4, 0);
    Vector      ortho = basis.Orthonormalize(v);

    Assert.That(ortho.IsZero, Is.True, "Orthonormalizing vector in subspace should yield zero vector.");
  }

  [Test]
  public void Orthonormalize_GeneralVector() {
    LinearBasis basis    = new LinearBasis(V(1, 0, 0));
    Vector      v        = V(3, 4, 0);
    Vector      ortho    = basis.Orthonormalize(v);
    Vector      expected = V(0, 1, 0);

    AssertVectorsAreEqual(expected, ortho);
  }

  [Test]
  public void Orthonormalize_AgainstEmptyBasis() {
    LinearBasis basis    = new LinearBasis(3, 0);
    Vector      v        = V(3, 4, 0);
    Vector      ortho    = basis.Orthonormalize(v);
    Vector      expected = V(0.6, 0.8, 0);

    AssertVectorsAreEqual(expected, ortho);
  }

  [Test]
  public void Orthonormalize_AgainstFullBasis() {
    LinearBasis basis = new LinearBasis(3); // Full 3D basis
    Vector      v     = V(1, 2, 3);
    Vector      ortho = basis.Orthonormalize(v);

    Assert.That(ortho.IsZero, Is.True, "Orthonormalizing against full basis should yield zero vector.");
  }


  [Test]
  public void FindOrthogonalComplement_PartialBasis() {
    LinearBasis basis      = new LinearBasis(V(1, 0, 0, 0), V(0, 1, 0, 0)); // Span e1, e2 in 4D
    LinearBasis complement = basis.OrthogonalComplement();

    Assert.That(complement, Is.Not.Null);
    Assert.That(complement.SpaceDim, Is.EqualTo(4));
    Assert.That(complement.SubSpaceDim, Is.EqualTo(2));
    AssertBasisOrthonormal(complement);

    // Check that the complement spans the expected space (e.g., contains e3, e4)
    Assert.That(complement.Contains(V(0, 0, 1, 0)), Is.True);
    Assert.That(complement.Contains(V(0, 0, 0, 1)), Is.True);
    Assert.That(complement.Contains(V(1, 0, 0, 0)), Is.False);
  }

  [Test]
  public void FindOrthogonalComplement_FullBasis() {
    LinearBasis basis      = new LinearBasis(3); // Full 3D basis
    LinearBasis complement = basis.OrthogonalComplement();

    Assert.That(complement.Empty, Is.True, "Orthogonal complement of a full basis should be an empty basis.");
    Assert.That(complement.SubSpaceDim, Is.EqualTo(0));
    Assert.That(complement.SpaceDim, Is.EqualTo(3));
  }

  [Test]
  public void FindOrthogonalComplement_EmptyBasis() {
    LinearBasis basis        = new LinearBasis(4, 0); // Empty basis in 4D
    LinearBasis complement   = basis.OrthogonalComplement();
    LinearBasis expectedFull = new LinearBasis(4); // Expect full basis as complement

    Assert.That(complement.SpaceDim, Is.EqualTo(4));
    Assert.That(complement.SubSpaceDim, Is.EqualTo(4));
    Assert.That(complement.FullDim, Is.True);
    Assert.That(complement, Is.EqualTo(expectedFull));
  }

  [Test]
  public void FindOrthonormalVector_PartialBasis() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0, 0)); // Span e1 in 4D
    Vector      ortho = basis.OrthonormalVector();

    Assert.That(ortho.IsZero, Is.False);
    Assert.That(ortho.Length, Is.EqualTo(1.0).Within(Tools.Eps));
    Assert.That(ortho * basis[0], Is.EqualTo(0.0).Within(Tools.Eps), "Found vector should be orthogonal to basis");
  }

  [Test]
  public void FindOrthonormalVector_FullBasis_ReturnsZero() {
    LinearBasis basis = new LinearBasis(3); // Full 3D basis
    Vector      ortho = basis.OrthonormalVector();
    Assert.That(ortho.IsZero, Is.True, "OrthonormalVector for a full basis should return a zero vector.");
  }
#endregion

#region Equality and Enumeration Tests
  [Test]
  public void Equals_SameBasisObject() {
    LinearBasis basis = new LinearBasis(V(1, 0), V(0, 1));
    Assert.That(basis.Equals(basis), Is.True);
  }

  [Test]
  public void Equals_DifferentObjectsSameSpace() {
    LinearBasis basis1 = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis basis2 = new LinearBasis(V(0, 1, 0), V(1, 0, 0)); // Same vectors, different order
    LinearBasis basis3 =
      new LinearBasis
        (
         V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0)
        , V(1 / Math.Sqrt(2), -1 / Math.Sqrt(2), 0)
        ); // Different vectors, same plane

    Assert.That(basis1.Equals(basis2), Is.True);
    Assert.That(basis1.Equals(basis3), Is.True);
    Assert.That(basis2.Equals(basis3), Is.True);
  }

  [Test]
  public void Equals_DifferentSubspaces() {
    LinearBasis basisXY   = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis basisYZ   = new LinearBasis(V(0, 1, 0), V(0, 0, 1));
    LinearBasis basisX    = new LinearBasis(V(1, 0, 0));
    LinearBasis basisFull = new LinearBasis(3);

    Assert.That(basisXY.Equals(basisYZ), Is.False);
    Assert.That(basisXY.Equals(basisX), Is.False);
    Assert.That(basisX.Equals(basisXY), Is.False); // Check symmetry
    Assert.That(basisXY.Equals(basisFull), Is.False);
    Assert.That(basisFull.Equals(basisXY), Is.False);
  }

  [Test]
  public void Equals_DifferentSpaceDim() {
    LinearBasis basis3D = new LinearBasis(V(1, 0, 0));
    LinearBasis basis4D = new LinearBasis(V(1, 0, 0, 0));
    Assert.That(basis3D.Equals(basis4D), Is.False);
  }

  [Test]
  public void Equals_NullOrDifferentType() {
    LinearBasis basis = new LinearBasis(V(1, 0));
    Assert.That(basis.Equals(null), Is.False);
    Assert.That(basis.Equals(new Vector(new double[] { 1, 0 })), Is.False);
  }


  [Test]
  public void GetEnumerator_IteratesCorrectly() {
    Vector      v1    = V(1, 0, 0);
    Vector      v2    = V(0, 1, 0);
    LinearBasis basis = new LinearBasis(v1, v2);

    List<Vector> vectorsFromIterator = new List<Vector>();
    foreach (Vector v in basis) // Uses IEnumerable<Vector>
    {
      vectorsFromIterator.Add(v);
    }

    Assert.That(vectorsFromIterator.Count, Is.EqualTo(2));
    AssertVectorsAreEqual(basis[0], vectorsFromIterator[0]);
    AssertVectorsAreEqual(basis[1], vectorsFromIterator[1]);
  }

  [Test]
  public void GetEnumerator_EmptyBasis() {
    LinearBasis basis = new LinearBasis(3, 0);
    int         count = 0;
    foreach (Vector v in basis) {
      count++;
    }
    Assert.That(count, Is.EqualTo(0));
  }
#endregion

#region SpanSameSpace Tests
  [Test]
  public void SpanSameSpace_IdenticalObjects() {
    LinearBasis basis = LinearBasis.GenLinearBasis(4, 2);
    Assert.That(basis.SpanSameSpace(basis), Is.True);
  }

  [Test]
  public void SpanSameSpace_EqualCopies() {
    LinearBasis basis1 = LinearBasis.GenLinearBasis(3, 2);
    LinearBasis basis2 = new LinearBasis(basis1, true);
    Assert.That(basis1.SpanSameSpace(basis2), Is.True);
    Assert.That(basis2.SpanSameSpace(basis1), Is.True);
  }

  [Test]
  public void SpanSameSpace_EquivalentBases_SameVectorsDifferentOrder() {
    Vector      v1     = V(1, 0, 0);
    Vector      v2     = V(0, 1, 0);
    LinearBasis basis1 = new LinearBasis(v1, v2);
    LinearBasis basis2 = new LinearBasis(v2, v1); // Тот же базис, другой порядок
    Assert.That(basis1.SpanSameSpace(basis2), Is.True);
    Assert.That(basis2.SpanSameSpace(basis1), Is.True);
  }

  [Test]
  public void SpanSameSpace_EquivalentBases_RotatedXYPlane() {
    // Стандартный базис XY плоскости
    LinearBasis basisXY = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    // Повернутый базис XY плоскости
    double      ca       = Math.Cos(Math.PI / 4.0); // cos(45)
    double      sa       = Math.Sin(Math.PI / 4.0); // sin(45)
    LinearBasis basisRot = new LinearBasis(V(ca, sa, 0), V(-sa, ca, 0));

    Assert.That(basisXY.SubSpaceDim, Is.EqualTo(basisRot.SubSpaceDim));
    Assert.That(basisXY.SpanSameSpace(basisRot), Is.True, "Standard XY and rotated XY should span the same space.");
    Assert.That(basisRot.SpanSameSpace(basisXY), Is.True, "Symmetry check for rotated basis.");
  }

  [Test]
  public void SpanSameSpace_EquivalentBases_GeneratedVsStandard() {
    LinearBasis basisStd = new LinearBasis(3);

    GRandomLC   rnd      = new GRandomLC(54321);
    LinearBasis basisGen = LinearBasis.GenLinearBasis(3, 3, rnd);

    Assert.That(basisStd.FullDim, Is.True);
    Assert.That(basisGen.FullDim, Is.True);
    Assert.That(basisStd.SpanSameSpace(basisGen), Is.True, "Standard R3 and generated full R3 should span the same space.");
    Assert.That(basisGen.SpanSameSpace(basisStd), Is.True, "Symmetry check for generated R3.");
  }

  [Test]
  public void SpanSameSpace_DifferentSubSpaceDim() {
    LinearBasis basis1D    = new LinearBasis(new[] { V(1, 0, 0) });   // Line
    LinearBasis basis2D    = new LinearBasis(V(1, 0, 0), V(0, 1, 0)); // Plane
    LinearBasis basisEmpty = new LinearBasis(3, 0);                   // Empty

    Assert.That(basis1D.SpanSameSpace(basis2D), Is.False);
    Assert.That(basis2D.SpanSameSpace(basis1D), Is.False);
    Assert.That(basis1D.SpanSameSpace(basisEmpty), Is.False);
    Assert.That(basisEmpty.SpanSameSpace(basis1D), Is.False);
  }

  [Test]
  public void SpanSameSpace_SameSubSpaceDimDifferentSpace() {
    // XY плоскость
    LinearBasis basisXY = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    // XZ плоскость
    LinearBasis basisXZ = new LinearBasis(V(1, 0, 0), V(0, 0, 1));
    // Плоскость y=x
    LinearBasis basisDiag = new LinearBasis(V(1, 1, 0).Normalize(), V(0, 0, 1));

    Assert.That(basisXY.SpanSameSpace(basisXZ), Is.False, "XY vs XZ plane");
    Assert.That(basisXZ.SpanSameSpace(basisXY), Is.False);
    Assert.That(basisXY.SpanSameSpace(basisDiag), Is.False, "XY vs Diagonal plane");
    Assert.That(basisDiag.SpanSameSpace(basisXY), Is.False);
  }

  [Test]
  public void SpanSameSpace_DifferentSpaceDim() {
    LinearBasis basis2D = new LinearBasis(new[] { V(1, 0) });    // R^2
    LinearBasis basis3D = new LinearBasis(new[] { V(1, 0, 0) }); // R^3

    // У них SubSpaceDim = 1, но SpaceDim разный
    Assert.That(basis2D.SpanSameSpace(basis3D), Is.False);
    Assert.That(basis3D.SpanSameSpace(basis2D), Is.False);
  }

  [Test]
  public void SpanSameSpace_BothEmpty() {
    LinearBasis basisEmpty1  = new LinearBasis(3, 0);
    LinearBasis basisEmpty2  = new LinearBasis(3, 0);
    LinearBasis basisEmpty4D = new LinearBasis(4, 0);

    Assert.That(basisEmpty1.SpanSameSpace(basisEmpty2), Is.True, "Two empty bases in same SpaceDim.");
    Assert.That(basisEmpty1.SpanSameSpace(basisEmpty4D), Is.False, "Empty bases in different SpaceDim.");
  }
#endregion

#region Generation Test
  [Test]
  public void GenLinearBasis_FullDim() {
    LinearBasis lb = LinearBasis.GenLinearBasis(4);
    Assert.That(lb.SpaceDim, Is.EqualTo(4));
    Assert.That(lb.SubSpaceDim, Is.EqualTo(4));
    Assert.That(lb.FullDim, Is.True);
    AssertBasisOrthonormal(lb);
  }

  [Test]
  public void GenLinearBasis_PartialDim() {
    LinearBasis lb = LinearBasis.GenLinearBasis(5, 2);
    Assert.That(lb.SpaceDim, Is.EqualTo(5));
    Assert.That(lb.SubSpaceDim, Is.EqualTo(2));
    Assert.That(lb.FullDim, Is.False);
    AssertBasisOrthonormal(lb);
  }
#endregion

#region Random tests
  [Test]
  public void OrthonormalizeRND() {
    GRandomLC   rnd   = new GRandomLC(1234);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);
    Vector      v     = Vector.GenVector(5, rnd);

    Vector ov       = basis.Orthonormalize(v);
    Vector expected = V(-0.17574983491041277, -0.3573953264012456, -0.22696505419765584, 0.8059592355030082, 0.37456261302480787);

    Assert.That(ov.Equals(expected) || ov.Equals(-expected), "Some orthogonalization of vector v");
  }

  [Test]
  public void AddVectorRND() {
    GRandomLC          rnd   = new GRandomLC(2345);
    LinearBasisMutable basis = LinearBasisMutable.GenLinearBasis(5, 3, rnd);
    Vector             v     = Vector.GenVector(5, rnd);

    basis.AddVector(v);

    List<Vector> basisList =
      new List<Vector>()
        {
          V(-0.5469069029145192, 0.7375266873774018, -0.06093629951375099, -0.04428435868483195, -0.3889381543741992)
        , V(-0.5437514327000299, -0.5605588954339749, -0.5887103066895489, -0.06269356730321592, -0.19899194858787822)
        , V(0.35056986554378083, 0.14703950888480366, -0.46820894124701984, 0.7644356160825109, -0.22781292412948903)
        };

    List<Vector> exp1 = new List<Vector>(basisList);
    List<Vector> exp2 = new List<Vector>(basisList);
    Vector       ov   = V(-0.00709854200521387, -0.3363850420894928, 0.5556769114749364, 0.18883245131301185, -0.7364510774957459);
    exp1.Add(ov);
    exp2.Add(-ov);

    Assert.That(basis.SequenceEqual(exp1) || basis.SequenceEqual(exp2), "Add vector to the basis");
  }

  [Test]
  public void FindOrthogonalComplement() {
    GRandomLC   rnd   = new GRandomLC(3456);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 2, rnd);

    LinearBasis complement = basis.OrthogonalComplement();

    LinearBasis expected =
      new LinearBasis
        (
         V(0.49828681673115194, 0.5043344719202018, 0.6554103491752181, 0.01653950264222742, 0.25984747016523546)
        , V(-0.516489212458386, 0.1974367874017911, 0.23895366498487525, 0.796883632509154, -0.046208555743365354)
        , V(-0.03138801268197455, -0.5352593332246187, 0.10435700671977013, 0.12897224902012222, 0.8276400262112715)
        );

    Assert.That(complement.SpanSameSpace(expected), Is.True);
  }

  [Test]
  public void ProjectVectorToSubspace() {
    GRandomLC   rnd   = new GRandomLC(4567);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);

    Vector v        = Vector.GenVector(5, rnd);
    Vector expected = V(0.14525705641968203, -0.2535536397279014, -0.036500911569481743);

    Vector prv = basis.ProjectVectorToSubSpace(v);

    Assert.That(prv, Is.EqualTo(expected), "Proj vector to some subspace.");
  }

  [Test]
  public void ProjectVectorToSS_inOrig() {
    GRandomLC   rnd   = new GRandomLC(4567);
    LinearBasis basis = LinearBasis.GenLinearBasis(5, 3, rnd);

    Vector v        = Vector.GenVector(5, rnd);
    Vector expected = V(-0.09590126543490077, -0.1165857398613803, -0.014731846409157014, -0.07624884049551936, -0.240626633558726);

    Vector prv = basis.ProjectVectorToSubSpace_in_OrigSpace(v);

    Assert.That(prv, Is.EqualTo(expected), "Proj vector to some subspace.");
  }
#endregion

}
