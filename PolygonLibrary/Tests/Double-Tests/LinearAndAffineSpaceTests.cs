using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests;

[TestFixture]
public class LinearSpaceTests { }

[TestFixture]
public class AffineSpaceTests {
  /// <summary>
  /// 4D-test
  /// </summary>
  [Test]
  public void ProjectToAffineSpace_4() {
    Vector origin = new Vector(new double[] { 0, 0, 0, 0 });

    List<Vector> basis = new List<Vector> { new Vector(new double[] { 1, 0, 0, 0 }), new Vector(new double[] { 0, 1, 0, 0 }) };

    SortedSet<Vector> swarm =
      new SortedSet<Vector>
        {
          new Vector(new double[] { 0, 0, 0, 0 })
        , new Vector(new double[] { 1, 0, 0, 0 })
        , new Vector(new double[] { 0, 1, 0, 0 })
        , new Vector(new double[] { 1, 1, 1, 1 })
        };

    SortedSet<Vector> expected =
      new SortedSet<Vector>
        {
          new Vector(new double[] { 0, 0 })
        , new Vector(new double[] { 1, 0 })
        , new Vector(new double[] { 0, 1 })
        , new Vector(new double[] { 1, 1 })
        };

    AffineBasis         aBasis = AffineBasis.FromVectors(origin, basis, false);
    IEnumerable<Vector> result = aBasis.ProjectPoints(swarm);

    bool areEqual = expected.Count == result.Count() && expected.All(x => result.Any(y => x == y));
    Assert.That(areEqual, $"The following sets are not equal:\n -- {result} \n -- {expected}.");
  }

}
