using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers; // V() и другие хелперы
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.SharedTests;

[TestFixture]
public class AffineBasisTests {

  // Helper для сравнения векторов уже есть в VectorTests, но можно и сюда добавить для удобства
  private void AssertVectorsAreEqual(Vector v1, Vector v2, string message = "") {
    Assert.That(v1.SpaceDim, Is.EqualTo(v2.SpaceDim), $"Vector dimensions differ. {message}");
    for (int i = 0; i < v1.SpaceDim; i++) {
      Assert.That(Tools.EQ(v1[i], v2[i]), Is.True, $"Vector component {i} differs. Expected: {v2[i]}, Got: {v1[i]}. {message}");
    }
  }

  // Helper для проверки ортонормированности LinBasis (если нужно)
  private void AssertBasisOrthonormal(AffineBasis ab) {
    LinearBasis lb = ab.LinBasis;

    if (lb.Empty)
      return;

    for (int i = 0; i < lb.SubSpaceDim; i++) {
      Assert.That(Tools.EQ(lb[i].Length, 1.0), Is.True, $"Basis vector {i} is not normalized.");
      for (int j = i + 1; j < lb.SubSpaceDim; j++) {
        Assert.That(Tools.EQ(lb[i] * lb[j], 0.0), Is.True, $"Basis vectors {i} and {j} are not orthogonal.");
      }
    }
#if DEBUG
    // Проверка внутреннего состояния
    Assert.DoesNotThrow(() => AffineBasis.CheckCorrectness(ab), "Internal CheckCorrectness failed.");
#endif
  }


#region Constructor Tests
  [Test]
  public void Constructor_DefaultDim_CreatesFullDimStandardBasisAtOrigin() {
    AffineBasis ab = new AffineBasis(3);

    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(3));
    Assert.That(ab.IsFullDim, Is.True);
    Assert.That(ab.IsEmpty, Is.False);
    AssertVectorsAreEqual(ab.Origin, Vector.Zero(3));
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 1, 0));
    AssertVectorsAreEqual(ab[2], V(0, 0, 1));
    AssertBasisOrthonormal(ab);
  }
  //  todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Constructor_OriginOnly_CreatesZeroSubspaceDimBasis() {
  //   Vector      origin = V(1, 2, 3);
  //   AffineBasis ab     = new AffineBasis(origin);
  //
  //   Assert.That(ab.SpaceDim, Is.EqualTo(3));
  //   Assert.That(ab.SubSpaceDim, Is.EqualTo(0));
  //   Assert.That(ab.IsFullDim, Is.False);
  //   Assert.That(ab.IsEmpty, Is.True);
  //   AssertVectorsAreEqual(ab.Origin, origin);
  //   Assert.Throws<ArgumentOutOfRangeException>
  //     (
  //      ()
  //        => {
  //        var x = ab[0];
  //      }
  //    , "Accessing index 0 of empty basis should throw."
  //     );
  // }

  [Test]
  public void Constructor_OriginAndLinearBasis_NeedCopyTrue() {
    Vector      origin = V(1, 1, 1);
    LinearBasis lb     = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab     = new AffineBasis(origin, lb, needCopy: true);

    AssertVectorsAreEqual(ab.Origin, origin);
    Assert.That(ab.LinBasis.Equals(lb), Is.True);
    Assert.That(ab.LinBasis, Is.Not.SameAs(lb)); // Must be a copy

    // Modify original lb, ab should not change
    lb.AddVector(V(0, 0, 1));
    Assert.That
      (ab.SubSpaceDim, Is.EqualTo(2), "Affine basis should not change when original LinearBasis is modified (NeedCopy=true).");
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Constructor_OriginAndLinearBasis_NeedCopyFalse() {
    Vector      origin = V(1, 1, 1);
    LinearBasis lb     = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab     = new AffineBasis(origin, lb, needCopy: false); // Internal constructor usage? Assumes lb is valid.

    AssertVectorsAreEqual(ab.Origin, origin);
    Assert.That(ab.LinBasis.Equals(lb), Is.True);
    Assert.That(ab.LinBasis, Is.SameAs(lb)); // Must be the same instance

    // Modify original lb, ab SHOULD change
    lb.AddVector(V(0, 0, 1));
    Assert.That
      (ab.SubSpaceDim, Is.EqualTo(3), "Affine basis should change when original LinearBasis is modified (NeedCopy=false).");
    AssertBasisOrthonormal(ab); // lb was modified correctly, so ab should still be valid
  }


  [Test]
  public void Constructor_FromPoints_Line() {
    Vector p1 = V(1, 1, 1);
    Vector p2 = V(3, 1, 1);  // Along X axis from p1
    Vector p3 = V(-1, 1, 1); // Also along X axis from p1

    AffineBasis ab =
      new AffineBasis
        (
         new List<Vector>
           {
             p1
           , p2
           , p3
           }
        );

    AssertVectorsAreEqual(ab.Origin, p1);
    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(1), "Should only find one independent direction (p2-p1).");
    AssertVectorsAreEqual(ab[0], V(1, 0, 0), "Basis vector should be normalized direction."); // (p2-p1).Normalize()
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Constructor_FromPoints_Plane() {
    Vector p1 = V(0, 0, 0);
    Vector p2 = V(2, 0, 0); // X direction
    Vector p3 = V(0, 3, 0); // Y direction
    Vector p4 = V(1, 1, 0); // In XY plane

    AffineBasis ab =
      new AffineBasis
        (
         new List<Vector>
           {
             p1
           , p2
           , p3
           , p4
           }
        );

    AssertVectorsAreEqual(ab.Origin, p1);
    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2), "Should find two independent directions.");
    // LinBasis should span the XY plane, e.g., contains e1 and e2
    Assert.That(ab.LinBasis.Contains(V(1, 0, 0)), Is.True);
    Assert.That(ab.LinBasis.Contains(V(0, 1, 0)), Is.True);
    Assert.That(ab.LinBasis.Contains(V(0, 0, 1)), Is.False);
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Constructor_FromPoints_SinglePoint() {
    Vector      p1 = V(5, 6, 7);
    AffineBasis ab = new AffineBasis(new List<Vector> { p1 });

    AssertVectorsAreEqual(ab.Origin, p1);
    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(0));
    Assert.That(ab.IsEmpty, Is.True);
  }

  [Test]
  public void Constructor_CopyConstructor() {
    Vector      origin   = V(1, 2, 3);
    LinearBasis lb       = LinearBasis.GenLinearBasis(3, 2); // Generate a 2D basis in 3D space
    AffineBasis original = new AffineBasis(origin, lb);
    AffineBasis copy     = new AffineBasis(original);

    AssertVectorsAreEqual(copy.Origin, original.Origin);
    Assert.That(copy.LinBasis.Equals(original.LinBasis), Is.True, "Linear bases should be equal.");
    Assert.That(copy.LinBasis, Is.Not.SameAs(original.LinBasis), "Linear basis should be a copy.");
    Assert.That(copy.SpaceDim, Is.EqualTo(original.SpaceDim));
    Assert.That(copy.SubSpaceDim, Is.EqualTo(original.SubSpaceDim));

    // Modify copy's linear basis, original should not change
    copy.AddVector(V(0, 0, 1)); // Assuming this doesn't make it non-orthonormal for simplicity
    Assert.That(original.SubSpaceDim, Is.EqualTo(2), "Original SubSpaceDim should remain unchanged.");
  }
