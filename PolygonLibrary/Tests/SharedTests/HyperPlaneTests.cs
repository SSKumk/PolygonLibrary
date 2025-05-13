using NUnit.Framework;
using CGLibrary;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.SharedTests.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tests.SharedTests;

[TestFixture]
public class HyperPlaneTests {

  // Helper для проверки внутренней согласованности (если нужно вызвать CheckCorrectness)
  private void AssertHyperPlaneConsistent(HyperPlane hp) {
#if DEBUG
    Assert.DoesNotThrow(() => HyperPlane.CheckCorrectness(hp));
#endif
    Vector n = hp.Normal;
    Assert.That(Tools.EQ(n.Length, 1.0), Is.True, "Normal should be unit length.");

    double c = hp.ConstantTerm;
    Assert.That(Tools.EQ(n * hp.Origin, c), Is.True, "Origin should satisfy N*Origin = C");

    AffineBasis ab = hp.AffBasis;
    foreach (Vector bvec in ab) {
      Assert.That(Tools.EQ(n * bvec), Is.True, $"Normal should be orthogonal to basis vector {bvec}");
    }
  }


#region Constructor Tests
  [Test]
  public void Constructor_NormalOrigin_NormalizeTrue() {
    Vector     normal = V(0, 0, 2); // Не нормирована
    Vector     origin = V(1, 2, 5);
    HyperPlane hp     = new HyperPlane(normal, origin, needNormalize: true);

    Assert.That(hp.SpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(hp.Origin, origin);
    AssertVectorsAreEqual(hp.Normal, V(0, 0, 1), "Normal should be normalized.");
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0), "Constant term should be N_norm * Origin.");
    AssertHyperPlaneConsistent(hp);
  }

  [Test]
  public void Constructor_NormalOrigin_NormalizeFalse() {
    Vector     normal = V(0, 0, 1); // Уже нормирована
    Vector     origin = V(1, 2, 5);
    HyperPlane hp     = new HyperPlane(normal, origin, needNormalize: false);

    Assert.That(hp.SpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(hp.Origin, origin);
    AssertVectorsAreEqual(hp.Normal, normal, "Normal should be used as is.");
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0));
    AssertHyperPlaneConsistent(hp);

#if DEBUG
#else
        // Случай с ненормированной нормалью и needNormalize=false (проверка Debug.Assert)
        Vector nonUnitNormal = V(0, 0, 2);
        HyperPlane hp_invalid = new HyperPlane(nonUnitNormal, origin, needNormalize: false);
        Assert.Throws<AssertionException>(() => AssertHyperPlaneConsistent(hp_invalid), "Consistency check should fail in Release for non-unit normal.");
