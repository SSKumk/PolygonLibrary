namespace LDG;

/// <summary>
/// Represent a collection of terminal sets. Please refer to the documentation for details on the supported types.
/// </summary>
public class TerminalSet<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  private readonly IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> _tmss;           // терминальные множества
  private readonly IEnumerator<Geometry<TNum, TConv>.ConvexPolytop> _tmssEnumerator; // Перечислитель терминальных множеств

  private readonly TransformReader<TNum, TConv>.Transform _tr; // Читальщик преобразований, применяемых к терминальным множествам
  private readonly int _dim;                                   // Размерность терминальных множеств

  /// <summary>
  /// Reads the terminal set configuration and builds the terminal sets based on the provided type.
  /// </summary>
  /// <param name="tmsName">The name of the terminal set configuration.</param>
  /// <param name="ph">The path holder providing access to polytope files.</param>
  /// <param name="gd">GameData will be modified only for the "Epigraph" type.
  /// For all other types, <paramref name="gd"/> is not used.</param>
  /// <param name="tr">The transform to be applied to the terminal sets.</param>
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
          "Explicit"  => new ExplicitTerminalSet<TNum, TConv>()
        , "Epigraph"  => new EpigraphTerminalSet<TNum, TConv>(ref gd)
        , "Minkowski" => new MinkowskiTerminalSet<TNum, TConv>()
        , "LevelSet"  => new LevelSetTerminalSet<TNum, TConv>()
        , _           => throw new ArgumentException($"Bridges.TerminalSet.Ctor: Unknown Type: {type}")
        };

    _tr  = tr;
    _dim = gd.ProjDim;

    _tmss           = reader.BuildTerminalSets(pr, ph);
    _tmssEnumerator = _tmss.GetEnumerator();
  }

  /// <summary>
  /// Retrieves the next terminal set in the sequence, applying the specified transform if necessary.
  /// </summary>
  /// <param name="tms">The next terminal set, or null if no more sets are available.</param>
  /// <returns><c>true</c> if a terminal set was retrieved; otherwise, <c>false</c>.</returns>
  public bool GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms) {
    bool hasNext = _tmssEnumerator.MoveNext();
    tms = !hasNext ? null : TransformReader<TNum, TConv>.DoTransform(_tmssEnumerator.Current, _tr, _dim);

    return hasNext;
  }

}
