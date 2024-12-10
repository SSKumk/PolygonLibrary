namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class PolytopeReaderFactory {

    public static ConvexPolytop ReadPolytope(ParamReader pr, out string name) {
      name = pr.ReadString("Name");

      return GetReader(pr).ReadPolytope(pr);
    }

    private static IPolytopeReader GetReader(ParamReader pr) {
      string type = pr.ReadString("Type");

      return type switch
               {
                 "CGLibrary" => new CGLibraryReader()
                 // , "Convex Hull"  => new ConvexHullReader()
                 // , "Hyper Planes" => new HyperPlanesReader()
                 // , "Generator"    => new GeneratorReader()
               , _ => throw new ArgumentException($"Toolkit.PolytopeReader.PolytopeReaderFactory.GetReader: Unknown Type: {type}")
               };
    }

  }

}
