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
  public class Facet {

    //todo Может её отнаследовать от HyperPlane и добавить только точки вершин? И заполнять при первой возможности?

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
    public int PolytopDim => _FL is not null ? FL.Top.PolytopDim : GW.PolytopDim;

    private GiftWrapping? _gw = null;

    private GiftWrapping GW => _gw ??= new GiftWrapping(VRep.Vertices);

    private VPolytop? _VRep = null; // todo список вершин или же vpolytop?

    public VPolytop VRep {
      get
        {
          if (_VRep is null) {
            if (_FL is not null) {
              _VRep = new VPolytop(FL.Vertices);
            } else {
              if (_HRep is not null) {
                _VRep = HRepToVRep_Naive(HRep);
              }
            }
          }

          Debug.Assert(_VRep is not null, $"ConvexPolytop.VRep: _VRep is null after constructing. Something went wrong!");

          return _VRep;
        }
    }

    public HashSet<Vector> Vertices => VRep.Vertices;

    private HPolytop? _HRep = null;

    public HPolytop HRep {
      get
        {
          if (_HRep is null) {
            if (_FL is not null) {
              // Если представлен в виде FL, но не HRep, то достанем эту информацию из решётки. VRep уже есть из конструктора.
              _HRep = new HPolytop
                (FL.Lattice[^2].Select(n => new HyperPlane(new AffineBasis(n.AffBasis), (FL.Top.InnerPoint, false))).ToList());
            } else {
              if (_VRep is not null) { // Если дано вершинное описаение, то прокатываем поарок.
                _HRep = GW.HPolytop;
              }
            }
          }
          Debug.Assert(_HRep is not null, $"ConvexPolytop.HRep: _HRep is null after constructing. Something went wrong!");

          return _HRep;
        }
    }

    private FaceLattice? _FL = null;

    public FaceLattice FL {
      get
        {
          if (_FL is null) {
            if (_VRep is not null) {
              _FL = GW.FaceLattice;
            } else {
              if (_HRep is not null) { // FL = null, VRep = null; Печаль ;( HRep --> VRep -(GW)-> FL
                _VRep = VRep;          // VRep will be constructed
                _FL   = GW.FaceLattice;
              }
            }
          }
          Debug.Assert(_FL is not null, $"ConvexPolytop.FL: _FL is null after constructing. Something went wrong!");

          return _FL;
        }
    }
#endregion

#region Constructors
    private enum ConvexPolytopForm { VRep, HRep, FL }

    private ConvexPolytop(HashSet<Vector> VP, bool toConvexify, ConvexPolytopForm form) {
      SpaceDim = VP.First().Dim;
      if (toConvexify) {
        _gw   = new GiftWrapping(VP);
        _VRep = GW.VPolytop;
      } else {
        _VRep = new VPolytop(VP);
      }
      switch (form) {
        case ConvexPolytopForm.VRep: break; // уже всё сделали
        case ConvexPolytopForm.HRep:
          _HRep = GW.HPolytop;

          break;
        case ConvexPolytopForm.FL:
          _FL = GW.FaceLattice;

          break;
      }
    }

    private ConvexPolytop(List<HyperPlane> HPs, bool doHRedundancy, ConvexPolytopForm form) {
      SpaceDim = HPs.First().Normal.Dim;
      if (doHRedundancy) {
        throw new NotImplementedException("Надо сделать! Нужен симлекс метод и алгоритм Фукуды.");
      } else {
        _HRep = new HPolytop(HPs);
      }

      switch (form) {
        case ConvexPolytopForm.HRep: break; // всё готово
        case ConvexPolytopForm.VRep:
          _VRep = HRepToVRep_Naive(new HPolytop(HPs));

          break;
        case ConvexPolytopForm.FL:
          _VRep = HRepToVRep_Naive(new HPolytop(HPs));
          _FL   = GW.FaceLattice;

          break;
      }
    }

    private ConvexPolytop(FaceLattice FL, ConvexPolytopForm form) {
      _FL = FL;
      switch (form) {
        case ConvexPolytopForm.FL: break; // всё есть
        case ConvexPolytopForm.VRep:
          _VRep = new VPolytop(FL.Vertices);

          break;
        case ConvexPolytopForm.HRep:
          _HRep = new HPolytop
            (FL.Lattice[^2].Select(n => new HyperPlane(new AffineBasis(n.AffBasis), (FL.Top.InnerPoint, false))).ToList());

          break;
      }
    }

    private ConvexPolytop(GiftWrapping gw) {
      _FL   = gw.FaceLattice;
      _VRep = gw.VPolytop;
      _HRep = gw.HPolytop;
    }
#endregion

#region Fabrics
    public static ConvexPolytop AsVPolytop(HashSet<Vector> S, bool toConvexify = false)
      => new ConvexPolytop(S, toConvexify, ConvexPolytopForm.VRep);

    public static ConvexPolytop AsHPolytop(HashSet<Vector> S, bool toConvexify = false)
      => new ConvexPolytop(S, toConvexify, ConvexPolytopForm.HRep);

    public static ConvexPolytop AsFLPolytop(HashSet<Vector> S, bool toConvexify = false)
      => new ConvexPolytop(S, toConvexify, ConvexPolytopForm.FL);

    public static ConvexPolytop AsVPolytop(List<HyperPlane> HPs, bool doHRedundancy = false) // todo List or HashSet ?!
      => new ConvexPolytop(HPs, doHRedundancy, ConvexPolytopForm.VRep);

    public static ConvexPolytop AsHPolytop(List<HyperPlane> HPs, bool doHRedundancy = false)
      => new ConvexPolytop(HPs, doHRedundancy, ConvexPolytopForm.HRep);

    public static ConvexPolytop AsFLPolytop(List<HyperPlane> HPs, bool doHRedundancy = false)
      => new ConvexPolytop(HPs, doHRedundancy, ConvexPolytopForm.FL);

    public static ConvexPolytop AsVPolytop(FaceLattice  faceLattice) => new ConvexPolytop(faceLattice, ConvexPolytopForm.VRep);
    public static ConvexPolytop AsHPolytop(FaceLattice  faceLattice) => new ConvexPolytop(faceLattice, ConvexPolytopForm.HRep);
    public static ConvexPolytop AsFLPolytop(FaceLattice faceLattice) => new ConvexPolytop(faceLattice, ConvexPolytopForm.FL);
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
      List<Vector> VList = Vertices.Order().ToList();
      List<Facet>  FSet  = CalcFacets();
      using (StreamWriter writer = new StreamWriter(filePath)) {
        writer.WriteLine($"PDim: {FL.Top.PolytopDim}");
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
