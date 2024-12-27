namespace LDG;

/// <summary>
/// Represents a terminal set reader for the "Explicit" type, which builds terminal sets based on explicitly provided polytopes.
/// </summary>
public class ExplicitTerminalSet<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Builds terminal sets based on explicitly defined polytopes by reading the polytope names from the parameter reader.
  /// And sorts it by inclusion relation.
  /// </summary>
  /// <param name="pr">The parameter reader used to extract the polytope names from the terminal set configuration file.</param>
  /// <param name="ph">The path holder used to access polytope files.</param>
  /// <returns>A collection of terminal sets based on the explicitly defined polytopes.</returns>
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> BuildTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph
    ) {
    int      k     = pr.ReadNumber<int>("Qnt");
    string[] names = pr.Read1DArray<string>("Polytopes", k);

    Geometry<TNum, TConv>.ConvexPolytop[] polytopes =
      names.Select(name => ITerminalSetReader<TNum, TConv>.DoPolytope(name, ph)).ToArray();

    Array.Sort(polytopes, new InclusionComparer());

    return polytopes;
  }

  /// <summary>
  /// We assume that the polytopes are contained non-strictly within each other.
  /// </summary>
  private class InclusionComparer : IComparer<Geometry<TNum, TConv>.ConvexPolytop> {

    public int Compare(Geometry<TNum, TConv>.ConvexPolytop? x, Geometry<TNum, TConv>.ConvexPolytop? y) {
      if (x is null && y is null)
        return 0;
      if (x is null)
        return -1;
      if (y is null)
        return 1;

      if (x.Vrep.SetEquals(y.Vrep)) {
        return 0;
      }

      if (y.Contains(x.Vrep.First())) {
        if (x.Vrep.All(y.Contains)) {
          return -1;
        }

        throw new ArgumentException("InclusionComparer.Compare: Polytopes are not included one into another.");
      }
      if (y.Vrep.All(x.Contains)) {
        return 1;
      }

      throw new ArgumentException("InclusionComparer.Compare: Polytopes are not included one into another.");
    }

  }

}
