namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class PolytopeReader {

    public static ConvexPolytop Read(ParamReader pr) {
      _ = pr.ReadString("Name");

      return GetReader(pr).ReadPolytope(pr);
    }

    public static IPolytopeReader GetReader(ParamReader pr) {
      string type = pr.ReadString("Type");

      return type switch
               {
                 "CGLibrary" => new CGLibraryReader()
                 // , "Convex Hull"  => new ConvexHullReader()
                 // , "Hyper Planes" => new HyperPlanesReader()
                 // , "Generator"    => new GeneratorReader()
               , _ => throw new ArgumentException($"Toolkit.PolytopeReader.PolytopeReader.GetReader: Unknown Type: {type}")
               };
    }

  }

}
