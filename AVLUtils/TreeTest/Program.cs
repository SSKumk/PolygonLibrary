using System;
using System.Collections.Generic;
using System.Linq;
using AVLUtils;

namespace TreeTest
{
  internal class Program
  {
    public class MyPoint : IComparable<MyPoint>
    {
      public readonly int x;
      public readonly int y;

      public MyPoint () => x = y = 0;

      public MyPoint (int nx, int ny)
      {
        x = nx;
        y = ny;
      }

      public override string ToString () => "(" + x + ";" + y + ")";

      public int CompareTo (MyPoint other)
      {
        int res = x.CompareTo (other.x);
        if (res != 0) {
          return res;
        } else {
          return y.CompareTo (other.y);
        }
      }
    }

    private static void ClrScr()
    {
      for (int i = 0; i < 30; i++) {
        Console.WriteLine ();
      }
    }

    private static void Main ()
    {
      ConsoleKeyInfo ans;
      do
      {
        ClrScr ();
        Console.WriteLine ("0 - exit");
        Console.WriteLine ("1 - tree test");
        Console.WriteLine ("2 - set test");
        Console.WriteLine ("3 - dictionary test");
        Console.Write (" Your choice: ");
        ans = Console.ReadKey ();

        switch (ans.KeyChar)
        {
          case '1':
            TreeTest ();
            break;
          case '2':
            SetTest ();
            break;
          case '3':
            DictTest ();
            break;
        }

      } while (ans.KeyChar != '0');

    }

    private static void SetTest()
    {
      Console.WriteLine ("\nSet test isn't implemented yet...");
      Console.ReadKey();
    }

