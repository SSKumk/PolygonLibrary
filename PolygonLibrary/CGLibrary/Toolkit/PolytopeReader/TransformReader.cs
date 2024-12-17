namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
  public class TransformReader {
    public struct Transform(TNum? scale, Vector? shift, Matrix? rotate) {
      public TNum? scale = scale;
      public Vector? shift = shift;
      public Matrix? Rotate = rotate;
    }

    public static Transform ReadTransform(ParamReader pr) {
      bool  isScale = pr.ReadBool("Scale");
      TNum? scale   = isScale ? pr.ReadNumber<TNum>("Value") : null;

      bool    isShift = pr.ReadBool("Shift");
      Vector? shift   = isShift ? pr.ReadVector("Shift") : null;

      bool    isRotate = pr.ReadBool("Rotate");
      Matrix? rotate   = null;
      if (isRotate) {
        int dim = pr.ReadNumber<int>("Dim");
        rotate = new Matrix(pr.Read2DArray<TNum>("Matrix", dim, dim));
      }

      return new Transform(scale, shift, rotate);
    }
    
    public static ConvexPolytop DoTransform(ConvexPolytop polytop, Transform tr, int dim) {
      if (tr.scale is not null) {
        polytop = polytop.Scale((TNum)tr.scale, Vector.Zero(dim));
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
}
