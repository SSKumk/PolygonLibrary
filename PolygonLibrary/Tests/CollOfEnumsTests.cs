using NUnit.Framework;

using PolygonLibrary.Toolkit;

namespace Tests
{
  [TestFixture]
  public class CollOfEnumsTests
  {
    int[]
      ar1 = new int[] { 1, 2, 3, 4, 5 },
      ar2 = new int[] { 10, 20, 30, 40, 50, 60 },
      ar3 = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

    [Test]
    public void TestCompositeCollection1() {
      CollectionOfEnumerables<int> coll;
      IEnumerator<int> en;

      List<int> res = new List<int>();

      coll = new CollectionOfEnumerables<int>(ar1, ar2);
      en = coll.GetEnumerator();
      while (en.MoveNext()) {
        res.Add(en.Current);
      }

      en.Dispose();

      Assert.That(res, Has.Count.EqualTo(11));
    }

    [Test]
    public void TestCompositeCollection2() {
      CollectionOfEnumerables<int> coll;
      IEnumerator<int> en;

      List<int> res = new List<int>();

      coll = new CollectionOfEnumerables<int>(ar1, ar2);
      en = coll.GetEnumerator(100);
      while (en.MoveNext()) {
        res.Add(en.Current);
      }

      en.Dispose();

      Assert.That(res, Has.Count.EqualTo(11));
    }

    [Test]
    public void TestCompositeCollection3() {
      CollectionOfEnumerables<int> coll;
      IEnumerator<int> en;

      List<int> res = new List<int>();

      coll = new CollectionOfEnumerables<int>(ar1, ar2);
      en = coll.GetEnumerator(20);
      while (en.MoveNext()) {
        res.Add(en.Current);
      }

      en.Dispose();

      Assert.That(res, Has.Count.EqualTo(4));
    }

    [Test]
    public void TestCompositeCollection4() {
      CollectionOfEnumerables<int> coll;
      IEnumerator<int> en;

      List<int> res = new List<int>();

      coll = new CollectionOfEnumerables<int>(ar1, ar2);
      coll.Add(ar3);
      en = coll.GetEnumerator();
      while (en.MoveNext()) {
        res.Add(en.Current);
      }

      en.Dispose();

      Assert.That(res, Has.Count.EqualTo(21));
    }

    [Test]
    public void TestCompositeCollection5() {
      CollectionOfEnumerables<int> coll;
      IEnumerator<int> en;

      List<int> res = new List<int>();

      coll = new CollectionOfEnumerables<int>(ar1, ar2);
      coll.Add(ar3);
      coll.Add (ar1);
      en = coll.GetEnumerator ();
      while (en.MoveNext ()) {
        res.Add (en.Current);
      }
      en.Dispose();

      Assert.That(res, Has.Count.EqualTo(26));
    }
  }
}
