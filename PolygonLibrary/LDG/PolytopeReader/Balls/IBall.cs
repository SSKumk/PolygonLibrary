namespace LDG;




  public interface IBall<TNum, TConv>
    where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
    IFloatingPoint<TNum>, IFormattable
    where TConv : INumConvertor<TNum> {
    /// <summary>
    /// Reads the specific parameters for this type of ball from the given parameter reader.
    /// </summary>
    /// <param name="pr">The parameter reader.</param>
    void ReadParameters(Geometry<TNum, TConv>.ParamReader pr);
  }
