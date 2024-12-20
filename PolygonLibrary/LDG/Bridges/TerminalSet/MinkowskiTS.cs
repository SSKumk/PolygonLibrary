namespace LDG;

public class MinkowskiTS<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop> ReadTerminalSets(
      Geometry<TNum, TConv>.ParamReader pr
    , LDGPathHolder<TNum, TConv>        ph
    ) {
    Geometry<TNum, TConv>.ConvexPolytop polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Polytope"), ph);

    TNum[] ks = pr.ReadVector("Constants").GetAsArray();


    return ks.Select(k => polytope.Scale(k, polytope.InnerPoint));
  }

}
