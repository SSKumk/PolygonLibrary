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
               Debug.Assert(x.Value.F1.Normal is not null, "IncidenceInfo: x.Value.F1.Normal != null");
               Debug.Assert(x.Value.F2.Normal is not null, "IncidenceInfo: x.Value.F2.Normal != null");


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
    /// Gets the hyper plane corresponding to this face.
    /// </summary>
    public HyperPlane HPlane => new HyperPlane(Vertices.First(), Normal);

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

      return this.Normal.Equals(other.Normal) && this.Vertices.SetEquals(other.Vertices);
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

        foreach (Point vertex in Vertices.Order()) {
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

        foreach (Point vertex in Vertices.Order()) {
          hash = HashCode.Combine(hash, vertex.GetHashCode());
        }
        _hash = hash;
      }

      return _hash.Value;
    }

  }

  /// <summary>
  /// Represents a full-dimensional convex polytop in a d-dimensional space.
  /// </summary>
  public class ConvexPolytop {

    /// <summary>
    /// Gets the dimension of the space in which the polytop is treated.
    /// </summary>
    public int SpaceDim => Vertices.First().Dim;

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public int PolytopDim { get; }

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public HashSet<Point> Vertices => Polytop.Vertices;

    /// <summary>
    /// The list of hyperplanes forming the polytop.
    /// </summary>
    private List<HyperPlane>? _HRepr = null;

    /// <summary>
    /// Get the polytop as a hyperplane representation. Its normals are oriented outwards.
    /// </summary>
    /// <returns>The list of hyperplanes.</returns>
    public List<HyperPlane> HRepresentation {
      get
        {
          if (_HRepr is null) {
            List<HyperPlane> res = new List<HyperPlane>();
            foreach (Face face in Faces) {
              res.Add(face.HPlane);
            }
            _HRepr = res;
          }

          return _HRepr;
        }
    }

    public VPolytop Polytop { get; init; }

    /// <summary>
    /// Gets the faces of the polytop.
    /// </summary>
    public HashSet<Face> Faces { get; }

    /// <summary>
    /// Gets the set of edges of the polytop.
    /// </summary>
    public HashSet<Edge> Edges { get; }

    // /// <summary>
    // /// Gets the incidence information of the faces. Each edge is associated with a pair of incidence faces with it.
    // /// </summary>
    // public IncidenceInfo FaceIncidence { get; }

    // /// <summary>
    // /// Gets the fan information of the polytop. Each point is associated with a set of incidence faces with it.
    // /// </summary>
    // public FansInfo Fans { get; }

    /// <summary>
    /// Initializes a new instance of the Polytop class.
    /// </summary>
    /// <param name="Vs">The vertices of the polytop.</param>
    /// <param name="polytopDim">The dimension of the polytop.</param>
    /// <param name="faces">The faces of the polytop.</param>
    /// <param name="edges">The edges of the polytop.</param>
    /// <param name="type">The type of the polytop.</param>
    /// <param name="faceIncidence">The incidence information of the faces.</param>
    /// <param name="fans">The fan information of the polytop.</param>
    public ConvexPolytop(IEnumerable<Point> Vs, int polytopDim, IEnumerable<Face> faces, IEnumerable<Edge> edges) {
      Polytop    = new VPolytop(Vs);
      PolytopDim = polytopDim;
      Faces      = new HashSet<Face>(faces);
      Edges      = new HashSet<Edge>(edges);
    }

    /// <summary>
    /// Converts the polytop to a convex polygon.
    /// </summary>
    /// <param name="basis">The affine basis used for the conversion.</param>
    /// <returns>A convex polygon representing the polytop in 2D space.</returns>
    public ConvexPolygon ToConvexPolygon(AffineBasis basis) {
      if (PolytopDim != 2) {
        throw new ArgumentException($"The dimension of the polygon must equal to 2. Found = {PolytopDim}.");
      }

      return new ConvexPolygon(basis.ProjectPoints(Vertices));
    }

    /// <summary>
    /// Writes Polytop to the file in 'PolytopTXT_format'.
    /// </summary>
    /// <param name="filePath">The path to the file to write in.</param>
    public void WriteTXT(string filePath) {
      List<Point> VList = Vertices.Order().ToList();
      using (StreamWriter writer = new StreamWriter(filePath)) {
        writer.WriteLine($"PDim: {PolytopDim}");
        writer.WriteLine($"SDim: {SpaceDim}");
        writer.WriteLine();
        writer.WriteLine($"Vertices: {Vertices.Count}");
        writer.WriteLine(string.Join('\n', VList.Select(v => v.ToFileFormat())));
        writer.WriteLine();
        writer.WriteLine($"Faces: {Faces.Count}");
        foreach (Face face in Faces.OrderBy(F => new Point(F.Normal))) {
          writer.WriteLine($"N: {face.Normal.ToFileFormat()}");
          writer.WriteLine(string.Join(' ', face.Vertices.Select(v => VList.IndexOf(v))));
        }
      }
    }

    /// <summary>
    /// Converts the H-representation of convex polytop to the V-representation by checking all possible d-tuples of the hyperplanes.
    /// </summary>
    /// <param name="H">The hyperplane arrangement.</param>
    /// <returns>The V-representation of the convex polytop.</returns>
    public static HashSet<Point> HRepToVRep_Naive(List<HyperPlane> H) {
      int n = H.Count;
      int d = H.First().SubSpaceDim + 1;

      HashSet<Point> Vs          = new HashSet<Point>();
      Combination    combination = new Combination(n, d);
      do {                 // Перебираем все сочетания из d элементов из набора гиперплоскостей
        if (GaussSLE.Solve // Ищем точку пересечения
              (
               (r, l) => H[combination[r]].Normal[l]
             , r => H[combination[r]].ConstantTerm
             , d
             , GaussSLE.GaussChoice.All
             , out TNum[] x
              )) {
          bool  belongs = true;
          Point point   = new Point(x);
          foreach (HyperPlane hp in H) {
            if (hp.ContainsPositive(point)) {
              belongs = false;

              break;
            }
          }
          if (belongs) {
            Vs.Add(point);
          }
        }
      } while (combination.Next());

      return Vs;
    }

  }

}