#endif
  }

  [Test]
  public void Constructor_NormalConstant_CorrectOriginCalculation() {
    Vector     normal   = V(0, 3, 0); // Не нормирована
    double     constant = 6.0;
    HyperPlane hp       = new HyperPlane(normal, constant);

    Assert.That(hp.SpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(hp.Normal, V(0, 1, 0), "Normal should be normalized.");
    Assert.That(Tools.EQ(hp.ConstantTerm, 2.0), "ConstantTerm should be normalized.");

    Vector expectedOrigin = V(0, 2, 0);
    AssertVectorsAreEqual
      (hp.Origin, expectedOrigin, "Calculated Origin is incorrect."); // <-- Эта проверка упадет из-за ошибки в конструкторе!
    AssertHyperPlaneConsistent(hp);
  }

  [Test]
  public void Constructor_AffineBasis_CorrectDerivation() {
    Vector      o  = V(0, 0, 1);
    AffineBasis ab = new AffineBasis(o, new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }));
    HyperPlane  hp = new HyperPlane(ab);

    Assert.That(hp.SpaceDim, Is.EqualTo(3));
    AssertVectorsAreEqual(hp.Origin, o);
    Vector n = hp.Normal;
    Assert.That(Tools.EQ(n.Length, 1.0), Is.True);
    Assert.That(Vector.AreParallel(n, V(0, 0, 1)), Is.True, "Normal should be parallel to Z-axis.");
    Assert.That(Tools.EQ(Math.Abs(hp.ConstantTerm), 1.0), Is.True);
    Assert.That(Tools.EQ(hp.ConstantTerm, hp.Normal * hp.Origin), Is.True);
    AssertHyperPlaneConsistent(hp);
    Assert.That(hp.AffBasis, Is.SameAs(ab));
  }

  [Test]
  public void Constructor_AffineBasis_WithOrientation() {
    Vector      o             = V(0, 0, 0);
    AffineBasis ab            = new AffineBasis(o, new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }));
    Vector      pointPositive = V(0, 0, 5);
    Vector      pointNegative = V(0, 0, -5);

    // 1. Ориентация по точке в положительном полупространстве
    HyperPlane hpPos = new HyperPlane(ab, toOrient: (pointPositive, true));
    AssertVectorsAreEqual(hpPos.Normal, V(0, 0, 1), "Normal should point towards positive point.");
    Assert.That(Tools.EQ(hpPos.ConstantTerm, 0.0));
    AssertHyperPlaneConsistent(hpPos);

    // 2. Ориентация по точке в отрицательном полупространстве (isPositive = false)
    HyperPlane hpNeg = new HyperPlane(ab, toOrient: (pointNegative, false));
    AssertVectorsAreEqual
      (hpNeg.Normal, V(0, 0, 1), "Normal should point towards the 'positive' side, even if orientation point is negative.");
    Assert.That(Tools.EQ(hpNeg.ConstantTerm, 0.0));
    AssertHyperPlaneConsistent(hpNeg);

    // 3. Ориентация по точке в положительном полупространстве (isPositive = false) -> нормаль должна перевернуться
    HyperPlane hpFlip = new HyperPlane(ab, toOrient: (pointPositive, false));
    AssertVectorsAreEqual(hpFlip.Normal, V(0, 0, -1), "Normal should flip if orientation is reversed.");
    Assert.That(Tools.EQ(hpFlip.ConstantTerm, 0.0));
    AssertHyperPlaneConsistent(hpFlip);
  }

  [Test]
  public void Constructor_FromPoints_Plane() {
    // Плоскость x + y + z = 3
    Vector     p1 = V(3, 0, 0);
    Vector     p2 = V(0, 3, 0);
    Vector     p3 = V(0, 0, 3);
    Vector     p4 = V(1, 1, 1);
    HyperPlane hp = new HyperPlane(new[] { p1, p2, p3, p4 });

    Vector expectedNormal   = V(1, 1, 1).Normalize();
    double expectedConstant = expectedNormal * p1;

    Assert.That(hp.SpaceDim, Is.EqualTo(3));
    Assert.That(Vector.AreParallel(hp.Normal, expectedNormal), Is.True);
    Assert.That(Tools.EQ(Math.Abs(hp.ConstantTerm), Math.Abs(expectedConstant)), Is.True);
    // Проверим, что все точки лежат на плоскости
    Assert.That(hp.Contains(p1), Is.True);
    Assert.That(hp.Contains(p2), Is.True);
    Assert.That(hp.Contains(p3), Is.True);
    Assert.That(hp.Contains(p4), Is.True);
    AssertHyperPlaneConsistent(hp);
  }
#endregion

