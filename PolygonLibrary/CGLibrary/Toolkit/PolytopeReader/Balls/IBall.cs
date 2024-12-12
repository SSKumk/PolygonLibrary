namespace CGLibrary;


public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public interface IBall {
    /// <summary>
    /// Reads the specific parameters for this type of ball from the given parameter reader.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    void ReadParameters(ParamReader pr);
  }
}