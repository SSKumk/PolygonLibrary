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

      bool doRed = pr.ReadString("DoRed") == "True"; //todo: <---!
      int  qnt   = pr.ReadNumber<int>("VsQnt");
      int  d     = pr.ReadNumber<int>("VsDim");
      for (int i = 0; i < qnt; i++) {
        vs.Add(new Vector(pr.ReadNumberLine(d)));
      }

      return ConvexPolytop.CreateFromPoints(vs, doRed);
    }
  }

  public class HyperPlanes : IPolytopeReader {
    public ConvexPolytop ReadPolytope(ParamReader pr) {
      List<HyperPlane> HPs = new List<HyperPlane>();

      bool doRed = pr.ReadString("DoRed") == "True"; //todo: <---!
      int  qnt   = pr.ReadNumber<int>("HPsQnt");
      int  d     = pr.ReadNumber<int>("HPsDim");
      int  d1    = d++;
      for (int i = 0; i < qnt; i++) {
        TNum[] hp = pr.ReadNumberLine(d1);
        TNum[] v  = hp[..d];
        HPs.Add(new HyperPlane(new Vector(v, false), hp[^1]));
      }
      
      return ConvexPolytop.CreateFromHalfSpaces(HPs,doRed);
    }
  }


}