    private static void TreeTest ()
    {
      #region int testing
      AVLTree<int> tr = new AVLTree<int> ();

      int i;
      for (i = 1; i < 16; i++) {
        tr.Add (2 * i);
      }

      IEnumerator<int> ien = tr.GetEnumerator (100);

      int n, n1;

      foreach (int k in tr) {
        Console.WriteLine (k);
      }

      n = 6;
      if (tr.Next (n, out n1)) {
        Console.WriteLine ("Next to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Next to " + n + " is absent");
      }

      n = 7;
      if (tr.Next (n, out n1)) {
        Console.WriteLine ("Next to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Next to " + n + " is absent");
      }

      n = 30;
      if (tr.Next (n, out n1)) {
        Console.WriteLine ("Next to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Next to " + n + " is absent");
      }

      n = 36;
      if (tr.Next (n, out n1)) {
        Console.WriteLine ("Next to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Next to " + n + " is absent");
      }

      n = 6;
      if (tr.Prev (n, out n1)) {
        Console.WriteLine ("Previous to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Previous to " + n + " is absent");
      }

      n = 5;
      if (tr.Prev (n, out n1)) {
        Console.WriteLine ("Previous to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Previous to " + n + " is absent");
      }

      n = 2;
      if (tr.Prev (n, out n1)) {
        Console.WriteLine ("Previous to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Previous to " + n + " is absent");
      }

      n = -2;
      if (tr.Prev (n, out n1)) {
        Console.WriteLine ("Previous to " + n + " is " + n1);
      } else {
        Console.WriteLine ("Previous to " + n + " is absent");
      }

      ien.Dispose();
      Console.WriteLine ("---------------------");

      Console.ReadKey (true);
      #endregion

      #region Vector testing
      AVLTree<MyPoint> ptr = new AVLTree<MyPoint> ();
      Random r = new Random ();
      MyPoint p, p1 = null;

      for (i = 1; i < 16; i++)
      {
        p = new MyPoint (r.Next (10), r.Next (10));
        Console.WriteLine ($"{i,2}: adding " + p);
        ptr.Add (p);
        if (i == 10) {
          p1 = p;
        }
      }

      Console.WriteLine ("-----------------");
      foreach (MyPoint pp in ptr) {
        Console.WriteLine (pp);
      }

      Console.WriteLine ("-----------------");

      IEnumerator<MyPoint> en;
      
      en = ptr.GetEnumerator (p1);
      do
      {
        Console.WriteLine (en.Current);
      } while (en.MoveNext());

      en.Dispose();
      Console.WriteLine ("-----------------");

      Console.Write ("Next to " + p1 + " is ");
      MyPoint p2;
      if (ptr.Next (p1, out p2)) {
        Console.WriteLine (p2);
      } else {
        Console.WriteLine ("absent");
      }

      Console.WriteLine ("-----------------");

      en = ptr.GetReverseEnumerator ();
      while (en.MoveNext())
      {
        Console.WriteLine (en.Current);
      }

      en.Dispose();
      
      Console.WriteLine ("-----------------");

      en = ptr.GetReverseEnumerator (p1);
      Console.WriteLine (p1);
      while (en.MoveNext())
      {
        Console.WriteLine (en.Current);
      }
      
      en.Dispose();

      Console.ReadKey (true);
      #endregion

      #region Remove test
      int N = 50, N1 = N * 2 / 3;
      AVLTree<int> itr = new AVLTree<int> ();
      for (i = 1; i <= N; i++)
      {
        //Console.WriteLine("Number of elements before adding: " + itr.Count);
        //Console.WriteLine("The tree before adding:\n" + itr);
        //Console.WriteLine("-----------------------------------------------------------------");
        //Console.ReadKey(true);
        itr.Add (i);
      }

      Console.WriteLine ("Final number of elements: " + itr.Count);
      Console.WriteLine ("The final tree state:\n" + itr);
      Console.WriteLine ("-----------------------------------------------------------------\n");
      Console.ReadKey (true);

      Console.WriteLine ("Removing " + 6);
      itr.Remove (6);
      Console.WriteLine ("--------------------------------\n" + itr);
      Console.ReadKey (true);

      for (i = 1; i <= N1; i++)
      {
        int nn = r.Next (1, N);
        itr.Remove (nn);
        //Console.WriteLine("Removing " + nn);
        //Console.WriteLine("--------------------------------\n" + itr);
        //Console.ReadKey(true);
      }

      Console.WriteLine ("The tree after removing:\n" + itr + 
        "\n--------------------------------\n");

      for (i = 0; i < itr.Count; i++)
      {
        Console.WriteLine ("" + i + "th elem: " + itr[i]);
      }
      Console.ReadKey (true);
      #endregion

      #region AddRange test

      AVLTree<int> trInt = new AVLTree<int> ();
      int[] ar = new int[] { 5, 4, 7, 1, 9, 3, 2, 6, 8, 0, 11 };

      trInt.AddRange (ar);
      Console.WriteLine ("-----------------");
      Console.WriteLine ("Adding range:");

      foreach (int k in ar) {
        Console.WriteLine (k);
      }

      Console.WriteLine ("Resulting tree:");
      foreach (int k in trInt) {
        Console.WriteLine (k);
      }

      Console.WriteLine ("-----------------");
      Console.ReadKey (true);
      #endregion

      #region CopyTo test
      int[] arr = new int[20];
      for (i = 0; i < 20; i++) {
        arr[i] = 100 * (i - 10);
      }

      Console.Write ("The tree: ");
      foreach (int k in trInt) {
        Console.Write (k + " ");
      }

      Console.WriteLine ();

      Console.Write ("The array before: ");
      foreach (int k in arr) {
        Console.Write (k + " ");
      }

      Console.WriteLine ();

      Console.WriteLine ("CopyTo from the 5th position");
      trInt.CopyTo (arr, 5);

      Console.Write ("The array after: ");
      foreach (int k in arr) {
        Console.Write (k + " ");
      }

      Console.WriteLine ();

      Console.WriteLine ("-----------------");
      Console.ReadKey (true);
      #endregion

      #region Cyclic tree test
      AVLTree<int> cycTr = new AVLTree<int> ();
      cycTr.AddRange (ar);
      IEnumerator<int> enc;

      enc = cycTr.GetCyclicEnumerator ();
      enc.MoveNext ();
      Console.Write ("Direct cyclic tree: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicEnumerator (2);
      Console.Write ("Direct cyclic tree from a present value 2: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicEnumerator (10);
      Console.Write ("Direct cyclic tree from an absent value 10: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicEnumerator (12);
      Console.Write ("Direct cyclic tree from a large value 12: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicReverseEnumerator ();
      enc.MoveNext ();
      Console.Write ("Reverse cyclic tree: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicReverseEnumerator (2);
      Console.Write ("Reverse cyclic tree from a present value 2: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicReverseEnumerator (10);
      Console.Write ("Reverse cyclic tree from an absent value 10: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      enc = cycTr.GetCyclicReverseEnumerator (-2);
      Console.Write ("Reverse cyclic tree from a small value -2: ");
      for (int k = 0; k < 15; k++)
      {
        Console.Write (enc.Current + " ");
        enc.MoveNext ();
      }
      Console.WriteLine ();

      Console.WriteLine ("-----------------");
      Console.ReadKey (true);
      #endregion
    }

    private static void DictTest ()
    {
      Console.WriteLine ("\n");
      int kInit = (int)Math.Pow (26, 2), k, i;
      AVLDictionary<string, int> d = new AVLDictionary<string, int> ();
      Random rnd = new Random ();

      // filling dictionary
      for (i = 0, k = kInit; i < 10; i++, k += 10)
      {
        int num = rnd.Next(10,99);
        string str = Int2Str (k);
        switch (i) {
          case < 3:
            d.Add (str, num);
            break;

          case < 5:
            d.Add (new KeyValuePair<string, int> (str, num));
            break;

          default:
            d[str] = num;
            break;
        }
      }

      // Printing dictionary through foreach
      Console.WriteLine ("Printing using foreach: ");
      foreach (KeyValuePair<string, int> p in d) {
        Console.Write ("('" + p.Key + "';" + p.Value + ") ");
      }

      Console.WriteLine ();

      // Printing dictionary through indexer
      Console.WriteLine ("Printing using indexer: ");
      for (i = 0, k = kInit; i < 10; i++, k += 10)
      {
        string str = Int2Str (k);
        Console.Write ("('" + str + "';" + d[str] + ") ");
      }
      Console.WriteLine ();
      
      

      // Change the first element and print through keys and values collections
      d["baa"] = -10;
      IEnumerator<string> keyEn = d.Keys.GetEnumerator ();
      IEnumerator<int> valEn = d.Values.GetEnumerator ();
      Console.WriteLine ("Change the first element and printing through keys and values collection:");
      while (keyEn.MoveNext ())
      {
        valEn.MoveNext ();
        Console.Write ("('" + keyEn.Current + "';" + valEn.Current + ") ");
      }
      Console.WriteLine ();
      
      keyEn.Dispose();
      valEn.Dispose();

      string str1 = Int2Str (kInit + 50);
      KeyValuePair<string,int> pp;
      d.Next (str1, out pp);
      Console.WriteLine ("('" + str1 + "';" + d[str1] + ") -next-> " + "('" + pp.Key + "';" + pp.Value + ") ");
      d.Prev (str1, out pp);
      Console.WriteLine ("('" + str1 + "';" + d[str1] + ") -prev-> " + "('" + pp.Key + "';" + pp.Value + ") ");

      str1 = d.Keys.Min();
      if (d.Prev (str1, out pp)) {
        Console.WriteLine ("There is a predecessor of '" + str1 + "'");
      } else {
        Console.WriteLine ("There is no predecessor of '" + str1 + "'");
      }

      str1 = d.Keys.Max ();
      if (d.Next (str1, out pp)) {
        Console.WriteLine ("There is a successor of '" + str1 + "'");
      } else {
        Console.WriteLine ("There is no successor of '" + str1 + "'");
      }

      // Cyclic enumerators
      str1 = Int2Str (kInit + 50);
      pp = new KeyValuePair<string,int>(str1, 0);
      IEnumerator<KeyValuePair<string, int>> en;
      en = d.GetCyclicEnumerator (pp);
      Console.WriteLine ("Direct cyclic enumerator:");
      for (i = 0; i < 15; i++, en.MoveNext()) {
        Console.Write ("('" + en.Current.Key + "';" + en.Current.Value + ") ");
      }

      Console.WriteLine ();

      en = d.GetCyclicReverseEnumerator (pp);
      Console.WriteLine ("Reverse cyclic enumerator:");
      for (i = 0; i < 15; i++, en.MoveNext ()) {
        Console.Write ("('" + en.Current.Key + "';" + en.Current.Value + ") ");
      }

      Console.WriteLine ();

      Console.ReadKey ();
    }

    private static string Int2Str (int n)
    {
      if (n == 0) {
        return "";
      } else {
        return Int2Str (n / 26) + (char)(n % 26 + 'a');
      }
    }

  }
}
