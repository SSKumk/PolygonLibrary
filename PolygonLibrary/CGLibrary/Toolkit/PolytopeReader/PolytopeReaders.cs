namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class CGLibraryReader : IPolytopeReader {

    public ConvexPolytop ReadPolytope(ParamReader pr) => ConvexPolytop.CreateFromReader(pr);

  }

  public class ConvexHullReader : IPolytopeReader {

    public ConvexPolytop ReadPolytope(ParamReader pr) {
      List<Vector> vs = new List<Vector>();

      bool doRed = pr.ReadBool("DoRed");
      int  qnt   = pr.ReadNumber<int>("VsQnt");
      int  d     = pr.ReadNumber<int>("VsDim");
      for (int i = 0; i < qnt; i++) {
        vs.Add(new Vector(pr.ReadNumberLine(d)));
      }

      return ConvexPolytop.CreateFromPoints(vs, doRed);
    }
  }

  public class HyperPlanesReader : IPolytopeReader {
    public ConvexPolytop ReadPolytope(ParamReader pr) {
      List<HyperPlane> HPs = new List<HyperPlane>();

      bool doRed = pr.ReadBool("DoRed");
      int  qnt   = pr.ReadNumber<int>("HPsQnt");
      int  d     = pr.ReadNumber<int>("HPsDim");
      int  d1    = d + 1;
      for (int i = 0; i < qnt; i++) {
        TNum[] hp = pr.ReadNumberLine(d1);
        TNum[] v  = hp[..d];
        HPs.Add(new HyperPlane(new Vector(v, false), hp[^1]));
      }
      
      return ConvexPolytop.CreateFromHalfSpaces(HPs,doRed);
    }
  }
  
  public class GeneratorReader : IPolytopeReader {

    
    public ConvexPolytop ReadPolytope(ParamReader pr) {
      string genType = pr.ReadString("GeneratorType");

      switch (genType) {
        case "RectAxisParallel": {
          Vector left = pr.ReadVector("Left");
          Vector right = pr.ReadVector("Right");
          return ConvexPolytop.RectAxisParallel(left, right);
        }
        case "Sphere": {
          int dim = pr.ReadNumber<int>("Dim");
          int azimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
          int polarDivision = dim > 2 ? pr.ReadNumber<int>("PolarDivision") : 0;

          return ConvexPolytop.Sphere(Vector.Zero(dim), Tools.One, polarDivision, azimuthsDivisions);
        }
        case "Ellipsoid": {
          int    dim               = pr.ReadNumber<int>("Dim");
          Vector semiAxis              = pr.ReadVector("SemiAxis");
          int    azimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
          int    polarDivision     = dim > 2 ? pr.ReadNumber<int>("PolarDivision") : 0;
          
          return ConvexPolytop.Ellipsoid(polarDivision,azimuthsDivisions,Vector.Zero(dim),semiAxis );
        }
      }

      throw new ArgumentException($"PolytopeReaders.Generator: The Type = {genType} is unknown!");
    }
  }


}
