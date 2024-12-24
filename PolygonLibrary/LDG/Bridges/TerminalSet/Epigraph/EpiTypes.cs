namespace LDG;

/// <summary>
/// Represents different types of epigraphs and provides mechanisms to read their parameters according to the documentation.
/// </summary>
public abstract class EpiTypes<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents an epigraph type that is the distance to a point.
  /// </summary>
  public class DistToPoint : IEpiType<TNum, TConv> {

    /// <summary>
    /// Gets the point to which the distance is calculated.
    /// </summary>
    public Geometry<TNum, TConv>.Vector Point { get; private set; } = null!;

    /// <summary>
    /// Reads the parameters required to configure the epigraph type for distance-to-point calculations.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract the "Point" parameter from the terminal set configuration file.</param>
    /// <param name="ph">Provides access to files describing polytopes. Required only for distance-to-polytope calculations.</param>
    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
      Point = pr.ReadVector("Point");
    }
  }

  /// <summary>
  /// Represents an epigraph type that is the distance to a convex polytope.
  /// </summary>
  public class DistToPolytope : IEpiType<TNum, TConv> {

    /// <summary>
    /// Gets the convex polytope used for constructing terminal sets.
    /// </summary>
    public Geometry<TNum, TConv>.ConvexPolytop Polytope { get; private set; } = null!;

    /// <summary>
    /// Reads the parameters required to configure the epigraph type for distance-to-polytope calculations.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract the "Polytope" parameter from the terminal set configuration file.</param>
    /// <param name="ph">Provides access to files describing polytopes. Required only for distance-to-polytope calculations.</param>
    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
      Polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Polytope"), ph);
    }
  }
}
