using System.Numerics;
using CGLibrary;
using Graphics.Draw;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics;

public class VisTools {

  private static readonly SortedDictionary<double, ConvexPolytop> _spheres =
    new SortedDictionary<double, ConvexPolytop>(Tools.TComp);

  public static ConvexPolytop Sphere(double radius) {
    if (_spheres.TryGetValue(radius, out ConvexPolytop? sphere)) {
      return sphere;
    }
    ConvexPolytop sp = ConvexPolytop.Sphere(Geometry<double, DConvertor>.Vector.Zero(3), radius, 6, 6).GetInFLrep();
    _spheres.Add(radius, sp);

    return sp;
  }


  public struct Color {

    public int red;
    public int green;
    public int blue;

    public static Color Default => new Color(192, 192, 192);

    public Color(int red, int green, int blue) {
      ValidateColorComponent(red, nameof(red));
      ValidateColorComponent(green, nameof(green));
      ValidateColorComponent(blue, nameof(blue));

      this.red   = red;
      this.green = green;
      this.blue  = blue;
    }

    private void ValidateColorComponent(int value, string componentName) {
      if (value < 0 || value > 255) {
        throw new ArgumentException($"The value of '{componentName}' must lie within [0, 255]! Found {value}.");
      }
    }

  }

  public class FacetColor : Facet {

    public Color color;

    public FacetColor(
        IEnumerable<Geometry<double, DConvertor>.Vector> vertices
      , Geometry<double, DConvertor>.Vector              normal
      , Color?                                           color
      ) : base(vertices, normal) {
      this.color = color ?? Color.Default;
    }

  }

  public class Facet {

    public readonly IEnumerable<Geometry<double, DConvertor>.Vector> Vertices;
    public readonly Geometry<double, DConvertor>.Vector              Normal;

    public Facet(IEnumerable<Geometry<double, DConvertor>.Vector> vertices, Geometry<double, DConvertor>.Vector normal) {
      Vertices = vertices;
      Normal   = normal;
    }

  }

  public class VectorMixedProductComparer : IComparer<Geometry<double, DConvertor>.Vector> {

    private readonly Geometry<double, DConvertor>.Vector _outerNormal;
    private readonly Geometry<double, DConvertor>.Vector _firstPoint;

    public VectorMixedProductComparer(
        Geometry<double, DConvertor>.Vector outerNormal
      , Geometry<double, DConvertor>.Vector firstPoint
      ) {
      _outerNormal = outerNormal;
      _firstPoint  = firstPoint;
    }

    /// <summary>
    /// The cross-product of two 3D-vectors.
    /// </summary>
    /// <param name="v">The first vector.</param>
    /// <param name="u">The second vector.</param>
    /// <returns>The outward normal to the plane of v and u.</returns>
    public static Geometry<double, DConvertor>.Vector CrossProduct3D(
        Geometry<double, DConvertor>.Vector v
      , Geometry<double, DConvertor>.Vector u
      ) {
      double[] crossProduct = new double[3];
      crossProduct[0] = v[1] * u[2] - v[2] * u[1];
      crossProduct[1] = v[2] * u[0] - v[0] * u[2];
      crossProduct[2] = v[0] * u[1] - v[1] * u[0];

      return new Geometry<double, DConvertor>.Vector(crossProduct);
    }

    public int Compare(Geometry<double, DConvertor>.Vector? v1, Geometry<double, DConvertor>.Vector? v2) {
      if (v1 is null && v2 is null)
        return 0;
      if (v1 is null)
        return -1;
      if (v2 is null)
        return 1;

      return Tools.CMP(CrossProduct3D(v1 - _firstPoint, v2 - _firstPoint) * _outerNormal);
    }

  }


  public static void AddToFacetColorList(List<FacetColor> FList, ConvexPolytop polytop, Color? color = null) {
    foreach (FLNode F in polytop.FLrep.Lattice[2]) {
      HyperPlane hp = new HyperPlane(F.AffBasis, false, (polytop.FLrep.Top.InnerPoint, false));
      FList.Add
        (
         new FacetColor
           (
            F.Vertices.ToList().OrderByDescending(v => v, new VectorMixedProductComparer(hp.Normal, F.Vertices.First())).ToArray()
          , hp.Normal
          , color
           )
        );
    }
  }


