using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class AffineBasisProjectionAndContainmentTests {

  [Test]
  public void ProjectPoints_Reflect() {
    Vector origin = V(0, 0);
    List<Vector> basisVectors = new() { V(-1, 0), V(0, -1) };
    SortedSet<Vector> swarm = new() { V(1, 1), V(2, 3), V(-1, 4) };
    SortedSet<Vector> expected = new() { V(-1, -1), V(-2, -3), V(1, -4) };

    AffineBasis basis = AffineBasis.FromVectors(origin, basisVectors);
    IEnumerable<Vector> result = basis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

  [Test]
  public void ProjectPoints_ReflectAndShift() {
    Vector origin = V(2, 2);
    List<Vector> basisVectors = new() { V(-1, 0), V(0, -1) };
    SortedSet<Vector> swarm = new() { V(1, 1), V(2, 4), V(-4, 4) };
    SortedSet<Vector> expected = new() { V(1, 1), V(0, -2), V(6, -2) };

    AffineBasis basis = AffineBasis.FromVectors(origin, basisVectors);
    IEnumerable<Vector> result = basis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Plane() {
    Vector origin = V(0, 0, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Vector pAbove = V(3, 4, 5);
    Vector pOn = V(3, 4, 1);
    Vector pOrigin = basis.Origin;

    Vector projAbove = basis.ProjectPointToSubSpace_in_OrigSpace(pAbove);
    Vector projOn = basis.ProjectPointToSubSpace_in_OrigSpace(pOn);
    Vector projOrigin = basis.ProjectPointToSubSpace_in_OrigSpace(pOrigin);

    AreEqual(projAbove, V(3, 4, 1), "Projection of point above should land on plane.");
    AreEqual(projOn, pOn, "Projection of point on plane should be itself.");
    AreEqual(projOrigin, pOrigin, "Projection of origin should be itself.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Line() {
    Vector origin = V(1, 1, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Vector pOn = V(5, 1, 1);
    Vector pOff = V(5, 2, 1);

    AreEqual(basis.ProjectPointToSubSpace_in_OrigSpace(pOn), pOn, "Projection of point on line should be itself.");
    AreEqual(basis.ProjectPointToSubSpace_in_OrigSpace(pOff), V(5, 1, 1), "Projection of point off line should land on line.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_in_OrigSpace_Point() {
    Vector origin = V(1, 2, 3);
    AffineBasis basis = new AffineBasis(origin);

    Vector point = V(5, 5, 5);
    Vector projection = basis.ProjectPointToSubSpace_in_OrigSpace(point);

    AreEqual(projection, origin, "Projection onto a point should be the point itself.");
  }

  [Test]
  public void Method_ProjectPointToSubSpace_Coordinates_Plane() {
    Vector origin = V(0, 0, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Vector point = V(3, 4, 5);
    Vector coords = basis.ProjectPointToSubSpace(point);

    Assert.That(coords.SpaceDim, Is.EqualTo(basis.SubSpaceDim), "Coordinate dimension should match SubSpaceDim.");
    AreEqual(coords, V(3, 4));
  }

  [Test]
  public void Method_ProjectPointToSubSpace_Coordinates_Line() {
    Vector origin = V(1, 1, 1);
    LinearBasis linearBasis = new LinearBasis(V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Vector point = V(3, 3, 1);
    Vector coords = basis.ProjectPointToSubSpace(point);

    Assert.That(coords.SpaceDim, Is.EqualTo(1));
    AreEqual(coords, V(Math.Sqrt(8)));
  }

  [Test]
  public void Method_TranslateToOriginal_Plane() {
    Vector origin = V(0, 0, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Vector originalPoint = basis.ToOriginalCoords(V(3, 4));
    AreEqual(originalPoint, V(3, 4, 1));
  }

  [Test]
  public void Method_TranslateToOriginal_Line() {
    Vector origin = V(1, 1, 1);
    Vector direction = V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0);
    LinearBasis linearBasis = new LinearBasis(direction);
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Vector originalPoint = basis.ToOriginalCoords(V(Math.Sqrt(8)));
    AreEqual(originalPoint, V(3, 3, 1));
  }

  [Test]
  public void Method_ToOriginalCoords_ForEnumerable() {
    AffineBasis basis = new AffineBasis(V(1, 2, 3), new LinearBasis(V(1, 0, 0), V(0, 1, 0)));
    List<Vector> points = basis.ToOriginalCoords(new[] { V(0, 0), V(2, -1) }).ToList();

    Assert.That(points.Count, Is.EqualTo(2));
    AreEqual(points[0], V(1, 2, 3));
    AreEqual(points[1], V(3, 1, 3));
  }

  [Test]
  public void Method_CanonicalOrigin_ReturnsProjectionOfZeroOntoAffineSpace() {
    AffineBasis lineYX = new AffineBasis(V(5, 5), new LinearBasis(V(1, 1)));
    AffineBasis shiftedLine = new AffineBasis(V(2, 3), new LinearBasis(V(1, 1)));

    AreEqual(lineYX.CanonicalOrigin, V(0, 0));
    AreEqual(shiftedLine.CanonicalOrigin, V(-0.5, 0.5));
  }

  [Test]
  public void Method_Contains_Plane() {
    Vector origin = V(0, 0, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis);

    Assert.That(basis.Contains(V(5, -2, 1)), Is.True);
    Assert.That(basis.Contains(V(5, -2, 2)), Is.False);
    Assert.That(basis.Contains(basis.Origin), Is.True);
  }

  [Test]
  public void Method_Contains_FullDim() {
    AffineBasis basis = new AffineBasis(3);
    Assert.That(basis.Contains(V(12, -5, 100)), Is.True, "Full dimensional affine basis should contain all points.");
  }

  [Test]
  public void Method_Contains_Point() {
    Vector origin = V(1, 2, 3);
    AffineBasis basis = new AffineBasis(origin);

    Assert.That(basis.Contains(origin), Is.True, "0-dim basis should contain its origin.");
    Assert.That(basis.Contains(V(1, 2, 4)), Is.False, "0-dim basis should not contain other points.");
    Assert.That(basis.Contains(Vector.Zero(3)), Is.False, "0-dim basis at non-zero origin should not contain zero.");
  }

  [Test]
  public void Method_Contains_Point_Cube() {
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 0, 1));
    AffineBasis basis = new AffineBasis(V(0, 1, 0), linearBasis);

    Assert.That(basis.Contains(V(1, 1, 1)), Is.True);
    Assert.That(basis.Contains(V(1, 1, 0)), Is.True);
    Assert.That(basis.Contains(V(0, 1, 1)), Is.True);
    Assert.That(basis.Contains(V(0.5, 1, 0.5)), Is.True);
  }

  [Test]
  public void Method_OrthogonalComplementVector_DelegatesToLinearBasisComplement() {
    AffineBasis basis = new AffineBasis(V(0, 0, 0, 0), new LinearBasis(V(1, 0, 0, 0)));
    Vector ortho = basis.OrthogonalComplementVector();

    Assert.That(ortho.IsZero, Is.False);
    Assert.That(ortho.Length, Is.EqualTo(1.0).Within(Tools.Eps));
    Assert.That(ortho * basis[0], Is.EqualTo(0.0).Within(Tools.Eps));
  }

}

