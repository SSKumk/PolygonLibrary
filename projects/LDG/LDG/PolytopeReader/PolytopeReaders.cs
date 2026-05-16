namespace LDG;

/// <summary>
/// Reads a polytope from a format defined by the CGLibrary.
/// The polytope is constructed from data in a specific format provided by the CGLibrary, please refer to the documentation. 
/// </summary>
public class CGLibraryReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads a polytope from the CGLibrary format and constructs the corresponding polytope object.
  /// </summary>
  /// <param name="pr">The parameter reader that provides the polytope data in the CGLibrary format.</param>
  /// <returns>The constructed convex polytope based on the data provided by the reader.</returns>
  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr)
    => Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(pr);

}

/// <summary>
/// Reads a polytope defined by a set of points, constructing the convex hull of the points.
/// The polytope is formed as the convex hull of a set of input vectors, with the option to remove redundant points based on the "DoRed" flag.
/// </summary>
public class ConvexHullReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads a polytope from a list of vectors and constructs the convex hull.
  /// The option to remove redundant points is determined by the "DoRed" flag, which is read from the input parameters.
  /// </summary>
  /// <param name="pr">The parameter reader containing the vectors that define the polytope, as well as the "DoRed" flag.</param>
  /// <returns>The constructed convex polytope formed by the convex hull of the input vectors.</returns>
  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr) {
    List<Geometry<TNum, TConv>.Vector> vs = new List<Geometry<TNum, TConv>.Vector>();

    bool doRed = pr.ReadBool("DoRed");
    int  qnt   = pr.ReadNumber<int>("VsQnt");
    int  d     = pr.ReadNumber<int>("VsDim");
    for (int i = 0; i < qnt; i++) {
      vs.Add(new Geometry<TNum, TConv>.Vector(pr.ReadNumberLine(d)));
    }


    return Geometry<TNum, TConv>.ConvexPolytop.CreateFromPoints(vs, doRed);
  }

}

/// <summary>
/// Reads a polytope defined by a set of hyperplanes.
/// The polytope is formed by the intersection of these hyperplanes, with the option to remove redundant hyperplanes using the "DoRed" flag.
/// </summary>
public class HyperPlanesReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads a polytope defined by a set of hyperplanes and constructs the corresponding polytope object.
  /// The option to remove redundant hyperplanes is determined by the "DoRed" flag.
  /// </summary>
  /// <param name="pr">The parameter reader containing the hyperplanes and the "DoRed" flag.</param>
  /// <returns>The constructed convex polytope defined by the intersection of the hyperplanes.</returns>
  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr) {
    List<Geometry<TNum, TConv>.HyperPlane> HPs = new List<Geometry<TNum, TConv>.HyperPlane>();

    bool doRed = pr.ReadBool("DoRed");
    int  qnt   = pr.ReadNumber<int>("HPsQnt");
    int  d     = pr.ReadNumber<int>("HPsDim");
    int  d1    = d + 1;
    for (int i = 0; i < qnt; i++) {
      TNum[] hp = pr.ReadNumberLine(d1);
      TNum[] v  = hp[..d];
      HPs.Add(new Geometry<TNum, TConv>.HyperPlane(new Geometry<TNum, TConv>.Vector(v, false), hp[^1]));
    }

    return Geometry<TNum, TConv>.ConvexPolytop.CreateFromHalfSpaces(HPs, doRed);
  }

}

/// <summary>
/// Reads a polytope generated based on specific parameters.
/// The polytope can be one of the following types:
/// <list type="bullet">
///     <item><description>"RectAxisParallel": A polytope defined by two vectors representing opposite corners of an axis-aligned rectangle.</description></item>
///     <item><description>"Sphere": A polytope generated as a sphere, defined by its dimension, azimuthal divisions, and polar divisions.</description></item>
///     <item><description>"Ellipsoid": A polytope generated as an ellipsoid, with a semi-axis and azimuthal and polar divisions.</description></item>
/// </list>
/// </summary>
public class GeneratorReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Reads a polytope based on the specified generator type.
  /// The generator type determines that polytope will be constructed.
  /// </summary>
  /// <param name="pr">The parameter reader containing the generator type and corresponding parameters.</param>
  /// <returns>The constructed polytope based on the specified generator type and parameters.</returns>
  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr) {
    string genType = pr.ReadString("GeneratorType");

    switch (genType) {
      case "RectAxisParallel": {
        Geometry<TNum, TConv>.Vector left  = pr.ReadVector("Left");
        Geometry<TNum, TConv>.Vector right = pr.ReadVector("Right");

        return Geometry<TNum, TConv>.ConvexPolytop.RectAxisParallel(left, right);
      }
      case "Sphere": {
        int dim               = pr.ReadNumber<int>("Dim");
        int azimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
        int polarDivision     = dim > 2 ? pr.ReadNumber<int>("PolarDivision") : 0;

        return Geometry<TNum, TConv>.ConvexPolytop.Sphere
          (Geometry<TNum, TConv>.Vector.Zero(dim), Geometry<TNum, TConv>.Tools.One, azimuthsDivisions, polarDivision);
      }
      case "Ellipsoid": {
        int                          dim               = pr.ReadNumber<int>("Dim");
        Geometry<TNum, TConv>.Vector semiAxis          = pr.ReadVector("SemiAxis");
        int                          azimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
        int                          polarDivision     = dim > 2 ? pr.ReadNumber<int>("PolarDivision") : 0;

        return Geometry<TNum, TConv>.ConvexPolytop.Ellipsoid
          (azimuthsDivisions, polarDivision, Geometry<TNum, TConv>.Vector.Zero(dim), semiAxis);
      }
    }

    throw new ArgumentException($"PolytopeReaders.Generator: The Type = {genType} is unknown!");
  }
}
