using CGLibrary;
using NUnit.Framework;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class ExtensionsCollectionTests {

  [Test]
  public void Subsets_ReturnRequestedLengthInCurrentRecursiveOrder() {
    List<int> values = new() { 1, 2, 3 };

    List<List<int>> subsets = values.Subsets(2);

    List<int[]> expected = new() {
      new[] { 1, 2 },
      new[] { 1, 3 },
      new[] { 2, 3 }
    };

    Assert.That(subsets.Select(s => s.ToArray()).ToList(), Is.EqualTo(expected));
  }

  [Test]
  public void AllSubsets_ReturnsBitmaskEnumerationIncludingEmptyAndFullSets() {
    List<int> values = new() { 1, 2, 3 };

    List<List<int>> subsets = values.AllSubsets();

    Assert.Multiple(() => {
      Assert.That(subsets, Has.Count.EqualTo(8));
      Assert.That(subsets[0], Is.Empty);
      Assert.That(subsets[^1], Is.EqualTo(new[] { 1, 2, 3 }));
    });
  }

  [Test]
  public void LinkedListCyclicShift_MovesFirstElementToEnd() {
    LinkedList<int> values = new(new[] { 2, 4, 1, 5 });

    values.CyclicShift();

    Assert.That(values.ToArray(), Is.EqualTo(new[] { 4, 1, 5, 2 }));
  }

  [Test]
  public void ToSortedSet_SortsAndDeduplicatesSequence() {
    int[] values = { 3, 1, 2, 3, 2, 1 };

    SortedSet<int> sorted = values.ToSortedSet();

    Assert.That(sorted.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
  }

}
