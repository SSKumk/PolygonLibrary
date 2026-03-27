using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class GammaPairComparisonAndFormattingTests {

  [Test]
  public void CompareTo_OrdersPairsByPolarAngle() {
    GammaPair g1 = new GammaPair(new Vector2D(1.0, 0.0), 1.0);
    GammaPair g2 = new GammaPair(new Vector2D(0.0, 1.0), 1.0);
    GammaPair g3 = new GammaPair(new Vector2D(-1.0, 0.0), 1.0);

    Assert.Multiple(() => {
      Assert.That(g1.CompareTo(g2), Is.LessThan(0));
      Assert.That(g2.CompareTo(g3), Is.LessThan(0));
      Assert.That(g1.CompareTo(g3), Is.LessThan(0));
    });
  }

  [Test]
  public void CompareTo_UsesNormalizedValueForPairsWithSameAngle() {
    GammaPair smaller = new GammaPair(new Vector2D(1.0, 0.0), 1.0);
    GammaPair equalScaled = new GammaPair(new Vector2D(2.0, 0.0), 2.0);
    GammaPair larger = new GammaPair(new Vector2D(3.0, 0.0), 6.0);

    Assert.Multiple(() => {
      Assert.That(smaller.CompareTo(equalScaled), Is.EqualTo(0));
      Assert.That(smaller.CompareTo(larger), Is.LessThan(0));
      Assert.That(larger.CompareTo(equalScaled), Is.GreaterThan(0));
    });
  }

  [Test]
  public void CompareTo_Null_ReturnsOne() {
    GammaPair pair = new GammaPair(new Vector2D(1.0, 0.0), 1.0);

    Assert.That(pair.CompareTo(null), Is.EqualTo(1));
  }

  [Test]
  public void ToString_UsesInvariantCultureForValue() {
    GammaPair pair = new GammaPair(new Vector2D(1.0, 2.0), 1.25);

    Assert.That(pair.ToString(), Is.EqualTo("[(1;2);1.25]"));
  }

}
