namespace LDG;

/// <summary>
/// Base class for epigraph type configurations. It holds common parameters
/// and defines a contract for reading them from a configuration file.
/// </summary>
public abstract class EpiTypeBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// The scale factor for the epigraph.
  /// </summary>
  protected TNum ScaleFactor { get; }

  /// <summary>
  /// Base constructor that ensures the ScaleFactor is provided.
  /// <param name="pr">The parameter reader used to extract data from the terminal set configuration file.</param>
  /// </summary>
  protected EpiTypeBase(Geometry<TNum, TConv>.ParamReader pr) {
    TNum num = pr.ReadNumber<TNum>("ScaleFactor");
    if (Geometry<TNum, TConv>.Tools.LE(num)) {
      throw new ArgumentException($"ScaleFactor must be greater than zero. Found ScaleFactor = {num}");
    }
    ScaleFactor = num;
  }

  /// <summary>
  /// Creates and returns the epigraph of the function defined by the derived class, represented as a convex polytope.
  /// </summary>
  /// <returns>
  /// A <see cref="Geometry{TNum, TConv}.ConvexPolytop"/> object representing the epigraph in a (d+1)-dimensional space.
  /// </returns>
  public abstract Geometry<TNum, TConv>.ConvexPolytop BuildEpigraph();

}
