namespace Bridges;

public class ExplicitTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> ReadTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph) {
    int      k     = pr.ReadNumber<int>("Qnt");
    string[] names = pr.Read1DArray<string>("Polytopes", k);
    // todo: here -- shift, scale, rotate

    return names.Select(name => ITerminalSetReader<TNum, TConv>.DoPolytope(name, ph));

  }

}
