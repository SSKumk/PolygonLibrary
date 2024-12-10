namespace CGLibrary;
public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class ExplicitTerminalSet : ITerminalSetReader {

    public IEnumerable<ConvexPolytop> ReadTerminalSets(ParamReader pr, Dictionary<string,string> name2Pol) {
      int      k     = pr.ReadNumber<int>("Qnt");
      string[] names = pr.Read1DArray<string>("Polytopes", k);
      for (int i = 0; i < k; i++) {
        yield return PolytopeReaderFactory.ReadPolytope();
      }
    }
  }
}
