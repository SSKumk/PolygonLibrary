namespace Bridges;

public class TerminalSet_Epigraph<TNum, TConv> : TerminalSetBase<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public TerminalSet_Epigraph(Geometry<TNum, TConv>.ParamReader pr, Geometry<TNum, TConv>.GameData gameData, string tsInfo) : base
    (pr, gameData, tsInfo) {


    terminalSetInfo += tsInfo;
  }

}
