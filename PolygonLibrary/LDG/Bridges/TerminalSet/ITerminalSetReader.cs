namespace LDG;

/// <summary>
/// Defines methods for reading terminal sets and creates polytopes used in solving a linear differential game.
/// </summary>
public interface ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads or constructs a convex polytope based on its name.
  /// </summary>
  /// <param name="name">The name of the polytope to read.</param>
  /// <param name="ph">The path holder providing access to polytope files.</param>
  /// <returns>A convex polytope described by the specified name.</returns>
  public static Geometry<TNum, TConv>.ConvexPolytop DoPolytope(string name, LDGPathHolder<TNum, TConv> ph) {
    return PolytopeReader<TNum, TConv>.Read(ph.OpenPolytopeReader(name));
  }

/// <summary>
/// Builds the terminal sets based on param reader of the terminal set configuration file.
/// </summary>
/// <param name="pr">The param reader of the terminal set file.</param>
/// <param name="ph">The path holder providing access to polytope files.</param>
/// <returns>An enumeration of convex polytopes representing the terminal sets.</returns>
  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> BuildTerminalSets(
    Geometry<TNum, TConv>.ParamReader pr
  , LDGPathHolder<TNum, TConv>        ph);
}
