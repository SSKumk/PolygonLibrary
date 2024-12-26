namespace LDG;

public interface IController<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum,TConv>.Vector Control(TNum t, Geometry<TNum,TConv>.Vector x);

}
