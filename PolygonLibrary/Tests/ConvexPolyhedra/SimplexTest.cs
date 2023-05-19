using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

namespace Tests.ConvexPolyhedra;

[TestFixture]
public class SimplexTest {

  [Test]
  public void ConstructSimplexTest_1() {
    Point origin = new Point(new Vector(3));
    Point e1     = new Point(Vector.CreateOrth(3, 1));
    Point e2     = new Point(Vector.CreateOrth(3, 2));
    Point e3     = new Point(Vector.CreateOrth(3, 3));

    Point[] Ps = new Point[] { origin, e1, e2, e3 };

    Simplex tetrahedron = new Simplex(Ps);
    Console.WriteLine(tetrahedron.Dim);
  }

}
