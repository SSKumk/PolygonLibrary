// using DoubleDouble;
// using Graphics.Draw;
// using LDG;
// using static CGLibrary.Geometry<double, Graphics.DConvertor>;
// using static Graphics.VisTools;
//
// namespace Graphics;
//
// public class VisualizationColor {
//
//   public struct ColorAndRadius {
//
//     public          Color color;
//     public readonly double         radius;
//
//     public ColorAndRadius(Color color, double radius) {
//       this.color  = color;
//       this.radius = radius;
//     }
//
//   }
//
//   public struct AimTraj {
//
//     public readonly List<Vector>   points;
//     public          ColorAndRadius aim; // цвет и размер aim-точки
//
//     public ColorAndRadius cyl; // цвет и размер цилиндра, соединяющего aim-точку и текущее положение системы
//
//     public AimTraj(List<Vector> points, ColorAndRadius aim, ColorAndRadius cyl) {
//       this.points = points;
//       this.aim    = aim;
//       this.cyl    = cyl;
//     }
//
//   }
//
//   public struct Traj {
//
//     public readonly List<Vector>   points;
//     public          ColorAndRadius trajPointSettings;
//
//     public ColorAndRadius? traversed = null;
//     public ColorAndRadius? remaining = null;
//
//     public Traj(List<Vector> points, ColorAndRadius trajPointSettings, ColorAndRadius? traversed, ColorAndRadius? remaining) {
//       this.points            = points;
//       this.trajPointSettings = trajPointSettings;
//       this.traversed         = traversed;
//       this.remaining         = remaining;
//     }
//
//   }
//
//   public struct TrajExtended { // траектория, точки прицеливания игроков и все настройки
//
//     // public double t0;
//     // public double T;
//
//     public Traj traj;
//
//     public AimTraj? fp;
//     public AimTraj? sp;
//
//     public TrajExtended(double t0, double t, Traj traj, AimTraj? fp, AimTraj? sp) {
//       // this.t0   = t0;
//       // T         = t;
//       this.traj = traj;
//       this.fp   = fp;
//       this.sp   = sp;
//     }
//
//   }
//
//
//   private readonly LDGPathHolder<double, DConvertor> ph;
//
//   public readonly string VisPath;
//   public readonly string VisConf;
//
//   public readonly string OutFolderName;
//   public readonly string GameDirName;
//
//   public readonly double tMax;
//
//   public readonly List<int> BrNames = new List<int>(); // имена папок мостов, которые нужно рисовать
//
//   public readonly Dictionary<int, SortedDictionary<double, ConvexPolytop>> Ws =
//     new Dictionary<int, SortedDictionary<double, ConvexPolytop>>(); // Словарь номер моста --> мост (чтобы не все грузить, а только нужные мосты)
//
//   public readonly List<TrajExtended> Trajs = new List<TrajExtended>();
//
//   public VisualizationColor(string pathLdg, string visConf, string numType, double precision) {
//     VisPath = Path.Combine(pathLdg, "Visualization");
//     VisConf = visConf;
//
//     string      confPath = Path.Combine(VisPath, "!Configs");
//     ParamReader pr       = new ParamReader(Path.Combine(confPath, VisConf + ".visconfig"));
//     OutFolderName = pr.ReadString("Name");
//     GameDirName   = pr.ReadString("GameDirName");
//
//     ph = new LDGPathHolder<double, DConvertor>(pathLdg, GameDirName, numType, precision);
//
//
//     if (pr.ReadBool("DrawBridges")) { // узнали какие именно мосты надо грузить
//       BrNames = pr.ReadList<int>("Bridges");
//     }
//
//     foreach (int i in BrNames) { // загрузили мосты
//       Ws.Add(i, ph.LoadBridge(i));
//     }
//
//     tMax = double.NegativeInfinity;
//     foreach (var ind_W in Ws) { // узнали время, начиная с которого есть все мосты
//       var x = ind_W.Value.Keys.First();
//       if (x > tMax) {
//         tMax = x;
//       }
//     }
//
//     SortedSet<string> names     = new SortedSet<string>();
//     int               trajCount = pr.ReadNumber<int>("TrajectoryCount");
//     for (int i = 0; i < trajCount; i++) {
//       string name = pr.ReadString("Name");
//       if (!names.Add(name)) {
//         throw new ArgumentException($"The trajectory with name {name} is already processed!");
//       }
//
//       string       trajPath = Path.Combine(ph.PathTrajectories, name);
//       List<Vector> Ps       = new ParamReader(Path.Combine(trajPath, "game.traj")).ReadVectors("Trajectory");
//
//       ColorAndRadius trajPoint = ReadColorAndRadius(pr);
//
//       ColorAndRadius? traversed = null;
//       if (pr.ReadBool("DrawTraversed")) {
//         traversed = ReadColorAndRadius(pr);
//       }
//       ColorAndRadius? remaining = null;
//       if (pr.ReadBool("DrawRemaining")) {
//         remaining = ReadColorAndRadius(pr);
//       }
//
//       ColorAndRadius fpAimPoint;
//       ColorAndRadius fpAimCyl;
//       List<Vector>?  fpAim;
//       AimTraj?       fpAimTraj = null;
//       if (pr.ReadBool("DrawAimFP")) {
//         fpAimPoint = ReadColorAndRadius(pr);
//         fpAimCyl   = ReadColorAndRadius(pr);
//         fpAim      = new ParamReader(Path.Combine(trajPath, "fp.aim")).ReadVectors("Aim");
//         fpAimTraj  = new AimTraj(fpAim, fpAimPoint, fpAimCyl);
//       }
//       ColorAndRadius spAimPoint;
//       ColorAndRadius spAimCyl;
//       List<Vector>?  spAim;
//       AimTraj?       spAimTraj = null;
//       if (pr.ReadBool("DrawAimSP")) {
//         spAimPoint = ReadColorAndRadius(pr);
//         spAimCyl   = ReadColorAndRadius(pr);
//         spAim      = new ParamReader(Path.Combine(trajPath, "sp.aim")).ReadVectors("Aim");
//         spAimTraj  = new AimTraj(spAim, spAimPoint, spAimCyl);
//       }
//
//       ParamReader prT = new ParamReader(Path.Combine(trajPath, ".times"));
//       double      t0  = prT.ReadNumber<double>("MinTime");
//       double      T   = prT.ReadNumber<double>("MaxTime");
//
//       Trajs.Add(new TrajExtended(t0, T, new Traj(Ps, trajPoint, traversed, remaining), fpAimTraj, spAimTraj));
//     }
//   }
//
//   //
//   // public void AllDataAtOneFrame() {
//   //   PlyDrawer plyDrawer     = new PlyDrawer();
//   //   string    pathOutFolder = Path.Combine(VisPath, OutFolderName);
//   //   // if (Directory.Exists(pathOutFolder)) {
//   //   //   throw new ArgumentException($"The folder with name '{OutFolderName}' already exits in {VisPath} path!");
//   //   // }
//   //   Directory.CreateDirectory(pathOutFolder);
//   //
//   //   int i = 0;
//   //   for (double t = tMax; Tools.LT(t, tr.gd.T); t += tr.gd.dt, i++) {
//   //     // подготавливаем один кадр (frame)
//   //
//   //     SortedSet<Vector> vertices = new SortedSet<Vector>(); // все вершины на данном кадре
//   //     List<FacetColor>       facets   = new List<FacetColor>();       // все грани на данном кадре
//   //
//   //     // рисовать мосты, если есть
//   //     AddBridgesToFrame(t, vertices, facets);
//   //
//   //     // рисовать траектории
//   //     foreach (TrajExtended trajAndAim in Trajs) {
//   //       // добавить точку траектории текущего кадра
//   //       AddTrajPointToFrame(trajAndAim.traj, i, vertices, facets);
//   //
//   //       // часть пройденной траектории
//   //       AddTraversedPathToFrame(trajAndAim.traj, i, vertices, facets);
//   //
//   //       // часть оставшейся траектории
//   //       AddRemainingPathToFrame(trajAndAim.traj, i, vertices, facets);
//   //
//   //       // точка прицеливания первого игрока
//   //       AddAimPoints(trajAndAim.fp, i, vertices, facets);
//   //       // соединяем точку прицеливания первого игрока и точку траектории
//   //       AddAimCylinders(trajAndAim.fp, trajAndAim.traj, i, vertices, facets);
//   //
//   //       // точка прицеливания первого игрока
//   //       AddAimPoints(trajAndAim.sp, i, vertices, facets);
//   //       // соединяем точку прицеливания первого игрока и точку траектории
//   //       AddAimCylinders(trajAndAim.sp, trajAndAim.traj, i, vertices, facets);
//   //     }
//   //
//   //
//   //     plyDrawer.SaveFrame(Path.Combine(pathOutFolder, $"{Tools<double, DConvertor>.ToPrintTNum(t)}"), vertices, facets);
//   //   }
//   // }
//   //
//   // public void DrawSeparate() {
//   //   PlyDrawer plyDrawer     = new PlyDrawer();
//   //   string    pathOutFolder = Path.Combine(VisPath, OutFolderName);
//   //   Directory.CreateDirectory(pathOutFolder);
//   //   int i = 0;
//   //   for (double t = tMax; Tools.LT(t, tr.gd.T); t += tr.gd.dt, i++) {
//   //     // под очередной момент заводим папку
//   //     string pathMoment = Path.Combine(pathOutFolder, Tools<double, DConvertor>.ToPrintTNum(t));
//   //     Directory.CreateDirectory(pathMoment);
//   //
//   //     // Рисуем каждое сечение моста отдельно
//   //     foreach (int brName in BrNames) {
//   //       double t1 = t;
//   //       DrawFrame
//   //         (pathMoment, $"{brName}", plyDrawer, (vertices, facets) => { AddBridgeSectionToFrame(brName, t1, vertices, facets); });
//   //     }
//   //
//   //     int tr = -1;
//   //     // Рисуем траектории
//   //     foreach (TrajExtended trajAndAim in Trajs) {
//   //       tr += 1;
//   //       int    i1       = i;
//   //       string pathTraj = Path.Combine(pathMoment, $"{tr}");
//   //       Directory.CreateDirectory(pathTraj);
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "trajPoint"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddTrajPointToFrame(trajAndAim.traj, i1, vertices, facets);
//   //          }
//   //         );
//   //
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "traversed"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddTraversedPathToFrame(trajAndAim.traj, i1, vertices, facets);
//   //          }
//   //         );
//   //
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "remaining"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddRemainingPathToFrame(trajAndAim.traj, i1, vertices, facets);
//   //          }
//   //         );
//   //
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "aimFp"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddAimPoints(trajAndAim.fp, i1, vertices, facets);
//   //          }
//   //         );
//   //
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "cylFp"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddAimCylinders(trajAndAim.fp, trajAndAim.traj, i1, vertices, facets);
//   //          }
//   //         );
//   //
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "aimSp"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddAimPoints(trajAndAim.sp, i1, vertices, facets);
//   //          }
//   //         );
//   //
//   //       DrawFrame
//   //         (
//   //          pathTraj
//   //        , "cylSp"
//   //        , plyDrawer
//   //        , (vertices, facets)
//   //            => {
//   //            AddAimCylinders(trajAndAim.sp, trajAndAim.traj, i1, vertices, facets);
//   //          }
//   //         );
//   //     }
//   //   }
//   // }
//
//   // public void DrawSeparateInOneDir(double dt) {
//   //   PlyDrawer plyDrawer     = new PlyDrawer();
//   //   string    pathOutFolder = Path.Combine(VisPath, OutFolderName);
//   //   Directory.CreateDirectory(pathOutFolder);
//   //
//   //   for (double t = tMax; Tools.LT(t, tr.gd.T); t += dt) { // tr.gd.dt
//   //     // Рисуем каждое сечение моста отдельно
//   //     foreach (int brName in BrNames) {
//   //       double t1 = t;
//   //       DrawFrame
//   //         (
//   //          pathOutFolder
//   //        , $"{brName}-{Tools<double, DConvertor>.ToPrintTNum(t)}"
//   //        , plyDrawer
//   //        , (vertices, facets) => { AddBridgeSectionToFrame(brName, t1, vertices, facets); }
//   //         );
//   //     }
//   //   }
//   // }
//
//   public static void DrawFrame(string basePath, string fileName, IDrawer drawer, ConvexPolytop polytop, Color? color = null)
//     => DrawFrame
//       (
//        basePath
//      , fileName
//      , drawer
//      , (vs, fs)
//          => {
//          vs.UnionWith(polytop.Vrep);
//          VisTools.AddToFacetList(fs, polytop, color);
//        }
//       );
//
//   public static void DrawFrame(
//       string                                 basePath
//     , string                                 frameName
//     , IDrawer                                drawer
//     , Action<SortedSet<Vector>, List<FacetColor>> addToFrame
//     ) {
//     SortedSet<Vector> vertices = new SortedSet<Vector>();
//     List<FacetColor>       facets   = new List<FacetColor>();
//
//     addToFrame(vertices, facets);
//
//     drawer.SaveFrame(Path.Combine(basePath, frameName), vertices, facets);
//   }
//
//   private void AddBridgeSectionToFrame(ConvexPolytop section, SortedSet<Vector> vertices, List<FacetColor> facets) {
//     vertices.UnionWith(section.Vrep);
//     VisTools.AddToFacetList(facets, section, null);
//   }
//
//   // private void AddBridgesToFrame(double t, SortedSet<Vector> vertices, List<FacetColor> facets) {
//   //   foreach (int brName in BrNames) {
//   //     AddBridgeSectionToFrame(brName, t, vertices, facets);
//   //   }
//   // }
//
//   private static void AddTrajPointToFrame(Traj traj, int i, SortedSet<Vector> vertices, List<FacetColor> facets) {
//     ConvexPolytop point = VisTools.Sphere(traj.trajPointSettings.radius).Shift(traj.points[i]);
//     VisTools.AddToFacetList(facets, point, traj.trajPointSettings.color);
//     vertices.UnionWith(point.Vrep);
//   }
//
//   private static void AddTraversedPathToFrame(Traj traj, int i, SortedSet<Vector> vertices, List<FacetColor> facets) {
//     if (traj.traversed is null) {
//       return;
//     }
//
//     VisTools.SeveralPolytopes traversedCylinders = VisTools.MakeCylinderOnTraj(traj.points, 0, i, traj.traversed.Value.radius);
//     vertices.UnionWith(traversedCylinders.vertices);
//     foreach (ConvexPolytop cylinder in traversedCylinders.polytopes) {
//       VisTools.AddToFacetList(facets, cylinder, traj.traversed.Value.color);
//     }
//   }
//
//   private static void AddRemainingPathToFrame(Traj traj, int i, SortedSet<Vector> vertices, List<FacetColor> facets) {
//     if (traj.remaining is null) {
//       return;
//     }
//     VisTools.SeveralPolytopes remainingCylinders =
//       VisTools.MakeCylinderOnTraj(traj.points, i, traj.points.Count - 1, traj.remaining.Value.radius);
//     vertices.UnionWith(remainingCylinders.vertices);
//     foreach (ConvexPolytop cylinder in remainingCylinders.polytopes) {
//       VisTools.AddToFacetList(facets, cylinder, traj.remaining.Value.color);
//     }
//   }
//
//   private static void AddAimPoints(AimTraj? aimTraj, int i, SortedSet<Vector> vertices, List<FacetColor> facets) {
//     if (aimTraj is null) {
//       return;
//     }
//
//     AimTraj       aimTrajValue = aimTraj.Value;
//     ConvexPolytop point        = VisTools.Sphere(aimTrajValue.aim.radius).Shift(aimTrajValue.points[i]);
//     vertices.UnionWith(point.Vrep);
//
//     VisTools.AddToFacetList(facets, point, aimTrajValue.aim.color);
//   }
//
//   private static void AddAimCylinders(AimTraj? aimTraj, Traj traj, int i, SortedSet<Vector> vertices, List<FacetColor> facets) {
//     if (aimTraj is null) {
//       return;
//     }
//
//     AimTraj       aimTrajValue = aimTraj.Value;
//     ConvexPolytop cylinder     = VisTools.Cylinder(traj.points[i], aimTrajValue.points[i], aimTrajValue.cyl.radius);
//     vertices.UnionWith(cylinder.Vrep);
//     VisTools.AddToFacetList(facets, cylinder, aimTrajValue.cyl.color);
//   }
//
//   private static ColorAndRadius ReadColorAndRadius(ParamReader pr) {
//     int[]  ar     = pr.Read1DArray<int>("Color", 3);
//     double radius = pr.ReadNumber<double>("Radius");
//
//     return new ColorAndRadius(new Color(ar[0], ar[1], ar[2]), radius);
//   }
//
//   private static ConvexPolytop Validate(ConvexPolytop P) {
//     switch (P.SpaceDim) {
//       case 1: throw new NotImplementedException("If P.Dim == 1. Непонятно, что делать.");
//       case 2: P = P.LiftUp(3, 0); break;
//       case 3: break;
//       default:
//         throw new ArgumentException($"The dimension of the space must be equal less or equal 3! Found spaceDim = {P.SpaceDim}.");
//     }
//
//     return P;
//   }
//
//
// }
