using System.Globalization;

namespace LDG;

public class TrajectoryMain<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly LDGPathHolder<TNum, TConv> ph;
  public readonly BridgeCreator<TNum, TConv> br;
  public readonly GameData<TNum, TConv>      gd;

  public readonly Dictionary<int, TNum> times = new Dictionary<int, TNum>(); // номер моста --> в его t0.
  public readonly TNum                  tMin;

  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> D =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>();

  public readonly SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix> E =
    new SortedDictionary<TNum, Geometry<TNum, TConv>.Matrix>();

  public readonly List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>> Ws =
    new List<SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>>();


  public string                       outputTrajName; // имя папки, где будут лежать результаты счёта траектории
  public TNum                         t0;             // начальный момент времени
  public TNum                         T;              // терминальный момент времени
  public Geometry<TNum, TConv>.Vector x0;             // начальная точка

  public TrajectoryMain(string ldgPath, string problemFileName) {
    br = new BridgeCreator<TNum, TConv>(ldgPath, problemFileName);
    ph = br.ph;
    gd = br.gd;


    { // загружаем минимальные времена мостов
      int          i  = 1;
      StreamReader sr = new StreamReader(Path.Combine(ph.PathBrs, "!times.txt"));
      while (!sr.EndOfStream) {
        times.Add(i, TNum.Parse(sr.ReadLine(), CultureInfo.InvariantCulture));
        i++;
      }
    }
    tMin = times.MinBy(p => p.Value).Value;

    if (Geometry<TNum, TConv>.Tools.GE(tMin, gd.T)) {
      throw new ArgumentException($"The t0 should be less then T. Found t0 = {gd.t0} < T = {gd.T}");
    }

    // грузим все мосты

    int brDir = Directory.GetDirectories(ph.PathBrs).Length;
    for (int j = 1; j <= brDir; j++) {
      var W = new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>(Geometry<TNum, TConv>.Tools.TComp);
      for (TNum t = times[j]; Geometry<TNum, TConv>.Tools.LE(t, gd.T); t += gd.dt) {
        ph.LoadBridgeSection(W, j, t);
      }
      Ws.Add(W);
    }
  }

  public void CalcTraj(string trajName) {
    Geometry<TNum, TConv>.ParamReader pr = ph.OpenTrajConfigReader(trajName);
    outputTrajName = pr.ReadString("Name");
    t0             = pr.ReadNumber<TNum>("t0");
    T              = pr.ReadNumber<TNum>("T");
    x0             = pr.ReadVector("x0");

    Directory.CreateDirectory(Path.Combine(ph.PathTrajectories, outputTrajName));

    // Считываем настройки управлений игроков

    IController<TNum, TConv> fp = ControlFactory<TNum, TConv>.ReadFirstPlayer(pr, Ws);
    IController<TNum, TConv> sp = ControlFactory<TNum, TConv>.ReadSecondPlayer(pr, Ws);

    var trajectory = new List<Geometry<TNum, TConv>.Vector>() { x0 }; // сама траектория
    var fpControls = new List<Geometry<TNum, TConv>.Vector>();        // реализация управлений первого игрока
    var spControls = new List<Geometry<TNum, TConv>.Vector>();        // реализация управлений второго игрока
    var fpAims     = new List<Geometry<TNum, TConv>.Vector>();        // точки прицеливания первого игрока
    var spAims     = new List<Geometry<TNum, TConv>.Vector>();        // точки прицеливания второго игрока

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

    using var pwTr  = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "traj.txt"));
    using var pwFPC = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "fp.control"));
    using var pwSPC = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "sp.control"));
    using var pwFPA = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "fp.aim"));
    using var pwSPA = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathTrajectories, outputTrajName, "sp.aim"));

    pwTr.WriteVectors("Trajectory", trajectory);
    pwTr.WriteVectors("FPControls", fpControls);
    pwTr.WriteVectors("SPControls", spControls);
    pwTr.WriteVectors("FPAims", fpAims);
    pwTr.WriteVectors("SPAims", spAims);
  }

}
