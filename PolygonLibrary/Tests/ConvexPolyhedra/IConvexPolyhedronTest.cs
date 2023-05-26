using System.Diagnostics;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

namespace Tests.ConvexPolyhedra;

[TestFixture]
public class IConvexPolyhedronTest {

  [Test]
  public void ConstructSimplexTest_1() {
    Point       origin = new Point(new Vector(5));
    Point       e1     = new Point(Vector.CreateOrth(5, 1));
    Point       e2     = new Point(Vector.CreateOrth(5, 2));
    Point       e3     = new Point(Vector.CreateOrth(5, 3));
    Point       e4     = new Point(Vector.CreateOrth(5, 4));
    Point       e5     = new Point(Vector.CreateOrth(5, 5));

    Point[] simplex5D = new Point[] { origin, e1, e2, e3, e4,e5 };
    Point[] simplex4D = new Point[] { origin, e1, e2, e3, e4 };
    Point[] simplex3D = new Point[] { origin, e1, e2, e3 };

    Simplex tetrahedron5D = new Simplex(simplex5D, 5);
    Simplex tetrahedron4D  = new Simplex(simplex4D, 4);
    Simplex tetrahedron3D  = new Simplex(simplex3D, 3);
    Simplex tetrahedron3D_ = new Simplex(tetrahedron3D);
  }
  

  [Test]
  public void ConstructNonSimplexTest_1() {
    Point origin = new Point(new Vector(5));
    Point e1     = new Point(Vector.CreateOrth(5, 1));
    Point e2     = new Point(Vector.CreateOrth(5, 2));
    Point e3     = new Point(Vector.CreateOrth(5, 3));
    Point e4     = new Point(Vector.CreateOrth(5, 4));
    Point e5     = new Point(Vector.CreateOrth(5, 5));
    
  }
}
