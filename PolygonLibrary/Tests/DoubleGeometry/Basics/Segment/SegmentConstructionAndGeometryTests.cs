using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

[TestFixture]
public class SegmentConstructionAndGeometryTests {

  [Test]
  public void Constructors_CreateSegmentWithExpectedEndpoints() {
    Segment byCoords = new Segment(1.0, 2.0, 5.0, 4.0);
    Segment byPoints = new Segment(new Vector2D(1.0, 2.0), new Vector2D(5.0, 4.0));
    Segment copy = new Segment(byPoints);

    Assert.Multiple(() => {
      SegmentAssert.AreEqual(byCoords[0], new Vector2D(1.0, 2.0));
      SegmentAssert.AreEqual(byCoords[1], new Vector2D(5.0, 4.0));
      SegmentAssert.AreEqual(byPoints[0], new Vector2D(1.0, 2.0));
      SegmentAssert.AreEqual(byPoints[1], new Vector2D(5.0, 4.0));
      SegmentAssert.AreEqual(copy[0], byPoints[0]);
      SegmentAssert.AreEqual(copy[1], byPoints[1]);
    });
  }

  [Test]
  public void DirectionalNormalLengthPolarAngleAndVerticality_AreConsistent() {
    Segment oblique = new Segment(new Vector2D(1.0, 2.0), new Vector2D(5.0, 4.0));
    Segment vertical = new Segment(new Vector2D(2.0, -1.0), new Vector2D(2.0, 3.0));

    Assert.Multiple(() => {
      SegmentAssert.AreEqual(oblique.Directional, new Vector2D(4.0, 2.0));
      SegmentAssert.AreEqual(oblique.Normal, new Vector2D(2.0, -4.0));
      Assert.That(Tools.EQ(oblique.Directional * oblique.Normal), Is.True);
      Assert.That(Tools.EQ(oblique.DirectionalNormalized.Length, 1.0), Is.True);
      Assert.That(Vector2D.AreCodirected(oblique.DirectionalNormalized, oblique.Directional), Is.True);
      Assert.That(Tools.EQ(oblique.Length, double.Sqrt(20.0)), Is.True);
      Assert.That(Tools.EQ(oblique.PolarAngle, oblique.Directional.PolarAngle), Is.True);
      Assert.That(oblique.IsVertical, Is.False);
      Assert.That(vertical.IsVertical, Is.True);
    });
  }

  [Test]
  public void CompareEqualsAndToString_FollowCurrentOrderedContract() {
    Segment reference = new Segment(new Vector2D(1.0, 2.0), new Vector2D(5.0, 4.0));
    Segment same = new Segment(new Vector2D(1.0, 2.0), new Vector2D(5.0, 4.0));
    Segment reversed = new Segment(new Vector2D(5.0, 4.0), new Vector2D(1.0, 2.0));
    Segment larger = new Segment(new Vector2D(2.0, 2.0), new Vector2D(5.0, 4.0));

    Assert.Multiple(() => {
      Assert.That(reference.CompareTo(same), Is.EqualTo(0));
      Assert.That(reference.CompareTo(larger), Is.LessThan(0));
      Assert.That(reference.CompareTo(reversed), Is.LessThan(0));
      Assert.That(reference.Equals((object)same), Is.True);
      Assert.That(reference.Equals((object)reversed), Is.False);
      Assert.That(reference.ToString(), Is.EqualTo("[(1;2);(5;4)]"));
    });
  }

}
