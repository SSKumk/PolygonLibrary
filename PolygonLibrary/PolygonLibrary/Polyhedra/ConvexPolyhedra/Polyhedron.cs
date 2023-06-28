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
    // Basis    = new AffineBasis(affineBasis);
    Basis = affineBasis;
  }

  /// <summary>
  /// Determines whether the specified object is equal to face.
  /// Two faces are equal if they have same sets of their vertices. 
  /// </summary>
  /// <param name="obj">The object to compare with face.</param>
  /// <returns>True if the specified object is equal to face, False otherwise</returns>
  public override bool Equals(object? obj) {
    if (obj == null || this.GetType() != obj.GetType()) {
      return false;
    }

    Face other = (Face)obj;

    return this.Vertices.SetEquals(other.Vertices);
  }

  /// <summary>
  /// Internal field for the hash of the face
  /// </summary>
  private int? _hash = null;

  /// <summary>
  /// Returns a hash code for the face based on specified set of vertices.
  /// </summary>
  /// <returns>A hash code for the specified set of vertices.</returns>
  public override int GetHashCode() {
    if (_hash is null) {
      int hash = 0;

      foreach (Point vertex in Vertices.OrderBy(v => v)) {
        hash = HashCode.Combine(hash, vertex.GetHashCode());
      }
      _hash = hash;
    }

    return _hash.Value;
  }

}

public class Edge {

  public HashSet<Point> Vertices { get; }

  public Edge(IEnumerable<Point> Vs) { Vertices = new HashSet<Point>(Vs); }

  /// <summary>
  /// Determines whether the specified object is equal to edge.
  /// Two edges are equal if they have same sets of their vertices. 
  /// </summary>
  /// <param name="obj">The object to compare with edge.</param>
  /// <returns>True if the specified object is equal to edge, False otherwise</returns>
  public override bool Equals(object? obj) {
    if (obj == null || this.GetType() != obj.GetType()) {
      return false;
    }

    Edge other = (Edge)obj;

    return this.Vertices.SetEquals(other.Vertices);
  }

  /// <summary>
  /// Internal field for the hash of the edge
  /// </summary>
  private int? _hash = null;

  /// <summary>
  /// Returns a hash code for the edge based on specified set of vertices.
  /// </summary>
  /// <returns>A hash code for the specified set of vertices.</returns>
  public override int GetHashCode() {
    if (_hash is null) {
      int hash = 0;

      foreach (Point vertex in Vertices.OrderBy(v => v)) {
        hash = HashCode.Combine(hash, vertex.GetHashCode());
      }
      _hash = hash;
    }

    return _hash.Value;
  }

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


  public Polyhedron(IEnumerable<Point>   Vs
                  , int                  polyhedronDim
                  , IEnumerable<Face>    faces
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
    // Basis         = new AffineBasis(basis);
    Basis         = basis;
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