using System.Globalization;
using DoubleDouble;
// using static CGLibrary.Geometry<double, Graphics.DConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Graphics.DDConvertor>;

namespace Graphics;

public class Visualization {

  private class VectorMixedProductComparer : IComparer<Vector> {

    private readonly Vector _outerNormal;
    private readonly Vector _firstPoint;

    public VectorMixedProductComparer(Vector outerNormal, Vector firstPoint) {
      _outerNormal = outerNormal;
      _firstPoint  = firstPoint;
    }

    /// <summary>
    /// The cross-product of two 3D-vectors.
    /// </summary>
    /// <param name="v">The first vector.</param>
    /// <param name="u">The second vector.</param>
    /// <returns>The outward normal to the plane of v and u.</returns>
    public static Vector CrossProduct3D(Vector v, Vector u) {
      ddouble[] crossProduct = new ddouble[3];
      crossProduct[0] = v[1] * u[2] - v[2] * u[1];
      crossProduct[1] = v[2] * u[0] - v[0] * u[2];
      crossProduct[2] = v[0] * u[1] - v[1] * u[0];

      return new Vector(crossProduct);
    }

    public int Compare(Vector? v1, Vector? v2) {
      if (v1 is null && v2 is null)
        return 0;
      if (v1 is null)
        return -1;
      if (v2 is null)
        return 1;

      return Tools.CMP(CrossProduct3D(v1 - _firstPoint, v2 - _firstPoint) * _outerNormal);
    }

  }

  public class Facet {

    public readonly IEnumerable<Vector> Vs;
    public readonly Vector              Normal;

    public Facet(IEnumerable<Vector> vs, Vector normal) {
      Vs     = vs;
      Normal = normal;
    }

  }

  public static void WritePLY(ConvexPolytop P, ParamWriter prW) {
    P = Validate(P);

    List<Vector> VList = P.Vrep.ToList();
    List<Facet>  FList = new List<Facet>();
    AddToFList(ref FList, P);

    WritePLY_File(prW, VList, FList);
  }

  public static void WritePLY(ConvexPolytop P, Vector x, ParamWriter prW) {
    P = Validate(P);

    List<Vector> VList = P.Vrep.ToList();
    List<Facet>  FList = new List<Facet>();
    AddToFList(ref FList, P);

    ConvexPolytop cube = ConvexPolytop.RectParallel(-0.001 * Vector.Ones(3), 0.001 * Vector.Ones(3)).Shift(x);
    VList.AddRange(cube.Vrep);
    AddToFList(ref FList, cube);

    WritePLY_File(prW, VList, FList);
  }

  private static void WritePLY_File(ParamWriter prW, List<Vector> VList, List<Facet> FList) {
    // Пишем в файл в формате .ply
    // шапка
    prW.WriteLine("ply");
    prW.WriteLine("format ascii 1.0");
    prW.WriteLine($"element vertex {VList.Count}");
    prW.WriteLine("property float x");
    prW.WriteLine("property float y");
    prW.WriteLine("property float z");
    prW.WriteLine($"element face {FList.Count}");
    prW.WriteLine("property list uchar int vertex_index");
    prW.WriteLine("end_header");
    // вершины
    foreach (Vector v in VList) {
      prW.WriteLine(v.ToStringBraceAndDelim(null, null, ' '));
    }
    // грани
    foreach (Facet F in FList) {
      prW.Write($"{F.Vs.Count()} ");
      foreach (Vector vertex in F.Vs) {
        prW.Write($"{VList.IndexOf(vertex)} ");
      }
      prW.WriteLine();
    }
  }

  private static ConvexPolytop Validate(ConvexPolytop P) {
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

    switch (P.SpaceDim) {
      case 1: throw new NotImplementedException("If P.Dim == 1. Непонятно, что делать."); break;
      case 2: P = P.LiftUp(3, 0); break;
      case 3: break;
      default:
        throw new ArgumentException($"The dimension of the space must be equal less or equal 3! Found spaceDim = {P.SpaceDim}.");
    }

    return P;
  }

  public static void AddToFList(ref List<Facet> FList, ConvexPolytop polytop) {
    foreach (FLNode F in polytop.FLrep.Lattice[2]) {
      HyperPlane hp = new HyperPlane(F.AffBasis, true, (polytop.FLrep.Top.InnerPoint, false));
      FList.Add
        (
         new Facet
           (
            F.Vertices.ToList().OrderByDescending(v => v, new VectorMixedProductComparer(hp.Normal, F.Vertices.First())).ToArray()
          , hp.Normal
           )
        );
    }
  }

}

public class Program {

  private static readonly string pathData =
    // "E:\\Work\\PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_Computations";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations";

  public static void Main() {
    Tools.Eps = 1e-16;

    int       dim    = 3;
    SolverLDG solver = new SolverLDG(pathData, "SimpleMotion", true);
     var tMin = solver.Solve(true);
    var traj = solver.Euler(new Vector(new ddouble[]{0,1,2}), tMin, solver.gd.T);
    // var traj = solver.Euler(0.5 * Vector.Ones(dim), tMin, solver.gd.T);
    // var traj = solver.Euler(0.5*Vector.MakeOrth(dim,2), tMin, solver.gd.T);

    Directory.CreateDirectory(solver.PicturesPath);
    int i = 0;
    for (ddouble t = tMin; Tools.LT(t, solver.gd.T); t += solver.gd.dt, i++) {
      using ParamWriter prW = new ParamWriter(Path.Combine(solver.PicturesPath, $"{DDConvertor.ToDouble(t):0.00}).ply"));

      Visualization.WritePLY(solver.W[t], traj[i], prW);
    }
  }

}
