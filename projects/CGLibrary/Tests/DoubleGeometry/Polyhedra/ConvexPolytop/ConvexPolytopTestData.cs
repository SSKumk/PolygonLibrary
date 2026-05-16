using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Polyhedra;

internal static class ConvexPolytopTestData {

  private static Vector BuildShift(int dim) => new(Enumerable.Range(1, dim).Select(i => 0.1 * i).ToArray());

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

  public static ConvexPolytop CreateShiftedStandardSimplex(int dim) {
    Vector shift = BuildShift(dim);

    List<Vector> vertices = [ -shift ];
    for (int i = 0; i < dim; i++) {
      double[] coords = new double[dim];
      coords[i] = 1.0;
      vertices.Add(new Vector(coords) - shift);
    }

    return ConvexPolytop.CreateFromPoints(vertices, true);
  }

  public static List<HyperPlane> CreateDegenerateVertexHrep(int dim) {
    List<HyperPlane> hps = new(2 * dim + 1);

    for (int i = 1; i <= dim; i++) {
      hps.Add(new HyperPlane(Vector.MakeOrth(dim, i), Tools.Zero));
    }

    hps.Add(new HyperPlane(-Vector.Ones(dim), Tools.Zero));

    for (int i = 1; i <= dim; i++) {
      hps.Add(new HyperPlane(-Vector.MakeOrth(dim, i), Tools.One));
    }

    return hps;
  }

  public static ConvexPolytop CreateDegenerateVertexPolytope(int dim) => ConvexPolytop.CreateFromHalfSpaces(CreateDegenerateVertexHrep(dim));

}
