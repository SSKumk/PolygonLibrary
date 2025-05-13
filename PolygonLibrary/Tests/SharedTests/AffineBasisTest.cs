using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.SharedTests;

[TestFixture]
public class AffineBasisTests {

  // Helper для проверки ортонормированности LinBasis
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
    Assert.DoesNotThrow(() => AffineBasis.CheckCorrectness(ab), "Internal CheckCorrectness failed.");
#endif
  }


#region Constructor Tests
  [Test]
  public void Constructor_DefaultDim_CreatesFullDimStandardBasisAtOrigin() {
    AffineBasis ab = new AffineBasis(3);

    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(3));
    Assert.That(ab.FullDim, Is.True);
    Assert.That(ab.Empty, Is.False);
    AssertVectorsAreEqual(ab.Origin, Vector.Zero(3));
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 1, 0));
    AssertVectorsAreEqual(ab[2], V(0, 0, 1));
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Constructor_OriginOnly_CreatesZeroSubspaceDimBasis() {
    Vector      origin = V(1, 2, 3);
    AffineBasis ab     = new AffineBasis(origin);

    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(0));
    Assert.That(ab.FullDim, Is.False);
    Assert.That(ab.Empty, Is.True);
    AssertVectorsAreEqual(ab.Origin, origin);
  }

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
    AffineBasis ab     = new AffineBasis(origin, lb, needCopy: false);

    AssertVectorsAreEqual(ab.Origin, origin);
    Assert.That(ab.LinBasis.Equals(lb), Is.True);
    Assert.That(ab.LinBasis, Is.SameAs(lb)); // Must be the same instance

    lb.AddVector(V(0, 0, 1));
    Assert.That
      (ab.SubSpaceDim, Is.EqualTo(3), "Affine basis should change when original LinearBasis is modified (NeedCopy=false).");
    AssertBasisOrthonormal(ab); // lb was modified correctly, so ab should still be valid
  }


  [Test]
  public void Constructor_FromPoints_Line() {
    Vector p1 = V(1, 1, 1);
    Vector p2 = V(3, 1, 1);
    Vector p3 = V(-1, 1, 1);

    AffineBasis ab = new AffineBasis(new List<Vector> { p1, p2, p3 });

    AssertVectorsAreEqual(ab.Origin, p1);
    Assert.That(ab.SpaceDim, Is.EqualTo(3));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(1), "Should only find one independent direction (p2-p1).");
    AssertVectorsAreEqual(ab[0], V(1, 0, 0), "Basis vector should be normalized direction.");
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Constructor_FromPoints_Plane() {
    Vector p1 = V(0, 0, 0);
    Vector p2 = V(2, 0, 0);
    Vector p3 = V(0, 3, 0);
    Vector p4 = V(1, 1, 0);

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
    Assert.That(ab.Empty, Is.True);
  }

  [Test]
  public void Constructor_CopyConstructor() {
    Vector      origin   = V(1, 2, 3);
    LinearBasis lb       = LinearBasis.GenLinearBasis(spaceDim:3, subSpaceDim:2);
    AffineBasis original = new AffineBasis(origin, lb);
    AffineBasis copy     = new AffineBasis(original);

    AssertVectorsAreEqual(copy.Origin, original.Origin);
    Assert.That(copy.LinBasis.Equals(original.LinBasis), Is.True, "Linear bases should be equal.");
    Assert.That(copy.LinBasis, Is.Not.SameAs(original.LinBasis), "Linear basis should be a copy.");
    Assert.That(copy.SpaceDim, Is.EqualTo(original.SpaceDim));
    Assert.That(copy.SubSpaceDim, Is.EqualTo(original.SubSpaceDim));

    // Modify copy's linear basis, original should not change
    copy.AddVector(V(0, 0, 1));
    Assert.That(original.SubSpaceDim, Is.EqualTo(2), "Original SubSpaceDim should remain unchanged.");
  }
#endregion

#region Factory Tests
  [Test]
  public void Factory_FromVectors() {
    Vector      o  = V(1, 1, 0);
    Vector      v1 = V(2, 0, 0);
    Vector      v2 = V(0, 0, 3);
    AffineBasis ab = AffineBasis.FromVectors(o, new List<Vector> { v1, v2 });

    AssertVectorsAreEqual(ab.Origin, o);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 0, 1));
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Factory_FromPoints() {
    Vector      o  = V(1, 1, 1);
    Vector      p1 = V(3, 1, 1);
    Vector      p2 = V(1, 1, 4);
    AffineBasis ab = AffineBasis.FromPoints(o, new List<Vector> { p1, p2 });

    AssertVectorsAreEqual(ab.Origin, o);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(ab[0], V(1, 0, 0));
    AssertVectorsAreEqual(ab[1], V(0, 0, 1));
    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void Factory_GenAffineBasis() {
    AffineBasis ab = AffineBasis.GenAffineBasis(spaceDim:4, subSpaceDim:2);
    Assert.That(ab.SpaceDim, Is.EqualTo(4));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(2));
    AssertBasisOrthonormal(ab);
  }
#endregion

