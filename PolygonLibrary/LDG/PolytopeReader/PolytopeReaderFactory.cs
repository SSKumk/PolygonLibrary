namespace LDG;

/// <summary>
/// This abstract class is responsible for reading the parameters of a polytope based on its type and constructing the corresponding polytope.
/// It determines the specific reader based on the 'Type' field in the parameters and delegates the reading task to the appropriate reader class.
/// </summary>
public abstract class PolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads the parameters of a polytope from the given parameter reader and constructs the corresponding convex polytope.
  /// The method identifies the type of the polytope and delegates the actual reading to the appropriate reader class.
  /// </summary>
  /// <param name="pr">The parameter reader that contains the polytopes data.</param>
  /// <returns>A polytope object of the type specified in the parameters.</returns>
  /// <exception cref="ArgumentException">
  /// Thrown if the polytope type is unknown.
  /// </exception>
  public static Geometry<TNum, TConv>.ConvexPolytop Read(Geometry<TNum, TConv>.ParamReader pr) => GetReader(pr).ReadPolytope(pr);

  /// <summary>
  /// Determines the correct reader for the polytope based on its 'Type' parameter and returns the appropriate reader instance.
  /// </summary>
  /// <param name="pr">The parameter reader containing the 'Type' of the polytope.</param>
  /// <returns>An instance of the appropriate polytope reader.</returns>
  /// <exception cref="ArgumentException">
  /// Thrown if the polytope type is unknown.
  /// </exception>
  private static IPolytopeReader<TNum, TConv> GetReader(Geometry<TNum, TConv>.ParamReader pr) {
    string type = pr.ReadString("Type");

    return type switch
             {
               "CGLibrary"    => new CGLibraryReader<TNum, TConv>()
             , "Convex Hull"  => new ConvexHullReader<TNum, TConv>()
             , "Hyper Planes" => new HyperPlanesReader<TNum, TConv>()
             , "Generator"    => new GeneratorReader<TNum, TConv>()
             , _              => throw new ArgumentException($"Toolkit.PolytopeReader.PolytopeReader.GetReader: Unknown Type: {type}")
             };
  }
}
