namespace LDG;

/// <summary>
/// Represents a terminal set reader for the "Minkowski" type, which builds terminal sets based on Minkowski scaling of a given polytope.
/// </summary>
public class MinkowskiTerminalSet<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  
  /// <summary>
  /// Builds terminal sets by scaling a given polytope using Minkowski's method, based on specified constants.
  /// </summary>
  /// <param name="pr">The parameter reader used to extract the polytope and constants from the terminal set configuration file.</param>
  /// <param name="ph">The path holder providing access to polytope files.</param>
  /// <returns>A sequence of terminal sets, each scaled by a some positive constant.</returns>
  /// <exception cref="ArgumentException">Thrown if any constant is less than or equal to zero.</exception>
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> BuildTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph
    ) {
    Geometry<TNum, TConv>.ConvexPolytop polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Polytope"), ph);

    TNum[] ks = pr.ReadVector("Constants").GetAsArray();
    Array.Sort(ks);
    if (ks.Any(Geometry<TNum, TConv>.Tools.LE)) {
      throw new ArgumentException("MinkowskiTerminalSet.BuildTerminalSets: All constants must be greater than zero!");
    }
    

    return ks.Select(k => polytope.Scale(k, polytope.InnerPoint));
  }

}