#region Method Tests
  [Test]
  public void Method_OrientNormal_FlipsCorrectly() {
    // Плоскость z = 5 (N = (0,0,1), C = 5)
    HyperPlane hp         = new HyperPlane(V(0, 0, 1), V(1, 1, 5));
    Vector     pointAbove = V(0, 0, 10); // Positive side
    Vector     pointBelow = V(0, 0, 0);  // Negative side

    AssertVectorsAreEqual(hp.Normal, V(0, 0, 1));
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0));

    // 1. Orient using point below, should stay same (isPositive = false)
    hp.OrientNormal(pointBelow, false);
    AssertVectorsAreEqual(hp.Normal, V(0, 0, 1), "Orientation 1 failed.");
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0), "Orientation 1 C failed.");

    // 2. Orient using point above, should stay same (isPositive = true)
    hp.OrientNormal(pointAbove, true);
    AssertVectorsAreEqual(hp.Normal, V(0, 0, 1), "Orientation 2 failed.");
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0), "Orientation 2 C failed.");

    // 3. Orient using point below, should flip (isPositive = true)
    hp.OrientNormal(pointBelow, true);
    AssertVectorsAreEqual(hp.Normal, V(0, 0, -1), "Orientation 3 failed.");
    Assert.That(Tools.EQ(hp.ConstantTerm, -5.0), "Orientation 3 C failed."); // Константа тоже должна флипнуться

    // 4. Orient using point above, should flip back (isPositive = true)
    hp.OrientNormal(pointAbove, true);
    AssertVectorsAreEqual(hp.Normal, V(0, 0, 1), "Orientation 4 failed.");
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0), "Orientation 4 C failed.");
  }

  [Test]
  public void Method_Eval() {
    // Плоскость z = 5 (N = (0,0,1), C = 5)
    HyperPlane hp      = new HyperPlane(V(0, 0, 1), V(0, 0, 5));
    Vector     p_on    = V(1, 1, 5);
    Vector     p_above = V(1, 1, 6); // N*p - C = 1*6 - 5 = 1
    Vector     p_below = V(1, 1, 4); // N*p - C = 1*4 - 5 = -1

    Assert.That(Tools.EQ(hp.Eval(p_on)), Is.True);
    Assert.That(Tools.EQ(hp.Eval(p_above), 1.0), Is.True);
    Assert.That(Tools.EQ(hp.Eval(p_below), -1.0), Is.True);
  }

  [Test]
  public void Methods_Contains_Positive_Negative() {
    HyperPlane hp      = new HyperPlane(V(0, 0, 1), V(0, 0, 5)); // z = 5
    Vector     p_on    = V(1, 1, 5);
    Vector     p_above = V(1, 1, 6);
    Vector     p_below = V(1, 1, 4);

    // Contains
    Assert.That(hp.Contains(p_on), Is.True);
    Assert.That(hp.Contains(p_above), Is.False);
    Assert.That(hp.Contains(p_below), Is.False);

    // ContainsPositive (> 0)
    Assert.That(hp.ContainsPositive(p_on), Is.False);
    Assert.That(hp.ContainsPositive(p_above), Is.True);
    Assert.That(hp.ContainsPositive(p_below), Is.False);

    // ContainsNegative (< 0)
    Assert.That(hp.ContainsNegative(p_on), Is.False);
    Assert.That(hp.ContainsNegative(p_above), Is.False);
    Assert.That(hp.ContainsNegative(p_below), Is.True);

    // ContainsNegativeNonStrict (<= 0)
    Assert.That(hp.ContainsNegativeNonStrict(p_on), Is.True);
    Assert.That(hp.ContainsNegativeNonStrict(p_above), Is.False);
    Assert.That(hp.ContainsNegativeNonStrict(p_below), Is.True);
  }

  [Test]
  public void Methods_Filter() {
    HyperPlane hp      = new HyperPlane(V(0, 0, 1), V(0, 0, 5)); // z = 5
    Vector     p_on1   = V(1, 1, 5);
    Vector     p_on2   = V(2, 2, 5);
    Vector     p_above = V(1, 1, 6);
    Vector     p_below = V(1, 1, 4);
    List<Vector> swarm =
      new List<Vector>
        {
          p_on1
        , p_above
        , p_on2
        , p_below
        };

    List<Vector> filteredIn    = hp.FilterIn(swarm).ToList();
    List<Vector> filteredNotIn = hp.FilterNotIn(swarm).ToList();

    Assert.That(filteredIn.Count, Is.EqualTo(2));
    Assert.Contains(p_on1, filteredIn);
    Assert.Contains(p_on2, filteredIn);

    Assert.That(filteredNotIn.Count, Is.EqualTo(2));
    Assert.Contains(p_above, filteredNotIn);
    Assert.Contains(p_below, filteredNotIn);
  }

  [Test]
  public void Method_AllAtOneSide() {
    HyperPlane hp       = new HyperPlane(V(0, 0, 1), V(0, 0, 5)); // z = 5
    Vector     p_on1    = V(1, 1, 5);
    Vector     p_on2    = V(2, 2, 5);
    Vector     p_above1 = V(1, 1, 6);
    Vector     p_above2 = V(0, 0, 10);
    Vector     p_below1 = V(1, 1, 4);
    Vector     p_below2 = V(0, 0, 0);

    // Case 1: All on plane
    var res1 = hp.AllAtOneSide(new[] { p_on1, p_on2 });
    Assert.That(res1.atOneSide, Is.True);
    Assert.That(res1.where, Is.EqualTo(0));

    // Case 2: All above (positive)
    var res2 = hp.AllAtOneSide(new[] { p_above1, p_above2 });
    Assert.That(res2.atOneSide, Is.True);
    Assert.That(res2.where, Is.EqualTo(1));

    // Case 3: All below (negative)
    var res3 = hp.AllAtOneSide(new[] { p_below1, p_below2 });
    Assert.That(res3.atOneSide, Is.True);
    Assert.That(res3.where, Is.EqualTo(-1));

    // Case 4: Mixed (above and below)
    var res4 = hp.AllAtOneSide(new[] { p_above1, p_below1 });
    Assert.That(res4.atOneSide, Is.False);

    // Case 5: Mixed (on and above)
    var res5 = hp.AllAtOneSide(new[] { p_on1, p_above1 });
    Assert.That(res5.atOneSide, Is.False);

    // Case 6: Mixed (on and below)
    var res6 = hp.AllAtOneSide(new[] { p_on1, p_below1 });
    Assert.That(res6.atOneSide, Is.False);

    // Case 7: Mixed (all three)
    var res7 = hp.AllAtOneSide(new[] { p_on1, p_above1, p_below1 });
    Assert.That(res7.atOneSide, Is.False);
  }

  [Test]
  public void ContainsTest() {
    Vector origin = new Vector(new double[] { 0, 0, 0 });
    Vector v1     = new Vector(new double[] { 1, 1, 1 });
    Vector v2     = new Vector(new double[] { 1, -1, 1 });
    Vector v3     = new Vector(new double[] { 0, 0, 1 });

    AffineBasis aBasis = AffineBasis.FromVectors(origin, new List<Vector>() { v1, v2 });

    HyperPlane hp = new HyperPlane(aBasis);

    Vector p1 = Vector.LinearCombination(v1, 3, v2, 5);
    Vector p2 = Vector.LinearCombination(v1, -3, v2, 5);
    Vector p3 = Vector.LinearCombination(v1, -3, v2, -5);
    Vector p4 = Vector.LinearCombination(v1, 3, v2, -5);

    Assert.That(hp.Contains(p1), Is.True);
    Assert.That(hp.Contains(p2), Is.True);
    Assert.That(hp.Contains(p3), Is.True);
    Assert.That(hp.Contains(p4), Is.True);

    Assert.That(hp.Contains(p1 + (v3 - origin)), Is.False);
    Assert.That(hp.Contains(p1 - (v3 - origin)), Is.False);
  }

  [Test]
  public void TestFilter() {
    Vector origin = new Vector(new double[] { 0, 0, 0 });
    Vector e1     = new Vector(new double[] { 1, 0, 0 });
    Vector e2     = new Vector(new double[] { 0, 1, 0 });
    Vector e3     = new Vector(new double[] { 0, 0, 1 });

    AffineBasis aBasis = AffineBasis.FromVectors(origin, new Vector[] { e1, e2 });

    HyperPlane hp = new HyperPlane(aBasis);

    List<Vector> Swarm =
      new List<Vector>()
        {
          Vector.LinearCombination(e1, 3, e2, 5)
        , Vector.LinearCombination(e1, -3, e2, 5)
        , Vector.LinearCombination(e1, -3, e2, -5)
        , Vector.LinearCombination(e1, 3, e2, -5)
        , Vector.LinearCombination(e1, 3, e3, 5)
        , Vector.LinearCombination(e1, -3, e3, 5)
        , Vector.LinearCombination(e1, -3, e3, -5)
        , Vector.LinearCombination(e1, 3, e3, -5)
        , Vector.LinearCombination(e1, 3, e3, 4)
        };

    Assert.That(hp.AllAtOneSide(Swarm).Item1, Is.False);

    IEnumerable<Vector> inPlane    = hp.FilterIn(Swarm);
    IEnumerable<Vector> notInPlane = hp.FilterNotIn(Swarm);

    Assert.That(hp.AllAtOneSide(inPlane), Is.EqualTo((true, 0)));
    Assert.That(inPlane.Count(), Is.EqualTo(4));
    Assert.That(notInPlane.Count(), Is.EqualTo(5));
  }
