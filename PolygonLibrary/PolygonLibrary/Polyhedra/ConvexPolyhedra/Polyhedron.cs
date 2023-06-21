using System;
using System.Linq;
using System.Collections.Generic;
using PolygonLibrary.Basics;
using PolygonLibrary.Polygons.ConvexPolygons;
using PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra;

/// <summary>
/// Type of permanent storage of face incidence information.
/// For each pair (F1, F2) of incident faces, it is assumed that HashCode(F1) is less or equal than HashCode(F2)
/// </summary>
public class IncidenceInfo : Dictionary<Edge, (Face F1, Face F2)> {

  public IncidenceInfo(IncidenceInfo incid) : base(incid) { }

  public IncidenceInfo(SubIncidenceInfo info) : base
    (
     new Dictionary<Edge, (Face F1, Face F2)>
       (
        info.Select
          (
           x => new KeyValuePair<Edge, (Face F1, Face F2)>
             (
              new Edge(x.Key.OriginalVertices)
            , (new Face(x.Value.F1.OriginalVertices, x.Value.F1.Basis), new Face(x.Value.F2.OriginalVertices, x.Value.F2.Basis))
             )
          )
       )
    ) { }

  /// <summary>
  /// Base constructor
  /// </summary>
  public IncidenceInfo() { }

}

public class FansInfo : Dictionary<Point, HashSet<Face>> {

  public FansInfo(FansInfo fansInfo) : base(fansInfo) { }

  public FansInfo(HashSet<BaseSubCP> Fs) {
    foreach (BaseSubCP F in Fs) {
      foreach (Point vertex in F.OriginalVertices) {
        if (TryGetValue(vertex, out HashSet<Face>? value)) {
          value.Add(new Face(F.OriginalVertices, F.Basis));
        } else {
          base.Add(vertex, new HashSet<Face>() { new Face(F.OriginalVertices, F.Basis) });
        }
      }
    }
  }

}

public class Face {

  public HashSet<Point> Vertices { get; }

  public AffineBasis Basis { get; }

  public Vector? Normal = null;

  public Face(IEnumerable<Point> Vs, AffineBasis affineBasis) {
    Vertices = new HashSet<Point>(Vs);
    Basis    = new AffineBasis(affineBasis);
  }

}

public class Edge {

  public HashSet<Point> Vertices { get; }

  public Edge(IEnumerable<Point> Vs) { Vertices = new HashSet<Point>(Vs); }

}

public class Polyhedron : BaseConvexPolyhedron {

  /// <summary>
  /// Gets the dimension of the polyhedron.
  /// </summary>
  public override int PolyhedronDim { get; }

  /// <summary>
  /// Gets the type of the convex polyhedron.
  /// </summary>
  public override ConvexPolyhedronType Type { get; }

  /// <summary>
  /// Gets the set of vertices of the polyhedron.
  /// </summary>
  public override HashSet<Point> Vertices { get; }

  /// <summary>
  /// Gets the affine basis of polyhedron.
  /// </summary>
  public override AffineBasis Basis { get; }

  /// <summary>
  /// Gets the faces of polyhedron
  /// </summary>
  public HashSet<Face> Faces { get; }

  /// <summary>
  /// Gets the set of the edges of polyhedron.
  /// </summary>
  public HashSet<Edge> Edges { get; }

  public IncidenceInfo FaceIncidence { get; }

  public FansInfo Fans { get; }


  public Polyhedron(IEnumerable<Point> Vs
                  , int                polyhedronDim
                  , IEnumerable<Face>  faces
                    , IEnumerable<Edge>    edges
                  , ConvexPolyhedronType type
                  , AffineBasis          basis
                  , IncidenceInfo        faceIncidence
                  , FansInfo             fans) {
    Vertices      = new HashSet<Point>(Vs);
    PolyhedronDim = polyhedronDim;
    Faces         = new HashSet<Face>(faces);
    Edges         = new HashSet<Edge>(edges);
    Type          = type;
    Basis         = new AffineBasis(basis);
    FaceIncidence = new IncidenceInfo(faceIncidence);
    Fans          = new FansInfo(fans);
  }

  public ConvexPolygon ToConvexPolygon(AffineBasis? basis = null) {
#if DEBUG
    if (PolyhedronDim != 2) {
      throw new ArgumentException($"The dimension of the polygon must equal to 2. Found = {PolyhedronDim}.");
    }
#endif

    if (basis is null) {
      return new ConvexPolygon(Basis.ProjectPoints(Vertices));
    } else {
      return new ConvexPolygon(basis.ProjectPoints(Vertices));
    }
  }

}
