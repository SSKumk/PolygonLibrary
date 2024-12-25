using System.Globalization;
namespace LDG;

public class TrajectoryMain<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly LDGPathHolder<TNum, TConv> ph;
  public readonly BridgeCreator<TNum, TConv> br;

  public readonly string outputTrajName;
  public readonly TNum t0;
  public readonly TNum T;
  public readonly Geometry<TNum, TConv>.Vector x0;

  public readonly Dictionary<int, TNum> times = new Dictionary<int, TNum>(); // номер моста --> в его t0.
  public readonly TNum tMin;

  public TrajectoryMain(string ldgPath, string problemFileName, string trajName) {
    br = new BridgeCreator<TNum, TConv>(ldgPath, problemFileName);
    ph = br.ph;

    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(Path.Combine(ph.PathTrajectories, trajName + ".trajconfig"));
    outputTrajName = pr.ReadString("Name");
    t0             = pr.ReadNumber<TNum>("t0");
    T              = pr.ReadNumber<TNum>("T");

    // загружаем минимальные времена мостов
    int          i  = 1;
    StreamReader sr = new StreamReader(ph.PathBr + "!times.txt");
    while (sr.EndOfStream) {
      times.Add(i,TNum.Parse(sr.ReadLine(), CultureInfo.InvariantCulture));
      i++;
    }

    x0 = pr.ReadVector("x0");

    IControl fp = Control.Read("FPControl");
    IControl sp = Control.Read("SPControl");

    CalcTraj(t0,T,x0,fp.Control(), sp.Control());
  }
}
