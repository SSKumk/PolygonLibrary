using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>; // Используем double
using static Tests.SharedTests.StaticHelpers;              // Используем общий V()
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Tests.SharedTests; // Используем общй namespace

[TestFixture]
public class VectorTests {


  // Helper to check if ArgumentException related to dimension mismatch is thrown
  private void AssertThrowsDimMismatch(TestDelegate code, string message = "") {
    Assert.Throws(Is.InstanceOf<Exception>().And.Message.Contains("different dimensions"), code, message);
  }

#region Constructors and Factories Tests
  [Test]
  public void Constructor_Default_CreatesZeroVector() {
    int    dim = 3;
    Vector v   = new Vector(dim);
    Assert.That(v.SpaceDim, Is.EqualTo(dim));
    Assert.That(v.IsZero, Is.True);
    AssertVectorsAreEqual(v, Vector.Zero(dim));
  }

  [Test]
  public void Constructor_FromDoubleArray_NeedCopyTrue() {
    double[] originalArray = new double[] { 1.0, 2.0, 3.0 };
    Vector   v             = new Vector(originalArray, needCopy: true);

    Assert.That(v.SpaceDim, Is.EqualTo(3));
    Assert.That(v[0], Is.EqualTo(1.0));
    Assert.That(v[1], Is.EqualTo(2.0));
    Assert.That(v[2], Is.EqualTo(3.0));

    // Modify original array, vector should not change
    originalArray[0] = 10.0;
    Assert.That(v[0], Is.EqualTo(1.0), "Vector should have its own copy of the array.");
  }

  [Test]
  public void Constructor_FromDoubleArray_NeedCopyFalse() {
    double[] originalArray = new double[] { 1.0, 2.0, 3.0 };
    Vector   v             = new Vector(originalArray, needCopy: false);

    Assert.That(v.SpaceDim, Is.EqualTo(3));
    Assert.That(v[0], Is.EqualTo(1.0));

    // Modify original array, vector SHOULD change
    originalArray[0] = 10.0;
    Assert.That(v[0], Is.EqualTo(10.0), "Vector should reference the original array.");
  }

  [Test]
  public void Constructor_CopyConstructor() {
    Vector original = V(1, 2, 3);
    Vector copy     = new Vector(original);

    AssertVectorsAreEqual(original, copy);
    Assert.That(copy, Is.Not.SameAs(original));
  }

  [Test]
  public void Factory_Zero() {
    Vector z = Vector.Zero(4);
    Assert.That(z.SpaceDim, Is.EqualTo(4));
    Assert.That(z.IsZero, Is.True);
    for (int i = 0; i < 4; ++i)
      Assert.That(Tools.EQ(z[i], 0.0));
  }

