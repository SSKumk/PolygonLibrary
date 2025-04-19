using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using System.Collections.Generic;
using System.Linq;
using System;
using static Tests.SharedTests.StaticHelpers;

namespace Tests.SharedTests;

[TestFixture]
public class LinearBasisTests {


  // Helper for comparing vectors with tolerance
  private void AssertVectorsAreEqual(Vector v1, Vector v2, string message = "") {
    Assert.That(v1.SpaceDim, Is.EqualTo(v2.SpaceDim), $"Vector dimensions differ. {message}");
    for (int i = 0; i < v1.SpaceDim; i++) {
      Assert.That(v1[i], Is.EqualTo(v2[i]).Within(Tools.Eps), $"Vector component {i} differs. {message}");
    }
  }

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
    Assert.DoesNotThrow(() => LinearBasis.CheckCorrectness(lb), "CheckCorrectness failed.");
#endif
  }

#region Constructor Tests
  [Test]
  public void Constructor_Empty() {
    LinearBasis basis = new LinearBasis(3, 0);
    Assert.That(basis.Empty, Is.True);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(0));
    Assert.That(basis.IsFullDim, Is.False);
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
    Assert.That(basis.IsFullDim, Is.False);
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
    Assert.That(basis.IsFullDim, Is.True);
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
    Assert.That(basis.IsFullDim, Is.False);
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
        , // Dependent + orthogonal component
          V(0, 0, 4)
        , V(-1, 1, 5.7412) // Should be projected out mostly
        };

    LinearBasis basis = new LinearBasis(vectors, orthogonalize: true);

    Assert.That(basis.SpaceDim, Is.EqualTo(3), "SpaceDim should be determined from vectors.");
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3), "Should find 3 linearly independent vectors.");
    Assert.That(basis.IsFullDim, Is.True);
    Assert.That(basis.Empty, Is.False);
    AssertBasisOrthonormal(basis);

    // Check if the space spanned is correct (e.g., contains original independent vectors)
    Assert.That(basis.Contains(V(1, 0, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 1, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 0, 1)), Is.True);
  }

  [Test]
  public void Constructor_WithVectors_NoOrthogonalize_InvalidInput_Throws() {
    List<Vector> vectors = new List<Vector>() { V(1, 0, 0), V(0, 1, 0), V(1, 1, 0) };

#if DEBUG
    Assert.Throws<ArgumentException>
      (
       () => new LinearBasis(vectors, orthogonalize: false)
     , "[DEBUG] Constructor with orthogonalize:false should throw if CheckCorrectness fails due to non-orthonormal input."
      );
#else
        LinearBasis basis = null;
        Assert.DoesNotThrow(() => basis = new LinearBasis(vectors, orthogonalize: false));
        Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
        Assert.Throws<AssertionException>(() => AssertBasisOrthonormal(basis));
        Console.WriteLine("[RELEASE] Constructor with orthogonalize:false and invalid input created a non-orthonormal basis.");
#endif
  }

  [Test]
  public void Constructor_Copy() {
    LinearBasis basis1 = new LinearBasis(new[] { V(1, 2, 0), V(0, 0, 3) });
    LinearBasis basis2 = new LinearBasis(basis1);

    Assert.That(basis2.SpaceDim, Is.EqualTo(basis1.SpaceDim));
    Assert.That(basis2.SubSpaceDim, Is.EqualTo(basis1.SubSpaceDim));
    Assert.That(basis2.IsFullDim, Is.EqualTo(basis1.IsFullDim));
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

    LinearBasis basis = null;
    // Эта операция должна пройти успешно и в Debug, и в Release
    Assert.DoesNotThrow(() => basis = new LinearBasis(vectors, orthogonalize: false));

    Assert.That(basis, Is.Not.Null);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(V(1, 0, 0), basis[0]);
    AssertVectorsAreEqual(V(0, 1, 0), basis[1]);
    AssertVectorsAreEqual(V(0, 0, 1), basis[2]);
    AssertBasisOrthonormal(basis); // Эта проверка должна пройти
  }
#endregion

