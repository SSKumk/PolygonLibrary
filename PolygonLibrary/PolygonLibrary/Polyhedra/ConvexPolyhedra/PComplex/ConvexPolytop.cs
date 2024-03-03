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

  // /// <summary>
  // /// Type of permanent storage of face incidence information.
  // /// For each pair (F1, F2) of incident faces, it is assumed that HashCode(F1) is less or equal than HashCode(F2)
  // /// </summary>
  // public class IncidenceInfo : Dictionary<Edge, (Facet F1, Facet F2)> {
  //
  //   public IncidenceInfo(IncidenceInfo incid) : base(incid) { }
  //
  //   public IncidenceInfo(SubIncidenceInfo info) : base
  //     (
  //      new Dictionary<Edge, (Facet F1, Facet F2)>
  //        (
  //         info.Select
  //           (
  //            x => {
  //              Debug.Assert(x.Value.F1.Normal is not null, "IncidenceInfo: x.Value.F1.Normal != null");
  //              Debug.Assert(x.Value.F2.Normal is not null, "IncidenceInfo: x.Value.F2.Normal != null");
  //
  //
  //              return new KeyValuePair<Edge, (Facet F1, Facet F2)>
  //                (
  //                 new Edge(x.Key.OriginalVertices)
  //               , (new Facet(x.Value.F1.OriginalVertices, x.Value.F1.Normal)
  //                , new Facet(x.Value.F2.OriginalVertices, x.Value.F2.Normal))
  //                );
  //            }
  //           )
  //        )
  //     ) { }
  //
  // }
  //
  // /// <summary>
  // /// Type of permanent storage of fans information.
  // /// Dictionary: point --> set of faces incident with a point.
  // /// </summary>
  // public class FansInfo : Dictionary<Vector, HashSet<Facet>> {
  //
  //   public FansInfo(FansInfo fansInfo) : base(fansInfo) { }
  //
  //   public FansInfo(HashSet<BaseSubCP> Fs) {
  //     foreach (BaseSubCP F in Fs) {
  //       foreach (Vector vertex in F.OriginalVertices) {
  //         Debug.Assert(F.Normal is not null, "F.Normal != null");
  //
  //         if (TryGetValue(vertex, out HashSet<Facet>? value)) {
  //           value.Add(new Facet(F.OriginalVertices, F.Normal));
  //         } else {
  //           base.Add(vertex, new HashSet<Facet>() { new Facet(F.OriginalVertices, F.Normal) });
  //         }
  //       }
  //     }
  //   }
  //
  // }

  /// <summary>
  /// Represents an facet of the Polytop.
  /// </summary>
  public class Facet { //todo Может её отнаследовать от HyperPlane и добавить только точки вершин? И заполнять при первой возможности?

    /// <summary>
    /// Gets the vertices of the face.
    /// </summary>
    public List<Vector> Vertices { get; }

    /// <summary>
    /// Gets the normal vector of the face.
    /// </summary>
    public Vector Normal { get; }

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

    // /// <summary>
    // /// Gets the dimension of the polytop.
    // /// </summary>
    // public int PolytopDim => ???; todo Как быть с этим полем?!

    //todo FL знает о связи гиперграней и точек, которые лежат в них!

    // VRep
    private VPolytop? _VRep = null;

    public VPolytop VRep {
      get
        {
          if (_VRep is null) {
            if (_HRep is not null) { // Если FL != null, то VRep уже есть
              _VRep = HRepToVRep_Naive(HRep);
            }
          }

          Debug.Assert(_VRep is not null, $"ConvexPolytop.VRep: _VRep is null after constructing. Something went wrong!");

          return _VRep;
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
            if (_FL is not null) {
              // Если представлен в виде FL, но не HRep, то достанем эту информацию из решётки. VRep уже есть из конструктора.
              _HRep = new HPolytop
                (FL.Lattice[^2].Select(n => new HyperPlane(new AffineBasis(n.AffBasis), (FL.Top.InnerPoint, false))));
            } else {
              if (_VRep is not null) { // Если дано вершинное описаение, то прокатываем поарок.
                GiftWrapping gw = new GiftWrapping(VRep.Vertices);
                _HRep = gw.HPolytop;
                _FL   = gw.FaceLattice; // Вроде невозможно, что FL != null, VRep != null, HRep = null;
              }
            }
          }
          Debug.Assert(_HRep is not null, $"ConvexPolytop.HRep: _HRep is null after constructing. Something went wrong!");

          return _HRep;
        }
    }

    // FL
    private FaceLattice? _FL = null;

    public FaceLattice FL {
      get
        {
          if (_FL is null) {
            if (_VRep is not null) { // Раз уж гоним GW, то и на решётку потратимся
              GiftWrapping gw = new GiftWrapping(VRep.Vertices);
              _VRep = gw.VPolytop;
              _FL   = gw.FaceLattice;
            } else {
              if (_HRep is not null) { // FL = null, VRep = null; Печаль ;( HRep --> VRep -(GW)-> FL
                _VRep = VRep;          // VRep will be constructed
                _FL   = GiftWrapping.WrapFaceLattice(VRep.Vertices);
              }
            }
          }
          Debug.Assert(_FL is not null, $"ConvexPolytop.FL: _FL is null after constructing. Something went wrong!");

          return _FL;
        }
    }
