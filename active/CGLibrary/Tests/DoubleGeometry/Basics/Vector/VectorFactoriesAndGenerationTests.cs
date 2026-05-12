using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.DoubleGeometry.Basics.VectorAssert;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class VectorFactoriesAndGenerationTests {

  [Test]
  public void StandardFactories_CreateExpectedVectors() {
    Vector z = Vector.Zero(4);
    Assert.That(z.SpaceDim, Is.EqualTo(4));
    Assert.That(z.IsZero, Is.True);
    for (int i = 0; i < 4; ++i) {
      Assert.That(Tools.EQ(z[i], 0.0));
    }

    Vector e2 = Vector.MakeOrth(3, 2);
    Assert.That(e2.SpaceDim, Is.EqualTo(3));
    AreEqual(e2, V(0, 1, 0));
    Assert.That(Tools.EQ(e2.Length, 1.0));

    Vector ones = Vector.Ones(2);
    Assert.That(ones.SpaceDim, Is.EqualTo(2));
    AreEqual(ones, V(1, 1));
  }

  [Test]
  public void GenVector_WithSeededRandom_IsDeterministicAndNonZero() {
    GRandomLC firstRandom = new GRandomLC(123u);
    GRandomLC secondRandom = new GRandomLC(123u);

    Vector first = Vector.GenVector(4, firstRandom);
    Vector second = Vector.GenVector(4, secondRandom);

    AreEqual(first, second);
    Assert.That(first.IsZero, Is.False);
  }

  [Test]
  public void GenVector_WithBounds_RespectsDimensionAndConfiguredRange() {
    GRandomLC random = new GRandomLC(456u);
    Vector generated = Vector.GenVector(16, -2.0, 3.0, random);

    Assert.That(generated.SpaceDim, Is.EqualTo(16));
    for (int i = 0; i < generated.SpaceDim; i++) {
      Assert.That(generated[i], Is.GreaterThanOrEqualTo(-2.0));
      Assert.That(generated[i], Is.LessThanOrEqualTo(3.0));
    }
  }

  [Test]
  public void GenVectorInt_WithBounds_ProducesIntegralCoordinatesAndIsDeterministic() {
    GRandomLC firstRandom = new GRandomLC(789u);
    GRandomLC secondRandom = new GRandomLC(789u);

    Vector first = Vector.GenVectorInt(10, -2, 3, firstRandom);
    Vector second = Vector.GenVectorInt(10, -2, 3, secondRandom);

    AreEqual(first, second);
    Assert.That(first.SpaceDim, Is.EqualTo(10));
    for (int i = 0; i < first.SpaceDim; i++) {
      Assert.That(first[i], Is.GreaterThanOrEqualTo(-2.0));
      Assert.That(first[i], Is.LessThanOrEqualTo(3.0));
      Assert.That(Tools.EQ(first[i], Math.Round(first[i])), Is.True, $"Value {first[i]} should be an integer.");
    }
  }

}
