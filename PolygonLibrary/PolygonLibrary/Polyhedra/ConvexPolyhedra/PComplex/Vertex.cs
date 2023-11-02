using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class Vertex : ConvexPolytop {

    public override int PolytopDim => 0;
    public override ConvexPolytopType Type => ConvexPolytopType.Vertex;

    public Point Point => Vertices.First();

    public Vertex(Point v) {
      Faces = null;
      Edges = null;
      Vertices = new HashSet<Point>() { v };
      Super = null;
    }

    public override string ToString() { return Vertices.First().ToString(); }

  }

}
