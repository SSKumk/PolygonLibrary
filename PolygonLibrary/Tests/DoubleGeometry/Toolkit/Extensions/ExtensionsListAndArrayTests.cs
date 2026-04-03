using CGLibrary;
using NUnit.Framework;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class ExtensionsListAndArrayTests {

  [Test]
  public void ListBinarySearchByPredicate_FindsFirstMatchAndClampsRequestedRange() {
    List<int> values = new() { 1, 3, 5, 7, 9 };

    int firstAtLeastSix = values.BinarySearchByPredicate(x => x >= 6);
    int clampedRange = values.BinarySearchByPredicate(x => x >= 5, -10, 100);
    int noMatch = values.BinarySearchByPredicate(x => x >= 10);

    Assert.Multiple(() => {
      Assert.That(firstAtLeastSix, Is.EqualTo(3));
      Assert.That(clampedRange, Is.EqualTo(2));
      Assert.That(noMatch, Is.EqualTo(-1));
    });
  }

  [Test]
  public void ListCyclicHelpers_FindWrappedBoundaryAndNormalizeIndices() {
    List<int> values = new() { 7, 9, 1, 3, 5 };

    int wrappedIndex = values.BinaryCyclicSearchByPredicate(x => x >= 5, 2, 6);

    Assert.Multiple(() => {
      Assert.That(wrappedIndex, Is.EqualTo(4));
      Assert.That(values.GetAtCyclic(-1), Is.EqualTo(5));
      Assert.That(values.GetAtCyclic(5), Is.EqualTo(7));
    });
  }

  [Test]
  public void GetAtCyclic_EmptyList_ThrowsArgumentException() {
    List<int> values = new();

    Assert.That(() => values.GetAtCyclic(0), Throws.TypeOf<ArgumentException>());
  }

  [Test]
  public void Shuffle_WithSameSeed_IsDeterministicAndPreservesElements() {
    List<int> first = new() { 1, 2, 3, 4, 5, 6 };
    List<int> second = new() { 1, 2, 3, 4, 5, 6 };

    first.Shuffle(new RandomLC(10u));
    second.Shuffle(new RandomLC(10u));

    Assert.Multiple(() => {
      Assert.That(first, Is.EqualTo(second));
      Assert.That(first.OrderBy(x => x).ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6 }));
    });
  }

  [Test]
  public void ArrayBinarySearchByPredicate_FindsFirstMatchAndSupportsWrappedRanges() {
    int[] values = { 1, 3, 5, 7, 9 };
    int[] cyclicValues = { 7, 9, 1, 3, 5 };

    int firstAtLeastFive = values.BinarySearchByPredicate(x => x >= 5);
    int wrappedIndex = cyclicValues.BinaryCyclicSearchByPredicate(x => x >= 5, 2, 6);

    Assert.Multiple(() => {
      Assert.That(firstAtLeastFive, Is.EqualTo(2));
      Assert.That(wrappedIndex, Is.EqualTo(4));
    });
  }

}
