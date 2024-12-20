namespace LDG;

public class CGLibraryReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr)
    => Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(pr);

}

public class ConvexHullReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr) {
    List<Geometry<TNum, TConv>.Vector> vs = new List<Geometry<TNum, TConv>.Vector>();

    bool doRed = pr.ReadBool("DoRed");
    int  qnt   = pr.ReadNumber<int>("VsQnt");
    int  d     = pr.ReadNumber<int>("VsDim");
    for (int i = 0; i < qnt; i++) {
      vs.Add(new Geometry<TNum, TConv>.Vector(pr.ReadNumberLine(d)));
    }


    return Geometry<TNum, TConv>.ConvexPolytop.CreateFromPoints(vs, doRed);
  }

}

public class HyperPlanesReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr) {
    List<Geometry<TNum, TConv>.HyperPlane> HPs = new List<Geometry<TNum, TConv>.HyperPlane>();

    bool doRed = pr.ReadBool("DoRed");
    int  qnt   = pr.ReadNumber<int>("HPsQnt");
    int  d     = pr.ReadNumber<int>("HPsDim");
    int  d1    = d + 1;
    for (int i = 0; i < qnt; i++) {
      TNum[] hp = pr.ReadNumberLine(d1);
      TNum[] v  = hp[..d];
      HPs.Add(new Geometry<TNum, TConv>.HyperPlane(new Geometry<TNum, TConv>.Vector(v, false), hp[^1]));
    }

    return Geometry<TNum, TConv>.ConvexPolytop.CreateFromHalfSpaces(HPs, doRed);
  }

}

public class GeneratorReader<TNum, TConv> : IPolytopeReader<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum, TConv>.ConvexPolytop ReadPolytope(Geometry<TNum, TConv>.ParamReader pr) {
    string genType = pr.ReadString("GeneratorType");

    switch (genType) {
      case "RectAxisParallel": {
        Geometry<TNum, TConv>.Vector left  = pr.ReadVector("Left");
        Geometry<TNum, TConv>.Vector right = pr.ReadVector("Right");

        return Geometry<TNum, TConv>.ConvexPolytop.RectAxisParallel(left, right);
      }
      case "Sphere": {
        int dim               = pr.ReadNumber<int>("Dim");
        int azimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
        int polarDivision     = dim > 2 ? pr.ReadNumber<int>("PolarDivision") : 0;

        return Geometry<TNum, TConv>.ConvexPolytop.Sphere
          (Geometry<TNum, TConv>.Vector.Zero(dim), Geometry<TNum, TConv>.Tools.One, polarDivision, azimuthsDivisions);
      }
      case "Ellipsoid": {
        int                          dim               = pr.ReadNumber<int>("Dim");
        Geometry<TNum, TConv>.Vector semiAxis          = pr.ReadVector("SemiAxis");
        int                          azimuthsDivisions = pr.ReadNumber<int>("AzimuthsDivisions");
        int                          polarDivision     = dim > 2 ? pr.ReadNumber<int>("PolarDivision") : 0;

        return Geometry<TNum, TConv>.ConvexPolytop.Ellipsoid
          (polarDivision, azimuthsDivisions, Geometry<TNum, TConv>.Vector.Zero(dim), semiAxis);
      }
    }

    throw new ArgumentException($"PolytopeReaders.Generator: The Type = {genType} is unknown!");
  }

}