#endregion

#region Factory Tests
  [Test]
  public void Factory_FromVectors() {
    Vector      o  = V(1, 1, 0);
    Vector      v1 = V(2, 0, 0);                                              // Not normalized
    Vector      v2 = V(0, 0, 3);                                              // Not normalized, but orthogonal to v1
    AffineBasis ab = AffineBasis.FromVectors(o, new List<Vector> { v1, v2 }); // Orthogonalize = true default

    AssertVectorsAreEqual(ab.Origin, o);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    // Check if basis vectors are normalized versions of input
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 0, 1));
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Factory_FromPoints() {
    Vector      o  = V(1, 1, 1);
    Vector      p1 = V(3, 1, 1); // Direction (2,0,0) -> e1
    Vector      p2 = V(1, 1, 4); // Direction (0,0,3) -> e3
    AffineBasis ab = AffineBasis.FromPoints(o, new List<Vector> { p1, p2 });

    AssertVectorsAreEqual(ab.Origin, o);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 0, 1));
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Factory_GenAffineBasis() {
    AffineBasis ab = AffineBasis.GenAffineBasis(4, 2); // 2D subspace in 4D space
    Assert.That(ab.SpaceDim, Is.EqualTo(4));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    Assert.That(ab.Origin.IsZero, Is.False); // Highly unlikely to be zero
    AssertBasisOrthonormal(ab);              // Checks LinBasis
  }
