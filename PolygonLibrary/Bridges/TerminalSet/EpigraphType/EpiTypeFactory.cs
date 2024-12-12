namespace Bridges.EpigraphType;

public class EpiTypeFactory<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public static IEpiType<TNum,TConv> Read(Geometry<TNum,TConv>.ParamReader pr, LDGPathHolder<TNum,TConv> dh) {
    string epiType = pr.ReadString("Type");
    IEpiType<TNum,TConv> epigraph =
      epiType switch
        {
          "DistToPoint"    => new EpiTypes<TNum,TConv>.DistToPoint()
        , "DistToPolytope" => new EpiTypes<TNum,TConv>.DistToPolytope()
        , _ => throw new ArgumentException
                 (
                  $"Unsupported epigraph type: '{epiType}'.\nIn file {pr.filePath}\n" +
                  $"Please refer to the documentation for supported types."
                 )
        };
    
    epigraph.ReadParameters(pr, dh);
    return epigraph;
  }
}
