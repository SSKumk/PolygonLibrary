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

  public void CalcTraj(string trajName) {
    Geometry<TNum, TConv>.ParamReader pr = ph.OpenTrajConfigReader(trajName);

    string outputTrajName = pr.ReadString("Name"); // имя папки, где будут лежать результаты счёта траектории
    TNum t0 = pr.ReadNumber<TNum>("t0"); // начальный момент времени
    TNum T = pr.ReadNumber<TNum>("T"); // терминальный момент времени
    Geometry<TNum, TConv>.Vector x0 = pr.ReadVector("x0"); // начальная точка

    string pathTrajName = Path.Combine(ph.PathTrajectories, outputTrajName);
    if (Directory.Exists(pathTrajName)) {
      throw new ArgumentException($"The folder with name '{outputTrajName}' already exits in {ph.PathTrajectories} path!");
    }
    Directory.CreateDirectory(pathTrajName);

    // Считываем настройки управлений игроков

    IController<TNum, TConv> fp = ControlFactory<TNum, TConv>.ReadFirstPlayer(pr, Ws);
    IController<TNum, TConv> sp = ControlFactory<TNum, TConv>.ReadSecondPlayer(pr, Ws);

    var trajectory = new List<Geometry<TNum, TConv>.Vector>() { x0 }; // сама траектория
    var fpControls = new List<Geometry<TNum, TConv>.Vector>();        // реализация управлений первого игрока
    var spControls = new List<Geometry<TNum, TConv>.Vector>();        // реализация управлений второго игрока
    var fpAims     = new List<Geometry<TNum, TConv>.Vector>();        // точки прицеливания первого игрока
    var spAims     = new List<Geometry<TNum, TConv>.Vector>();        // точки прицеливания второго игрока

    if (Geometry<TNum, TConv>.Tools.LT(t0, tMax)) {
      throw new ArgumentException
        (
         $"TrajectoryMain.CalcTraj: The specified time t0 = {t0} is less than the time tMax = {tMax}. All bridges are only computed for times greater than or equal to tMax." +
         $"In the file {pr.filePath}"
        );
    }

    Geometry<TNum, TConv>.Vector x = x0;
    for (TNum t = tMax; Geometry<TNum, TConv>.Tools.LT(t, T); t += gd.dt) {
      Geometry<TNum, TConv>.Vector proj_x = gd.Xstar(t) * x;

      Geometry<TNum, TConv>.Vector fpControl = fp.Control(t, proj_x, out Geometry<TNum, TConv>.Vector aimFp, gd);
      Geometry<TNum, TConv>.Vector spControl = sp.Control(t, proj_x, out Geometry<TNum, TConv>.Vector aimSp, gd);

      // Выполняем шаг Эйлера
      x += gd.dt * (gd.A * x + gd.B * fpControl + gd.C * spControl);

      // сохраняем всё
      trajectory.Add(x);
      fpControls.Add(fpControl);
      spControls.Add(spControl);
      fpAims.Add(aimFp);
      spAims.Add(aimSp);
    }

    using var pwTr  = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "game.traj"));
    using var pwFPC = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "fp.control"));
    using var pwSPC = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "sp.control"));
    using var pwFPA = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "fp.aim"));
    using var pwSPA = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "sp.aim"));
    using var pwT   = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, ".times"));

    pwTr.WriteVectors("Trajectory", trajectory);
    pwFPC.WriteVectors("Control", fpControls);
    pwSPC.WriteVectors("Control", spControls);
    pwFPA.WriteVectors("Aim", fpAims);
    pwSPA.WriteVectors("Aim", spAims);
    pwT.WriteNumber("MinTime", t0);
    pwT.WriteNumber("MaxTime", T);
  }

}