#region AddVector Tests
  [Test]
  public void AddVector_ToEmpty_Orthogonalize() {
    LinearBasis basis = new LinearBasis(3, 0);
    bool        added = basis.AddVector(V(0, 5, 0), true);

    Assert.That(added, Is.True);
    Assert.That(basis.Empty, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(V(0, 1, 0), basis[0]);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_ToEmpty_NoOrthogonalize_InvalidInput_Throws() {
    LinearBasis basis         = new LinearBasis(3, 0);
    Vector      nonNormalized = V(0, 5, 0);

#if DEBUG
    Assert.Throws<ArgumentException>
      (
       () => basis.AddVector(nonNormalized, false)
     , "[DEBUG] AddVector with orthogonalize:false should throw if CheckCorrectness fails (vector not normalized)."
      );
#else
    Assert.DoesNotThrow(() => basis.AddVector(nonNormalized, false));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    Assert.Throws<AssertionException>(() => AssertBasisOrthonormal(basis));
    Console.WriteLine("[RELEASE] AddVector with orthogonalize:false and non-normalized input created a non-orthonormal basis.");
#endif
  }


  [Test]
  public void AddVector_Independent_Orthogonalize() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0));
    bool        added = basis.AddVector(V(0, 2, 0), true);

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(V(1, 0, 0), basis[0]);
    AssertVectorsAreEqual(V(0, 1, 0), basis[1]);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_IndependentNonOrthogonal_Orthogonalize() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0));
    bool        added = basis.AddVector(V(1, 2, 0), true); // Non-orthogonal to basis[0]

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(V(1, 0, 0), basis[0]);
    // The added vector should be the normalized component of (1,2,0) orthogonal to (1,0,0), which is (0,2,0) normalized -> (0,1,0)
    AssertVectorsAreEqual(V(0, 1, 0), basis[1]);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_Dependent_Orthogonalize() {
    LinearBasis basis = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    bool        added = basis.AddVector(V(3, 4, 0), true); // Lies in the span of the basis

    Assert.That(added, Is.False);                  // Should not add a dependent vector
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2)); // Dimension should not change
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_Zero_Orthogonalize() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0));
    bool        added = basis.AddVector(Vector.Zero(3), true);

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_ToFullBasis_Orthogonalize() {
    LinearBasis basis = new LinearBasis(2); // Full 2D basis
    bool        added = basis.AddVector(V(1, 1), true);

    Assert.That(added, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.IsFullDim, Is.True);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_Independent_NoOrthogonalize_InvalidInput_Throws() {
    LinearBasis basis         = new LinearBasis(V(1, 0, 0)); // Создаем корректный базис
    Vector      invalidVector = V(1, 2, 0);                  // Не нормирован и не ортогонален

#if DEBUG
    Assert.Throws<ArgumentException>
      (
       () => basis.AddVector(invalidVector, false)
     , "[DEBUG] AddVector with orthogonalize:false should throw if CheckCorrectness fails (vector not normalized or not orthogonal)."
      );
#else
        Assert.DoesNotThrow(() => basis.AddVector(invalidVector, false));
        Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
        Assert.Throws<AssertionException>(() => AssertBasisOrthonormal(basis));
         Console.WriteLine("[RELEASE] AddVector with orthogonalize:false and invalid (non-orth/non-norm) input created a non-orthonormal basis.");
#endif
  }

  [Test]
  public void AddVector_Dependent_NoOrthogonalize_InvalidInput_Throws() {
    LinearBasis basis           = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // Корректный базис
    Vector      dependentVector = V(1, 1, 0); // Зависим => не ортогонален, также не нормирован

#if DEBUG
    Assert.Throws<ArgumentException>
      (
       () => basis.AddVector(dependentVector, false)
     , "[DEBUG] AddVector with orthogonalize:false should throw if CheckCorrectness fails (vector not orthogonal/normalized)."
      );
#else
        Assert.DoesNotThrow(() => basis.AddVector(dependentVector, false));
        Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
        Assert.Throws<AssertionException>(() => AssertBasisOrthonormal(basis));
        Console.WriteLine("[RELEASE] AddVector with orthogonalize:false and invalid (dependent/non-norm) input created a non-orthonormal basis.");
#endif
  }

  [Test]
  public void AddVectors_AddsMultiple() {
    LinearBasis basis = new LinearBasis(4, 0);
    var vectors =
      new List<Vector>
        {
          V(1, 0, 0, 0)
        , V(1, 1, 0, 0)
        , V(0, 0, 1, 0)
        , V(0, 0, 0, 5)
        };
    basis.AddVectors(vectors); // Orthogonalize = true default

    Assert.That(basis.SubSpaceDim, Is.EqualTo(4));
    Assert.That(basis.IsFullDim, Is.True);
    AssertBasisOrthonormal(basis);
  }

  [Test]
  public void AddVector_NoOrthogonalize_ValidInput() {
    LinearBasis basis       = new LinearBasis(V(1, 0, 0, 0));
    Vector      validVector = V(0, 1, 0, 0);

    bool added = false;
    Assert.DoesNotThrow(() => added = basis.AddVector(validVector, false));

    Assert.That(added, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AssertBasisOrthonormal(basis); // Проверка должна пройти
  }
#endregion

#region Projection and Contains Tests
  [Test]
  public void ProjectPointToSubSpace_in_OrigSpace_Simple() {
    LinearBasis basis     = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // XY plane in 3D
    Vector      v         = V(3, 4, 5);
    Vector      projected = basis.ProjectPointToSubSpace_in_OrigSpace(v);
    Vector      expected  = V(3, 4, 0);

    AssertVectorsAreEqual(expected, projected);
  }

  [Test]
  public void ProjectPointToSubSpace_in_OrigSpace_VectorInSubspace() {
    LinearBasis basis     = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    Vector      v         = V(3, 4, 0); // Already in subspace
    Vector      projected = basis.ProjectPointToSubSpace_in_OrigSpace(v);

    AssertVectorsAreEqual(v, projected, "Projection of vector already in subspace should be the vector itself.");
  }

  [Test]
  public void ProjectPointToSubSpace_in_OrigSpace_VectorOrthogonalToSubspace() {
    LinearBasis basis     = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    Vector      v         = V(0, 0, 5); // Orthogonal to subspace
    Vector      projected = basis.ProjectPointToSubSpace_in_OrigSpace(v);
    Vector      expected  = V(0, 0, 0);

    AssertVectorsAreEqual(expected, projected, "Projection of vector orthogonal to subspace should be zero vector.");
  }

  [Test]
  public void ProjectPointToSubSpace_in_OrigSpace_FullDimBasis() {
    LinearBasis basis     = new LinearBasis(3); // Full 3D basis
    Vector      v         = V(3, 4, 5);
    Vector      projected = basis.ProjectPointToSubSpace_in_OrigSpace(v);

    AssertVectorsAreEqual(v, projected, "Projection onto full dimension basis should be the vector itself.");
  }

  [Test]
  public void Contains_VectorInSubspace() {
    LinearBasis basis = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // XY plane
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
    // Project onto basis coordinates
    LinearBasis basis           = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // b1, b2
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
    LinearBasis basis = new LinearBasis(new[] { b1, b2 });

    Vector v               = V(2, 2, 3);
    Vector projectedCoords = basis.ProjectVectorToSubSpace(v);
    Vector expectedCoords  = V(v * b1, v * b2);

    Assert.That(projectedCoords.SpaceDim, Is.EqualTo(2));
    Assert.That(projectedCoords[0], Is.EqualTo(expectedCoords[0]).Within(Tools.Eps));
    Assert.That(projectedCoords[1], Is.EqualTo(expectedCoords[1]).Within(Tools.Eps));
  }

  [Test]
  public void ProjectVectorsToSubSpace_Multiple() {
    LinearBasis basis     = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
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
    LinearBasis basis = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    Vector      v     = V(3, 4, 0); // In the subspace
    Vector      ortho = basis.Orthonormalize(v);

    Assert.That(ortho.IsZero, Is.True, "Orthonormalizing vector in subspace should yield zero vector.");
  }

  [Test]
  public void Orthonormalize_GeneralVector() {
    LinearBasis basis    = new LinearBasis(V(1, 0, 0));
    Vector      v        = V(3, 4, 0); // v = (3,0,0) + (0,4,0). Component orthogonal to basis is (0,4,0).
    Vector      ortho    = basis.Orthonormalize(v);
    Vector      expected = V(0, 1, 0); // Normalized orthogonal component

    AssertVectorsAreEqual(expected, ortho);
  }

  [Test]
  public void Orthonormalize_AgainstEmptyBasis() {
    LinearBasis basis    = new LinearBasis(3, 0); // Empty basis
    Vector      v        = V(3, 4, 0);
    Vector      ortho    = basis.Orthonormalize(v);
    Vector      expected = V(0.6, 0.8, 0); // Just normalized input vector

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
    LinearBasis  basis      = new LinearBasis(new[] { V(1, 0, 0, 0), V(0, 1, 0, 0) }); // Span e1, e2 in 4D
    LinearBasis? complement = basis.FindOrthogonalComplement();

    Assert.That(complement, Is.Not.Null);
    Assert.That(complement!.SpaceDim, Is.EqualTo(4));
    Assert.That(complement.SubSpaceDim, Is.EqualTo(2)); // Should be 4 - 2 = 2
    AssertBasisOrthonormal(complement);

    // Check that complement vectors are orthogonal to original basis vectors
    foreach (Vector cv in complement) {
      foreach (Vector bv in basis) {
        Assert.That(cv * bv, Is.EqualTo(0.0).Within(Tools.Eps), $"Complement vector {cv} not orthogonal to basis vector {bv}");
      }
    }

    // Check that the complement spans the expected space (e.g., contains e3, e4)
    Assert.That(complement.Contains(V(0, 0, 1, 0)), Is.True);
    Assert.That(complement.Contains(V(0, 0, 0, 1)), Is.True);
    Assert.That(complement.Contains(V(1, 0, 0, 0)), Is.False); // Should not contain original basis vectors
  }

  [Test]
  public void FindOrthogonalComplement_FullBasis() {
    LinearBasis  basis      = new LinearBasis(3); // Full 3D basis
    LinearBasis? complement = basis.FindOrthogonalComplement();

    Assert.That(complement, Is.Null);
  }

  [Test]
  public void FindOrthogonalComplement_EmptyBasis() {
    LinearBasis  basis        = new LinearBasis(4, 0); // Empty basis in 4D
    LinearBasis? complement   = basis.FindOrthogonalComplement();
    LinearBasis  expectedFull = new LinearBasis(4); // Expect full basis as complement

    Assert.That(complement, Is.Not.Null);
    Assert.That(complement!.SpaceDim, Is.EqualTo(4));
    Assert.That(complement.SubSpaceDim, Is.EqualTo(4));
    Assert.That(complement.IsFullDim, Is.True);
    // Check if it spans the same space as the standard basis
    Assert.That(complement.Equals(expectedFull), Is.True);
  }

  [Test]
  public void FindOrthonormalVector_PartialBasis() {
    LinearBasis basis = new LinearBasis(V(1, 0, 0, 0)); // Span e1 in 4D
    Vector      ortho = basis.FindOrthonormalVector();

    Assert.That(ortho.IsZero, Is.False);
    Assert.That(ortho.Length, Is.EqualTo(1.0).Within(Tools.Eps));
    Assert.That(ortho * basis[0], Is.EqualTo(0.0).Within(Tools.Eps), "Found vector should be orthogonal to basis");
  }

  [Test]
  public void FindOrthonormalVector_FullBasis_Throws() {
    LinearBasis basis = new LinearBasis(3); // Full 3D basis
    Assert.Throws<ArgumentException>(() => basis.FindOrthonormalVector());
  }
#endregion

#region Equality and Enumeration Tests
  [Test]
  public void Equals_SameBasisObject() {
    LinearBasis basis = new LinearBasis(new[] { V(1, 0), V(0, 1) });
    Assert.That(basis.Equals(basis), Is.True);
  }

  [Test]
  public void Equals_DifferentObjectsSameSpace() {
    LinearBasis basis1 = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    LinearBasis basis2 = new LinearBasis(new[] { V(0, 1, 0), V(1, 0, 0) }); // Same vectors, different order
    LinearBasis basis3 =
      new LinearBasis
        (
         new[] { V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0), V(1 / Math.Sqrt(2), -1 / Math.Sqrt(2), 0) }
        ); // Different vectors, same plane

    Assert.That(basis1.Equals(basis2), Is.True);
    Assert.That(basis1.Equals(basis3), Is.True);
    Assert.That(basis2.Equals(basis3), Is.True);
  }

  [Test]
  public void Equals_DifferentSubspaces() {
    LinearBasis basisXY   = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    LinearBasis basisYZ   = new LinearBasis(new[] { V(0, 1, 0), V(0, 0, 1) });
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
    LinearBasis basis = new LinearBasis(new[] { v1, v2 });

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

#region Generation Test
  [Test]
  public void GenLinearBasis_FullDim() {
    LinearBasis lb = LinearBasis.GenLinearBasis(4);
    Assert.That(lb.SpaceDim, Is.EqualTo(4));
    Assert.That(lb.SubSpaceDim, Is.EqualTo(4));
    Assert.That(lb.IsFullDim, Is.True);
    AssertBasisOrthonormal(lb);
  }

  [Test]
  public void GenLinearBasis_PartialDim() {
    LinearBasis lb = LinearBasis.GenLinearBasis(5, 2);
    Assert.That(lb.SpaceDim, Is.EqualTo(5));
    Assert.That(lb.SubSpaceDim, Is.EqualTo(2));
    Assert.That(lb.IsFullDim, Is.False);
    AssertBasisOrthonormal(lb);
  }
#endregion

}
