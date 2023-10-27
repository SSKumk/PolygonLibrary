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

  public class Polytop : ConvexPolytop {

    public override ConvexPolytopType Type => ConvexPolytopType.Polytop;

    private         int? _polytopDim = null;
    public override int  PolytopDim => _polytopDim ??= Faces!.First().PolytopDim + 1;

    public Polytop(HashSet<ConvexPolytop> faces) {
      Faces    = faces;
      Vertices = new HashSet<Point>(faces.SelectMany(F => F.Vertices));
      Edges    = new HashSet<ConvexPolytop>(faces.SelectMany(F => F.Faces!));
      Super    = null;
    }

    private static ConvexPolytop Construct(BaseSubCP BCP) {
      if (BCP is SubTwoDimensional stwo) {
        return new Polygon2D(stwo);
      }
      HashSet<ConvexPolytop> faces = new HashSet<ConvexPolytop>();
      foreach (ConvexPolytop? Face in BCP.Faces!.Select(Construct)) {
        faces.Add(Face);
      }

      return new Polytop(faces);
    }

    private Polytop(BaseSubCP sP) {
      if (sP is not SubSimplex && sP is not SubNonSimplex) {
        throw new ArgumentException("Polytop must be of type SubSimplex or SubNonSimplex.");
      }
      Polytop P = (Polytop)Construct(sP);
      Faces    = P.Faces;
      Vertices = P.Vertices;
      Edges    = P.Edges;
      AddSuperToFaces(P);
      Super    = P.Super;
    }

    public Polytop(SubNonSimplex sP) : this((BaseSubCP)sP) { }
    public Polytop(SubSimplex    sP) : this((BaseSubCP)sP) { }

  }

}
