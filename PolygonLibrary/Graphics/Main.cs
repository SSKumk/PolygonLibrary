using Graphics.Draw;
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

  public struct ColorAndRadius {

    public Color color;
    public readonly double radius;

    public ColorAndRadius(Color color, double radius) {
      this.color  = color;
      this.radius = radius;
    }

  }

  public struct AimTraj {

    public List<Vector> points;
    public ColorAndRadius aim; // цвет и размер aim-точки

    public ColorAndRadius cyl; // цвет и размер цилиндра, соединяющего aim-точку и текущее положение системы

    public AimTraj(List<Vector> points, ColorAndRadius aim, ColorAndRadius cyl) {
      this.points = points;
      this.aim    = aim;
      this.cyl    = cyl;
    }

  }

  public struct Traj {

    public readonly List<Vector> Ps;
    public ColorAndRadius trajPointSettings;

    public ColorAndRadius? traversed = null;
    public ColorAndRadius? remaining = null;

    public Traj(List<Vector> ps, ColorAndRadius trajPointSettings, ColorAndRadius? traversed, ColorAndRadius? remaining) {
      Ps                     = ps;
      this.trajPointSettings = trajPointSettings;
      this.traversed         = traversed;
      this.remaining         = remaining;
    }

  }

  public struct TrajExtended { // траектория, точки прицеливания игроков и все настройки

    public double t0;
    public double T;

    public Traj traj;

    public AimTraj? fp;
    public AimTraj? sp;

    public TrajExtended(double t0, double t, Traj traj, AimTraj? fp, AimTraj? sp) {
      this.t0   = t0;
      T         = t;
      this.traj = traj;
      this.fp   = fp;
      this.sp   = sp;
    }

  }


  public class Facet {

    public readonly IEnumerable<Vector> Vertices;
    public readonly Vector Normal;

    public Color color;

    // public Facet(IEnumerable<Vector> vertices, Vector normal) : this(vertices, normal, Color.Default) { }

    public Facet(IEnumerable<Vector> vertices, Vector normal, Color? color) {
      Vertices   = vertices;
      Normal     = normal;
      this.color = color ?? Color.Default;
    }

  }

  private readonly TrajectoryMain<double, DConvertor> tr;

  public readonly string VisPath;
  public readonly string VisData;
  public readonly string VisConf;

  public readonly string OutFolderName;
  public readonly string GameDirName;

  public readonly string NumericalType;
  public readonly string Precision;

  public readonly List<int> BrNames; // имена папок мостов, которые нужно рисовать
  public readonly List<TrajExtended> Trajs = new List<TrajExtended>();

  public Visualization(string pathLdg, string problemFileName, string visData, string visConf) {
    tr = new TrajectoryMain<double, DConvertor>(pathLdg, problemFileName);

    VisPath = Path.Combine(pathLdg, "Visualization");
    VisData = visData;
    VisConf = visConf;

    string confPath = Path.Combine(VisPath, "!Configs");
    {
      ParamReader prD = new ParamReader(Path.Combine(confPath, VisData + ".visdata"));

      OutFolderName = prD.ReadString("Name");
      GameDirName   = prD.ReadString("GameDirName");
      NumericalType = prD.ReadString("NumericalType");
      Precision     = prD.ReadString("Precision");
    }

    ParamReader pr = new ParamReader(Path.Combine(confPath, VisConf + ".visconfig"));
    if (pr.ReadBool("DrawBridges")) {
      BrNames = pr.ReadList<int>("Bridges");
    }

    SortedSet<string> names     = new SortedSet<string>();
    int               trajCount = pr.ReadNumber<int>("TrajectoryCount");
    for (int i = 0; i < trajCount; i++) {
      string name = pr.ReadString("Name");
      if (!names.Add(name)) {
        throw new ArgumentException($"The trajectory with name {name} is already processed!");
      }

      string       trajPath = Path.Combine(tr.ph.PathTrajectories, name);
      List<Vector> Ps       = new ParamReader(Path.Combine(trajPath, "game.traj")).ReadVectors("Trajectory");

      ColorAndRadius trajPoint = ReadColorAndRadius(pr);

      ColorAndRadius? traversed = null;
      if (pr.ReadBool("DrawTraversed")) {
        traversed = ReadColorAndRadius(pr);
      }
      ColorAndRadius? remaining = null;
      if (pr.ReadBool("DrawRemaining")) {
        remaining = ReadColorAndRadius(pr);
      }

      ColorAndRadius fpAimPoint;
      ColorAndRadius fpAimCyl;
      List<Vector>?  fpAim;
      AimTraj?       fpAimTraj = null;
      if (pr.ReadBool("DrawAimFP")) {
        fpAimPoint = ReadColorAndRadius(pr);
        fpAimCyl   = ReadColorAndRadius(pr);
        fpAim      = new ParamReader(Path.Combine(trajPath, "fp.aim")).ReadVectors("Aim");
        fpAimTraj  = new AimTraj(fpAim, fpAimPoint, fpAimCyl);
      }
      ColorAndRadius spAimPoint;
      ColorAndRadius spAimCyl;
      List<Vector>?  spAim;
      AimTraj?       spAimTraj = null;
      if (pr.ReadBool("DrawAimSP")) {
        spAimPoint = ReadColorAndRadius(pr);
        spAimCyl   = ReadColorAndRadius(pr);
        spAim      = new ParamReader(Path.Combine(trajPath, "sp.aim")).ReadVectors("Aim");
        spAimTraj  = new AimTraj(spAim, spAimPoint, spAimCyl);
      }

      ParamReader prT = new ParamReader(Path.Combine(trajPath, ".times"));
      double      t0  = prT.ReadNumber<double>("MinTime");
      double      T   = prT.ReadNumber<double>("MaxTime");

      Trajs.Add(new TrajExtended(t0, T, new Traj(Ps, trajPoint, traversed, remaining), fpAimTraj, spAimTraj));
    }
  }

  public void MainDrawFunc() {
    PlyDrawer plyDrawer     = new PlyDrawer();
    string    pathOutFolder = Path.Combine(VisPath, OutFolderName);
    // if (Directory.Exists(pathOutFolder)) {
    //   throw new ArgumentException($"The folder with name '{OutFolderName}' already exits in {VisPath} path!");
    // }
    Directory.CreateDirectory(pathOutFolder);

    int i = 0;
    for (double t = tr.tMin; Tools.LT(t, tr.gd.T); t += tr.gd.dt, i++) {
      // подготавливаем один кадр (frame)

      SortedSet<Vector> vertices = new SortedSet<Vector>(); // все вершины на данном кадре
      List<Facet>       facets   = new List<Facet>();       // все грани на данном кадре

      // рисовать мосты, если есть
      AddBridgesToFrame(t, vertices, facets);

      // рисовать траектории
      foreach (TrajExtended trajAndAim in Trajs) {
        // добавить точку траектории текущего кадра
        AddTrajPointToFrame(trajAndAim.traj, i, vertices, facets);
        
        // часть пройденной траектории
        AddTraversedPathToFrame(trajAndAim.traj, i, vertices, facets);

        // часть оставшейся траектории
        AddRemainingPathToFrame(trajAndAim.traj, i, vertices, facets); 

        if (trajAndAim.fp is not null) { // точка прицеливания первого игрока
          AimTraj       aimTrajFP  = trajAndAim.fp.Value;
          ConvexPolytop pointFP    = VisTools.Sphere(aimTrajFP.aim.radius).Shift(aimTrajFP.points[i]);
          ConvexPolytop cylinderFP = VisTools.Cylinder(trajAndAim.traj.Ps[i], aimTrajFP.points[i], aimTrajFP.cyl.radius);
          vertices.UnionWith(pointFP.Vrep);
          vertices.UnionWith(cylinderFP.Vrep);

          VisTools.AddToFacetList(facets, pointFP, aimTrajFP.aim.color);
          VisTools.AddToFacetList(facets, cylinderFP, aimTrajFP.cyl.color);
        }

        if (trajAndAim.sp is not null) { // точка прицеливания второго игрока
          AimTraj       aimTrajSP  = trajAndAim.sp.Value;
          ConvexPolytop pointSP    = VisTools.Sphere(aimTrajSP.aim.radius).Shift(aimTrajSP.points[i]);
          ConvexPolytop cylinderSP = VisTools.Cylinder(trajAndAim.traj.Ps[i], aimTrajSP.points[i], aimTrajSP.cyl.radius);
          vertices.UnionWith(pointSP.Vrep);
          vertices.UnionWith(cylinderSP.Vrep);

          VisTools.AddToFacetList(facets, pointSP, aimTrajSP.aim.color);
          VisTools.AddToFacetList(facets, cylinderSP, aimTrajSP.cyl.color);
        }
      }


      plyDrawer.SaveFrame(Path.Combine(pathOutFolder, $"{Tools<double, DConvertor>.ToPrintTNum(t)}"), facets, vertices);
    }
  }

  private ColorAndRadius ReadColorAndRadius(ParamReader pr) {
    int[]  ar     = pr.Read1DArray<int>("Color", 3);
    double radius = pr.ReadNumber<double>("Radius");

    return new ColorAndRadius(new Color(ar[0], ar[1], ar[2]), radius);
  }

  private void AddBridgesToFrame(double t, SortedSet<Vector> vertices, List<Facet> facets) {
    foreach (int brName in BrNames) {
      if (brName >= tr.Ws.Count) {
        throw new ArgumentException($"There are only {tr.Ws.Count} bridges! Found given bridge name {brName}.");
      }
      ConvexPolytop section = tr.Ws[brName][t];
      vertices.UnionWith(section.Vrep);
      VisTools.AddToFacetList(facets, section, null);
    }
  }

  private void AddTrajPointToFrame(Traj traj, int i, SortedSet<Vector> vertices, List<Facet> facets) {
    ConvexPolytop point = VisTools.Sphere(traj.trajPointSettings.radius).Shift(traj.Ps[i]);
    VisTools.AddToFacetList(facets, point, traj.trajPointSettings.color);
    vertices.UnionWith(point.Vrep);
  }

  private void AddTraversedPathToFrame(Traj traj, int i, SortedSet<Vector> vertices, List<Facet> facets) {
    if (traj.traversed is null)
      return;

    VisTools.SeveralPolytopes traversedCylinders =
      VisTools.MakeCylinderOnTraj(traj.Ps, 0, i, traj.traversed.Value.radius);
    vertices.UnionWith(traversedCylinders.vertices);
    foreach (ConvexPolytop cylinder in traversedCylinders.polytopes) {
      VisTools.AddToFacetList(facets, cylinder, traj.traversed.Value.color);
    }
  }

  private void AddRemainingPathToFrame(Traj traj, int i, SortedSet<Vector> vertices, List<Facet> facets) {
    if (traj.remaining is null)
      return;

    VisTools.SeveralPolytopes remainingCylinders =
      VisTools.MakeCylinderOnTraj(traj.Ps, i, traj.Ps.Count - 1, traj.remaining.Value.radius);
    vertices.UnionWith(remainingCylinders.vertices);
    foreach (ConvexPolytop cylinder in remainingCylinders.polytopes) {
      VisTools.AddToFacetList(facets, cylinder, traj.remaining.Value.color);
    }
  }
  
  // private void AddAimPoints ...
  // private void AddAimCylinders ...










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
    // string pathLdg = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    string pathLdg = "E:\\Work\\LDG\\";

    // Visualization vis = new Visualization(pathLdg, "SimpleMotion.Test1", "SimpleMotion.Test1", "SimpleMotion.Test1");
    // vis.MainDrawFunc();

    SortedSet<int> a = new SortedSet<int>()
      {
        1, 2, 3
      };

    Console.WriteLine(string.Join(' ', a));
    A(a);
    Console.WriteLine(string.Join(' ', a));
  }

  private static void A(SortedSet<int> a) {
    a.UnionWith(new SortedSet<int>()
      {
        4, 5, 6
      });
  }

}