  [Test]
  public void Factory_MakeOrth() {
    Vector e2 = Vector.MakeOrth(3, 2);
    Assert.That(e2.SpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(e2, V(0, 1, 0));
    Assert.That(Tools.EQ(e2.Length, 1.0));
  }

  [Test]
  public void Factory_Ones() {
    Vector ones = Vector.Ones(2);
    Assert.That(ones.SpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(ones, V(1, 1));
  }

  // todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Constructor_InvalidDimension_Throws()
  // {
  //     Assert.Throws<Exception>(() => new Vector(0), "Constructor Vector(0) SHOULD throw.");
  //     Assert.Throws<Exception>(() => new Vector(-1), "Constructor Vector(-1) should throw.");
  //     Assert.Throws<Exception>(() => new Vector(new double[0]), "Constructor Vector(double[0]) should throw.");
  //     Assert.Throws<Exception>(() => Vector.Zero(0), "Vector.Zero(0) should throw.");
  //     Assert.Throws<Exception>(() => Vector.MakeOrth(3, 0), "Vector.MakeOrth(3, 0) should throw.");
  //     Assert.Throws<Exception>(() => Vector.MakeOrth(3, 4), "Vector.MakeOrth(3, 4) should throw.");
  //     Assert.Throws<Exception>(() => Vector.Ones(0), "Vector.Ones(0) should throw.");
  // }
#endregion

#region Properties and Access Tests
  [Test]
  public void Property_SpaceDim() {
    Assert.That(V(1, 2).SpaceDim, Is.EqualTo(2));
    Assert.That(V(1, 2, 3, 4, 5).SpaceDim, Is.EqualTo(5));
  }

  [Test]
  public void Indexer_Read() {
    Vector v = V(5, -2, 0);
    Assert.That(v[0], Is.EqualTo(5.0));
    Assert.That(v[1], Is.EqualTo(-2.0));
    Assert.That(v[2], Is.EqualTo(0.0));
  }

  // todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Indexer_Read_OutOfBounds_Throws()
  // {
  //     Vector v = V(1, 2);
  //     Assert.Throws<Exception>(() => { var x = v[-1]; }, "Indexer v[-1] should throw.");
  //     Assert.Throws<Exception>(() => { var x = v[2]; }, "Indexer v[2] should throw.");
  // }


  [Test]
  public void Method_GetAsArray_ReturnsCopy() {
    Vector   v   = V(10, 20);
    double[] arr = v.GetAsArray();

    Assert.That(arr.Length, Is.EqualTo(2));
    Assert.That(arr[0], Is.EqualTo(10.0));
    Assert.That(arr[1], Is.EqualTo(20.0));

    // Modify returned array, vector should not change
    arr[0] = 0.0;
    Assert.That(v[0], Is.EqualTo(10.0), "Vector should not be affected by modifying array returned by GetAsArray.");
  }


  [Test]
  public void Properties_Length_Length2_IsZero() {
    Vector zero = V(0, 0, 0);
    Vector v1   = V(3, 4, 0);    // Length 5, Length2 25
    Vector v2   = V(1, 1, 1, 1); // Length 2, Length2 4
    Vector v3   = V(0, 0, -2);   // Length 2, Length2 4

    // Zero vector
    Assert.That(Tools.EQ(zero.Length2, 0.0), Is.True);
    Assert.That(Tools.EQ(zero.Length, 0.0), Is.True);
    Assert.That(zero.IsZero, Is.True);

    // Non-zero vectors
    Assert.That(Tools.EQ(v1.Length2, 25.0), Is.True);
    Assert.That(Tools.EQ(v1.Length, 5.0), Is.True);
    Assert.That(v1.IsZero, Is.False);

    Assert.That(Tools.EQ(v2.Length2, 4.0), Is.True);
    Assert.That(Tools.EQ(v2.Length, 2.0), Is.True);
    Assert.That(v2.IsZero, Is.False);

    Assert.That(Tools.EQ(v3.Length2, 4.0), Is.True);
    Assert.That(Tools.EQ(v3.Length, 2.0), Is.True);
    Assert.That(v3.IsZero, Is.False);
  }
#endregion

#region Operators Tests
  [Test]
  public void Operator_UnaryMinus() {
    Vector v    = V(1, -2, 0);
    Vector negV = -v;
    AssertVectorsAreEqual(negV, V(-1, 2, 0));
    AssertVectorsAreEqual(-Vector.Zero(3), Vector.Zero(3));
  }

  [Test]
  public void Operator_Addition() {
    Vector v1  = V(1, 2, 3);
    Vector v2  = V(4, -1, 0);
    Vector sum = v1 + v2;
    AssertVectorsAreEqual(sum, V(5, 1, 3));

    AssertVectorsAreEqual(v1 + Vector.Zero(3), v1);
    AssertVectorsAreEqual(Vector.Zero(3) + v1, v1);
  }

  [Test]
  public void Operator_Addition_DimMismatch_Throws() {
    Vector v2 = V(1, 2);
    Vector v3 = V(1, 2, 3);
    AssertThrowsDimMismatch
      (
       ()
         => {
         var x = v2 + v3;
       }
     , "v2+v3 should throw DimMismatch"
      );
  }

  [Test]
  public void Operator_Subtraction() {
    Vector v1   = V(5, 1, 3);
    Vector v2   = V(4, -1, 0);
    Vector diff = v1 - v2;
    AssertVectorsAreEqual(diff, V(1, 2, 3));

    AssertVectorsAreEqual(v1 - Vector.Zero(3), v1);
    AssertVectorsAreEqual(Vector.Zero(3) - v1, -v1);
  }

  [Test]
  public void Operator_Subtraction_DimMismatch_Throws() {
    Vector v2 = V(1, 2);
    Vector v3 = V(1, 2, 3);
    AssertThrowsDimMismatch
      (
       ()
         => {
         var x = v2 - v3;
       }
     , "v2-v3 should throw DimMismatch"
      );
  }

  [Test]
  public void Operator_ScalarMultiplication() {
    Vector v      = V(1, -2, 3);
    double scalar = 2.0;

    AssertVectorsAreEqual(scalar * v, V(2, -4, 6)); // Left multiplication
    AssertVectorsAreEqual(v * scalar, V(2, -4, 6)); // Right multiplication

    AssertVectorsAreEqual(0.0 * v, Vector.Zero(3)); // Multiply by zero
    AssertVectorsAreEqual(1.0 * v, v);              // Multiply by one
    AssertVectorsAreEqual(-1.0 * v, -v);            // Multiply by minus one
  }

  [Test]
  public void Operator_ScalarDivision() {
    Vector v      = V(2, -4, 6);
    double scalar = 2.0;
    AssertVectorsAreEqual(v / scalar, V(1, -2, 3));
  }
  //   todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Operator_ScalarDivision_ByZero_Throws() {
  //   Vector v = V(1, 2);
  //   Assert.Throws<DivideByZeroException>
  //     (
  //      ()
  //        => {
  //        var x = v / 0.0;
  //      }
  //    , "Division by zero should throw."
  //     );
  // }

  [Test]
  public void Operator_DotProduct() {
    Vector v1     = V(1, 2, 3);
    Vector v2     = V(4, -1, 2);
    Vector v3     = V(-2, 1, 0); // Orthogonal to v1
    double dot_12 = v1 * v2;
    double dot_13 = v1 * v3;

    Assert.That(Tools.EQ(dot_12, 8.0), Is.True);
    Assert.That(Tools.EQ(dot_13, 0.0), Is.True, "Dot product of orthogonal vectors should be zero.");
    Assert.That(Tools.EQ(v1 * Vector.Zero(3), 0.0), Is.True, "Dot product with zero vector should be zero.");
    Assert.That(Tools.EQ(v1 * v1, v1.Length2), Is.True, "Dot product with self should be Length2.");
  }

  [Test]
  public void Operator_DotProduct_DimMismatch_Throws() {
    Vector v2 = V(1, 2);
    Vector v3 = V(1, 2, 3);
    AssertThrowsDimMismatch
      (
       ()
         => {
         var x = v2 * v3;
       }
     , "v2*v3 should throw DimMismatch"
      );
  }
#endregion

#region Comparison Tests
  [Test]
  public void Comparison_CompareTo() {
    Vector v1      = V(1, 2, 3);
    Vector v2      = V(1, 2, 4);
    Vector v3      = V(1, 3, 0);
    Vector v1_copy = V(1, 2, 3);

    Assert.That(v1.CompareTo(v1_copy), Is.EqualTo(0));
    Assert.That(v1.CompareTo(v2), Is.LessThan(0));
    Assert.That(v2.CompareTo(v1), Is.GreaterThan(0));
    Assert.That(v1.CompareTo(v3), Is.LessThan(0));
    Assert.That(v3.CompareTo(v1), Is.GreaterThan(0));

    Assert.That(v1.CompareTo(null), Is.EqualTo(1));
  }

  [Test]
  public void Comparison_CompareTo_DimMismatch_Throws() {
    Vector v2 = V(1, 2);
    Vector v3 = V(1, 2, 3);
    AssertThrowsDimMismatch(() => v2.CompareTo(v3), "v2.CompareTo(v3) should throw DimMismatch");
  }

  [Test]
  public void Comparison_Operators() {
    Vector v1      = V(1, 2, 3);
    Vector v2      = V(1, 2, 4);
    Vector v1_copy = new Vector(v1);

    Assert.That(v1 == v1_copy, Is.True);
    Assert.That(v1 != v1_copy, Is.False);
    Assert.That(v1 == v2, Is.False);
    Assert.That(v1 != v2, Is.True);

    Assert.That(v1 < v2, Is.True);
    Assert.That(v1 <= v2, Is.True);
    Assert.That(v1 <= v1_copy, Is.True);
    Assert.That(v1 > v2, Is.False);
    Assert.That(v1 >= v2, Is.False);
    Assert.That(v1 >= v1_copy, Is.True);

    Assert.That(v2 > v1, Is.True);
    Assert.That(v2 >= v1, Is.True);
    Assert.That(v2 < v1, Is.False);
    Assert.That(v2 <= v1, Is.False);
  }

  [Test]
  public void Comparison_EqualsOverride() {
    Vector v1      = V(1, 2, 3);
    Vector v1_copy = new Vector(v1);
    Vector v2       = V(1, 2, 4);
    object v1_obj   = v1_copy;
    object otherObj = new object();

    Assert.That(v1.Equals(v1_copy), Is.True);
    Assert.That(v1.Equals(v1_obj), Is.True);
    Assert.That(v1.Equals(v2), Is.False);
    Assert.That(v1.Equals(null), Is.False);
    Assert.That(v1.Equals(otherObj), Is.False);
  }

  [Test]
  public void Override_GetHashCode_Throws() {
    Vector v = V(1, 2);
    Assert.Throws<InvalidOperationException>(() => v.GetHashCode());
  }
#endregion

#region Methods Tests
  [Test]
  public void Method_Normalize() {
    Vector v           = V(3, 4, 0); // Length 5
    Vector normalizedV = v.Normalize();
    Vector expected    = V(0.6, 0.8, 0);

    AssertVectorsAreEqual(normalizedV, expected);
    Assert.That(Tools.EQ(normalizedV.Length, 1.0), Is.True, "Normalized vector length should be 1.");

    Vector alreadyNormalized = V(0, 1, 0);
    AssertVectorsAreEqual(alreadyNormalized.Normalize(), alreadyNormalized);
  }
  //   todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Method_Normalize_ZeroVector_Throws()
  // {
  //     Vector zero = Vector.Zero(3);
  //     Assert.Throws<Exception>(() => zero.Normalize(), "Normalize() on zero vector should throw.");
  //     // Again, assumes Debug.Assert throws or is replaced by a runtime check.
  // }

  [Test]
  public void Method_NormalizeZero() {
    Vector v                  = V(3, 4, 0);
    Vector zero               = Vector.Zero(3);
    Vector expectedNormalized = V(0.6, 0.8, 0);

    AssertVectorsAreEqual(v.NormalizeZero(), expectedNormalized);
    AssertVectorsAreEqual(zero.NormalizeZero(), zero, "NormalizeZero() on zero vector should return zero vector.");
  }

  [Test]
  public void Method_ProjectTo2DAffineSpace() {
    Vector p  = V(3, 4, 5);
    Vector o  = Vector.Zero(3);
    Vector u1 = V(1, 0, 0);
    Vector u2 = V(0, 1, 0);

    Vector projected = p.ProjectTo2DAffineSpace(o, u1, u2);
    Vector expected  = V(3, 4, 0);
    AssertVectorsAreEqual(projected, expected);

    o         = V(1, 1, 1);
    u1        = V(1, 0, 0);
    u2        = V(0, 0, 1);
    p         = V(3, 4, 5);
    projected = p.ProjectTo2DAffineSpace(o, u1, u2);
    expected  = V(2, 0, 4);
    AssertVectorsAreEqual(projected, expected);
  }

  [Test]
  public void Method_LiftUp() {
    Vector v2       = V(1, 2);
    Vector v4       = v2.LiftUp(4, 5.0);
    Vector expected = V(1, 2, 5, 5);
    AssertVectorsAreEqual(v4, expected);
  }
  //  todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Method_LiftUp_InvalidDim_Throws()
  // {
  //      Vector v3 = V(1, 2, 3);
  //      Assert.Throws<Exception>(() => v3.LiftUp(3, 0.0), "LiftUp to same dimension should throw.");
  //      Assert.Throws<Exception>(() => v3.LiftUp(2, 0.0), "LiftUp to lower dimension should throw.");
  // }

  [Test]
  public void Method_CosAngle() {
    Vector v1   = V(1, 0);
    Vector v2   = V(1, 1);
    Vector v3   = V(0, 1);
    Vector v4   = V(-1, 0);
    Vector v5   = V(2, 0);
    Vector zero = Vector.Zero(2);

    double cos_12 = Vector.CosAngle(v1, v2);
    double cos_13 = Vector.CosAngle(v1, v3);
    double cos_14 = Vector.CosAngle(v1, v4);
    double cos_15 = Vector.CosAngle(v1, v5);
    double cos_10 = Vector.CosAngle(v1, zero);
    double cos_00 = Vector.CosAngle(zero, zero);


    Assert.That(Tools.EQ(cos_12, 1.0 / Math.Sqrt(2.0)), Is.True);
    Assert.That(Tools.EQ(cos_13, 0.0), Is.True);
    Assert.That(Tools.EQ(cos_14, -1.0), Is.True);
    Assert.That(Tools.EQ(cos_15, 1.0), Is.True);
    Assert.That(Tools.EQ(cos_10, 1.0), Is.True, "CosAngle with zero vector should be 1.");
    Assert.That(Tools.EQ(cos_00, 1.0), Is.True, "CosAngle of zero vectors should be 1.");
  }

  [Test]
  public void Method_Angle() {
    Vector v1 = V(1, 0);
    Vector v2 = V(1, 1);
    Vector v3 = V(0, 1);
    Vector v4 = V(-1, 0);
    Vector v5 = V(2, 0);
    Vector v6 = V(1, -1);

    Assert.That(Tools.EQ(Vector.Angle(v1, v2), Math.PI / 4.0), Is.True);
    Assert.That(Tools.EQ(Vector.Angle(v1, v3), Math.PI / 2.0), Is.True);
    Assert.That(Tools.EQ(Vector.Angle(v1, v4), Math.PI), Is.True);
    Assert.That(Tools.EQ(Vector.Angle(v1, v5), 0.0), Is.True);
    Assert.That(Tools.EQ(Vector.Angle(v1, v6), Math.PI / 4.0), Is.True);

    // Test clamping - using Tools.Eps from double instantiation
    Vector v_almost_minus_one = (-1.0 - Tools.Eps * 0.5) * v1;
    Vector v_almost_one       = (1.0 + Tools.Eps * 0.5) * v1;
    Assert.That(Tools.EQ(Vector.Angle(v1, v_almost_minus_one), Math.PI), Is.True, "Angle should clamp near -1");
    Assert.That(Tools.EQ(Vector.Angle(v1, v_almost_one), 0.0), Is.True, "Angle should clamp near 1");
  }

  [Test]
  public void Method_OuterProduct() {
    Vector v1    = V(1, 2);
    Vector v2    = V(3, 4, 5);
    Matrix outer = v1.OuterProduct(v2);

    Assert.That(outer.Rows, Is.EqualTo(2));
    Assert.That(outer.Cols, Is.EqualTo(3));

    Assert.That(Tools.EQ(outer[0, 0], 3.0));
    Assert.That(Tools.EQ(outer[0, 1], 4.0));
    Assert.That(Tools.EQ(outer[0, 2], 5.0));
    Assert.That(Tools.EQ(outer[1, 0], 6.0));
    Assert.That(Tools.EQ(outer[1, 1], 8.0));
    Assert.That(Tools.EQ(outer[1, 2], 10.0));
  }

  [Test]
  public void Method_SubVector() {
    Vector v =
      V
        (
         0
       , 1
       , 2
       , 3
       , 4
       , 5
        );
    Vector sub1 = v.SubVector(2, 4);
    Vector sub2 = v.SubVector(0, 0);
    Vector sub3 = v.SubVector(5, 5);

    AssertVectorsAreEqual(sub1, V(2, 3, 4));
    AssertVectorsAreEqual(sub2, V(0));
    AssertVectorsAreEqual(sub3, V(5));
  }
  //   todo: Надо ли делать throw или же Debug.Assert (как сейчас)
  // [Test]
  // public void Method_SubVector_InvalidIndices_Throws()
  // {
  //     Vector v = V(0, 1, 2);
  //     Assert.Throws<Exception>(() => v.SubVector(-1, 1));
  //     Assert.Throws<Exception>(() => v.SubVector(0, 3));
  //     Assert.Throws<Exception>(() => v.SubVector(2, 1));
  // }

  [Test]
  public void StaticMethod_AffMul() {
    Vector v1     = V(3, 4, 5);
    Vector origin = V(1, 1, 1);
    Vector v2     = V(1, 0, 2);
    double affMul = Vector.AffMul(v1, origin, v2);
    Assert.That(Tools.EQ(affMul, 10.0), Is.True);
  }

  [Test]
  public void StaticMethod_MulByNumAndAdd() {
    Vector v1     = V(1, 2, 3);
    Vector v2     = V(10, 0, -10);
    double a      = 3.0;
    Vector result = Vector.MulByNumAndAdd(v1, a, v2);
    AssertVectorsAreEqual(result, V(13, 6, -1));
  }

  [Test]
  public void StaticMethods_Parallelism() {
    Vector v1   = V(1, 2, -1);
    Vector v2   = V(2, 4, -2);
    Vector v3   = V(-1, -2, 1);
    Vector v4   = V(1, 0, 0);
    Vector zero = Vector.Zero(3);

    Assert.That(Vector.AreParallel(v1, v2), Is.True);
    Assert.That(Vector.AreParallel(v1, v3), Is.True);
    Assert.That(Vector.AreParallel(v1, v4), Is.False);
    Assert.That(Vector.AreParallel(v1, zero), Is.True);
    Assert.That(Vector.AreParallel(zero, zero), Is.True);

    Assert.That(Vector.AreCodirected(v1, v2), Is.True);
    Assert.That(Vector.AreCodirected(v1, v3), Is.False);
    Assert.That(Vector.AreCodirected(v1, v4), Is.False);
    Assert.That(Vector.AreCodirected(v1, zero), Is.True);
    Assert.That(Vector.AreCodirected(zero, zero), Is.True);

    Assert.That(Vector.AreCounterdirected(v1, v2), Is.False);
    Assert.That(Vector.AreCounterdirected(v1, v3), Is.True);
    Assert.That(Vector.AreCounterdirected(v1, v4), Is.False);
    Assert.That(Vector.AreCounterdirected(v1, zero), Is.True);
    Assert.That(Vector.AreCounterdirected(zero, zero), Is.True);
  }


  [Test]
  public void StaticMethod_AreOrthogonal() {
    Vector v1   = V(1, 2, 0);
    Vector v2   = V(2, -1, 5);
    Vector v3   = V(1, 1, 1);
    Vector zero = Vector.Zero(3);

    Assert.That(Vector.AreOrthogonal(v1, v2), Is.True);
    Assert.That(Vector.AreOrthogonal(v1, v3), Is.False);
    Assert.That(Vector.AreOrthogonal(v1, zero), Is.True);
    Assert.That(Vector.AreOrthogonal(zero, zero), Is.True);
  }

  [Test]
  public void StaticMethod_LinearCombination_TwoVectors() {
    Vector v1 = V(1, 0);
    Vector v2 = V(0, 1);
    double w1 = 2.0;
    double w2 = 3.0;
    Vector lc = Vector.LinearCombination(v1, w1, v2, w2);
    AssertVectorsAreEqual(lc, V(2, 3));
  }

  [Test]
  public void StaticMethod_LinearCombination_List() {
    List<Vector> Vs = new List<Vector> { V(1, 0, 0), V(0, 1, 0), V(0, 0, 1) };
    List<double> Ws = new List<double> { 1.0, 2.0, 3.0 };
    Vector       lc = Vector.LinearCombination(Vs, Ws);
    AssertVectorsAreEqual(lc, V(1, 2, 3));
    //   todo: Надо ли делать throw или же Debug.Assert (как сейчас)
    // List<double> Ws_short = new List<double> { 1.0, 2.0 };
    // Assert.Throws<Exception>(() => Vector.LinearCombination(Vs, Ws_short));
    //
    // List<Vector> Vs_empty = new List<Vector>();
    // List<double> Ws_empty = new List<double>();
    // Assert.Throws<Exception>(() => Vector.LinearCombination(Vs_empty, Ws_empty));
  }

  [Test]
  public void Method_ToString() {
    var v = V(1.2, -3.45, 0.0);

    var s        = v.ToString();
    var expected = "(1.2,-3.45,0)";
    Assert.That(s, Is.EqualTo(expected));

    var s_custom        = v.ToStringBraceAndDelim('[', ']', ';');
    var expected_custom = "[1.2;-3.45;0]";
    Assert.That(s_custom, Is.EqualTo(expected_custom));

    var s_no_brace        = v.ToStringBraceAndDelim(null, null, ',');
    var expected_no_brace = "1.2,-3.45,0";
    Assert.That(s_no_brace, Is.EqualTo(expected_no_brace));
  }
#endregion

}
