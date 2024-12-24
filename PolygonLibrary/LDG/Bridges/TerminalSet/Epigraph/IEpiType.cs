namespace LDG;

/// <summary>
/// An interface for reading parameters of the terminal sets of the epigraph type. 
/// </summary>
public interface IEpiType<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum>  {

  /// <summary>
  ///Reads the parameters required for constructing terminal sets from the corresponding epigraph function.    
  /// </summary>
  /// <param name="pr">The parameter reader of the terminal set to read from.</param>
  /// <param name="ph">The path holder providing path to an all-game files. It used only for types involving polytopes.</param>
  void ReadParameters(Geometry<TNum,TConv>.ParamReader pr, LDGPathHolder<TNum,TConv> ph);
}
