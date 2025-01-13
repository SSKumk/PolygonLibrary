using LDG;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics;

public class Visualization {

  public struct Color {

    public int red;
    public int green;
    public int blue;

    public static Color Default => new Color(192, 192, 192);

    public Color(int red, int green, int blue) {
      this.red   = red;
      this.green = green;
      this.blue  = blue;
    }

  }

  public struct ColorAndRadius {

    public Color  color;
    public double radius;

    public ColorAndRadius(Color color, double radius) {
      this.color  = color;
      this.radius = radius;
    }

  }

  public struct AimTraj {

    public List<Vector>   points;
    public ColorAndRadius aim; // цвет и размер aim-точки

    public ColorAndRadius? aimCyl; // цвет и размер цилиндра, соединяющего aim-точку и текущее положение системы

    public AimTraj(List<Vector> points, ColorAndRadius aim, ColorAndRadius? aimCyl) {
      this.points = points;
      this.aim    = aim;
      this.aimCyl = aimCyl;
    }

  }

  public struct Traj {

    public List<Vector>   Ps;
    public ColorAndRadius trajPoint;

    public ColorAndRadius? traversed = null;
    public ColorAndRadius? remaining = null;

    public Traj(List<Vector> ps, ColorAndRadius trajPoint, ColorAndRadius? traversed, ColorAndRadius? remaining) {
      Ps             = ps;
      this.trajPoint = trajPoint;
      this.traversed = traversed;
      this.remaining = remaining;
    }

  }

  public struct TrajExtended { // траектория, точки прицеливания игроков и все настройки

    public Traj traj;

    public AimTraj? fp;
    public AimTraj? sp;

    public TrajExtended(Traj traj, AimTraj? fp, AimTraj? sp) {
      this.traj = traj;
      this.fp   = fp;
      this.sp   = sp;
    }

  }

  public struct SeveralPolytopes {

    public List<Vector>        vertices;  // список вершин
    public List<ConvexPolytop> polytopes; // набор выпуклых многогранников

    public SeveralPolytopes(List<Vector> vertices, List<ConvexPolytop> polytopes) {
      this.vertices  = vertices;
      this.polytopes = polytopes;
    }

  }

  public class Facet {

    public readonly IEnumerable<Vector> Vertices;
    public readonly Vector              Normal;

    public Color Color;

    public Facet(IEnumerable<Vector> vertices, Vector normal) : this(vertices, normal, Color.Default) { }

    public Facet(IEnumerable<Vector> vertices, Vector normal, Color color) {
      Vertices = vertices;
      Normal   = normal;
      Color    = color;
    }

  }

  private readonly TrajectoryMain<double, DConvertor> tr;

  public readonly string VisData;
  public readonly string VisConf;

  public readonly string OutFolderName;
  public readonly string GameDirName;

  public readonly string NumericalType;
  public readonly string Precision;

  public readonly List<int>       BrNames; // имена папок мостов, которые нужно рисовать
  public readonly List<TrajExtended> Trajs = new List<TrajExtended>();

  public Visualization(string pathLdg, string problemFileName, string visData, string visConf) {
    tr = new TrajectoryMain<double, DConvertor>(pathLdg, problemFileName);

    VisData = visData;
    VisConf = visConf;

    string confPath = Path.Combine(pathLdg, "Visualization", "!Configs");
    {
      ParamReader prD = new ParamReader(Path.Combine(confPath, VisData + ".visdata"));

      OutFolderName = prD.ReadString("Name");
      GameDirName   = prD.ReadString("GameDirName");
      NumericalType = prD.ReadString("NumericalType");
      Precision     = prD.ReadString("Precision");
    }

    ParamReader pr      = new ParamReader(Path.Combine(confPath, VisConf + ".visconfig"));
    BrNames = pr.ReadList<int>("Bridges");

    int trajCount = pr.ReadNumber<int>("TrajectoryCount");
    for (int i = 1; i <= trajCount; i++) {
      string       name     = pr.ReadString("Name");
      string       trajPath = Path.Combine(new string[] { pathLdg, "_Out", GameDirName, "Trajectories", name });
      List<Vector> Ps       = new ParamReader(Path.Combine(trajPath, "game.traj")).ReadVectors("Trajectory");

      ColorAndRadius  trajPoint = ReadColorAndRadius(pr);
      ColorAndRadius? traversed = null;
      if (pr.ReadBool("DrawTraversed")) {
        traversed = ReadColorAndRadius(pr);
      }
      ColorAndRadius? remaining = null;
      if (pr.ReadBool("DrawTraversed")) {
        remaining = ReadColorAndRadius(pr);
      }

      ColorAndRadius  fpAimPoint;
      ColorAndRadius? fpAimCyl;
      List<Vector>?   fpAim;
      AimTraj?        fpAimTraj = null;
      if (pr.ReadBool("DrawAimFP")) {
        fpAimPoint = ReadColorAndRadius(pr);
        fpAimCyl   = ReadColorAndRadius(pr);
        fpAim      = new ParamReader(Path.Combine(trajPath, "fp.aim")).ReadVectors("Aim");
        fpAimTraj  = new AimTraj(fpAim, fpAimPoint, fpAimCyl);
      }
      ColorAndRadius  spAimPoint;
      ColorAndRadius? spAimCyl;
      List<Vector>?   spAim;
      AimTraj?        spAimTraj = null;
      if (pr.ReadBool("DrawAimSP")) {
        spAimPoint = ReadColorAndRadius(pr);
        spAimCyl   = ReadColorAndRadius(pr);
        spAim      = new ParamReader(Path.Combine(trajPath, "sp.aim")).ReadVectors("Aim");
        spAimTraj  = new AimTraj(spAim, spAimPoint, spAimCyl);
      }

      Trajs.Add(new TrajExtended(new Traj(Ps, trajPoint, traversed, remaining), fpAimTraj, spAimTraj));
    }
  }