#endregion

#region Method Tests (Operations)
  [Test]
  public void Method_AddVector() {
    Vector      o  = V(1, 1, 1);
    AffineBasis ab = new AffineBasis(o, new LinearBasis(V(1, 0, 0))); // Start with X-axis basis
    Assert.That(ab.SubSpaceDim, Is.EqualTo(1));

    bool added1 = ab.AddVector(V(0, 5, 0)); // Add Y-direction (will be normalized)
    Assert.That(added1, Is.True);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 1, 0));

    bool added2 = ab.AddVector(V(3, 0, 0)); // Add dependent vector
    Assert.That(added2, Is.False);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));

    bool added3 = ab.AddVector(V(0, 0, 1)); // Add Z-direction
    Assert.That(added3, Is.True);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(ab[2], V(0, 0, 1));

    bool added4 = ab.AddVector(V(1, 1, 1)); // Add to full basis
    Assert.That(added4, Is.False);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(3));

    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Plane() {
    Vector      o  = V(0, 0, 1); // XY plane shifted to z=1
    LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p_above  = V(3, 4, 5);
    Vector p_on     = V(3, 4, 1);
    Vector p_origin = ab.Origin;

    Vector proj_above  = ab.ProjectPointToSubSpace_in_OrigSpace(p_above);
    Vector proj_on     = ab.ProjectPointToSubSpace_in_OrigSpace(p_on);
    Vector proj_origin = ab.ProjectPointToSubSpace_in_OrigSpace(p_origin);

    AssertVectorsAreEqual(proj_above, V(3, 4, 1), "Projection of point above should land on plane.");
    AssertVectorsAreEqual(proj_on, p_on, "Projection of point on plane should be itself.");
    AssertVectorsAreEqual(proj_origin, p_origin, "Projection of origin should be itself.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Line() {
    Vector      o  = V(1, 1, 1);                  // Origin of the line
    LinearBasis lb = new LinearBasis(V(1, 0, 0)); // Line along X-axis
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p_on  = V(5, 1, 1);
    Vector p_off = V(5, 2, 1); // Same X, different Y

    Vector proj_on  = ab.ProjectPointToSubSpace_in_OrigSpace(p_on);
    Vector proj_off = ab.ProjectPointToSubSpace_in_OrigSpace(p_off);

    // Formula: Origin + LinBasis.ProjMatrix * (p - Origin)
    // For p_off: p - Origin = (4, 1, 0). LinBasis.ProjMatrix = [1 0 0; 0 0 0; 0 0 0]
    // ProjMatrix * (p - Origin) = (4, 0, 0)
    // Result = Origin + (4, 0, 0) = (1,1,1) + (4,0,0) = (5,1,1)
    AssertVectorsAreEqual(proj_on, p_on, "Projection of point on line should be itself.");
    AssertVectorsAreEqual(proj_off, V(5, 1, 1), "Projection of point off line should land on line.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Point() {
    Vector      o  = V(1, 2, 3); // 0-dim subspace
    AffineBasis ab = new AffineBasis(o);

    Vector p    = V(5, 5, 5);
    Vector proj = ab.ProjectPointToSubSpace_in_OrigSpace(p);

    AssertVectorsAreEqual(proj, o, "Projection onto a point should be the point itself.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_Coordinates_Plane() {
    Vector      o  = V(0, 0, 1);                                        // XY plane shifted to z=1
    LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // Standard e1, e2 basis
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p = V(3, 4, 5);
    // p - Origin = (3, 4, 4)
    // Project (3,4,4) onto basis e1, e2 --> coords are ( (3,4,4)*e1, (3,4,4)*e2 ) = (3, 4)
    Vector coords         = ab.ProjectPointToSubSpace(p);
    Vector expectedCoords = V(3, 4); // Coordinates in the basis e1, e2

    Assert.That(coords.SpaceDim, Is.EqualTo(ab.SubSpaceDim), "Coordinate dimension should match SubSpaceDim.");
    AssertVectorsAreEqual(coords, expectedCoords);
  }

  [Test]
  public void Method_ProjectPointToSubSpace_Coordinates_Line() {
    Vector      o  = V(1, 1, 1);                                                // Origin of the line
    LinearBasis lb = new LinearBasis(V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0)); // Line along (1,1,0) direction
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p = V(3, 3, 1); // Point on the line: o + sqrt(8)*basis_vector
    // p - Origin = (2, 2, 0)
    // Project (2,2,0) onto basis vector b = (1/sqrt(2), 1/sqrt(2), 0)
    // Coord = (p - Origin) * b = (2,2,0) * (1/sqrt(2), 1/sqrt(2), 0) = 2/sqrt(2) + 2/sqrt(2) = 4/sqrt(2) = 2*sqrt(2)
    // The coordinate represents the distance along the normalized basis vector. Length of (2,2,0) is sqrt(8) = 2*sqrt(2).
    Vector coords         = ab.ProjectPointToSubSpace(p);
    Vector expectedCoords = V(Math.Sqrt(8)); // Coordinate along the single basis vector

    Assert.That(coords.SpaceDim, Is.EqualTo(1));
    AssertVectorsAreEqual(coords, expectedCoords);
  }

  [Test]
  public void Method_TranslateToOriginal_Plane() {
    Vector      o  = V(0, 0, 1);
    LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // e1, e2
    AffineBasis ab = new AffineBasis(o, lb);

    Vector coords = V(3, 4); // Coordinates relative to basis (e1, e2)
    // Expected original = Origin + coords[0]*Basis[0] + coords[1]*Basis[1]
    // = (0,0,1) + 3*(1,0,0) + 4*(0,1,0) = (3, 4, 1)
    Vector originalPoint = ab.TranslateToOriginal(coords);
    Vector expectedPoint = V(3, 4, 1);

    AssertVectorsAreEqual(originalPoint, expectedPoint);
  }

  [Test]
  public void Method_TranslateToOriginal_Line() {
    Vector      o  = V(1, 1, 1);
    Vector      b  = V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0);
    LinearBasis lb = new LinearBasis(b);
    AffineBasis ab = new AffineBasis(o, lb);

    Vector coords = V(Math.Sqrt(8)); // Coordinate relative to basis vector b (distance along b)
    // Expected original = Origin + coords[0]*Basis[0]
    // = (1,1,1) + sqrt(8) * (1/sqrt(2), 1/sqrt(2), 0)
    // = (1,1,1) + 2*sqrt(2) * (1/sqrt(2), 1/sqrt(2), 0)
    // = (1,1,1) + (2, 2, 0) = (3, 3, 1)
    Vector originalPoint = ab.TranslateToOriginal(coords);
    Vector expectedPoint = V(3, 3, 1);

    AssertVectorsAreEqual(originalPoint, expectedPoint);
  }
  //  todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Method_TranslateToOriginal_DimMismatch_Throws() {
  //   Vector      o  = V(0, 0, 1);
  //   LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }); // SubSpaceDim = 2
  //   AffineBasis ab = new AffineBasis(o, lb);
  //
  //   Vector coords1D = V(5);
  //   Vector coords3D = V(1, 2, 3);
  //
  //   // Debug.Assert should catch this, potentially throws ArgumentException if made explicit
  //   Assert.Throws<Exception>
  //     (() => ab.TranslateToOriginal(coords1D), "TranslateToOriginal should throw if coord dim != SubSpaceDim");
  //   Assert.Throws<Exception>
  //     (() => ab.TranslateToOriginal(coords3D), "TranslateToOriginal should throw if coord dim != SubSpaceDim");
  // }

  [Test]
  public void Method_Contains_Plane() {
    Vector      o  = V(0, 0, 1);
    LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p_on     = V(5, -2, 1);
    Vector p_off    = V(5, -2, 2);
    Vector p_origin = ab.Origin;

    Assert.That(ab.Contains(p_on), Is.True);
    Assert.That(ab.Contains(p_off), Is.False);
    Assert.That(ab.Contains(p_origin), Is.True);
  }

  [Test]
  public void Method_Contains_FullDim() {
    AffineBasis ab = new AffineBasis(3); // Standard full basis
    Vector      p  = V(12, -5, 100);
    Assert.That(ab.Contains(p), Is.True, "Full dimensional affine basis should contain all points.");
  }

  [Test]
  public void Method_Contains_Point() {
    Vector      o  = V(1, 2, 3);
    AffineBasis ab = new AffineBasis(o); // 0-dim basis

    Assert.That(ab.Contains(o), Is.True, "0-dim basis should contain its origin.");
    Assert.That(ab.Contains(V(1, 2, 4)), Is.False, "0-dim basis should not contain other points.");
    Assert.That(ab.Contains(Vector.Zero(3)), Is.False, "0-dim basis at non-zero origin should not contain zero.");
  }
#endregion

#region Override Tests
  [Test]
  public void Override_Equals_SameObject() {
    AffineBasis ab = AffineBasis.GenAffineBasis(3, 2);
    Assert.That(ab.Equals(ab), Is.True);
  }

  [Test]
  public void Override_Equals_DifferentObjectsSameSubspace() {
    Vector o = V(1, 1, 1);
    // Corrected constructor call
    LinearBasis lb  = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab1 = new AffineBasis(o, lb, false);
    AffineBasis ab2 = new AffineBasis(o, lb, false);
    Assert.That(ab1.Equals(ab2), Is.True);

    AffineBasis ab3 = new AffineBasis(o, new LinearBasis(lb));
    Assert.That(ab1.Equals(ab3), Is.True);

    Vector      o4  = o + ab1[0] * 5.0 + ab1[1] * (-3.0);
    AffineBasis ab4 = new AffineBasis(o4, new LinearBasis(lb));
    Assert.That(ab1.Equals(ab4), Is.True);

    // Corrected constructor call
    LinearBasis lb_rot = new LinearBasis(new[] { V(0, 1, 0), V(-1, 0, 0) });
    AffineBasis ab5    = new AffineBasis(o, lb_rot);
    Assert.That(ab1.Equals(ab5), Is.True);
  }

  [Test]
  public void Override_Equals_DifferentSubspaces() {
    // Corrected constructor calls
    AffineBasis ab_XY = new AffineBasis(Vector.Zero(3), new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }));
    AffineBasis ab_XZ = new AffineBasis(Vector.Zero(3), new LinearBasis(new[] { V(1, 0, 0), V(0, 0, 1) }));
    AffineBasis ab_X  = new AffineBasis(Vector.Zero(3), new LinearBasis(new[] { V(1, 0, 0) }));
    Assert.That(ab_XY.Equals(ab_XZ), Is.False);
    Assert.That(ab_XY.Equals(ab_X), Is.False);

    Vector o1 = V(0, 0, 0);
    Vector o2 = V(0, 0, 1);
    // Corrected constructor call
    LinearBasis lb  = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab1 = new AffineBasis(o1, lb);
    AffineBasis ab2 = new AffineBasis(o2, lb);
    Assert.That(ab1.Equals(ab2), Is.False);

    AffineBasis ab_3D = new AffineBasis(3);
    AffineBasis ab_4D = new AffineBasis(4);
    Assert.That(ab_3D.Equals(ab_4D), Is.False);
  }

  [Test]
  public void Override_Equals_NullOrDifferentType() {
    AffineBasis ab = new AffineBasis(3);
    Assert.That(ab.Equals(null), Is.False);
    Assert.That(ab.Equals(V(1, 2, 3)), Is.False);
  }

  [Test]
  public void Override_GetEnumerator() {
    // Corrected constructor call
    AffineBasis ab      = new AffineBasis(V(1, 1, 1), new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }));
    int         count   = 0;
    var         vectors = new List<Vector>();
    foreach (Vector v in ab) {
      vectors.Add(v);
      count++;
    }
    Assert.That(count, Is.EqualTo(2));
    AssertVectorsAreEqual(vectors[0], ab.LinBasis[0]);
    AssertVectorsAreEqual(vectors[1], ab.LinBasis[1]);
  }
#endregion

}
