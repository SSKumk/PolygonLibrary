namespace LDG;

public class TerminalSet<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  private readonly IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> _tmss;
  private readonly IEnumerator<Geometry<TNum, TConv>.ConvexPolytop> _tmssEnumerator;

  private readonly TransformReader<TNum, TConv>.Transform _tr;
  private          int                                    _dim;

  public TerminalSet(
      string                                 tmsName
    , LDGPathHolder<TNum, TConv>             ph
    , ref GameData<TNum, TConv>              gd
    , TransformReader<TNum, TConv>.Transform tr
    ) {
    // открыли ридер терминального множества
    Geometry<TNum, TConv>.ParamReader pr   = ph.OpenTerminalSetReader(tmsName);
    string                            _    = pr.ReadString("Name");
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

    _tr  = tr;
    _dim = gd.projDim;

    _tmss           = reader.ReadTerminalSets(pr, ph);
    _tmssEnumerator = _tmss.GetEnumerator();
  }

  public bool GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms) {
    bool hasNext = _tmssEnumerator.MoveNext();
    tms = !hasNext ? null : TransformReader<TNum, TConv>.DoTransform(_tmssEnumerator.Current, _tr, _dim);

    return hasNext;
  }

}
