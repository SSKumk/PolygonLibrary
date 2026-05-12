using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests;

public partial class ConvexPolygonTests {

  private void CyclicListComparison(List<Vector2D> l1, List<Vector2D> l2, string mes) {
    Assert.That(l1.Count, Is.EqualTo(l2.Count), mes + ": lengths of the lists are different");
    int i2 = l2.IndexOf(l1[0]);
    Assert.That(i2, Is.GreaterThanOrEqualTo(0), mes + ": the second list does not contain the point " + l1[0]);
    for (int i1 = 0; i1 < l1.Count; i1++, i2 = (i2 + 1) % l2.Count) {
      Assert.That(
         l1[i1].CompareTo(l2[i2]), Is.EqualTo(0),
         mes + ": point #" + i1 + " " + l1[i1] + " of the 1st list is not equal to point #" + i2 + " " + l2[i2] +
         " of the 2nd list"
      );
    }
  }

}
