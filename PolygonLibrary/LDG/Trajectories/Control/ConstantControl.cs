namespace LDG;

public class ConstantControl<TNum, TConv> : IController<TNum,TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum, TConv>.Vector Control(TNum t, Geometry<TNum, TConv>.Vector x) {
    throw new NotImplementedException();
  }

}
