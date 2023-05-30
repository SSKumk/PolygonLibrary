using System.Diagnostics;
using System.Globalization;
using NUnit.Framework;
using PolygonLibrary.Basics;
using PolygonLibrary.Polyhedra.ConvexPolyhedra;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

namespace Tests.GW_hDTests;

[TestFixture]
public class ISubspacePointTests {

  /// <summary>
  /// 6D-point project to 5D --> 4D --> 3D --> 2D
  /// </summary>
  [Test]
  public void ProjectTo_1() {

    AffineBasis aBasis5 = new AffineBasis(6,5);
    AffineBasis aBasis4 = new AffineBasis(5,4);
    AffineBasis aBasis3 = new AffineBasis(4,3);
    AffineBasis aBasis2 = new AffineBasis(3,2);

    Point          p6 = new Point(new double[] { 1, 2, 3, 4, 5, 6 });
    ISubspacePoint p5 = SubPoint.ProjectTo(aBasis5, p6);
    ISubspacePoint p4 = p5.ProjectTo(aBasis4);
    ISubspacePoint p3 = p4.ProjectTo(aBasis3);
    ISubspacePoint p2 = p3.ProjectTo(aBasis2);

    Console.WriteLine($"{p5}. P = {p5.Parent}. O = {p5.Original}");
    Console.WriteLine($"{p4}. P = {p4.Parent}. O = {p4.Original}");
    Console.WriteLine($"{p3}. P = {p3.Parent}. O = {p3.Original}");
    Console.WriteLine($"{p2}. P = {p2.Parent}. O = {p2.Original}");

    Point p3c = new Point((Point)p3);
    Debug.Assert((SubPoint)p2.Parent! == p3c);
    Debug.Assert(p3.Parent!.Equals(p4));
    Debug.Assert(p4.Parent!.Equals(p5));
    Debug.Assert(p5.Parent!.Equals(p6));

    Debug.Assert(p2.Original == p6);
    Debug.Assert(p3.Original == p6);
    Debug.Assert(p4.Original == p6);
    Debug.Assert(p5.Original == p6);
  }
}
