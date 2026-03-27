using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.AffineBasisAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class AffineBasisFactoriesAndMutationTests {

  [Test]
  public void Factory_FromVectors() {
    Vector origin = V(1, 1, 0);
    Vector v1 = V(2, 0, 0);
    Vector v2 = V(0, 0, 3);
    AffineBasis basis = AffineBasis.FromVectors(origin, new List<Vector> { v1, v2 });

    AreEqual(basis.Origin, origin);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AreEqual(basis[0], V(1, 0, 0));
    AreEqual(basis[1], V(0, 0, 1));
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Factory_FromPoints() {
    Vector origin = V(1, 1, 1);
    Vector p1 = V(3, 1, 1);
    Vector p2 = V(1, 1, 4);
    AffineBasis basis = AffineBasis.FromPoints(origin, new List<Vector> { p1, p2 });

    AreEqual(basis.Origin, origin);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AreEqual(basis[0], V(1, 0, 0));
    AreEqual(basis[1], V(0, 0, 1));
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Factory_GenAffineBasis() {
    AffineBasis basis = AffineBasis.GenAffineBasis(spaceDim: 4, subSpaceDim: 2);
    Assert.That(basis.SpaceDim, Is.EqualTo(4));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Method_AddVector() {
    Vector origin = V(1, 1, 1);
    AffineBasisMutable basis = new AffineBasisMutable(origin, new LinearBasis(V(1, 0, 0)), false);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1));

    bool added1 = basis.AddVector(V(0, 5, 0));
    Assert.That(added1, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AreEqual(basis[0], V(1, 0, 0));
    AreEqual(basis[1], V(0, 1, 0));
    Matrix projectionMatrixBefore = basis.LinBasis.ProjMatrix;

    bool added2 = basis.AddVector(V(3, 0, 0));
    Assert.That(added2, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    Assert.That(basis.LinBasis.ProjMatrix, Is.EqualTo(projectionMatrixBefore));

    bool added3 = basis.AddVector(V(0, 0, 1));
    Assert.That(added3, Is.True);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    AreEqual(basis[2], V(0, 0, 1));
    Assert.That(basis.LinBasis.ProjMatrix, Is.Not.EqualTo(projectionMatrixBefore), "If AddVector adds the vector it must set _projMatrix to null!");

    bool added4 = basis.AddVector(V(1, 1, 1));
    Assert.That(added4, Is.False);
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    IsBasisOrthonormal(basis);
  }

}
