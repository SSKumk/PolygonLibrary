using NUnit.Framework;
using static CGLibrary.Geometry<double, DConvertor>;

namespace Tests.ConvexPolyhedra;

[TestFixture]
public class HashSetTests {

  [Test]
  public void HashCodeTest1() {
    Point origin = new Point(new Vector(5));
    Point e1     = new Point(Vector.CreateOrth(5, 1));
    Point e2     = new Point(Vector.CreateOrth(5, 2));
    Point e3     = new Point(Vector.CreateOrth(5, 3));
    Point e4     = new Point(Vector.CreateOrth(5, 4));
    Point e5     = new Point(Vector.CreateOrth(5, 5));

    Point e6 = new Point(new double[] { 1.0000000001, 0, 0, 0, 0 });

    HashSet<Point> hs1 = new HashSet<Point>() { e1, e2 };
    HashSet<Point> hs2 = new HashSet<Point>() { e2, e6 };

    Console.WriteLine($"HashCode e1 = {e1.GetHashCode()}");
    Console.WriteLine($"HashCode e6 = {e6.GetHashCode()}");

    Console.WriteLine($"HashCode 1 and 6 = {HashCode.Combine(1, 6)}");
    Console.WriteLine($"HashCode 6 and 1 = {HashCode.Combine(6, 1)}");


    Console.WriteLine(hs1.SetEquals(hs2));

    int code1;
    int code2;

    code1 = 0;

    foreach (Point vertex in hs1.OrderBy(v => v)) {
      code1 = HashCode.Combine(code1, vertex.GetHashCode());
    }

    code2 = 0;

    foreach (Point vertex in hs2.OrderBy(v => v)) {
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

    foreach (Point vertex in hs1.OrderBy(v => v)) {
      code1 = HashCode.Combine(code1, vertex.GetHashCode());
    }

    code2 = 0;

    foreach (Point vertex in hs2.OrderBy(v => v)) {
      code2 = HashCode.Combine(code2, vertex.GetHashCode());
    }
    Console.WriteLine(code1);
    Console.WriteLine(code2);
  }

  [Test]
  public void PointHashTest() {
    Point p1 = new Point(new[] { 0.99999999999, 1, 1 });
    Point p2 = new Point(new[] { 1.0, 1, 1 });
    
    Console.WriteLine(p1.GetHashCode());
    Console.WriteLine(p2.GetHashCode());
  }

}
