namespace Bridges;

public class Minkowski<TNum, TConv> : ITerminalSetReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public IEnumerable<Geometry<TNum, TConv>.ConvexPolytop>
    ReadTerminalSets(Geometry<TNum, TConv>.ParamReader pr
                   , LDGPathHolder<TNum, TConv>        dh
                   , Geometry<TNum, TConv>.GameData    gd) {
    Geometry<TNum,TConv>.ConvexPolytop polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Name"), dh);

    // todo: here -- shift, scale, rotate
    TNum[] ks = pr.ReadVector("Constants").GetAsArray();

    throw new NotImplementedException("сначала надо подвигать многогранник, а потом уже увеличивать на переданные константы");

    // return ks.Select(k => polytope.Scale(k, ));

  }
}
