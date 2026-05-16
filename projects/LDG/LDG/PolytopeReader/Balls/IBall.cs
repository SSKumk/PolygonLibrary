namespace LDG;

/// <summary>
/// The interface for reading ball parameters. 
/// </summary>
public interface IBall<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
  /// <summary>
  /// Reads the parameters for the ball from the given parameter reader.
  /// </summary>
  /// <param name="pr">The parameter reader used to read the ball's specific parameters.</param>
  void ReadParameters(Geometry<TNum, TConv>.ParamReader pr);
}
