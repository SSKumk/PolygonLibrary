namespace LDG;

public interface IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr);

}
