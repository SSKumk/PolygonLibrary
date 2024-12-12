namespace Bridges.EpigraphType;

public abstract class EpiTypes<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class DistToPoint : IEpiType<TNum, TConv> {
    public Geometry<TNum, TConv>.Vector Point { get; private set; } = null!;

    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> dh) {
      Point = pr.ReadVector("Point");
    }
  }

  public class DistToPolytope : IEpiType<TNum, TConv> {
    public Geometry<TNum, TConv>.ConvexPolytop Polytope { get; private set; } = null!;


    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> dh) {
      Polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Name"), dh);
    }
  }

}
