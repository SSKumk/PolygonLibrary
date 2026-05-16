namespace LDG;

/// <summary>
/// Defines the interface for classes that are responsible for reading the parameters of a polytope and constructing the corresponding polytope object.
/// </summary>
public interface IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads the parameters of a polytope from the given parameter reader and constructs the corresponding polytope object.
  /// </summary>
  /// <param name="pr">The parameter reader containing the polytopes data.</param>
  /// <returns>A convex polytope read from the reader.</returns>
  Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr);
}
