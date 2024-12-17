namespace Bridges;

public class TerminalSet<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  private readonly IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> _tmss;
  private readonly IEnumerator<Geometry<TNum, TConv>.ConvexPolytop> _tmssEnumerator;

  public TerminalSet(string tmsName, LDGPathHolder<TNum, TConv> dh, ref Geometry<TNum, TConv>.GameData gd) {
    // открыли ридер терминального множества
    Geometry<TNum, TConv>.ParamReader pr   = dh.OpenTerminalSetReader(tmsName);
    string                            type = pr.ReadString("TSType");

    ITerminalSetReader<TNum, TConv> reader =
      type switch
        {
          "Explicit"  => new ExplicitTS<TNum, TConv>()
        , "Epigraph"  => new EpigraphTS<TNum, TConv>(ref gd)
        , "Minkowski" => new MinkowskiTS<TNum, TConv>()
        , "LevelSet"  => new LevelSetTS<TNum, TConv>()
        , _           => throw new ArgumentException($"Bridges.TerminalSet.Ctor: Unknown Type: {type}")
        };

    _tmss           = reader.ReadTerminalSets(pr, dh);
    _tmssEnumerator = _tmss.GetEnumerator();
  }

  public bool GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms) {
    bool hasNext = _tmssEnumerator.MoveNext();
    tms = hasNext ? _tmssEnumerator.Current : null;

    return hasNext;
  }

}
