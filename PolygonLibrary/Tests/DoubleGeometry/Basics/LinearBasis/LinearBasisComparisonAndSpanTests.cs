using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class LinearBasisComparisonAndSpanTests {

  [Test]
  public void Equals_SameBasisObject() {
    LinearBasis basis = new LinearBasis(V(1, 0), V(0, 1));
    Assert.That(basis.Equals(basis), Is.True);
  }

  [Test]
  public void Equals_DifferentObjectsSameSpace() {
    LinearBasis basis1 = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis basis2 = new LinearBasis(V(0, 1, 0), V(1, 0, 0));
    LinearBasis basis3 = new LinearBasis(
      V(1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0),
      V(1 / Math.Sqrt(2), -1 / Math.Sqrt(2), 0)
    );

    Assert.That(basis1.Equals(basis2));
    Assert.That(basis1.Equals(basis3));
    Assert.That(basis2.Equals(basis3));
  }

  [Test]
  public void Equals_DifferentSubspaces() {
    LinearBasis basisXY = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis basisYZ = new LinearBasis(V(0, 1, 0), V(0, 0, 1));
    LinearBasis basisX = new LinearBasis(V(1, 0, 0));
    LinearBasis basisFull = new LinearBasis(3);

    Assert.That(basisXY, Is.Not.EqualTo(basisYZ));
    Assert.That(basisXY, Is.Not.EqualTo(basisX));
    Assert.That(basisX, Is.Not.EqualTo(basisXY));
    Assert.That(basisXY, Is.Not.EqualTo(basisFull));
    Assert.That(basisFull, Is.Not.EqualTo(basisXY));
  }

  [Test]
  public void Equals_DifferentSpaceDim() {
    LinearBasis basis3D = new LinearBasis(V(1, 0, 0));
    LinearBasis basis4D = new LinearBasis(V(1, 0, 0, 0));
    Assert.That(!basis3D.Equals(basis4D));
  }

  [Test]
  public void Equals_NullOrDifferentType() {
    LinearBasis basis = new LinearBasis(V(1, 0));
    Assert.That(basis, Is.Not.EqualTo(null));
    Assert.That(basis, Is.Not.EqualTo(new object()));
  }

  [Test]
  public void CompareTo_Null_Returns1() {
    LinearBasis lb1 = new LinearBasis(V(1, 0, 0));
    Assert.That(lb1.CompareTo(null), Is.EqualTo(1));
  }

  [Test]
  public void CompareTo_EqualBases_Returns0() {
    LinearBasis lb1 = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis lb2 = new LinearBasis(V(1, 1, 0), V(1, -1, 0));

    Assert.That(lb1.CompareTo(lb2), Is.EqualTo(0));
  }

  [Test]
  public void CompareTo_Order_BySpaceDim() {
    LinearBasis lbR2 = new LinearBasis(2, 1);
    LinearBasis lbR3 = new LinearBasis(3, 1);

    Assert.That(lbR2.CompareTo(lbR3), Is.LessThan(0));
    Assert.That(lbR3.CompareTo(lbR2), Is.GreaterThan(0));
  }

  [Test]
  public void CompareTo_Order_BySubspaceDim() {
    LinearBasis lbLineInR3 = new LinearBasis(3, 1);
    LinearBasis lbPlaneInR3 = new LinearBasis(3, 2);

    Assert.That(lbLineInR3.CompareTo(lbPlaneInR3), Is.LessThan(0));
    Assert.That(lbPlaneInR3.CompareTo(lbLineInR3), Is.GreaterThan(0));
  }

  [Test]
  public void CompareTo_Order_ByRREF() {
    LinearBasis lbXY = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis lbXZ = new LinearBasis(V(1, 0, 0), V(0, 0, 1));
    LinearBasis lbYZ = new LinearBasis(V(0, 1, 0), V(0, 0, 1));

    Assert.That(lbXY.CompareTo(lbXZ), Is.GreaterThan(0));
    Assert.That(lbXZ.CompareTo(lbXY), Is.LessThan(0));
    Assert.That(lbXY.CompareTo(lbYZ), Is.GreaterThan(0));
    Assert.That(lbXZ.CompareTo(lbYZ), Is.GreaterThan(0));
  }

  [Test]
  public void GetEnumerator_IteratesCorrectly() {
    Vector v1 = V(1, 0, 0);
    Vector v2 = V(0, 1, 0);
    LinearBasis basis = new LinearBasis(v1, v2);

    List<Vector> vectorsFromIterator = new();
    foreach (Vector v in basis) {
      vectorsFromIterator.Add(v);
    }

    Assert.That(vectorsFromIterator.Count, Is.EqualTo(2));
    AreEqual(vectorsFromIterator[0], basis[0]);
    AreEqual(vectorsFromIterator[1], basis[1]);
  }

  [Test]
  public void GetEnumerator_EmptyBasis() {
    LinearBasis basis = new LinearBasis(3, 0);
    int count = 0;
    foreach (Vector _ in basis) {
      count++;
    }

    Assert.That(count, Is.EqualTo(0));
  }

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
    Vector v1 = V(1, 0, 0);
    Vector v2 = V(0, 1, 0);
    LinearBasis basis1 = new LinearBasis(v1, v2);
    LinearBasis basis2 = new LinearBasis(v2, v1);

    Assert.That(basis1.SpanSameSpace(basis2), Is.True);
    Assert.That(basis2.SpanSameSpace(basis1), Is.True);
  }

  [Test]
  public void SpanSameSpace_EquivalentBases_RotatedXYPlane() {
    LinearBasis basisXY = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    double ca = Math.Cos(Math.PI / 4.0);
    double sa = Math.Sin(Math.PI / 4.0);
    LinearBasis basisRot = new LinearBasis(V(ca, sa, 0), V(-sa, ca, 0));

    Assert.That(basisXY.SubSpaceDim, Is.EqualTo(basisRot.SubSpaceDim));
    Assert.That(basisXY.SpanSameSpace(basisRot), Is.True, "Standard XY and rotated XY should span the same space.");
    Assert.That(basisRot.SpanSameSpace(basisXY), Is.True, "Symmetry check for rotated basis.");
  }

  [Test]
  public void SpanSameSpace_EquivalentBases_GeneratedVsStandard() {
    LinearBasis basisStd = new LinearBasis(3);
    GRandomLC rnd = new GRandomLC(54321);
    LinearBasis basisGen = LinearBasis.GenLinearBasis(3, 3, rnd);

    Assert.That(basisStd.FullDim, Is.True);
    Assert.That(basisGen.FullDim, Is.True);
    Assert.That(basisStd.SpanSameSpace(basisGen), Is.True, "Standard R3 and generated full R3 should span the same space.");
    Assert.That(basisGen.SpanSameSpace(basisStd), Is.True, "Symmetry check for generated R3.");
  }

  [Test]
  public void SpanSameSpace_DifferentSubSpaceDim() {
    LinearBasis basis1D = new LinearBasis(new[] { V(1, 0, 0) });
    LinearBasis basis2D = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis basisEmpty = new LinearBasis(3, 0);

    Assert.That(basis1D.SpanSameSpace(basis2D), Is.False);
    Assert.That(basis2D.SpanSameSpace(basis1D), Is.False);
    Assert.That(basis1D.SpanSameSpace(basisEmpty), Is.False);
    Assert.That(basisEmpty.SpanSameSpace(basis1D), Is.False);
  }

  [Test]
  public void SpanSameSpace_SameSubSpaceDimDifferentSpace() {
    LinearBasis basisXY = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    LinearBasis basisXZ = new LinearBasis(V(1, 0, 0), V(0, 0, 1));
    LinearBasis basisDiag = new LinearBasis(V(1, 1, 0).Normalize(), V(0, 0, 1));

    Assert.That(basisXY.SpanSameSpace(basisXZ), Is.False, "XY vs XZ plane");
    Assert.That(basisXZ.SpanSameSpace(basisXY), Is.False);
    Assert.That(basisXY.SpanSameSpace(basisDiag), Is.False, "XY vs Diagonal plane");
    Assert.That(basisDiag.SpanSameSpace(basisXY), Is.False);
  }

  [Test]
  public void SpanSameSpace_DifferentSpaceDim() {
    LinearBasis basis2D = new LinearBasis(new[] { V(1, 0) });
    LinearBasis basis3D = new LinearBasis(new[] { V(1, 0, 0) });

    Assert.That(basis2D.SpanSameSpace(basis3D), Is.False);
    Assert.That(basis3D.SpanSameSpace(basis2D), Is.False);
  }

  [Test]
  public void SpanSameSpace_BothEmpty() {
    LinearBasis basisEmpty1 = new LinearBasis(3, 0);
    LinearBasis basisEmpty2 = new LinearBasis(3, 0);
    LinearBasis basisEmpty4D = new LinearBasis(4, 0);

    Assert.That(basisEmpty1.SpanSameSpace(basisEmpty2), Is.True, "Two empty bases in same SpaceDim.");
    Assert.That(basisEmpty1.SpanSameSpace(basisEmpty4D), Is.False, "Empty bases in different SpaceDim.");
  }

}
