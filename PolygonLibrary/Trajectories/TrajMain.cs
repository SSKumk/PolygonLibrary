using System.Globalization;
using System.Numerics;
using CGLibrary;
using PolygonLibrary.Toolkit;

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

  /// <summary>
  /// Collection of matrices D for the instants from the time grid
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> D =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);

  /// <summary>
  /// Collection of matrices E for the instants from the time grid
  /// </summary>
  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> E =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>(Geometry<TNum, TConv>.Tools.TComp);

  /// <summary>
  /// The bridge of the game
  /// </summary>
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
        string _ = prR.ReadString("md5");

        if (taskDynamicHash != gd.DynamicHash) {
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
    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(trajConfPath+".controlconfig");

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

      //todo: поднять класс сюда. передать туда E,D, gd.
      // в нём читать
      // а потом только для заданного 't' производить вычисления
      ControlType PStrj = ReadControlType(pr, 'P');
      ControlType QStrj = ReadControlType(pr, 'Q');
      if (PStrj == ControlType.Constant) { pr.ReadVector("PConstant"); }// todo: как быть с константным управлением?


      List<Geometry<TNum, TConv>.Vector> trajectory = new List<Geometry<TNum, TConv>.Vector>() { x0 };
      List<Geometry<TNum, TConv>.Vector> aimpointsP = new List<Geometry<TNum, TConv>.Vector>();
      List<Geometry<TNum, TConv>.Vector> aimpointsQ = new List<Geometry<TNum, TConv>.Vector>();


      if (Geometry<TNum, TConv>.Tools.LT(t0, tMin)) {
        throw new ArgumentException($"t0 < tMin! t0 = {t0}, tMin = {tMin}");
      }

      Geometry<TNum, TConv>.Vector x = x0;
      for (TNum t = t0; Geometry<TNum, TConv>.Tools.LT(t, T); t += gd.dt) {
        FirstPlayerControl<TNum, TConv> fpControl =
          new FirstPlayerControl<TNum, TConv>
          (
           gd.Xstar(t)*x
         , D[t]
         , E[t]
         , W[t]
         , gd
         , PStrj
          );
        SecondPlayerControl<TNum, TConv> spControl =
          new SecondPlayerControl<TNum, TConv>
          (
           gd.Xstar(t)*x
         , D[t]
         , E[t]
         , W[t]
         , gd
         , QStrj
          );


        // Выполняем шаг Эйлера
        x += gd.dt * (gd.A * x + gd.B * fpControl.Control() + gd.C * spControl.Control());
        trajectory.Add(x);
        aimpointsP.Add(fpControl.AimPoint);
        aimpointsQ.Add(spControl.AimPoint);
      }

      using Geometry<TNum, TConv>.ParamWriter pwT = new Geometry<TNum, TConv>.ParamWriter($"{OutputDir}{name}_tr-{num}.traj");
      using Geometry<TNum, TConv>.ParamWriter pwP = new Geometry<TNum, TConv>.ParamWriter($"{OutputDir}{name}_aimP-{num}.aimp");
      using Geometry<TNum, TConv>.ParamWriter pwQ = new Geometry<TNum, TConv>.ParamWriter($"{OutputDir}{name}_aimQ-{num}.aimq");

      //todo: 1) сохранять в папку с именем игры
      
      pwT.WriteString("md5-dynamic", gd.DynamicHash);
      pwT.WriteString("md5", Hashes.GetMD5Hash($"{name}{gd.t0}{gd.T}{x0}{PStrj}{QStrj}")); //todo: детальное описание стратегии
      pwT.WriteVectors("Trajectory", trajectory);

      pwP.WriteString("md5-dynamic", gd.DynamicHash);
      pwP.WriteString("md5", Hashes.GetMD5Hash($"{name}{gd.t0}{gd.T}{x0}{PStrj}{QStrj}")); //todo: детальное описание стратегии
      pwP.WriteVectors("Trajectory", aimpointsP);

      pwQ.WriteString("md5-dynamic", gd.DynamicHash);
      pwQ.WriteString("md5", Hashes.GetMD5Hash($"{name}{gd.t0}{gd.T}{x0}{PStrj}{QStrj}")); //todo: детальное описание стратегии
      pwQ.WriteVectors("Trajectory", aimpointsQ);
    }
  }

  public enum ControlType { Optimal, Constant }

  public ControlType ReadControlType(Geometry<TNum, TConv>.ParamReader pr, char player) {
    return pr.ReadString($"{player}Control") switch
             {
               "Optimal"  => ControlType.Optimal
             , "Constant" => ControlType.Constant
             , _          => throw new ArgumentException("!!!")
             };
  }

}

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    // string bridgeDir     = "F:\\Works\\IMM\\Аспирантура\\LDG\\Bridges\\Simple Motion_T=10\\First TS_01\\System.Double\\1e-008\\";
    // string configDir     = "F:\\Works\\IMM\\Аспирантура\\LDG\\Trajectories\\Configs\\";
    // string outputDir     = "F:\\Works\\IMM\\Аспирантура\\LDG\\Trajectories\\";
    // string gameConfigDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\Bridges\\Configs\\";
    
    string bridgeDir     = "E:\\Work\\LDG\\Bridges\\Simple Motion_T=10\\Explicit_01\\System.Double\\1e-008\\";
    string configDir     = "E:\\Work\\LDG\\Trajectories\\Configs\\";
    string outputDir     = "E:\\Work\\LDG\\Trajectories\\";
    string gameConfigDir = "E:\\Work\\LDG\\Bridges\\Configs\\";

    
    
    
    TrajMain<double, DConvertor> trajCalc =
      new TrajMain<double, DConvertor>(gameConfigDir + "SimpleMotion.gameconfig", bridgeDir, configDir, outputDir);

    trajCalc.CalcTraj(configDir + "SimpleMotion");
  }

}
