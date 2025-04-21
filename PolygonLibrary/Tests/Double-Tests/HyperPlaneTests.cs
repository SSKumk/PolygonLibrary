using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
namespace Tests.Double_Tests; 

[TestFixture]
public class HyperPlaneTests {

  [Test]
  public void ConstructorWithPointAndNormalTest() {
    Vector      origin     = new Vector(new double[] { 0, 0, 0 });
    Vector     normal     = new Vector(new double[] { 1, 1, 1 });
    HyperPlane hyperplane = new HyperPlane(normal, origin);
  }

  [Test]
  public void ConstructorWithAffineBasisTest() {
    List<Vector> vectors = new List<Vector>()
      {
        new Vector(new double[] { 1, 0, 0 })
      , new Vector(new double[] { 0, 1, 0 })
      };

    AffineBasis affineBasis = AffineBasis.FromVectors(new Vector(new double[] { 1, 1, 1 }), vectors);
    HyperPlane  hyperplane  = new HyperPlane(affineBasis, false);
  }



}