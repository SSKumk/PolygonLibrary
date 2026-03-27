using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class VectorConstructionAndIdentityTests {

  [Test]
  public void Constructor_Default_CreatesZeroVector() {
    int dim = 3;
    Vector v = new Vector(dim);
    Assert.That(v.SpaceDim, Is.EqualTo(dim));
    Assert.That(v.IsZero, Is.True);
    AreEqual(v, Vector.Zero(dim));
  }

  [Test]
  public void Constructor_FromIntEnumerable_ConvertsCoordinatesToDouble() {
    Vector v = new Vector(new[] { 1, -2, 3 });

    AreEqual(v, V(1, -2, 3));
  }

  [Test]
  public void Constructor_FromArray_NeedCopyTrue_CopiesInputArray() {
    double[] originalArray = { 1.0, 2.0, 3.0 };
    Vector v = new Vector(originalArray, needCopy: true);

    Assert.That(v.SpaceDim, Is.EqualTo(3));
    Assert.That(v[0], Is.EqualTo(1.0));
    Assert.That(v[1], Is.EqualTo(2.0));
    Assert.That(v[2], Is.EqualTo(3.0));

    originalArray[0] = 10.0;
    Assert.That(v[0], Is.EqualTo(1.0), "Vector should have its own copy of the array.");
  }

  [Test]
  public void Constructor_FromArray_NeedCopyFalse_UsesOriginalArray() {
    double[] originalArray = { 1.0, 2.0, 3.0 };
    Vector v = new Vector(originalArray, needCopy: false);

    Assert.That(v.SpaceDim, Is.EqualTo(3));
    Assert.That(v[0], Is.EqualTo(1.0));

    originalArray[0] = 10.0;
    Assert.That(v[0], Is.EqualTo(10.0), "Vector should reference the original array.");
  }

  [Test]
  public void Constructor_CopyConstructor_CreatesIndependentEquivalentObject() {
    Vector original = V(1, 2, 3);
    Vector copy = new Vector(original);

    AreEqual(original, copy);
    Assert.That(copy, Is.Not.SameAs(original));
  }

  [Test]
  public void Constructor_FromVector2D_CreatesTwoDimensionalVector() {
    Vector2D source = new Vector2D(1.5, -2.0);
    Vector v = new Vector(source);

    AreEqual(v, V(1.5, -2.0));
  }

  [Test]
  public void SpaceDimAndIndexer_ReturnCoordinateStorageShape() {
    Assert.That(V(1, 2).SpaceDim, Is.EqualTo(2));
    Assert.That(V(1, 2, 3, 4, 5).SpaceDim, Is.EqualTo(5));

    Vector v = V(5, -2, 0);
    Assert.That(v[0], Is.EqualTo(5.0));
    Assert.That(v[1], Is.EqualTo(-2.0));
    Assert.That(v[2], Is.EqualTo(0.0));
  }

  [Test]
  public void ExplicitCastToArray_ReturnsCopyOfCoordinates() {
    Vector v = V(10, 20);
    double[] arr = (double[])v;

    Assert.That(arr.Length, Is.EqualTo(2));
    Assert.That(arr[0], Is.EqualTo(10.0));
    Assert.That(arr[1], Is.EqualTo(20.0));

    arr[0] = 0.0;
    Assert.That(v[0], Is.EqualTo(10.0), "Vector should not be affected by modifying array returned by explicit cast.");
  }

  [Test]
  public void Method_GetAsArray_ReturnsCopy() {
    Vector v = V(10, 20);
    double[] arr = v.GetCopyAsArray();

    Assert.That(arr.Length, Is.EqualTo(2));
    Assert.That(arr[0], Is.EqualTo(10.0));
    Assert.That(arr[1], Is.EqualTo(20.0));

    arr[0] = 0.0;
    Assert.That(v[0], Is.EqualTo(10.0), "Vector should not be affected by modifying array returned by GetCopyAsArray.");
  }

  [Test]
  public void IsZero_ReflectsZeroAndNonZeroVectors() {
    Assert.That(Vector.Zero(3).IsZero, Is.True);
    Assert.That(V(Tools.Eps * 2.0, 0.0, 0.0).IsZero, Is.False);
  }

}
