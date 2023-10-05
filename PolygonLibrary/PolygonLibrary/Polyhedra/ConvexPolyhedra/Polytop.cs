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
             x => {
               Debug.Assert(x.Value.F1.Normal is not null, "x.Value.F1.Normal != null");
               Debug.Assert(x.Value.F2.Normal is not null, "x.Value.F2.Normal != null");


               return new KeyValuePair<Edge, (Face F1, Face F2)>
                 (
                  new Edge(x.Key.OriginalVertices)
                , (new Face(x.Value.F1.OriginalVertices, x.Value.F1.Normal)
                 , new Face(x.Value.F2.OriginalVertices, x.Value.F2.Normal))
                 );
             }
            )
         )
      ) { }

  }

  /// <summary>
  /// Type of permanent storage of fans information.
  /// Dictionary: point --> set of faces incident with a point.
  /// </summary>
  public class FansInfo : Dictionary<Point, HashSet<Face>> {

    public FansInfo(FansInfo fansInfo) : base(fansInfo) { }

    public FansInfo(HashSet<BaseSubCP> Fs) {
      foreach (BaseSubCP F in Fs) {
        foreach (Point vertex in F.OriginalVertices) {
          Debug.Assert(F.Normal is not null, "F.Normal != null");

          if (TryGetValue(vertex, out HashSet<Face>? value)) {
            value.Add(new Face(F.OriginalVertices, F.Normal));
          } else {
            base.Add(vertex, new HashSet<Face>() { new Face(F.OriginalVertices, F.Normal) });
          }
        }
      }
    }

  }

  /// <summary>
  /// Represents an face of the Polytop.
  /// </summary>
  public class Face {

    /// <summary>
    /// Gets the vertices of the face.
    /// </summary>
    public HashSet<Point> Vertices { get; }

    /// <summary>
    /// Gets the normal vector of the face.
    /// </summary>
    public Vector Normal { get; }

    /// <summary>
    /// Initializes a new instance of the Face class.
    /// </summary>
    /// <param name="Vs">The vertices of the face.</param>
    /// <param name="normal">The outward normal vector of the face.</param>
    public Face(IEnumerable<Point> Vs, Vector normal) {
      Vertices = new HashSet<Point>(Vs);
      Normal   = normal;
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

  /// <summary>
  /// Represents an edge of the Polytop.
  /// </summary>
  public class Edge {

    /// <summary>
    /// Gets the vertices of the edge.
    /// </summary>
    public HashSet<Point> Vertices { get; }

    /// <summary>
    /// Initializes a new instance of the Edge class.
    /// </summary>
    /// <param name="Vs">The vertices of the edge.</param>
    public Edge(IEnumerable<Point> Vs) => Vertices = new HashSet<Point>(Vs);

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

  /// <summary>
  /// Represents a full-dimensional convex polytope in a d-dimensional space.
  /// </summary>
  public class Polytop : BaseConvexPolyhedron {

    /// <summary>
    /// Gets the dimension of the polytope.
    /// </summary>
    public override int PolytopDim { get; }

    /// <summary>
    /// Gets the type of the convex polytope.
    /// </summary>
    public override ConvexPolyhedronType Type { get; }

    /// <summary>
    /// Gets the set of vertices of the polytope.
    /// </summary>
    public override HashSet<Point> Vertices { get; }

    /// <summary>
    /// Gets the faces of the polytope.
    /// </summary>
    public HashSet<Face> Faces { get; }

    /// <summary>
    /// Gets the set of edges of the polytope.
    /// </summary>
    public HashSet<Edge> Edges { get; }

    /// <summary>
    /// Gets the incidence information of the faces. Each edge is associated with a pair of incidence faces with it.
    /// </summary>
    public IncidenceInfo FaceIncidence { get; }

    /// <summary>
    /// Gets the fan information of the polytope. Each point is associated with a set of incidence faces with it.
    /// </summary>
    public FansInfo Fans { get; }

    /// <summary>
    /// Initializes a new instance of the Polytop class.
    /// </summary>
    /// <param name="Vs">The vertices of the polytope.</param>
    /// <param name="polytopDim">The dimension of the polytope.</param>
    /// <param name="faces">The faces of the polytope.</param>
    /// <param name="edges">The edges of the polytope.</param>
    /// <param name="type">The type of the polytope.</param>
    /// <param name="faceIncidence">The incidence information of the faces.</param>
    /// <param name="fans">The fan information of the polytope.</param>
    public Polytop(IEnumerable<Point>   Vs
                 , int                  polytopDim
                 , IEnumerable<Face>    faces
                 , IEnumerable<Edge>    edges
                 , ConvexPolyhedronType type
                 , IncidenceInfo        faceIncidence
                 , FansInfo             fans) {
      Vertices      = new HashSet<Point>(Vs);
      PolytopDim    = polytopDim;
      Faces         = new HashSet<Face>(faces);
      Edges         = new HashSet<Edge>(edges);
      Type          = type;
      FaceIncidence = faceIncidence;
      Fans          = fans;
    }

    /// <summary>
    /// Converts the polytope to a convex polygon.
    /// </summary>
    /// <param name="basis">The affine basis used for the conversion.</param>
    /// <returns>A convex polygon representing the polytope in 2D space.</returns>
    public ConvexPolygon ToConvexPolygon(AffineBasis basis) {
      if (PolytopDim != 2) {
        throw new ArgumentException($"The dimension of the polygon must equal to 2. Found = {PolytopDim}.");
      }

      return new ConvexPolygon(basis.ProjectPoints(Vertices));
    }

    public void WriteTXT(string filePath) {
      using (StreamWriter writer = new StreamWriter(filePath)) {
        writer.Write("Type: ");
        switch (Type) {
          case ConvexPolyhedronType.Simplex:
            writer.WriteLine("Simplex");

            break;
          case ConvexPolyhedronType.NonSimplex:
            writer.WriteLine("NoNSimplex");

            break;
          case ConvexPolyhedronType.TwoDimensional:
            writer.WriteLine("TwoDimensional");

            break;
        }
        writer.WriteLine($"PDim: {PolytopDim}");
        writer.WriteLine($"SDim: {SpaceDim}");
        writer.WriteLine();
        writer.WriteLine($"Vertices: {Vertices.Count}");
        writer.WriteLine(string.Join('\n', Vertices.Select(v => v.ToFileFormat())));
        writer.WriteLine();
        writer.WriteLine($"Faces: {Faces.Count}");
        foreach (Face face in Faces) {
          writer.WriteLine($"N: {face.Normal.ToFileFormat()}");
          writer.WriteLine(string.Join('\n', face.Vertices.Select(v => v.ToFileFormat())));
        }
      }
    }

  }

}