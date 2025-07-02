namespace LDG;

/// <summary>
/// An abstract factory for reading epigraph's parameters.
/// </summary>
public abstract class EpiTypeFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads the type of epigraph from the parameter reader and creates an instance of the corresponding EpiTypeBase.
  /// </summary>
  /// <param name="pr">The terminal set parameter reader to read from.</param>
  /// <param name="ph">Provides access to files describing polytopes. Required for epigraph types that involve explicit polytopes.</param>
  /// <returns>An instance of the created epigraph type with corresponding information.</returns>
  /// <exception cref="ArgumentException">Thrown when an unsupported epigraph type is encountered.</exception>
  public static EpiTypeBase<TNum, TConv> Read(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
    string epiType = pr.ReadString("Type");
    EpiTypeBase<TNum, TConv> epigraph =
      epiType switch
        {
          "DistToPointByUnitBall" => new EpiTypes<TNum, TConv>.DistToPointByUnitBall(pr, ph)
        , "DistToPointByNorm"     => new EpiTypes<TNum, TConv>.DistToPointByNorm(pr)
        , "DistToPolytopeByNorm"  => new EpiTypes<TNum, TConv>.DistToPolytope(pr, ph)
        , _ => throw new ArgumentException
                 (
                  $"Unsupported epigraph type: '{epiType}'.\nIn file {pr.filePath}\n" +
                  $"Please refer to the documentation for supported types."
                 )
        };

    return epigraph;
  }

}
