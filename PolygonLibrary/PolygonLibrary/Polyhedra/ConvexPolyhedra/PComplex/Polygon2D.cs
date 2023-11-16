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

//   public class Polygon2D : ConvexPolytop {

//     public override int               PolytopDim => 2;
//     public override ConvexPolytopType Type       => ConvexPolytopType.Polygon2D;

//     public List<Point> VList;

//     public Polygon2D(HashSet<PolytopSegment> faces, List<Point> Vs) {
//       VList    = Vs;
//       Vertices = new HashSet<Point>(Vs);
//       Faces    = faces.Select(F => (ConvexPolytop)F).ToHashSet();
//       Edges    = new HashSet<ConvexPolytop>(faces.SelectMany(F => F.Faces!));
//       Super    = null;
//     }

//     private static Polygon2D Construct(SubTwoDimensional subPolygon) {
//       Polygon2D polygon2D = new Polygon2D
//         (
//          new HashSet<PolytopSegment>(subPolygon.Faces!.Select(F => new PolytopSegment((SubTwoDimensionalEdge)F)))
//        , new List<Point>(subPolygon.VerticesList.Select(s => s.Original))
//         );
//       AddSuperToFaces(polygon2D);

//       return polygon2D;
//     }

//     public Polygon2D(SubTwoDimensional subPolygon) {
//       Polygon2D P = Construct(subPolygon);
//       VList    = P.VList;
//       Vertices = P.Vertices;
//       Faces    = P.Faces;
//       Edges    = P.Edges;
//       Super    = P.Super;
//     }

//   }

// }
