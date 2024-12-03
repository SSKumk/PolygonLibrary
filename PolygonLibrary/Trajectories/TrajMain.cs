using System.Globalization;
using System.Numerics;
using CGLibrary;
using CGLibrary.Toolkit;

namespace Trajectories;

public class TrajMain<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string BridgeDir;
  public readonly string TrajConfigDir;
  public readonly string OutputDir;

  public readonly string GameConfigPath;

  public Geometry<TNum, TConv>.GameData gd;

  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> D =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);

  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> E =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);

  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);

  public readonly TNum tMin;

  public TrajMain(string gameConfigPath, string bridgeDir, string trajConfigDir, string outputDir) {
    BridgeDir      = bridgeDir;
    TrajConfigDir  = trajConfigDir;
    GameConfigPath = gameConfigPath;
    OutputDir      = outputDir;

    gd = new Geometry<TNum, TConv>.GameData(new Geometry<TNum, TConv>.ParamReader(GameConfigPath));


    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(bridgeDir + "tMin.txt");
    tMin = pr.ReadNumber<TNum>("tMin");


    if (Geometry<TNum, TConv>.Tools.GE(tMin, gd.T)) {
      throw new ArgumentException($"The t0 should be less then T. Found t0 = {gd.t0} < T = {gd.T}");
    }

    TNum t = tMin;
    do {
      if (!W.ContainsKey(t)) {
        Geometry<TNum, TConv>.ParamReader prR =
          new Geometry<TNum, TConv>.ParamReader(BridgeDir + $"{Geometry<TNum, TConv>.SolverLDG.ToPrintTNum(t)})W.wsection");
        string taskDynamicHash = prR.ReadString("md5-dynamic");
        string _               = prR.ReadString("md5");

        if (taskDynamicHash != gd.DynamicsHash) {
          throw new ArgumentException
            ("TrajMain.ReadBridge: The hash-dynamic in the file does not match the expected game data hash.");
        }

        W.Add(t, Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(prR));
      }
      t += gd.dt;
    } while (Geometry<TNum, TConv>.Tools.LE(t, gd.T));


    t = gd.t0;
    do {
      D[t] = gd.Xstar(t) * gd.B;
      E[t] = gd.Xstar(t) * gd.C;

      t += gd.dt;
    } while (Geometry<TNum, TConv>.Tools.LE(t, gd.T));
  }


  public void CalcTraj(string trajConfPath) {
    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(trajConfPath + ".controlconfig");

    int trajCount = pr.ReadNumber<int>("Count");
    for (int i = 0; i < trajCount; i++) {
      string num = $"{i + 1:00}";


      string                       name = pr.ReadString("Name");
      TNum                         t0   = pr.ReadNumber<TNum>("t0");
      TNum                         T    = pr.ReadNumber<TNum>("T");
      Geometry<TNum, TConv>.Vector x0   = pr.ReadVector("x0");

      if (x0.SpaceDim != gd.n) {
        throw new ArgumentException("The dimension of the x0-vector should be equal to n!");
      }

      PlayerControl<TNum, TConv> game = new PlayerControl<TNum, TConv>(E, D, W, gd);

      FirstPlayerControl<TNum, TConv>  fpControl = new FirstPlayerControl<TNum, TConv>(pr, game);
      SecondPlayerControl<TNum, TConv> spControl = new SecondPlayerControl<TNum, TConv>(pr, game);


      List<Geometry<TNum, TConv>.Vector> trajectory = new List<Geometry<TNum, TConv>.Vector>() { x0 };


      if (Geometry<TNum, TConv>.Tools.LT(t0, tMin)) {
        throw new ArgumentException($"t0 < tMin! t0 = {t0}, tMin = {tMin}");
      }

      Geometry<TNum, TConv>.Vector x = x0;
      for (TNum t = t0; Geometry<TNum, TConv>.Tools.LT(t, T); t += gd.dt) {
        Geometry<TNum, TConv>.Vector proj_x = gd.Xstar(t) * x;
        // Выполняем шаг Эйлера
        x += gd.dt * (gd.A * x + gd.B * fpControl.Control(t, proj_x) + gd.C * spControl.Control(t, proj_x));
        trajectory.Add(x);
      }

      using Geometry<TNum, TConv>.ParamWriter pwT = new Geometry<TNum, TConv>.ParamWriter($"{OutputDir}{name}_tr-{num}.traj");
      using Geometry<TNum, TConv>.ParamWriter pwP = new Geometry<TNum, TConv>.ParamWriter($"{OutputDir}{name}_aimP-{num}.aimp");
      using Geometry<TNum, TConv>.ParamWriter pwQ = new Geometry<TNum, TConv>.ParamWriter($"{OutputDir}{name}_aimQ-{num}.aimq");
      // todo: Реализации управлений первого и второго игроков


      string md5Hash = Hashes.GetMD5Hash($"{name}{gd.t0}{gd.T}{x0}{fpControl.controlTypeInfo}{spControl.controlTypeInfo}");

      WriteHashes(pwT, "Trajectory", gd.DynamicsHash, md5Hash, trajectory);
      WriteHashes(pwP, "AimP", gd.DynamicsHash, md5Hash, fpControl.AimPoints);
      WriteHashes(pwQ, "AimQ", gd.DynamicsHash, md5Hash, spControl.AimPoints);
    }
  }

  private void WriteHashes(
      Geometry<TNum, TConv>.ParamWriter  pw
    , string                             trajType
    , string                             dynamicsHash
    , string                             trajConfigHash
    , List<Geometry<TNum, TConv>.Vector> traj
    ) {
    pw.WriteString("md5-dynamic", dynamicsHash);
    pw.WriteString("md5", trajConfigHash);
    pw.WriteVectors(trajType, traj);
  }

}

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string bridgeDir     = "F:\\Works\\IMM\\Аспирантура\\LDG\\Bridges\\Simple Motion_T=10\\First TS_01\\System.Double\\1e-008\\";
    string configDir     = "F:\\Works\\IMM\\Аспирантура\\LDG\\Trajectories\\Configs\\";
    string outputDir     = "F:\\Works\\IMM\\Аспирантура\\LDG\\Trajectories\\";
    string gameConfigDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\Bridges\\Configs\\";

    // string bridgeDir     = "E:\\Work\\LDG\\Bridges\\Simple Motion_T=10\\Explicit_01\\System.Double\\1e-008\\";
    // string configDir     = "E:\\Work\\LDG\\Trajectories\\Configs\\";
    // string outputDir     = "E:\\Work\\LDG\\Trajectories\\";
    // string gameConfigDir = "E:\\Work\\LDG\\Bridges\\Configs\\";


    TrajMain<double, DConvertor> trajCalc =
      new TrajMain<double, DConvertor>(gameConfigDir + "SimpleMotion.gameconfig", bridgeDir, configDir, outputDir);

    trajCalc.CalcTraj(configDir + "SimpleMotion");
  }

}
