using System;
using System.Collections;
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
  public class IncidenceInfo : Dictionary<Edge, (Facet F1, Facet F2)> {

    public IncidenceInfo(IncidenceInfo incid) : base(incid) { }

    public IncidenceInfo(SubIncidenceInfo info) : base
      (
       new Dictionary<Edge, (Facet F1, Facet F2)>
         (
          info.Select
            (
             x => {
               Debug.Assert(x.Value.F1.Normal is not null, "IncidenceInfo: x.Value.F1.Normal != null");
               Debug.Assert(x.Value.F2.Normal is not null, "IncidenceInfo: x.Value.F2.Normal != null");


               return new KeyValuePair<Edge, (Facet F1, Facet F2)>
                 (
                  new Edge(x.Key.OriginalVertices)
                , (new Facet(x.Value.F1.OriginalVertices, x.Value.F1.Normal)
                 , new Facet(x.Value.F2.OriginalVertices, x.Value.F2.Normal))
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
  public class FansInfo : Dictionary<Vector, HashSet<Facet>> {

    public FansInfo(FansInfo fansInfo) : base(fansInfo) { }

    public FansInfo(HashSet<BaseSubCP> Fs) {
      foreach (BaseSubCP F in Fs) {
        foreach (Vector vertex in F.OriginalVertices) {
          Debug.Assert(F.Normal is not null, "F.Normal != null");

          if (TryGetValue(vertex, out HashSet<Facet>? value)) {
            value.Add(new Facet(F.OriginalVertices, F.Normal));
          } else {
            base.Add(vertex, new HashSet<Facet>() { new Facet(F.OriginalVertices, F.Normal) });
          }
        }
      }
    }

  }

  /// <summary>
  /// Represents an facet of the Polytop.
  /// </summary>
  public class Facet {

    /// <summary>
    /// Gets the vertices of the face.
    /// </summary>
    public List<Vector> Vertices { get; }

    /// <summary>
    /// Gets the normal vector of the face.
    /// </summary>
    public Vector Normal { get; }

    /// <summary>
    /// Gets the hyper plane corresponding to this face.
    /// </summary>
    public HyperPlane HPlane => new HyperPlane(Vertices.First(), Normal);

    /// <summary>
    /// Initializes a new instance of the Facet class.
    /// </summary>
    /// <param name="Vs">The vertices of the face.</param>
    /// <param name="normal">The outward normal vector of the face.</param>
    public Facet(IEnumerable<Vector> Vs, Vector normal) {
      Vertices = new List<Vector>(Vs);
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

      Facet other = (Facet)obj;

      return this.Normal.Equals(other.Normal) && (new HashSet<Vector>(this.Vertices)).SetEquals(other.Vertices);
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

        foreach (Vector vertex in Vertices.Order()) {
          hash = HashCode.Combine(hash, vertex.GetHashCode());
        }
        _hash = hash;
      }

      return _hash.Value;
    }

  }

  // /// <summary>
  // /// Represents an edge of the Polytop.
  // /// </summary>
  // public class Edge {
  //
  //   /// <summary>
  //   /// Gets the vertices of the edge.
  //   /// </summary>
  //   public HashSet<Vector> Vertices { get; }
  //
  //   /// <summary>
  //   /// Initializes a new instance of the Edge class.
  //   /// </summary>
  //   /// <param name="Vs">The vertices of the edge.</param>
  //   public Edge(IEnumerable<Vector> Vs) => Vertices = new HashSet<Vector>(Vs);
  //
  //   /// <summary>
  //   /// Determines whether the specified object is equal to edge.
  //   /// Two edges are equal if they have same sets of their vertices.
  //   /// </summary>
  //   /// <param name="obj">The object to compare with edge.</param>
  //   /// <returns>True if the specified object is equal to edge, False otherwise</returns>
  //   public override bool Equals(object? obj) {
  //     if (obj == null || this.GetType() != obj.GetType()) {
  //       return false;
  //     }
  //
  //     Edge other = (Edge)obj;
  //
  //     return this.Vertices.SetEquals(other.Vertices);
  //   }
  //
  //   /// <summary>
  //   /// Internal field for the hash of the edge
  //   /// </summary>
  //   private int? _hash = null;
  //
  //   /// <summary>
  //   /// Returns a hash code for the edge based on specified set of vertices.
  //   /// </summary>
  //   /// <returns>A hash code for the specified set of vertices.</returns>
  //   public override int GetHashCode() {
  //     if (_hash is null) {
  //       int hash = 0;
  //
  //       foreach (Vector vertex in Vertices.Order()) {
  //         hash = HashCode.Combine(hash, vertex.GetHashCode());
  //       }
  //       _hash = hash;
  //     }
  //
  //     return _hash.Value;
  //   }
  //
  // }

  /// <summary>
  /// Represents a full-dimensional convex polytop in a d-dimensional space.
  /// </summary>
  public class ConvexPolytop {

#region Fields and Properties
    /// <summary>
    /// Gets the dimension of the space in which the polytop is treated.
    /// </summary>
    public int SpaceDim { get; }

    /// <summary>
    /// Gets the dimension of the polytop.
    /// </summary>
    public int PolytopDim => ???;

    //todo FL знает о связи гиперграней и точек, которые лежат в них!

    // VRep
    private VPolytop? _VRep = null;

    public VPolytop VRep {
      get
        {
          if (_VRep is null) { }

          return;
        }
    }

    /// <summary>
    /// Gets the set of vertices of the polytop.
    /// </summary>
    public HashSet<Vector> Vertices => VRep.Vertices;


    // HRep
    private HPolytop? _HRep = null;

    public HPolytop HRep {
      get
        {
          if (_HRep is null) {
            if (_VRep is not null) {
              GiftWrapping gw = new GiftWrapping(VRep.Vertices);
              gw.HRepresentation
            }
          }

          return;
        }
    }

    // FL
    private FaceLattice? _FL = null;

    public FaceLattice FL {
      get
        {
          if (_FL is null) { }

          return;
        }
    }
#endregion

    /// <summary>
    /// Get the polytop as a hyperplane representation. Its normals are oriented outwards.
    /// </summary>
    /// <returns>The list of hyperplanes.</returns>
    public List<HyperPlane> HRepresentation {
      get
        {
          if (_HRepr is null) {
            List<HyperPlane> res = new List<HyperPlane>();
            foreach (Facet face in Faces) {
              res.Add(face.HPlane);
            }
            _HRepr = res;
          }

          return _HRepr;
        }
    }


    // /// <summary>
    // /// Gets the incidence information of the faces. Each edge is associated with a pair of incidence faces with it.
    // /// </summary>
    // public IncidenceInfo FaceIncidence { get; }

    // /// <summary>
    // /// Gets the fan information of the polytop. Each point is associated with a set of incidence faces with it.
    // /// </summary>
    // public FansInfo Fans { get; }

#region Constructors
    public ConvexPolytop(VPolytop VP) {
      _VRep    = VP;
      SpaceDim = VP.Vertices.First().Dim;
    }

    public ConvexPolytop(IEnumerable<Vector> Vs) {
      _VRep    = new VPolytop(Vs);
      SpaceDim = Vs.First().Dim;
    }

    public ConvexPolytop(HPolytop HP) {
      _HRep    = HP;
      SpaceDim = HP.Faces.First().Normal.Dim;
    }

    public ConvexPolytop(FaceLattice FL) {
      _FL   = FL;
      _VRep = new VPolytop(FL.Vertices);
      // todo HRep можно получить, взяв ABasis из уровня гиперграней. Но, надо понять, как ориентировать нормаль. (Взять какую-то выпуклую комбинацию всех вершин?)
    }
#endregion

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
    /// <param name="needGW">If the flag is true, then the GW procedure is applied and the order of the vertices is established.</param>
    public void WriteTXT(string filePath, bool needGW = false) {
      List<Vector>  VList = Vertices.Order().ToList();
      HashSet<Facet> FSet  = Faces;
      if (needGW && PolytopDim == 3) {
        // Это надо только для того, что бы красиво картинки в 3Д рисовать, так как в FL теряется инфа о порядке вершин
        FSet = GiftWrapping.WrapPolytop(VList).Faces;
      }
      using (StreamWriter writer = new StreamWriter(filePath)) {
        writer.WriteLine($"PDim: {PolytopDim}");
        writer.WriteLine($"SDim: {SpaceDim}");
        writer.WriteLine();
        writer.WriteLine($"Vertices: {Vertices.Count}");
        writer.WriteLine(string.Join('\n', VList.Select(v => v.ToFileFormat())));
        writer.WriteLine();
        writer.WriteLine($"Faces: {Faces.Count}");
        foreach (Facet face in FSet.OrderBy(F => new Vector(F.Normal))) {
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
    public static HashSet<Vector> HRepToVRep_Naive(List<HyperPlane> H) {
      int n = H.Count;
      int d = H.First().SubSpaceDim + 1;

      HashSet<Vector>      Vs          = new HashSet<Vector>();
      Combination          combination = new Combination(n, d);
      Func<int, int, TNum> AFunc       = (r, l) => H[combination[r]].Normal[l];
      Func<int, TNum>      bFunc       = r => H[combination[r]].ConstantTerm;
      bool                 belongs;
      GaussSLE             gaussSLE = new GaussSLE(d, d);
      do { // Перебираем все сочетания из d элементов из набора гиперплоскостей
        gaussSLE.SetSystem(AFunc, bFunc, d, d, GaussSLE.GaussChoice.ColWise);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector point)) { // Ищем точку пересечения
          belongs = true;
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
