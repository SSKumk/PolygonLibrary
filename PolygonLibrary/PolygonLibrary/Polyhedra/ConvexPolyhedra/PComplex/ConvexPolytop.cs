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
               Debug.Assert(x.Value.F1.Normal is not null, "The normal to F1 is null!");
               Debug.Assert(x.Value.F2.Normal is not null, "The normal to F2 is null!");


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
          Debug.Assert(F.Normal is not null, "The normal to F is null!");

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
    public Face(IEnumerable<Point> Vs, Vector normal) { //todo может IEnumerable --> HashSet и тогда ещё меньше ссылок будет ??
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
  /// <para><b>Simplex</b> - the polytop is a simplex.</para>
  /// <b>Polytop</b> - the polytop is a complex structure.
  /// <para><b>Polygon2D</b> - the polytop is a 2D-plane polygon.</para>
  /// <b>Segment</b> - the polytop is a 1D-segment.
  /// <para><b>Vertex</b> - the polytop is a 0D-vertex.</para>
  /// </summary>
  public enum ConvexPolytopType {

    Polytop
  , Polygon2D
  , PolytopSegment
  , Vertex

  }


  /// <summary>
  /// Represents a full-dimensional convex polytop complex in a d-dimensional space.
  /// </summary>
  public abstract class ConvexPolytop {

    /// <summary>
    /// Gets the type of the convex polytope.
    /// </summary>
    public abstract ConvexPolytopType Type { get; }

    /// <summary>
    /// Gets the dimension of the polytope.
    /// </summary>
    public abstract int PolytopDim { get; }

    // /// <summary>
    // /// The node of the face lattice to which this polytop belongs.
    // /// If the node is null, polytop is considered selfish.
    // /// </summary>
    // public FaceLatticeNode? LatticeNode { get; set; }

    /// <summary>
    /// Gets the dimension of the space in which the polytop is treated.
    /// </summary>
    public int SpaceDim => Vertices.First().Dim;

    /// <summary>
    /// Gets the set of vertices of the polytope.
    /// </summary>
    public HashSet<Point> Vertices { get; protected init; }

    /// <summary>
    /// Gets the faces of the polytope.
    /// </summary>
    public HashSet<ConvexPolytop>? Faces { get; protected init; }

    public HashSet<ConvexPolytop>? Super { get; set; }

    protected static void AddSuperToFaces(ConvexPolytop P) {
      foreach (ConvexPolytop F in P.Faces!) {
        if (F.Super is null) {
          F.Super = new HashSet<ConvexPolytop>() { P };
        } else {
          F.Super.Add(P);
        }
      }
    }

    /// <summary>
    /// Gets the set of edges of the polytope.
    /// </summary>
    public HashSet<ConvexPolytop>? Edges { get; protected init; }

    public static ConvexPolytop ConstructFromSubCP(BaseSubCP BCP) {
      switch (BCP.Type) {
        case SubCPType.Simplex:        return new Polytop((BCP as SubSimplex)!);
        case SubCPType.NonSimplex:     return new Polytop((BCP as SubNonSimplex)!);
        case SubCPType.TwoDimensional: return new Polygon2D((BCP as SubTwoDimensional)!);
        case SubCPType.OneDimensional: return new PolytopSegment((BCP as SubTwoDimensionalEdge)!);
        default:                       throw new ArgumentOutOfRangeException(nameof(BCP));
      }
    }
    // /// <summary>
    // /// Gets the incidence information of the faces. Each edge is associated with a pair of incidence faces with it.
    // /// </summary>
    // public IncidenceInfo FaceIncidence { get; }

    // /// <summary>
    // /// Gets the fan information of the polytope. Each point is associated with a set of incidence faces with it.
    // /// </summary>
    // public FansInfo Fans { get; }

    // /// <summary>
    // /// Initializes a new instance of the Polytop class.
    // /// </summary>
    // /// <param name="Vs">The vertices of the polytope.</param>
    // public ConvexPolytop(IEnumerable<Point> Vs) {
    //   Vertices = new HashSet<Point>(Vs);
    // }
    //
    // public ConvexPolytop(){}

    /// <summary>
    /// Determines whether the specified object is equal to convex polytop.
    /// Two polyhedra are equal if they have same dimensions and sets of their vertices are equal.
    /// </summary>
    /// <param name="obj">The object to compare with convex polytop.</param>
    /// <returns>True if the specified object is equal to convex polytop, False otherwise</returns>
    public override bool Equals(object? obj) {
      if (obj == null || this.GetType() != obj.GetType()) {
        return false;
      }

      ConvexPolytop other = (ConvexPolytop)obj;

      if (this.PolytopDim != other.PolytopDim) {
        return false;
      }

      return this.Vertices.SetEquals(other.Vertices);
    }


    /// <summary>
    /// Internal field for the hash of the polytop
    /// </summary>
    private int? _hash = null;

    /// <summary>
    /// Returns a hash code for the convex polytop based on specified set of vertices and dimension.
    /// </summary>
    /// <returns>A hash code for the specified set of vertices and dimension.</returns>
    public override int GetHashCode() {
      if (_hash is null) {
        int hash = 0;

        if (Type == ConvexPolytopType.Vertex) {
          hash = HashCode.Combine(hash, Vertices.First().GetHashCode());
          // Console.WriteLine($"Calc hash for vertex: {Vertices.First()}");
        } else {
          foreach (ConvexPolytop P in Faces.OrderBy(F => F.GetHashCode())) {
            hash = HashCode.Combine(hash, P.GetHashCode());
          }
          // Console.WriteLine($"Calc hash for Face: {PolytopDim}");

        }
        _hash = HashCode.Combine(hash, PolytopDim);
      }

      return _hash.Value;
    }

    // /// <summary>
    // /// Converts the polytope to a convex polygon.
    // /// </summary>
    // /// <param name="basis">The affine basis used for the conversion.</param>
    // /// <returns>A convex polygon representing the polytope in 2D space.</returns>
    // public ConvexPolygon ToConvexPolygon(AffineBasis basis) {
    //   if (PolytopDim != 2) {
    //     throw new ArgumentException($"The dimension of the polygon must equal to 2. Found = {PolytopDim}.");
    //   }
    //
    //   return new ConvexPolygon(basis.ProjectPoints(Vertices));
    // }

    // /// <summary>
    // /// Writes Polytop to the file in 'PolytopTXT_format'.
    // /// </summary>
    // /// <param name="filePath">The path to the file to write in.</param>
    // public void WriteTXT(string filePath) {
    //   List<Point> VList = Vertices.ToList();
    //   using (StreamWriter writer = new StreamWriter(filePath)) {
    //     writer.Write("Type: ");
    //     switch (Type) {
    //       case ConvexPolytopType.Simplex:
    //         writer.WriteLine("Simplex");
    //
    //         break;
    //       case ConvexPolytopType.Polytop:
    //         writer.WriteLine("NoNSimplex");
    //
    //         break;
    //       case ConvexPolytopType.Polygon2D:
    //         writer.WriteLine("TwoDimensional");
    //
    //         break;
    //     }
    //     writer.WriteLine($"PDim: {PolytopDim}");
    //     writer.WriteLine($"SDim: {SpaceDim}");
    //     writer.WriteLine();
    //     writer.WriteLine($"Vertices: {Vertices.Count}");
    //     writer.WriteLine(string.Join('\n', VList.Select(v => v.ToFileFormat())));
    //     writer.WriteLine();
    //     writer.WriteLine($"Faces: {Faces.Count}");
    //     foreach (Face face in Faces) {
    //       writer.WriteLine($"N: {face.Normal.ToFileFormat()}");
    //       writer.WriteLine(string.Join(' ', face.Vertices.Select(v => VList.IndexOf(v))));
    //     }
    //   }
    // }

  }

}
