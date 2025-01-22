using System.Globalization;

namespace LDG;

public class TrajectoryMain<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly LDGPathHolder<TNum, TConv> ph; // Класс, работающий со структурой папок проекта LDG
  public readonly BridgeCreator<TNum, TConv> br; // Класс, строящий мосты
  public readonly GameData<TNum, TConv>      gd; // Класс, хранящий информацию об игре

  public readonly Dictionary<int, TNum> MinTimes = new Dictionary<int, TNum>(); // Словарь, отображающий номер моста в его tMin
  public readonly TNum                  tMin; // Наименьший момент времени из MinTimes, для которого есть сечение моста


  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws =
    new List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>>(); // Набор мостов

  public TrajectoryMain(string ldgPath, string problemFolderName) {
    br = new BridgeCreator<TNum, TConv>(ldgPath, problemFolderName);
    ph = br.ph;
    gd = br.gd;

    ph.LoadMinimalTimes(MinTimes); // загружаем минимальные времена мостов
    tMin = MinTimes.MinBy(p => p.Value).Value;

    if (Geometry<TNum, TConv>.Tools.GE(tMin, gd.T)) {
      throw new ArgumentException($"The t0 should be less then T. Found t0 = {gd.t0} < T = {gd.T}");
    }

    // грузим все мосты

    int brDir = Directory.GetDirectories(ph.PathBrs).Length;
    for (int j = 0; j < brDir; j++) {
      var W = new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);
      for (TNum t = MinTimes[j]; Geometry<TNum, TConv>.Tools.LE(t, gd.T); t += gd.dt) {
        ph.LoadBridgeSection(W, j, t);
      }
      Ws.Add(W);
    }
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

    if (Geometry<TNum, TConv>.Tools.LT(t0, tMin)) {
      throw new ArgumentException
        (
         $"The specified time t0 = {t0} is less than the minimum computed time tMin = {tMin}. Bridges are only computed for times greater than or equal to tMin."
        );
    }

    Geometry<TNum, TConv>.Vector x = x0;
    for (TNum t = tMin; Geometry<TNum, TConv>.Tools.LT(t, T); t += gd.dt) {
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
