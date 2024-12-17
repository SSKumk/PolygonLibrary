namespace Bridges.LevelSet;

public interface ILvlSetType<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  void ReadParameters(Geometry<TNum,TConv>.ParamReader pr, LDGPathHolder<TNum,TConv> ph);

}