  /// <summary>
  /// Generates a convex polytope representing a cylinder in 3D space.
  /// The cylinder is defined by two points (centers of the bases) and a radius.
  /// The circle approximation is fixed.
  /// </summary>
  /// <param name="p1">The center of the first base of the cylinder.</param>
  /// <param name="p2">The center of the second base of the cylinder.</param>
  /// <param name="radius">The radius of the cylinder.</param>
  /// <param name="segments">
  /// The number of segments used to approximate the circles at the bases.
  /// Default is 10, which means each circle is divided into 10 vertices.
  /// </param>
  /// <returns>
  /// A <see cref="ConvexPolytop"/> representing the cylinder.
  /// The polytop is constructed from the vertices of the two circular bases.
  /// </returns>
  public static ConvexPolytop Cylinder(
      Geometry<double, DConvertor>.Vector p1
    , Geometry<double, DConvertor>.Vector p2
    , double                              radius
    , int                                 segments = 10
    ) {
    Geometry<double, DConvertor>.Vector axe = p1 - p2; // ось цилиндра
    if (axe.Length < 1e-10) {
      throw new ArgumentException("Points p1 and p2 are too close or coincide.");
    }

    if (!new LinearBasis(axe).OrthogonalComplement()) {
      throw new Exception("VisTools.Cylinder: Strange internal error.");
    }

    Geometry<double, DConvertor>.Vector u1 = basePlane[0];
    Geometry<double, DConvertor>.Vector u2 = basePlane[1];

    List<Geometry<double, DConvertor>.Vector> vs = new List<Geometry<double, DConvertor>.Vector>();
    for (int i = 0; i < segments; i++) {
      double                              angle         = 2 * Math.PI * i / segments;
      Geometry<double, DConvertor>.Vector pointOnCircle = Math.Cos(angle) * u1 + Math.Sin(angle) * u2;
      Geometry<double, DConvertor>.Vector onCircle      = radius * pointOnCircle;
      vs.Add(p1 + onCircle);
      vs.Add(p2 + onCircle);
    }

    return ConvexPolytop.CreateFromPoints(vs);
  }

  public struct SeveralPolytopes {

    public SortedSet<Geometry<double, DConvertor>.Vector> vertices;  // список вершин
    public List<ConvexPolytop>                            polytopes; // набор выпуклых многогранников

    public SeveralPolytopes(SortedSet<Geometry<double, DConvertor>.Vector> vertices, List<ConvexPolytop> polytopes) {
      this.vertices  = vertices;
      this.polytopes = polytopes;
    }

  }

  public static SeveralPolytopes MakeCylinderOnTraj(
      List<Geometry<double, DConvertor>.Vector> traj
    , int                                       start
    , int                                       end
    , double                                    radius
    ) {
    SortedSet<Geometry<double, DConvertor>.Vector> vertices  = new SortedSet<Geometry<double, DConvertor>.Vector>();
    List<ConvexPolytop>                            polytopes = new List<ConvexPolytop>();

    for (int i = start; i < end; i++) {
      ConvexPolytop cylinder = Cylinder(traj[i], traj[i + 1], radius);
      vertices.UnionWith(cylinder.Vrep);
      polytopes.Add(cylinder);
    }

    return new SeveralPolytopes(vertices, polytopes);
  }


  public static void DrawPolytopePLY<TNum, TConv>(CGLibrary.Geometry<TNum, TConv>.ConvexPolytop polytop, string path)
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>, IFormattable
    where TConv : INumConvertor<TNum> {
    List<Facet> flist = new List<Facet>();
    Visualization<TNum, TConv>.AddToFacetList(flist, polytop);
    new PlyDrawer().SaveFrame(path, Visualization<TNum, TConv>.ToDSet(polytop.Vrep), flist);
  }

}
