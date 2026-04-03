using CGLibrary;
using NUnit.Framework;

namespace Tests.DoubleGeometry.Toolkit;

[TestFixture]
public class CombinationTests {

  [Test]
  public void Constructor_InitializesZeroBasedPrefixState() {
    Geometry<double, Tests.DConvertor>.Combination combination = new(5, 3);

    Assert.Multiple(() => {
      Assert.That(combination[0], Is.EqualTo(0));
      Assert.That(combination[1], Is.EqualTo(1));
      Assert.That(combination[2], Is.EqualTo(2));
    });
  }

  [Test]
  public void Next_EnumeratesCombinationsInLexicographicOrder() {
    Geometry<double, Tests.DConvertor>.Combination combination = new(4, 2);
    List<int[]> states = new() { new[] { combination[0], combination[1] } };

    while (combination.Next()) {
      states.Add(new[] { combination[0], combination[1] });
    }

    int[][] expected = {
      new[] { 0, 1 },
      new[] { 0, 2 },
      new[] { 0, 3 },
      new[] { 1, 2 },
      new[] { 1, 3 },
      new[] { 2, 3 }
    };

    Assert.That(states, Is.EqualTo(expected));
  }

  [Test]
  public void Next_OnMaximalCombination_ReturnsFalseAndKeepsState() {
    Geometry<double, Tests.DConvertor>.Combination combination = new(3, 3);

    bool hasNext = combination.Next();

    Assert.Multiple(() => {
      Assert.That(hasNext, Is.False);
      Assert.That(combination[0], Is.EqualTo(0));
      Assert.That(combination[1], Is.EqualTo(1));
      Assert.That(combination[2], Is.EqualTo(2));
    });
  }

}
