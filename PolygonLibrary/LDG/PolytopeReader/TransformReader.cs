namespace LDG;

/// <summary>
/// Abstract class for reading and applying transformations (scaling, shifting, and rotation) to polytopes.
/// This class helps to read transformation parameters from data and apply them to polytopes.
/// </summary>
public abstract class TransformReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Structure representing a transformation consisting of scaling, shifting, and rotation matrix.
  /// Scaling and shifting can be optional (null), and rotation may also be absent.
  /// </summary>
  public struct Transform(TNum? scale, Geometry<TNum, TConv>.Vector? shift, Geometry<TNum, TConv>.Matrix? rotate) {

    public TNum? scale = scale;
    public Geometry<TNum, TConv>.Vector? shift = shift;
    public Geometry<TNum, TConv>.Matrix? Rotate = rotate;

  }

  /// <summary>
  /// Reads transformation parameters from a param reader.
  /// </summary>
  /// <param name="pr">The parameter reader that provides the transformation data.</param>
  /// <returns>A Transform structure containing information about transformations (scale, shift, and rotation).</returns>
  public static Transform ReadTransform(Geometry<TNum, TConv>.ParamReader pr) {
    bool  isScale = pr.ReadBool("Scale");
    TNum? scale   = isScale ? pr.ReadNumber<TNum>("Value") : null;

    bool                          isShift = pr.ReadBool("Shift");
    Geometry<TNum, TConv>.Vector? shift   = isShift ? pr.ReadVector("Vector") : null;

    bool                          isRotate = pr.ReadBool("Rotate");
    Geometry<TNum, TConv>.Matrix? rotate   = null;
    if (isRotate) {
      int dim = pr.ReadNumber<int>("Dim");
      rotate = new Geometry<TNum, TConv>.Matrix(pr.Read2DArray<TNum>("Matrix", dim, dim));
    }

    return new Transform(scale, shift, rotate);
  }

  /// <summary>
  /// Applies the transformation (scaling, shifting, rotation) to a polytope.
  /// The transformations are applied sequentially: scale, then rotation, then shift.
  /// </summary>
  /// <param name="polytop">The polytope to which transformations will be applied.</param>
  /// <param name="tr">The Transform structure containing transformation data.</param>
  /// <param name="dim">The dimensionality of the polytope, necessary for correct application of transformations.</param>
  /// <returns>The polytope after applying the specified transformations.</returns>
  public static Geometry<TNum, TConv>.ConvexPolytop DoTransform(
      Geometry<TNum, TConv>.ConvexPolytop polytop
    , Transform                           tr
    , int                                 dim
    ) {
    if (tr.scale is not null) {
      polytop = polytop.Scale((TNum)tr.scale, Geometry<TNum, TConv>.Vector.Zero(dim));
    }
    if (tr.Rotate is not null) {
      polytop = polytop.Rotate(tr.Rotate);
    }
    if (tr.shift is not null) {
      polytop = polytop.Shift(tr.shift);
    }

    return polytop;
  }
}
