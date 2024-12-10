namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class CGLibraryReader : IPolytopeReader {

    public ConvexPolytop ReadPolytope(ParamReader pr) { throw new NotImplementedException(); }

  }


}