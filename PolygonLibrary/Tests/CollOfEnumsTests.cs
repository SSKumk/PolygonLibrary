using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using PolygonLibrary;
using PolygonLibrary.Toolkit;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;

namespace Tests
{
  [TestClass]
  public class CollOfEnumsTests
  {
    [TestMethod]
    public void TestCompositeCollection ()
    {
      int[]
        ar1 = new int[] { 1, 2, 3, 4, 5 },
        ar2 = new int[] { 10, 20, 30, 40, 50, 60 },
        ar3 = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

      CollectionOfEnumerables<int> coll;
      IEnumerator<int> en;

      List<int> res = new List<int>();

      coll = new CollectionOfEnumerables<int> (ar1, ar2);
      en = coll.GetEnumerator ();
      while (en.MoveNext ())
        res.Add (en.Current);
      Assert.AreEqual (res.Count, 11);

      res.Clear ();
      en = coll.GetEnumerator (100);
      while (en.MoveNext ())
        res.Add (en.Current);
      Assert.AreEqual (res.Count, 11);

      res.Clear ();
      en = coll.GetEnumerator (20);
      while (en.MoveNext ())
        res.Add (en.Current);
      Assert.AreEqual (res.Count, 4);

      res.Clear ();
      coll.Add (ar3);
      en = coll.GetEnumerator ();
      while (en.MoveNext ())
        res.Add (en.Current);
      Assert.AreEqual (res.Count, 21);

      res.Clear ();
      coll.Add (ar1);
      en = coll.GetEnumerator ();
      while (en.MoveNext ())
        res.Add (en.Current);
      Assert.AreEqual (res.Count, 26);
    }
  }
}
