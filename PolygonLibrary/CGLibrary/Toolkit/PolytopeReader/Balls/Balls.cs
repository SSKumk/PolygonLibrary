namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
  
  public class Ball_1 : IBall {
    public void ReadParameters(ParamReader pr) { }    // Ball_1 has no parameters
  }

  public class Ball_2 : IBall {
    public int PolarDivision { get; private set; }
    public int AzimuthsDivisions { get; private set; }

    public void ReadParameters(ParamReader pr) {
      PolarDivision    = pr.ReadNumber<int>("Polar");
      AzimuthsDivisions = pr.ReadNumber<int>("Azimuths");
    }
  }

  public class Ball_oo : IBall {
    public void ReadParameters(ParamReader pr) { }  // Ball_oo has no parameters
  }
}
