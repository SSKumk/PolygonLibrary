using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

internal static class ConvexPolytopTestData {

  public static readonly SortedSet<Vector> UnitSquareVertices =
    [
      ConvexPolytopAssert.V(0, 0),
      ConvexPolytopAssert.V(1, 0),
      ConvexPolytopAssert.V(1, 1),
      ConvexPolytopAssert.V(0, 1)
    ];

  public static readonly SortedSet<Vector> UnitSquareWithInnerPoints =
    [
      ..UnitSquareVertices,
      ConvexPolytopAssert.V(0.5, 0.5),
      ConvexPolytopAssert.V(0.2, 0.7)
    ];

  public static readonly List<HyperPlane> UnitSquareHrep =
    [
      new HyperPlane(-Vector.MakeOrth(2, 1), Tools.Zero),
      new HyperPlane(Vector.MakeOrth(2, 1), Tools.One),
      new HyperPlane(-Vector.MakeOrth(2, 2), Tools.Zero),
      new HyperPlane(Vector.MakeOrth(2, 2), Tools.One)
    ];

  public static readonly Matrix Rotate90Counterclockwise =
    new(new double[,] {
      { 0, -1 },
      { 1,  0 }
    });

  public static ConvexPolytop CreateUnitSquareVrep() => ConvexPolytop.CreateFromPoints(UnitSquareVertices);

  public static ConvexPolytop CreateUnitSquareFlrep() => ConvexPolytop.CreateFromPoints(UnitSquareVertices, true);

  public static ConvexPolytop CreateUnitSquareHrepOnly() => ConvexPolytop.CreateFromHalfSpaces(UnitSquareHrep);

}
