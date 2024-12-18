namespace Bridges.LevelSet;

public class LvlSetTypes<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {
  
  public class DistToPoint : ILvlSetType<TNum,TConv> {

    public Geometry<TNum,TConv>.Vector Point { get; private set; } = null;

    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
      Point = pr.ReadVector("Point");
    }
  }
  
  public class DistToPolytope : ILvlSetType<TNum, TConv> {
    public Geometry<TNum, TConv>.ConvexPolytop Polytope { get; private set; } = null!;


    public void ReadParameters(Geometry<TNum, TConv>.ParamReader pr, LDGPathHolder<TNum, TConv> ph) {
      Polytope = ITerminalSetReader<TNum, TConv>.DoPolytope(pr.ReadString("Polytope"), ph);
    }
  }
}
