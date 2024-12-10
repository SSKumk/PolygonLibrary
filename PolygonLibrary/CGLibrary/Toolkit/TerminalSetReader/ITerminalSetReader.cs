namespace CGLibrary;
public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public interface ITerminalSetReader {

    public IEnumerable<ConvexPolytop> ReadTerminalSets(ParamReader pr, Dictionary<string,string> name2Pol);

  }
}
