using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class GammaPairConstructionAndEqualityTests {

  [Test]
  public void Constructor_Default_CreatesPairWithE1AndZeroValue() {
    GammaPair pair = new GammaPair();

    Assert.Multiple(() => {
      Assert.That(pair.Normal, Is.EqualTo(Vector2D.E1));
      Assert.That(Tools.EQ(pair.Value), Is.True);
    });
  }

  [Test]
  public void Constructor_ValueWithoutNormalization_StoresRawNormalAndValue() {
    Vector2D normal = new Vector2D(3.0, 4.0);
    GammaPair pair = new GammaPair(normal, 10.0, false);

    Assert.Multiple(() => {
      Assert.That(pair.Normal, Is.EqualTo(normal));
      Assert.That(Tools.EQ(pair.Value, 10.0), Is.True);
    });
  }

  [Test]
  public void Constructor_ValueWithNormalization_NormalizesNormalAndScalesValue() {
    Vector2D normal = new Vector2D(3.0, 4.0);
    GammaPair pair = new GammaPair(normal, 10.0, true);

    Assert.Multiple(() => {
      Assert.That(pair.Normal, Is.EqualTo(new Vector2D(0.6, 0.8)));
      Assert.That(Tools.EQ(pair.Value, 2.0), Is.True);
    });
  }

  [Test]
  public void CopyConstructor_CopiesNormalAndValue() {
    GammaPair source = new GammaPair(new Vector2D(-2.0, 1.0), 7.0);
    GammaPair copy = new GammaPair(source);

    Assert.Multiple(() => {
      Assert.That(copy.Normal, Is.EqualTo(source.Normal));
      Assert.That(Tools.EQ(copy.Value, source.Value), Is.True);
      Assert.That(copy.Equals(source), Is.True);
    });
  }

  [Test]
  public void Equals_ReturnsTrueForSameAngleAndSameNormalizedValue() {
    GammaPair p1 = new GammaPair(new Vector2D(1.0, 1.0), double.Sqrt(2.0));
    GammaPair p2 = new GammaPair(new Vector2D(2.0, 2.0), 2.0 * double.Sqrt(2.0));

    Assert.That(p1.Equals(p2), Is.True);
  }

  [Test]
  public void Equals_ReturnsFalseWhenNormalizedValuesDiffer() {
    GammaPair p1 = new GammaPair(new Vector2D(1.0, 1.0), double.Sqrt(2.0));
    GammaPair p2 = new GammaPair(new Vector2D(2.0, 2.0), 3.0 * double.Sqrt(2.0));

    Assert.That(p1.Equals(p2), Is.False);
  }

  [Test]
  public void Equals_ReturnsFalseWhenPolarAnglesDiffer() {
    GammaPair p1 = new GammaPair(new Vector2D(1.0, 0.0), 1.0);
    GammaPair p2 = new GammaPair(new Vector2D(0.0, 1.0), 1.0);

    Assert.That(p1.Equals(p2), Is.False);
  }

}
