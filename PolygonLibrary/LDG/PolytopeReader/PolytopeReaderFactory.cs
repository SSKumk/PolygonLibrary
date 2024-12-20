namespace LDG;

public abstract class PolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public static Geometry<TNum, TConv>.ConvexPolytop Read(Geometry<TNum, TConv>.ParamReader pr) {
    _ = pr.ReadString("Name");

    return GetReader(pr).ReadPolytope(pr);
  }

  private static IPolytopeReader<TNum, TConv> GetReader(Geometry<TNum, TConv>.ParamReader pr) {
    string type = pr.ReadString("Type");

    return type switch
             {
               "CGLibrary" => new CGLibraryReader<TNum, TConv>()
             , "Convex Hull" => new ConvexHullReader<TNum, TConv>()
             , "Hyper Planes" => new HyperPlanesReader<TNum, TConv>()
             , "Generator" => new GeneratorReader<TNum, TConv>()
             , _ => throw new ArgumentException($"Toolkit.PolytopeReader.PolytopeReader.GetReader: Unknown Type: {type}")
             };
  }

}