#endregion

#region Factory Tests
  [Test]
  public void Factory_Make3D_xyParallel() {
    HyperPlane hp = HyperPlane.Make3D_xyParallel(5.0); // z = 5
    AssertVectorsAreEqual(hp.Normal, V(0, 0, 1));
    Assert.That(Tools.EQ(hp.ConstantTerm, 5.0));
    AssertVectorsAreEqual(hp.Origin, V(0, 0, 5)); // Origin должен быть (0,0,5)
    AssertHyperPlaneConsistent(hp);
  }
#endregion

#region Override Tests
  [Test]
  public void Override_ToString() {
    // Ожидаем формат "N1,N2,N3 C"
    HyperPlane hp       = new HyperPlane(V(0, 0, 1), 5.0); // z=5
    string     s        = hp.ToString();
    string     expected = "0,0,1 5"; // Пробел перед константой, без скобок у нормали
    Assert.That(s, Is.EqualTo(expected));

    HyperPlane hp2  = new HyperPlane(V(1, 2, 3).Normalize(), 10.0);
    string     s2   = hp2.ToString();
    string     cStr = hp2.ConstantTerm.ToString(null, CultureInfo.InvariantCulture);
    string     nStr = hp2.Normal.ToStringBraceAndDelim(null, null, ',');
    Assert.That(s2, Does.StartWith(nStr));
    Assert.That(s2, Does.EndWith(cStr));
    Assert.That(s2.Contains(" "), Is.True); // Проверяем наличие пробела-разделителя
  }

  [Test]
  public void Override_Equals() {
    // z = 5
    HyperPlane hp1      = new HyperPlane(V(0, 0, 1), 5.0);
    HyperPlane hp1_copy = new HyperPlane(V(0, 0, 1), 5.0);
    // z = 5, но с другой нормалью/константой: -z = -5
    HyperPlane hp1_flipped = new HyperPlane(V(0, 0, -1), -5.0);
    // z = 6 (параллельная)
    HyperPlane hp2_parallel = new HyperPlane(V(0, 0, 1), 6.0);
    // y = 5 (другая ориентация)
    HyperPlane hp3_other = new HyperPlane(V(0, 1, 0), 5.0);

    Assert.That(hp1.Equals(hp1_copy), Is.True, "Equals: Same object representation");
    Assert.That(hp1.Equals(hp1_flipped), Is.False, "Equals: Same plane, flipped representation");
    Assert.That(hp1.Equals(hp2_parallel), Is.False, "Equals: Parallel planes");
    Assert.That(hp1.Equals(hp3_other), Is.False, "Equals: Different planes");
    Assert.That(hp1.Equals(null), Is.False, "Equals: Null");
    Assert.That(hp1.Equals(V(0, 0, 1)), Is.False, "Equals: Different type");
  }
