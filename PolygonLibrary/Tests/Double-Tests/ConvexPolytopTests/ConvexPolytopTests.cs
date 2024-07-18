using System.Diagnostics;
using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;


namespace Tests.DoubleTests.ConvexPolytopTests
{
  [TestFixture]
  public class ConvexPolytopTests
  {
    [Test]
    public void FLNodeCompareToTest1()
    {
      FLNode fLNode1 = new FLNode(new Vector(new double[] { 1 }));
      FLNode fLNode2 = new FLNode(new Vector(new double[] { 0 }));
      FLNode fLNode3 = new FLNode(new Vector(new double[] { -1 }));
      FLNode? fLNode4 = null;

      Assert.That(fLNode1.CompareTo(fLNode1), Is.EqualTo(0));
      Assert.That(fLNode1.CompareTo(fLNode2), Is.EqualTo(1));
      Assert.That(fLNode1.CompareTo(fLNode3), Is.EqualTo(1));
      Assert.That(fLNode1.CompareTo(fLNode4), Is.EqualTo(1));

      Assert.That(fLNode2.CompareTo(fLNode1), Is.EqualTo(-1));
      Assert.That(fLNode2.CompareTo(fLNode2), Is.EqualTo(0));
      Assert.That(fLNode2.CompareTo(fLNode3), Is.EqualTo(1));
    }

    [Test]
    public void FLNodeCompareToTest2()
    {

      FLNode fLNode1 = new FLNode(new List<Vector>() { new Vector([1, 1]), new Vector([2, 2]), new Vector([2, 0]) });
      FLNode fLNode2 = new FLNode(new List<Vector>() { new Vector([1, 1]), new Vector([2, -2]), new Vector([-2, 0]) });
      FLNode fLNode3 = new FLNode(new List<Vector>() { new Vector([1, 1]), new Vector([2, 2]) });

      Assert.That(fLNode2.CompareTo(fLNode1), Is.EqualTo(-1));
      Assert.That(fLNode2.CompareTo(fLNode2), Is.EqualTo(0));
      Assert.That(fLNode2.CompareTo(fLNode3), Is.EqualTo(1));
    }
  }

}