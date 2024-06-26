using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests.GW_hDTests; 

[TestFixture]
public class HashSetTests {

  [Test]
  public void HashCodeTest1() {
    Vector origin = new Vector(new Vector(5));
    Vector e1     = new Vector(Vector.MakeOrth(5, 1));
    Vector e2     = new Vector(Vector.MakeOrth(5, 2));
    Vector e3     = new Vector(Vector.MakeOrth(5, 3));
    Vector e4     = new Vector(Vector.MakeOrth(5, 4));
    Vector e5     = new Vector(Vector.MakeOrth(5, 5));

    Vector e6 = new Vector(new double[] { 1.0000000001, 0, 0, 0, 0 });

    HashSet<Vector> hs1 = new HashSet<Vector>() { e1, e2 };
    HashSet<Vector> hs2 = new HashSet<Vector>() { e2, e6 };

    Console.WriteLine($"HashCode e1 = {e1.GetHashCode()}");
    Console.WriteLine($"HashCode e6 = {e6.GetHashCode()}");

    Console.WriteLine($"HashCode 1 and 6 = {HashCode.Combine(1, 6)}");
    Console.WriteLine($"HashCode 6 and 1 = {HashCode.Combine(6, 1)}");


    Console.WriteLine(hs1.SetEquals(hs2));

    int code1;
    int code2;

    code1 = 0;

    foreach (Vector vertex in hs1.OrderBy(v => v)) {
      code1 = HashCode.Combine(code1, vertex.GetHashCode());
    }

    code2 = 0;

    foreach (Vector vertex in hs2.OrderBy(v => v)) {
      code2 = HashCode.Combine(code2, vertex.GetHashCode());
    }
    Console.WriteLine(code1);
    Console.WriteLine(code2);


    hs2.Add(e1);
    Console.WriteLine(hs1.SetEquals(hs2));

    hs2.Add(e3);
    Console.WriteLine(hs1.SetEquals(hs2));

    hs1.Add(e3);
    code1 = 0;

    foreach (Vector vertex in hs1.OrderBy(v => v)) {
      code1 = HashCode.Combine(code1, vertex.GetHashCode());
    }

    code2 = 0;

    foreach (Vector vertex in hs2.OrderBy(v => v)) {
      code2 = HashCode.Combine(code2, vertex.GetHashCode());
    }
    Console.WriteLine(code1);
    Console.WriteLine(code2);
  }

  [Test]
  public void PointHashTest() {
    Vector p1 = new Vector(new[] { 0.99999999999, 1, 1 });
    Vector p2 = new Vector(new[] { 1.0, 1, 1 });
    
    Console.WriteLine(p1.GetHashCode());
    Console.WriteLine(p2.GetHashCode());
  }

}