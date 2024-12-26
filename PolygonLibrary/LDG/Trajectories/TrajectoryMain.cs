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

    // загружаем минимальные времена мостов
    int          i  = 1;
    StreamReader sr = new StreamReader(ph.PathBr + "!times.txt");
    while (sr.EndOfStream) {
      times.Add(i, TNum.Parse(sr.ReadLine(), CultureInfo.InvariantCulture));
      i++;
    }
    tMin = times.MinBy(p => p.Value).Value;

    if (Geometry<TNum, TConv>.Tools.GE(tMin, gd.T)) {
      throw new ArgumentException($"The t0 should be less then T. Found t0 = {gd.t0} < T = {gd.T}");
    }

    // грузим все мосты

    int brDir = Directory.GetDirectories(ph.PathBr).Length;
    for (int j = 1; j <= brDir; j++) {
      SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> W =
        new SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop>();
      for (TNum t = times[j]; Geometry<TNum, TConv>.Tools.LE(gd.T); t += gd.dt) {
        ph.LoadBridgeSection(W, i, t);
      }
      Ws.Add(W);
    }

    // вычисляем матрицы D и E (они легко вычисляются)
    {
      TNum t = tMin;
      do {
        D[t] = gd.Xstar(t) * gd.B;
        E[t] = gd.Xstar(t) * gd.C;

        t += gd.dt;
      } while (Geometry<TNum, TConv>.Tools.LE(t, gd.T));
    }
  }

  public void CalcTraj(string trajName) {
    Geometry<TNum, TConv>.ParamReader pr =
      new Geometry<TNum, TConv>.ParamReader(Path.Combine(ph.PathTrajectories, trajName + ".trajconfig"));
    outputTrajName = pr.ReadString("Name");
    t0             = pr.ReadNumber<TNum>("t0");
    T              = pr.ReadNumber<TNum>("T");
    x0             = pr.ReadVector("x0");


    // Считываем настройки управлений игроков

    IController<TNum,TConv> fp = ControlFactory<TNum,TConv>.Read("FPControl", pr, ph);
    IController<TNum,TConv> sp = ControlFactory<TNum,TConv>.Read("SPControl",pr,ph);
  }

}
