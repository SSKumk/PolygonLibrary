using DoubleDouble;
using Graphics.Draw;
using LDG;
using static CGLibrary.Geometry<double, Graphics.DConvertor>;

namespace Graphics;

public class Visualization {

  public struct TrajExtended { // траектория, точки прицеливания игроков
    public readonly List<Vector> traj;

    public readonly List<Vector> fp;
    public readonly List<Vector> sp;

    public TrajExtended(List<Vector> traj, List<Vector> fp, List<Vector> sp) {
      this.traj = traj;
      this.fp   = fp;
      this.sp   = sp;
    }

  }


  private readonly LDGPathHolder<double, DConvertor> ph;

  public readonly string VisPath;
  public readonly string VisConf;

  public readonly string OutFolderName;
  public readonly string GameDirName;

  public readonly double tMax;

  public readonly List<int> BrNames = new List<int>(); // имена папок мостов, которые нужно рисовать

  public readonly List<SortedDictionary<double, ConvexPolytop>> Ws; // Мосты

  public readonly List<TrajExtended> Trajs = new List<TrajExtended>();

  public Visualization(string pathLdg, string visConf, string numType, double precision) {
    VisPath = Path.Combine(pathLdg, "Visualization");
    VisConf = visConf;

    string      confPath = Path.Combine(VisPath, "!Configs");
    ParamReader pr       = new ParamReader(Path.Combine(confPath, VisConf + ".visconfig"));
    OutFolderName = pr.ReadString("Name");
    GameDirName   = pr.ReadString("GameDirName");

    ph = new LDGPathHolder<double, DConvertor>(pathLdg, GameDirName, numType, precision);

    Ws = ph.LoadBridges();


    tMax = double.NegativeInfinity;
    foreach (var W in Ws) { // узнали время, начиная с которого есть все мосты
      double x = W.Keys.First();
      if (x > tMax) {
        tMax = x;
      }
    }

    SortedSet<string> names     = new SortedSet<string>();
    int               trajCount = pr.ReadNumber<int>("TrajectoryCount");
    for (int i = 0; i < trajCount; i++) {
      string name = pr.ReadString("Name");
      if (!names.Add(name)) {
        throw new ArgumentException($"GraphicsMain.Ctor: The trajectory with name {name} is already processed!");
      }

      string       trajPath = Path.Combine(ph.PathTrajectories, name);
      List<Vector> Ps       = new ParamReader(Path.Combine(trajPath, "game.traj")).ReadVectors("Trajectory");

      List<Vector> fpAim = new ParamReader(Path.Combine(trajPath, "fp.aim")).ReadVectors("Aim");
      List<Vector> spAim = new ParamReader(Path.Combine(trajPath, "sp.aim")).ReadVectors("Aim");

      Trajs.Add(new TrajExtended(Ps, fpAim, spAim));
    }
  }

  public void ForBlender() {
    PlyDrawer plyDrawer     = new PlyDrawer();
    string    pathOutFolder = Path.Combine(VisPath, OutFolderName);
    Directory.CreateDirectory(pathOutFolder);
    // Тут будут мосты
    for (int k = 0; k < Ws.Count; k++) {
      var Ws_cutted = Ws[k].Where(pair => Tools.GE(pair.Key, tMax));
      int j         = 0;
      foreach (var bridge in Ws_cutted) {
        DrawFrame
          (
           pathOutFolder
         , $"{j}-{k}"
         , plyDrawer
         , (vertices, facets) => { AddBridgeSectionToFrame(bridge.Value, vertices, facets); }
          );
        j += 1;
      }
    }

    // траектории
    for (int i = 0; i < Trajs.Count; i++) {
      using ParamWriter pwTr = new ParamWriter(Path.Combine(pathOutFolder, $"traj-{i}.csv"));
      foreach (Vector v in Trajs[i].traj) {
        pwTr.WriteLine(v.ToStringBraceAndDelim(null, null, ','));
      }

      using ParamWriter pwAF = new ParamWriter(Path.Combine(pathOutFolder, $"aimFp-{i}.csv"));
      foreach (Vector v in Trajs[i].fp) {
        pwAF.WriteLine(v.ToStringBraceAndDelim(null, null, ','));
      }

      using ParamWriter pwAS = new ParamWriter(Path.Combine(pathOutFolder, $"aimSp-{i}.csv"));
      foreach (Vector v in Trajs[i].sp) {
        pwAS.WriteLine(v.ToStringBraceAndDelim(null, null, ','));
      }
    }
  }

  public static void DrawFrame(string basePath, string fileName, IDrawer drawer, ConvexPolytop polytop)
    => DrawFrame
      (
       basePath
     , fileName
     , drawer
     , (vs, fs)
         => {
         vs.UnionWith(polytop.Vrep);
         VisTools.AddToFacetList(fs, polytop);
       }
      );

  private static void DrawFrame(
      string                                          basePath
    , string                                          frameName
    , IDrawer                                         drawer
    , Action<SortedSet<Vector>, List<VisTools.Facet>> addToFrame
    ) {
    SortedSet<Vector>    vertices = new SortedSet<Vector>();
    List<VisTools.Facet> facets   = new List<VisTools.Facet>();

    addToFrame(vertices, facets);

    drawer.SaveFrame(Path.Combine(basePath, frameName), vertices, facets);
  }

  private void AddBridgeSectionToFrame(ConvexPolytop section, SortedSet<Vector> vertices, List<VisTools.Facet> facets) {
    vertices.UnionWith(section.Vrep);
    VisTools.AddToFacetList(facets, section);
  }

}
