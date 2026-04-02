using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.AffineBasisAssert;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class AffineBasisConstructionTests {

  [Test]
  public void Constructor_DefaultDim_CreatesFullDimStandardBasisAtOrigin() {
    AffineBasis basis = new AffineBasis(3);

    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3));
    Assert.That(basis.FullDim, Is.True);
    Assert.That(basis.Empty, Is.False);
    AreEqual(basis.Origin, Vector.Zero(3));
    AreEqual(basis[0], V(1, 0, 0));
    AreEqual(basis[1], V(0, 1, 0));
    AreEqual(basis[2], V(0, 0, 1));
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_OriginOnly_CreatesZeroSubspaceDimBasis() {
    Vector origin = V(1, 2, 3);
    AffineBasis basis = new AffineBasis(origin);

    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(0));
    Assert.That(basis.FullDim, Is.False);
    Assert.That(basis.Empty, Is.True);
    AreEqual(basis.Origin, origin);
  }

  [Test]
  public void Constructor_OriginAndLinearBasis_Immutable_NeedCopyFalse() {
    Vector origin = V(1, 1, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis, needCopy: false);

    AreEqual(basis.Origin, origin);
    Assert.That(basis.LinBasis, Is.EqualTo(linearBasis));
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_OriginAndLinearBasis_Immutable_NeedCopyTrue() {
    Vector origin = V(1, 1, 1);
    LinearBasis linearBasis = new LinearBasis(V(1, 0, 0), V(0, 1, 0));
    AffineBasis basis = new AffineBasis(origin, linearBasis, needCopy: true);

    AreEqual(basis.Origin, origin);
    Assert.That(basis.LinBasis, Is.EqualTo(linearBasis));
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_OriginAndLinearBasis_ThrowsForMutableBasisWithoutCopy() {
    Vector origin = V(1, 1, 1);
    LinearBasisMutable linearBasis = new LinearBasisMutable(V(1, 0, 0), V(0, 1, 0));

    Assert.Throws<ArgumentException>(
      () => new AffineBasis(origin, linearBasis, needCopy: false),
      "Found LinearBasisMutable in AffineBasis constructor!"
    );
  }

  [Test]
  public void Constructor_OriginAndLinearBasis_Mutable_NeedCopyTrue() {
    Vector origin = V(1, 1, 1);
    LinearBasisMutable linearBasis = new LinearBasisMutable(V(1, 0, 0), V(0, 1, 0));
    AffineBasisMutable basis = new AffineBasisMutable(origin, linearBasis, needCopy: true);

    AreEqual(basis.Origin, origin);
    Assert.That(basis.LinBasis, Is.EqualTo(linearBasis));
    Assert.That(basis.LinBasis, Is.Not.SameAs(linearBasis));

    linearBasis.AddVector(V(0, 0, 1));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2), "Affine basis should not change when original LinearBasis is modified (NeedCopy=true).");
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_OriginAndLinearBasis_Mutable_NeedCopyFalse() {
    Vector origin = V(1, 1, 1);
    LinearBasisMutable linearBasis = new LinearBasisMutable(V(1, 0, 0), V(0, 1, 0));
    AffineBasisMutable basis = new AffineBasisMutable(origin, linearBasis, needCopy: false);

    AreEqual(basis.Origin, origin);
    Assert.That(basis.LinBasis, Is.EqualTo(linearBasis));
    Assert.That(basis.LinBasis, Is.SameAs(linearBasis));

    linearBasis.AddVector(V(0, 0, 1));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(3), "Affine basis should change when original LinearBasis is modified (NeedCopy=false).");
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_FromPoints_Line() {
    Vector p1 = V(1, 1, 1);
    Vector p2 = V(3, 1, 1);
    Vector p3 = V(-1, 1, 1);

    AffineBasis basis = new AffineBasis(new List<Vector> { p1, p2, p3 });

    AreEqual(basis.Origin, p1);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(1), "Should only find one independent direction (p2-p1).");
    AreEqual(basis[0], V(1, 0, 0), "Basis vector should be normalized direction.");
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_FromPoints_Plane() {
    Vector p1 = V(0, 0, 0);
    Vector p2 = V(2, 0, 0);
    Vector p3 = V(0, 3, 0);
    Vector p4 = V(1, 1, 0);

    AffineBasis basis = new AffineBasis(new List<Vector> { p1, p2, p3, p4 });

    AreEqual(basis.Origin, p1);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2), "Should find two independent directions.");
    Assert.That(basis.LinBasis.Contains(V(1, 0, 0)), Is.True);
    Assert.That(basis.LinBasis.Contains(V(0, 1, 0)), Is.True);
    Assert.That(basis.LinBasis.Contains(V(0, 0, 1)), Is.False);
    IsBasisOrthonormal(basis);
  }

  [Test]
  public void Constructor_FromPoints_SinglePoint() {
    Vector point = V(5, 6, 7);
    AffineBasis basis = new AffineBasis(new List<Vector> { point });

    AreEqual(basis.Origin, point);
    Assert.That(basis.SpaceDim, Is.EqualTo(3));
    Assert.That(basis.SubSpaceDim, Is.EqualTo(0));
    Assert.That(basis.Empty, Is.True);
  }

  [Test]
  public void Constructor_CopyConstructor() {
    Vector origin = V(1, 2, 3);
    LinearBasis linearBasis = LinearBasis.GenLinearBasis(spaceDim: 3, subSpaceDim: 2);
    AffineBasis original = new AffineBasis(origin, linearBasis);
    AffineBasisMutable copy = new AffineBasisMutable(original, true);

    AreEqual(copy.Origin, original.Origin);
    Assert.That(copy.LinBasis, Is.EqualTo(original.LinBasis), "Linear bases should be equal.");
    Assert.That(copy.LinBasis, Is.Not.SameAs(original.LinBasis), "Linear basis should be a copy.");
    Assert.That(copy.SpaceDim, Is.EqualTo(original.SpaceDim));
    Assert.That(copy.SubSpaceDim, Is.EqualTo(original.SubSpaceDim));

    copy.AddVector(V(0, 0, 1));
    Assert.That(original.SubSpaceDim, Is.EqualTo(2), "Original SubSpaceDim should remain unchanged.");
  }

  [Test]
  public void Constructor_CopyConstructor_ImmutableSource_NeedCopyFalse_SharesLinearBasis() {
    Vector origin = V(1, 2, 3);
    AffineBasis original = new AffineBasis(origin, LinearBasis.GenLinearBasis(spaceDim: 3, subSpaceDim: 2));
    AffineBasis copy = new AffineBasis(original, needCopy: false);

    AreEqual(copy.Origin, original.Origin);
    Assert.That(copy.LinBasis, Is.SameAs(original.LinBasis), "Immutable AffineBasis should allow zero-copy sharing of the internal linear basis.");
    Assert.That(copy.SubSpaceDim, Is.EqualTo(original.SubSpaceDim));
    Assert.That(copy, Is.EqualTo(original));
  }

  [Test]
  public void Constructor_CopyConstructor_MutableSource_NeedCopyFalse_Throws() {
    AffineBasisMutable original = new AffineBasisMutable(V(1, 2, 3), new LinearBasisMutable(V(1, 0, 0), V(0, 1, 0)), needCopy: false);

    Assert.Throws<ArgumentException>(
      () => new AffineBasis(original, needCopy: false),
      "Found AffineBasisMutable in AffineBasis copy constructor!"
    );
  }

}