#endregion

#region Lazy Initialization Tests
  [Test]
  public void LazyInitialization_NormalFirst() {
    Vector      o  = V(1, 1, 1);
    AffineBasis ab = new AffineBasis(o, new LinearBasis(new[] { V(1, 0, 0), V(0, 1, 0) }));
    HyperPlane  hp = new HyperPlane(ab);

    // 1. Получаем Normal
    Vector normal = hp.Normal;
    AssertVectorsAreEqual(normal, V(0, 0, 1));

    // 2. Получаем AffineBasis (должен быть оригинальным)
    AffineBasis basis = hp.AffBasis;
    Assert.That(basis, Is.SameAs(ab));

    // 3. Получаем ConstantTerm
    double c         = hp.ConstantTerm;
    double expectedC = normal * o;
    Assert.That(Tools.EQ(c, expectedC), Is.True);

    AssertHyperPlaneConsistent(hp);
  }

  [Test]
  public void LazyInitialization_AffBasisFirst() {
    Vector     normal = V(0, 0, 1);
    Vector     origin = V(1, 1, 5);
    HyperPlane hp     = new HyperPlane(normal, origin, needNormalize: false);

    // 1. Получаем AffineBasis
    AffineBasis basis = hp.AffBasis;
    Assert.That(basis.SubSpaceDim, Is.EqualTo(2));
    AssertVectorsAreEqual(basis.Origin, origin);
    // Базис должен быть ортогонален normal=(0,0,1), т.е. лежать в XY плоскости
    Assert.That(Tools.EQ(basis[0] * normal), Is.True);
    Assert.That(Tools.EQ(basis[1] * normal), Is.True);

    // 2. Получаем Normal (должен быть оригинальным)
    Vector n = hp.Normal;
    AssertVectorsAreEqual(n, normal); // Должен вернуть исходную нормаль

    // 3. Получаем ConstantTerm
    double c = hp.ConstantTerm;
    Assert.That(Tools.EQ(c, 5.0), Is.True);

    AssertHyperPlaneConsistent(hp);
  }
#endregion

}
