namespace Bridges;

public class ExplicitTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> ReadTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>         dh
    ) {
    int      k     = pr.ReadNumber<int>("Qnt");
    string[] names = pr.Read1DArray<string>("Polytopes", k);
    for (int i = 0; i < k; i++) {
      yield return Geometry<TNum, TConv>.PolytopeReader.Read(dh.OpenPolytopeReader(names[i]));
    }
  }

}
