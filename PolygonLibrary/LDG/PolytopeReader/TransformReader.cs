namespace LDG;

public class TransformReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public struct Transform(TNum? scale, Geometry<TNum, TConv>.Vector? shift, Geometry<TNum, TConv>.Matrix? rotate) {

    public TNum?                         scale  = scale;
    public Geometry<TNum, TConv>.Vector? shift  = shift;
    public Geometry<TNum, TConv>.Matrix? Rotate = rotate;

  }

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
