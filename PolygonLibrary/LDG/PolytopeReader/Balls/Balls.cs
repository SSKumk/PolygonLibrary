namespace LDG;

public class Ball_1<TNum, TConv> : IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr) { } // Ball_1 has no parameters

}

public class Ball_2<TNum, TConv> : IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public int PolarDivision     { get; private set; }
  public int AzimuthsDivisions { get; private set; }

  public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr) {
    AzimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
    PolarDivision     = pr.ReadNumber<int>("PolarDivision");
  }

}

public class Ball_oo<TNum, TConv> : IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr) { } // Ball_oo has no parameters

}
