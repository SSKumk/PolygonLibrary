namespace Bridges;

public interface ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public static Geometry<TNum,TConv>.ConvexPolytop DoPolytope(string name, LDGPathHolder<TNum,TConv> ph) {
    return Geometry<TNum, TConv>.PolytopeReader.Read(ph.OpenPolytopeReader(name));
  }

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> ReadTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph);

}
