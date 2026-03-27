using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polygons;

[TestFixture]
public class GammaPairCrossPairsTests {

  [Test]
  public void CrossPairs_ComputesExpectedIntersectionsForLegacyCases() {
    GammaPair
      g1 = new GammaPair(new Vector2D(1, 0), 1),
      g2 = new GammaPair(new Vector2D(1, 0), 2),
      g3 = new GammaPair(new Vector2D(0, 1), 1),
      g4 = new GammaPair(new Vector2D(1, 1), 3);
    Vector2D
      p13 = new Vector2D(1, 1),
      p14 = new Vector2D(1, 2),
      p23 = new Vector2D(2, 1),
      p24 = new Vector2D(2, 1),
      p34 = new Vector2D(2, 1),
      p;

    Assert.Multiple(() => {
      p = GammaPair.CrossPairs(g1, g3);
      Assert.That(p, Is.EqualTo(p13), "Bad crossing g1 and g3");

      p = GammaPair.CrossPairs(g3, g1);
      Assert.That(p, Is.EqualTo(p13), "Bad crossing g3 and g1");

      p = GammaPair.CrossPairs(g1, g4);
      Assert.That(p, Is.EqualTo(p14), "Bad crossing g1 and g4");

      p = GammaPair.CrossPairs(g2, g3);
      Assert.That(p, Is.EqualTo(p23), "Bad crossing g2 and g3");

      p = GammaPair.CrossPairs(g2, g3);
      Assert.That(p, Is.EqualTo(p24), "Bad crossing g21 and g3");

      p = GammaPair.CrossPairs(g3, g4);
      Assert.That(p, Is.EqualTo(p34), "Bad crossing g3 and g4");
    });
  }

  [Test]
  public void CrossPairs_WorksForFractionalIntersection() {
    GammaPair g1 = new GammaPair(new Vector2D(2.0, 0.0), 1.0);
    GammaPair g2 = new GammaPair(new Vector2D(0.0, 4.0), 3.0);
    Vector2D intersection = GammaPair.CrossPairs(g1, g2);

    Assert.That(intersection, Is.EqualTo(new Vector2D(0.5, 0.75)));
  }

  [Test]
  public void CrossPairs_WorksForUnnormalizedNonParallelNormals() {
    GammaPair g1 = new GammaPair(new Vector2D(2.0, 0.0), 4.0);
    GammaPair g2 = new GammaPair(new Vector2D(1.0, 1.0), 5.0);
    Vector2D intersection = GammaPair.CrossPairs(g1, g2);

    Assert.That(intersection, Is.EqualTo(new Vector2D(2.0, 3.0)));
  }

}