#endregion

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
    }

    public ConvexPolytop(GiftWrapping gw) {
      _FL   = gw.FaceLattice;
      _VRep = gw.VPolytop;
      _HRep = gw.HPolytop;
    }
#endregion

#region Fabrics
    /// <summary>
    /// Wraps a given swarm of points to the convex polytop.
    /// </summary>
    /// <param name="S">The swarm of points.</param>
    /// <returns>The convex polytop.</returns>
    public static ConvexPolytop WrapPolytop(IEnumerable<Vector> S) => new ConvexPolytop(new GiftWrapping(S));
#endregion

#region Aux functions
    private List<Facet> CalcFacets() {
      List<Facet> Fs = new List<Facet>();
      foreach (FLNode face in FL.Lattice[^2]) {
        Fs.Add(new Facet(face.Vertices, new HyperPlane(new AffineBasis(face.AffBasis), (FL.Top.InnerPoint, false)).Normal));
      }

      return Fs;
    }
#endregion

    // /// <summary>
    // /// Converts the polytop to a convex polygon.
    // /// </summary>
    // /// <param name="basis">The affine basis used for the conversion.</param>
    // /// <returns>A convex polygon representing the polytop in 2D space.</returns>
    // public ConvexPolygon ToConvexPolygon(AffineBasis basis) {
    //   if (PolytopDim != 2) {
    //     throw new ArgumentException($"The dimension of the polygon must equal to 2. Found = {PolytopDim}.");
    //   }
    //
    //   return new ConvexPolygon(basis.ProjectPoints(Vertices));
    // } todo зависит от PolytopDim

    /// <summary>
    /// Writes Polytop to the file in 'PolytopTXT_format'. It use the description as face lattice.
    /// </summary>
    /// <param name="filePath">The path to the file to write in.</param>
    public void WriteTXT(string filePath) {
      List<Vector>   VList = Vertices.Order().ToList();
      List<Facet> FSet  = CalcFacets();
      using (StreamWriter writer = new StreamWriter(filePath)) {
        writer.WriteLine($"PDim: {FL.Top.Dim}");
        writer.WriteLine($"SDim: {SpaceDim}");
        writer.WriteLine();
        writer.WriteLine($"Vertices: {Vertices.Count}");
        writer.WriteLine(string.Join('\n', VList.Select(v => v.ToFileFormat())));
        writer.WriteLine();
        writer.WriteLine($"Faces: {FSet.Count}");
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
    public static VPolytop HRepToVRep_Naive(HPolytop H) {
      int n = H.Faces.Count;
      int d = H.SpaceDim;

      HashSet<Vector>      Vs          = new HashSet<Vector>();
      Combination          combination = new Combination(n, d);
      Func<int, int, TNum> AFunc       = (r, l) => H.Faces[combination[r]].Normal[l];
      Func<int, TNum>      bFunc       = r => H.Faces[combination[r]].ConstantTerm;
      bool                 belongs;
      GaussSLE             gaussSLE = new GaussSLE(d, d);
      do { // Перебираем все сочетания из d элементов из набора гиперплоскостей
        gaussSLE.SetSystem(AFunc, bFunc, d, d, GaussSLE.GaussChoice.ColWise);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector point)) { // Ищем точку пересечения
          belongs = true;
          foreach (HyperPlane hp in H.Faces) {
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

      return new VPolytop(Vs);
    }

  }

}