#region Method Tests
  [Test]
  public void Method_AddVector() {
    Vector      o  = V(1, 1, 1);
    AffineBasis ab = new AffineBasis(o, new LinearBasis(V(1, 0, 0)));
    Assert.That(ab.SubSpaceDim, Is.EqualTo(1));

    bool added1 = ab.AddVector(V(0, 5, 0));
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

    bool added4 = ab.AddVector(V(1, 1, 1));
    Assert.That(added4, Is.False);
    Assert.That(ab.SubSpaceDim, Is.EqualTo(3));

    AssertBasisOrthonormal(ab);
  }

  [Test]
  public void ProjectToAffineSpace_Reflect() {
    Vector origin = V(0, 0);
    List<Vector> basis = new List<Vector> { V(-1, 0), V(0, -1) };
    SortedSet<Vector> swarm = new SortedSet<Vector> { V(1, 1), V(2, 3), V(-1, 4) };
    SortedSet<Vector> expected = new SortedSet<Vector> { V(-1, -1), V(-2, -3), V(1, -4) };

    AffineBasis         aBasis = AffineBasis.FromVectors(origin, basis, false);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

  [Test]
  public void ProjectToAffineSpace_ReflectAndShift() {
    Vector origin = V(2, 2);
    List<Vector> basis = new List<Vector> { V(-1, 0), V(0, -1) };
    SortedSet<Vector> swarm = new SortedSet<Vector> { V(1, 1), V(2, 4), V(-4, 4) };
    SortedSet<Vector> expected = new SortedSet<Vector> { V(1, 1), V(0, -2), V(6, -2) };

    AffineBasis         aBasis = AffineBasis.FromVectors(origin, basis, false);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }


  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Plane() {
    Vector      o  = V(0, 0, 1);
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
    Vector      o  = V(1, 1, 1);
    LinearBasis lb = new LinearBasis(V(1, 0, 0));
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p_on  = V(5, 1, 1);
    Vector p_off = V(5, 2, 1);

    Vector proj_on  = ab.ProjectPointToSubSpace_in_OrigSpace(p_on);
    Vector proj_off = ab.ProjectPointToSubSpace_in_OrigSpace(p_off);

    AssertVectorsAreEqual(proj_on, p_on, "Projection of point on line should be itself.");
    AssertVectorsAreEqual(proj_off, V(5, 1, 1), "Projection of point off line should land on line.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Point() {
    Vector      o  = V(1, 2, 3);
    AffineBasis ab = new AffineBasis(o);

    Vector p    = V(5, 5, 5);
    Vector proj = ab.ProjectPointToSubSpace_in_OrigSpace(p);

    AssertVectorsAreEqual(proj, o, "Projection onto a point should be the point itself.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_Coordinates_Plane() {
    Vector      o  = V(0, 0, 1);
    LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p = V(3, 4, 5);
    Vector coords         = ab.ProjectPointToSubSpace(p);
    Vector expectedCoords = V(3, 4);

    Assert.That(coords.SpaceDim, Is.EqualTo(ab.SubSpaceDim), "Coordinate dimension should match SubSpaceDim.");
    AssertVectorsAreEqual(coords, expectedCoords);
  }

  [Test]
  public void Method_ProjectPointToSubSpace_Coordinates_Line() {
    Vector      o  = V(1, 1, 1);
    LinearBasis lb = new LinearBasis(V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0));
    AffineBasis ab = new AffineBasis(o, lb);

    Vector p = V(3, 3, 1);
    Vector coords         = ab.ProjectPointToSubSpace(p);
    Vector expectedCoords = V(Math.Sqrt(8));

    Assert.That(coords.SpaceDim, Is.EqualTo(1));
    AssertVectorsAreEqual(coords, expectedCoords);
  }

  [Test]
  public void Method_TranslateToOriginal_Plane() {
    Vector      o  = V(0, 0, 1);
    LinearBasis lb = new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) });
    AffineBasis ab = new AffineBasis(o, lb);

    Vector coords = V(3, 4);
    Vector originalPoint = ab.ToOriginalCoords(coords);
    Vector expectedPoint = V(3, 4, 1);

    AssertVectorsAreEqual(originalPoint, expectedPoint);
  }

  [Test]
  public void Method_TranslateToOriginal_Line() {
    Vector      o  = V(1, 1, 1);
    Vector      b  = V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0);
    LinearBasis lb = new LinearBasis(b);
    AffineBasis ab = new AffineBasis(o, lb);

    Vector coords = V(Math.Sqrt(8));
    Vector originalPoint = ab.ToOriginalCoords(coords);
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
  //     (() => ab.ToOriginalCoords(coords1D), "ToOriginalCoords should throw if coord dim != SubSpaceDim");
  //   Assert.Throws<Exception>
  //     (() => ab.ToOriginalCoords(coords3D), "ToOriginalCoords should throw if coord dim != SubSpaceDim");
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

    AffineBasis ab3 = new AffineBasis(o, new LinearBasis(lb, (bool)TODO));
    Assert.That(ab1.Equals(ab3), Is.True);

    Vector      o4  = o + ab1[0] * 5.0 + ab1[1] * (-3.0);
    AffineBasis ab4 = new AffineBasis(o4, new LinearBasis(lb, (bool)TODO));
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
