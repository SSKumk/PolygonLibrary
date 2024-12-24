namespace LDG;

/// <summary>
/// Represents a collection of level set types which stores the necessary parameters. The types used to define terminal sets 
/// based on specific functions.
/// </summary>
public abstract class LvlSetTypes<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Represents a level set type defined by the distance function to a specific point.
  /// </summary>
  public class DistToPoint : ILvlSetType<TNum, TConv> {

    /// <summary>
    /// Gets the point used to define the level set.
    /// </summary>
    public Geometry<TNum, TConv>.Vector Point { get; private set; } = null!;

    /// <summary>
    /// Reads the parameters required to define the distance-to-point level set.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract the "Point" parameter from the terminal set configuration file.</param>
    /// <param name="ph">Provides access to files describing polytopes. Required only for distance-to-polytope calculations.</param>
    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
      Point = pr.ReadVector("Point");
    }
  }

  /// <summary>
  /// Represents a level set type defined by the distance function to a specific convex polytope.
  /// </summary>
  public class DistToPolytope : ILvlSetType<TNum, TConv> {

    /// <summary>
    /// Gets the convex polytope used to define the level set.
    /// </summary>
    public Geometry<TNum, TConv>.ConvexPolytop Polytope { get; private set; } = null!;

    /// <summary>
    /// Reads the parameters required to define the distance-to-polytope level set.
    /// </summary>
    /// <param name="pr">The parameter reader used to extract the "Polytope" parameter.</param>
    /// <param name="ph">Provides access to files describing polytopes. Required only for distance-to-polytope calculations.</param>
    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
      Polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Polytope"), ph);
    }
  }
}

