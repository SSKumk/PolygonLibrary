using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class RandomLCTests {

  [Test]
  public void NextInt_SameSeed_ProducesSameSequenceWithinInclusiveBounds() {
    RandomLC first = new RandomLC(17u);
    RandomLC second = new RandomLC(17u);

    int[] firstValues = Enumerable.Range(0, 8).Select(_ => first.NextInt(-3, 4)).ToArray();
    int[] secondValues = Enumerable.Range(0, 8).Select(_ => second.NextInt(-3, 4)).ToArray();

    Assert.Multiple(() => {
      Assert.That(firstValues, Is.EqualTo(secondValues));
      Assert.That(firstValues.All(v => v >= -3 && v <= 4), Is.True);
    });
  }

  [Test]
  public void NextDouble_SameSeed_ProducesSameSequenceWithinRequestedBounds() {
    RandomLC first = new RandomLC(123u);
    RandomLC second = new RandomLC(123u);

    double[] firstValues = Enumerable.Range(0, 8).Select(_ => first.NextDouble(-1.5, 2.5)).ToArray();
    double[] secondValues = Enumerable.Range(0, 8).Select(_ => second.NextDouble(-1.5, 2.5)).ToArray();

    Assert.Multiple(() => {
      Assert.That(firstValues, Is.EqualTo(secondValues));
      Assert.That(firstValues.All(v => v >= -1.5 && v <= 2.5), Is.True);
    });
  }

  [Test]
  public void GRandomLC_NextPreciseAndNextFromInt_AreDeterministicForSameSeed() {
    GRandomLC first = new GRandomLC(31u);
    GRandomLC second = new GRandomLC(31u);

    double[] firstPrecise = Enumerable.Range(0, 5).Select(_ => first.NextPrecise(-2.0, 3.0)).ToArray();
    double[] secondPrecise = Enumerable.Range(0, 5).Select(_ => second.NextPrecise(-2.0, 3.0)).ToArray();

    GRandomLC firstInt = new GRandomLC(47u);
    GRandomLC secondInt = new GRandomLC(47u);

    double[] firstInts = Enumerable.Range(0, 5).Select(_ => firstInt.NextFromInt(-2, 3)).ToArray();
    double[] secondInts = Enumerable.Range(0, 5).Select(_ => secondInt.NextFromInt(-2, 3)).ToArray();

    Assert.Multiple(() => {
      Assert.That(firstPrecise, Is.EqualTo(secondPrecise));
      Assert.That(firstPrecise.All(v => v >= -2.0 && v <= 3.0), Is.True);
      Assert.That(firstInts, Is.EqualTo(secondInts));
      Assert.That(firstInts.All(v => v >= -2.0 && v <= 3.0), Is.True);
    });
  }

  [Test]
  public void GenArray_UsesProvidedGeneratorAndPreservesRequestedDimension() {
    GRandomLC first = new GRandomLC(101u);
    GRandomLC second = new GRandomLC(101u);

    double[] firstValues = GenArray(6, -0.25, 0.75, first);
    double[] secondValues = GenArray(6, -0.25, 0.75, second);

    Assert.Multiple(() => {
      Assert.That(firstValues, Has.Length.EqualTo(6));
      Assert.That(firstValues, Is.EqualTo(secondValues));
      Assert.That(firstValues.All(v => v >= -0.25 && v <= 0.75), Is.True);
    });
  }

  [Test]
  public void GenArrayInt_UsesProvidedGeneratorAndInclusiveIntegerBounds() {
    GRandomLC first = new GRandomLC(202u);
    GRandomLC second = new GRandomLC(202u);

    double[] firstValues = GenArrayInt(10, -2, 2, first);
    double[] secondValues = GenArrayInt(10, -2, 2, second);

    Assert.Multiple(() => {
      Assert.That(firstValues, Has.Length.EqualTo(10));
      Assert.That(firstValues, Is.EqualTo(secondValues));
      Assert.That(firstValues.All(v => v >= -2.0 && v <= 2.0), Is.True);
      Assert.That(firstValues.All(v => Tools.EQ(v, Math.Round(v))), Is.True);
    });
  }

}
