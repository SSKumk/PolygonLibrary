using System.Globalization;

namespace LDG;

public class TrajectoryMain<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly LDGPathHolder<TNum, TConv> ph; // Класс, работающий со структурой папок проекта LDG
  public readonly BridgeCreator<TNum, TConv> br; // Класс, строящий мосты
  public readonly GameData<TNum, TConv>      gd; // Класс, хранящий информацию об игре

  public readonly TNum tMax; // Момент времени из MinTimes, начиная с которого у каждого моста есть сечения


  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws; // Набор всех мостов

  public TrajectoryMain(string ldgPath, string problemFolderName, TNum precision) {
    br = new BridgeCreator<TNum, TConv>(ldgPath, problemFolderName, precision);
    ph = br.ph;
    gd = br.gd;

    tMax = ph.LoadMinimalTimes().MaxBy(p => p.Value).Value;

    // грузим все мосты
    Ws = ph.LoadBridges();
  }

  public void CalcTraj(string trajName, bool clearFolder = false) {
    Geometry<TNum, TConv>.ParamReader pr = ph.OpenTrajConfigReader(trajName);

    string outputTrajName = pr.ReadString("Name"); // имя папки, где будут лежать результаты счёта траектории
    TNum t0 = pr.ReadNumber<TNum>("t0"); // начальный момент времени
    TNum T = pr.ReadNumber<TNum>("T"); // терминальный момент времени
    Geometry<TNum, TConv>.Vector x0 = pr.ReadVector("x0"); // начальная точка

    if (x0.SpaceDim != gd.ProjDim) {
      throw new ArgumentException
        (
         $"The dimension of the x0 should be equal to the dimension of the equivalent phase vector = {gd.ProjDim}. Found x0.SpaceDim = {x0.SpaceDim}"
        );
    }

    string pathTrajName = Path.Combine(ph.PathTrajectories, outputTrajName);

    if (Directory.Exists(pathTrajName) && clearFolder) {
      Directory.Delete(pathTrajName, true);
    }

    if (Directory.Exists(pathTrajName) && Directory.GetFiles(pathTrajName).Length > 0) {
      throw new ArgumentException($"The folder with name '{outputTrajName}' already exits in {ph.PathTrajectories} path!");
    }
    Directory.CreateDirectory(pathTrajName);

    // Считываем настройки управлений игроков

    IController<TNum, TConv> fp = ControlFactory<TNum, TConv>.ReadFirstPlayer(pr, Ws);
    IController<TNum, TConv> sp = ControlFactory<TNum, TConv>.ReadSecondPlayer(pr, Ws);

    var trajectory = new List<Geometry<TNum, TConv>.Vector>(); // сама траектория
    var fpControls = new List<Geometry<TNum, TConv>.Vector>(); // реализация управлений первого игрока
    var spControls = new List<Geometry<TNum, TConv>.Vector>(); // реализация управлений второго игрока
    var fpAims     = new List<Geometry<TNum, TConv>.Vector>(); // точки прицеливания первого игрока
    var spAims     = new List<Geometry<TNum, TConv>.Vector>(); // точки прицеливания второго игрока

    if (Geometry<TNum, TConv>.Tools.LT(t0, tMax)) {
      Console.WriteLine
        (
         $"Warning: TrajectoryMain.CalcTraj: The specified time t0 = {t0} is less than the time tMax = {tMax}. All bridges are only computed for times greater than or equal to tMax." +
         $"In the file {pr.filePath}\nThe tMax is used as t0."
        );
    }


    Geometry<TNum, TConv>.Vector x = x0; // уже в пространстве эквивалентной игры
    for (TNum t = TNum.Max(tMax, t0); Geometry<TNum, TConv>.Tools.LT(t, T); t += gd.dt) {
      Geometry<TNum, TConv>.Vector fpControl = fp.Control(t, x, out Geometry<TNum, TConv>.Vector aimFp, gd);
      Geometry<TNum, TConv>.Vector spControl = sp.Control(t, x, out Geometry<TNum, TConv>.Vector aimSp, gd);

      // сохраняем всё
      trajectory.Add(x);
      fpControls.Add(fpControl);
      spControls.Add(spControl);
      fpAims.Add(aimFp);
      spAims.Add(aimSp);

      // Выполняем шаг Эйлера
      x += gd.dt * (gd.D[t] * fpControl + gd.E[t] * spControl);
    }

    using var pwTr  = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "game.traj"));
    using var pwFPC = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "fp.control"));
    using var pwSPC = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "sp.control"));
    using var pwFPA = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "fp.aim"));
    using var pwSPA = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "sp.aim"));

    pwTr.WriteVectors("Trajectory", trajectory);
    pwFPC.WriteVectors("Control", fpControls);
    pwSPC.WriteVectors("Control", spControls);
    pwFPA.WriteVectors("Aim", fpAims);
    pwSPA.WriteVectors("Aim", spAims);

    Console.WriteLine($"Traj {trajName} computed!");
  }

}
