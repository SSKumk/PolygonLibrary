namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class StableBridge : SortedDictionary<TNum, ConvexPolytop> {

    public StableBridge(IComparer<TNum> comparer) : base(comparer) { }

    public void WriteBr() {

    }

  }

}
