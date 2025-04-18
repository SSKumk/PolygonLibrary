using System.Numerics;
using CGLibrary;
using DoubleDouble;
using Graphics.Draw;
using LDG;

namespace Graphics;

public class Visualization<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public struct TrajExtended { // траектория, точки прицеливания игроков
    public readonly List<Geometry<double, DConvertor>.Vector> traj;

    public readonly List<Geometry<double, DConvertor>.Vector> fp;
    public readonly List<Geometry<double, DConvertor>.Vector> sp;

    public TrajExtended(
        List<Geometry<double, DConvertor>.Vector> traj
      , List<Geometry<double, DConvertor>.Vector> fp
      , List<Geometry<double, DConvertor>.Vector> sp
      ) {
      this.traj = traj;
      this.fp   = fp;
      this.sp   = sp;
    }

  }


  private readonly LDGPathHolder<TNum, TConv> ph;

  public readonly string VisPath;
  public readonly string VisConf;

  public readonly string OutFolderName;
  public readonly string GameDirName;

  public readonly List<int> BrNames = new List<int>(); // имена папок мостов, которые нужно рисовать

  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws; // Мосты

  public readonly List<TrajExtended> Trajs = new List<TrajExtended>();

  public Visualization(string pathLdg, string visConf, TNum numAccuracy) {
    VisPath = Path.Combine(pathLdg, "Visualization");
    VisConf = visConf;

    string                            confPath = Path.Combine(VisPath, "!Configs");
    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(Path.Combine(confPath, VisConf + ".visconfig"));
    OutFolderName = pr.ReadString("Name");
    GameDirName   = pr.ReadString("GameDirName");

    ph = new LDGPathHolder<TNum, TConv>(pathLdg, GameDirName, numAccuracy);

    Ws = ph.LoadBridges();

    SortedSet<string> names     = new SortedSet<string>();
    int               trajCount = pr.ReadNumber<int>("TrajectoryCount");
    for (int i = 0; i < trajCount; i++) {
      string name = pr.ReadString("Name");
      if (!names.Add(name)) {
        throw new ArgumentException($"GraphicsMain.Ctor: The trajectory with name {name} is already processed!");
      }

      string trajPath = Path.Combine(ph.PathTrajectories, name);
      List<Geometry<double, DConvertor>.Vector> Ps =
        ToDList(new Geometry<TNum, TConv>.ParamReader(Path.Combine(trajPath, "game.traj")).ReadVectors("Trajectory"));

      List<Geometry<double, DConvertor>.Vector> fpAim =
        ToDList(new Geometry<TNum, TConv>.ParamReader(Path.Combine(trajPath, "fp.aim")).ReadVectors("Aim"));
      List<Geometry<double, DConvertor>.Vector> spAim =
        ToDList(new Geometry<TNum, TConv>.ParamReader(Path.Combine(trajPath, "sp.aim")).ReadVectors("Aim"));

      Trajs.Add(new TrajExtended(Ps, fpAim, spAim));
    }
  }

  public void ForBlender() {
    PlyDrawer plyDrawer     = new PlyDrawer();
    string    pathOutFolder = Path.Combine(VisPath, OutFolderName, ph.NumType, ph.NumAccuracy);
    Directory.CreateDirectory(pathOutFolder);
    // Тут будут мосты
    for (int k = 0; k < Ws.Count; k++) {
      int j = 0;
      foreach (var bridge in Ws[k]) {
        DrawFrame
          (
           pathOutFolder
         , $"{j}-{k}"
         , plyDrawer
         , (vertices, facets)
             => {
             AddBridgeSectionToFrame(bridge.Value, vertices, facets);
           }
          );
        j += 1;
      }
    }

    // траектории
    for (int i = 0; i < Trajs.Count; i++) {
      using Geometry<TNum, TConv>.ParamWriter pwTr =
        new Geometry<TNum, TConv>.ParamWriter(Path.Combine(pathOutFolder, $"traj-{i}.csv"));
      double val = 1;

      foreach (Geometry<double, DConvertor>.Vector v in Trajs[i].traj) {
        pwTr.WriteLine(v.ToStringBraceAndDelim(null, null, ','));
      }

      using Geometry<TNum, TConv>.ParamWriter pwAF =
        new Geometry<TNum, TConv>.ParamWriter(Path.Combine(pathOutFolder, $"aimFp-{i}.csv"));
      foreach (Geometry<double, DConvertor>.Vector v in Trajs[i].fp) {
        pwAF.WriteLine(v.ToStringBraceAndDelim(null, null, ','));
      }

      using Geometry<TNum, TConv>.ParamWriter pwAS =
        new Geometry<TNum, TConv>.ParamWriter(Path.Combine(pathOutFolder, $"aimSp-{i}.csv"));
      foreach (Geometry<double, DConvertor>.Vector v in Trajs[i].sp) {
        pwAS.WriteLine(v.ToStringBraceAndDelim(null, null, ','));
      }
    }
  }

  // public static void DrawFrame(string basePath, string fileName, IDrawer drawer, Geometry<TNum,TConv>.ConvexPolytop polytop)
  //   => DrawFrame
  //     (
  //      basePath
  //    , fileName
  //    , drawer
  //    , (vs, fs)
  //        => {
  //        vs.UnionWith(polytop.Vrep);
  //        VisTools.AddToFacetList(fs, polytop);
  //      }
  //     );

  private static void DrawFrame(
      string                                                                       basePath
    , string                                                                       frameName
    , IDrawer                                                                      drawer
    , Action<SortedSet<Geometry<double, DConvertor>.Vector>, List<VisTools.Facet>> addToFrame
    ) {
    SortedSet<Geometry<double, DConvertor>.Vector> vertices = new SortedSet<Geometry<double, DConvertor>.Vector>();
    List<VisTools.Facet>                           facets   = new List<VisTools.Facet>();

    addToFrame(vertices, facets);

    drawer.SaveFrame(Path.Combine(basePath, frameName), vertices, facets);
  }

  private void AddBridgeSectionToFrame(
      Geometry<TNum, TConv>.ConvexPolytop            section
    , SortedSet<Geometry<double, DConvertor>.Vector> vertices
    , List<VisTools.Facet>                           facets
    ) {
    SortedSet<Geometry<double, DConvertor>.Vector> Vrep = ToDSet(section.Vrep);
    if (section.PolytopDim == 2) {
      Geometry<double, DConvertor>.Vector normal = Geometry<double, DConvertor>.Vector.MakeOrth(3, 3);
      facets.Add
        (
         new VisTools.Facet
           (
            Vrep
             .ToList()
             .OrderByDescending(v => v, new VisTools.VectorMixedProductComparer(normal, ToDVector(section.Vrep.First())))
             .ToArray()
          , normal
           )
        );
      vertices.UnionWith(Vrep);

      return;
    }
    vertices.UnionWith(Vrep);
    AddToFacetList(facets, section);
  }

  public static void AddToFacetList(List<VisTools.Facet> FList, Geometry<TNum, TConv>.ConvexPolytop polytop) {
    if (polytop.PolytopDim == 2) {
      var vertices = ToDList(polytop.Vrep);
      FList.Add
        (new VisTools.Facet(vertices, new Geometry<double, DConvertor>.AffineBasis(vertices).LinBasis.FindOrthonormalVector()));
    }
    else {
      foreach (Geometry<TNum, TConv>.FLNode F in polytop.FLrep.Lattice[2]) {
        Geometry<TNum, TConv>.HyperPlane hp =
          new Geometry<TNum, TConv>.HyperPlane(F.AffBasis, false, (polytop.FLrep.Top.InnerPoint, false));
        FList.Add
          (
           new VisTools.Facet
             (
              ToDList
                  (F.Vertices)
               .OrderByDescending
                  (v => v, new VisTools.VectorMixedProductComparer(ToDVector(hp.Normal), ToDVector(F.Vertices.First())))
               .ToArray()
            , ToDVector(hp.Normal)
             )
          );
      }
    }
  }


  public static Geometry<double, DConvertor>.Vector ToDVector(Geometry<TNum, TConv>.Vector vec) {
    int      spaceDim = vec.SpaceDim;
    double[] v        = new double[spaceDim];
    for (int j = 0; j < spaceDim; j++) {
      v[j] = TConv.ToDouble(vec[j]);
    }

    return new Geometry<double, DConvertor>.Vector(v, false);
  }

  public static List<Geometry<double, DConvertor>.Vector> ToDList(IEnumerable<Geometry<TNum, TConv>.Vector> lst) {
    List<Geometry<double, DConvertor>.Vector> res = new List<Geometry<double, DConvertor>.Vector>();
    foreach (Geometry<TNum, TConv>.Vector vec in lst) {
      res.Add(ToDVector(vec));
    }

    return res;
  }

  public static SortedSet<Geometry<double, DConvertor>.Vector> ToDSet(SortedSet<Geometry<TNum, TConv>.Vector> lst) {
    SortedSet<Geometry<double, DConvertor>.Vector> res = new SortedSet<Geometry<double, DConvertor>.Vector>();
    foreach (Geometry<TNum, TConv>.Vector vec in lst) {
      res.Add(ToDVector(vec));
    }

    return res;
  }

}
