// using System;
// using System.Linq;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Numerics;

// namespace CGLibrary;

// public partial class Geometry<TNum, TConv>
//   where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
//   IFloatingPoint<TNum>, IFormattable
//   where TConv : INumConvertor<TNum> {

//   public class PolytopSegment : ConvexPolytop {

//     public override int               PolytopDim => 1;
//     public override ConvexPolytopType Type       => ConvexPolytopType.PolytopSegment;

//     public Vertex fst => (Vertex)Faces!.First();
//     public Vertex snd => (Vertex)Faces!.Last();

//     public PolytopSegment(Vertex fst, Vertex snd) {
//       Edges    = null;
//       Faces    = new HashSet<ConvexPolytop>() { fst, snd };
//       Vertices = new HashSet<Point>() { fst.Vertices.First(), snd.Vertices.First() };
//       Super    = null;
//     }

//     private static PolytopSegment Construct(SubTwoDimensionalEdge subSegment) {
//       PolytopSegment seg = new PolytopSegment(new Vertex(subSegment.OriginalVertices.First())
//                                             , new Vertex(subSegment.OriginalVertices.Last()));
//       AddSuperToFaces(seg);

//       return seg;
//     }


//     public PolytopSegment(SubTwoDimensionalEdge subSegment) {
//       PolytopSegment seg = Construct(subSegment);
//       Edges    = seg.Edges;
//       Faces    = seg.Faces;
//       Vertices = seg.Vertices;
//       Super    = seg.Super;
//     }

//   }

// }