  public void MainDrawFunc() {
    for (double t = tr.tMin; Tools.LT(tr.T); t += tr.gd.dt) {
      // подготавливаем один кадр (frame)

      List<Vector> vertices = new List<Vector>(); // все вершины на данном кадре
      List<Facet>  facets   = new List<Facet>();  // все грани на данном кадре

      // рисовать мосты, если есть
      foreach (int brName in BrNames) {
        tr.Ws[brName]
      }
    }
    // Основной цикл по времени всей игры от t0 до T (где их достать?)
    // внутри цикла отдельная задача нарисовать мосты
    //              отдельная задача нарисовать траектории
    //              отдельная задача нарисовать точки прицеливания игроков
  }

  public SeveralPolytopes MakeCylinderOnTraj(List<Vector> traj, double radius) {
    SortedSet<Vector>   vertices  = new SortedSet<Vector>();
    List<ConvexPolytop> polytopes = new List<ConvexPolytop>();

    int k = traj.Count - 1;
    for (int i = 0; i < k; i++) {
      ConvexPolytop cylinder = Cylinder(traj[i], traj[i + 1], radius);
      vertices.UnionWith(cylinder.Vrep);
      polytopes.Add(cylinder);
    }

    return new SeveralPolytopes(vertices.ToList(), polytopes);
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
  public static ConvexPolytop Cylinder(Vector p1, Vector p2, double radius, int segments = 10) {
    Vector axe = p1 - p2; // ось цилиндра
    if (axe.Length < 1e-10) {
      throw new ArgumentException("Points p1 and p2 are too close or coincide.");
    }

    LinearBasis? basePlane = new LinearBasis(axe).FindOrthogonalComplement();
    Vector       u1        = basePlane![0];
    Vector       u2        = basePlane[1];

    List<Vector> vs = new List<Vector>();
    for (int i = 0; i < segments; i++) {
      double angle         = 2 * Math.PI * i / segments;
      Vector pointOnCircle = Math.Cos(angle) * u1 + Math.Sin(angle) * u2;
      Vector onCircle      = radius * pointOnCircle;
      vs.Add(p1 + onCircle);
      vs.Add(p2 + onCircle);
    }

    return ConvexPolytop.CreateFromPoints(vs);
  }


  private ColorAndRadius ReadColorAndRadius(ParamReader pr) {
    int[]  ar     = pr.Read1DArray<int>("Color", 3);
    double radius = pr.ReadNumber<double>("Radius");

    return new ColorAndRadius(new Color(ar[0], ar[1], ar[2]), radius);
  }

  public void WritePly() {
    // используя BrNames Trajs AimsFp AimsSp сначала собрать у каждого точки и грани, потом всё налепить в один файл!!!
  }


  public static void WritePLY(ConvexPolytop P, ParamWriter prW) {
    P = Validate(P);

    List<Vector> VList = P.Vrep.ToList();
    List<Facet>  FList = new List<Facet>();
    AddToFacetList(FList, P);

    WritePLY_File(prW, VList, FList);
  }

  public static void WritePLY(ConvexPolytop P, Vector x, ParamWriter prW) {
    P = Validate(P);

    List<Vector> VList = P.Vrep.ToList();
    List<Facet>  FList = new List<Facet>();
    AddToFacetList(FList, P);

    ConvexPolytop cube = ConvexPolytop.RectAxisParallel(-0.001 * Vector.Ones(3), 0.001 * Vector.Ones(3)).Shift(x);
    VList.AddRange(cube.Vrep);
    AddToFacetList(FList, cube);

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
      prW.Write($"{F.Vertices.Count()} ");
      foreach (Vector vertex in F.Vertices) {
        prW.Write($"{VList.IndexOf(vertex)} ");
      }
      prW.WriteLine();
    }
  }

  public static void AddToFacetList(List<Facet> FList, ConvexPolytop polytop) {
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
      double[] crossProduct = new double[3];
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


  private static ConvexPolytop Validate(ConvexPolytop P) {
    switch (P.SpaceDim) {
      case 1: throw new NotImplementedException("If P.Dim == 1. Непонятно, что делать.");
      case 2: P = P.LiftUp(3, 0); break;
      case 3: break;
      default:
        throw new ArgumentException($"The dimension of the space must be equal less or equal 3! Found spaceDim = {P.SpaceDim}.");
    }

    return P;
  }

}

public class Program {

  public static void Main() {
    string ldgPath = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    Vector p1      = Vector.GenVectorInt(3, -5, 5);
    Vector p2      = Vector.GenVectorInt(3, -5, 5);
    double radius  = 1;

    ConvexPolytop     cylinder = Visualization.Cylinder(p1, p2, radius, 2000);
    using ParamWriter pw       = new ParamWriter(ldgPath + "cylinder.ply");
    Visualization.WritePLY(cylinder, pw);
  }

}
