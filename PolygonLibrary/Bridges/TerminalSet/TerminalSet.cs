namespace Bridges;

public class TerminalSet<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> tmss;

  public TerminalSet(string tmsName, LDGDirHolder<TNum,TConv> dh) {
    // открыли ридер терминального множества
    Geometry<TNum, TConv>.ParamReader    pr   = dh.OpenTerminalSetReader(tmsName);
    string type = pr.ReadString("TS Type");

    ITerminalSetReader<TNum, TConv> reader =
      type switch
        {
          "Explicit" => new ExplicitTS<TNum, TConv>()
          // "Minkowski" => new MinkowskiTS<TNum, TConv>()
        , _          => throw new ArgumentException($"Bridges.TerminalSet.Ctor: Unknown Type: {type}")
        };

    tmss = reader.ReadTerminalSets(pr, dh);
  }

  public bool GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop tms) {
    throw new NotImplementedException("TODO");
  }

}
