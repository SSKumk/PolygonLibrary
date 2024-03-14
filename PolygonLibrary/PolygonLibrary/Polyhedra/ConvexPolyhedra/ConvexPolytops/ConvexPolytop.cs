using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Security.Claims;


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
    public Facet(IReadOnlyList<Vector> Vs, Vector normal) {
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
      if (obj == null || GetType() != obj.GetType()) {
        return false;
      }

      Facet other = (Facet)obj;

      return Normal.Equals(other.Normal) && (new HashSet<Vector>(Vertices)).SetEquals(other.Vertices);
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
        int hash = Normal.GetHashCode();

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

    private GiftWrapping GW => _gw ??= new GiftWrapping(VRep);

    private HashSet<Vector>? _VRep = null;

    public HashSet<Vector> VRep {
      get
        {
          if (_VRep is null) {
            if (_FL is not null) {
              _VRep = new HashSet<Vector>(FL.Vertices);
            } else if (_HRep is not null) {
              _VRep = HRepToVRep_Naive(HRep);
            }
          }

          Debug.Assert(_VRep is not null, $"ConvexPolytop.VRep: _VRep is null after constructing. Something went wrong!");

          return _VRep;
        }
    }

    public HashSet<Vector> Vertices => VRep;

    private List<HyperPlane>? _HRep = null;

    public List<HyperPlane> HRep {
      get
        {
          if (_HRep is null) {
            if (_FL is not null) {
              // Если представлен в виде FL, но не HRep, то достанем эту информацию из решётки. VRep уже есть из конструктора.
              _HRep = new List<HyperPlane>
                (FL.Lattice[^2].Select(n => new HyperPlane(new AffineBasis(n.AffBasis), (FL.Top.InnerPoint, false))).ToList());
            } else {
              if (_VRep is not null) { // Если дано вершинное описаение, то прокатываем поарок.
                _HRep = GW.HRep;
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

// todo Нужен ли GetHashCode()? Если да, то как его считать?

#region Constructors
    private enum ConvexPolytopForm { VRep, HRep, FL }

    private ConvexPolytop(HashSet<Vector> VP, bool toConvexify, ConvexPolytopForm form) {
      SpaceDim = VP.First().Dim;
      if (toConvexify) {
        _gw   = new GiftWrapping(VP);
        _VRep = GW.VRep;
      } else {
        _VRep = new HashSet<Vector>(VP);
      }
      switch (form) {
        case ConvexPolytopForm.VRep: break; // уже всё сделали
        case ConvexPolytopForm.HRep:
          _HRep = GW.HRep;

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
        _HRep = new List<HyperPlane>(HPs);
      }

      switch (form) {
        case ConvexPolytopForm.HRep: break; // всё готово
        case ConvexPolytopForm.VRep:
          _VRep = HRepToVRep_Naive(new List<HyperPlane>(HPs));

          break;
        case ConvexPolytopForm.FL:
          _VRep = HRepToVRep_Naive(new List<HyperPlane>(HPs));
          _FL   = GW.FaceLattice;

          break;
      }
    }

    private ConvexPolytop(FaceLattice FL, ConvexPolytopForm form) {
      SpaceDim = FL.Top.AffBasis.SpaceDim;
      _FL      = FL;
      switch (form) {
        case ConvexPolytopForm.FL: break; // всё есть
        case ConvexPolytopForm.VRep:
          _VRep = new HashSet<Vector>(FL.Vertices);

          break;
        case ConvexPolytopForm.HRep:
          _HRep = new List<HyperPlane>
            (FL.Lattice[^2].Select(n => new HyperPlane(new AffineBasis(n.AffBasis), (FL.Top.InnerPoint, false))).ToList());

          break;
      }
    }

    private ConvexPolytop(GiftWrapping gw) {
      _FL      = gw.FaceLattice;
      _VRep    = gw.VRep;
      _HRep    = gw.HRep;
      SpaceDim = VRep.First().Dim;
    }
#endregion

#region Fabrics
    public static ConvexPolytop AsVPolytop(HashSet<Vector> S, bool toConvexify = false)
      => new ConvexPolytop(S, toConvexify, ConvexPolytopForm.VRep);

    public static ConvexPolytop AsHPolytop(HashSet<Vector> S, bool toConvexify = false)
      => new ConvexPolytop(S, toConvexify, ConvexPolytopForm.HRep);

    public static ConvexPolytop AsFLPolytop(HashSet<Vector> S, bool toConvexify = false)
      => new ConvexPolytop(S, toConvexify, ConvexPolytopForm.FL);

    public static ConvexPolytop AsVPolytop(List<HyperPlane> HPs, bool doHRedundancy = false)
      => new ConvexPolytop(HPs, doHRedundancy, ConvexPolytopForm.VRep);

    public static ConvexPolytop AsHPolytop(List<HyperPlane> HPs, bool doHRedundancy = false)
      => new ConvexPolytop(HPs, doHRedundancy, ConvexPolytopForm.HRep);

    public static ConvexPolytop AsFLPolytop(List<HyperPlane> HPs, bool doHRedundancy = false)
      => new ConvexPolytop(HPs, doHRedundancy, ConvexPolytopForm.FL);

    public static ConvexPolytop AsVPolytop(FaceLattice  faceLattice) => new ConvexPolytop(faceLattice, ConvexPolytopForm.VRep);
    public static ConvexPolytop AsHPolytop(FaceLattice  faceLattice) => new ConvexPolytop(faceLattice, ConvexPolytopForm.HRep);
    public static ConvexPolytop AsFLPolytop(FaceLattice faceLattice) => new ConvexPolytop(faceLattice, ConvexPolytopForm.FL);
#endregion

#region Special polytopes
    /// <summary>
    /// Makes a full-dimension axis-parallel 0-1 cube of given dimension.
    /// </summary>
    /// <param name="dim">The dimension of the cube.</param>
    /// <returns>A convex polytop as VRep representing the hypercube.</returns>
    public static ConvexPolytop Cube01(int dim) => RectParallel(Vector.Zero(dim), Vector.Ones(dim));

    /// <summary>
    /// Makes a full-dimension axis-parallel rectangle based on two corners.
    /// </summary>
    /// <returns>A convex polytop as VRep representing the hyper rectangle.</returns>
    public static ConvexPolytop RectParallel(Vector left, Vector right) {
      Debug.Assert
        (
         left.Dim == right.Dim
       , $"ConvexPolytop.RectParallel: The dimension of the points must be equal! Found left = {left}, right = {right}"
        );

      List<List<TNum>> rect_prev = new List<List<TNum>>();
      List<List<TNum>> rect      = new List<List<TNum>>();
      rect_prev.Add(new List<TNum>() { left[0] });
      rect_prev.Add(new List<TNum>() { right[0] });

      for (int i = 1; i < left.Dim; i++) {
        rect.Clear();

        foreach (List<TNum> coords in rect_prev) {
          rect.Add(new List<TNum>(coords) { left[i] });
          rect.Add(new List<TNum>(coords) { right[i] });
        }

        rect_prev = new List<List<TNum>>(rect);
      }

      HashSet<Vector> Cube = new HashSet<Vector>();

      foreach (List<TNum> v in rect) {
        Cube.Add(new Vector(v.ToArray()));
      }

      return AsVPolytop(Cube);
    }

    /// <summary>
    /// Generates a d-simplex in d-space.
    /// </summary>
    /// <param name="simplexDim">The dimension of the simplex.</param>
    /// <returns>A convex polytop as VRep representing the random simplex.</returns>
    public static ConvexPolytop SimplexRND(int simplexDim) {
      GRandomLC random = new GRandomLC();

      HashSet<Vector> simplex = new HashSet<Vector>();
      do {
        for (int i = 0; i < simplexDim + 1; i++) {
          simplex.Add(new Vector(Vector.GenVector(simplexDim, TConv.FromInt(0), TConv.FromInt(10), random)));
        }
      } while (!new AffineBasis(simplex).IsFullDim);

      return AsVPolytop(simplex);
    }

    /// <summary>
    /// Makes the cyclic polytop in specified dimension with specified amount of points.
    /// </summary>
    /// <param name="pDim">The dimension of the cyclic polytop.</param>
    /// <param name="amountOfPoints">The amount of vertices in cyclic polytop.</param>
    /// <param name="step">The step of increasing the moment on the moments curve. init = 1 + step.</param>
    /// <returns>A convex polytop as VRep representing the cyclic polytop.</returns>
    public static ConvexPolytop CyclicPolytop(int pDim, int amountOfPoints, TNum step) {
      Debug.Assert
        (
         amountOfPoints > pDim
       , $"TestPolytopes.CyclicPolytop: The amount of points must be greater than the dimension of the space. Dim = {pDim}, amount = {amountOfPoints}"
        );
      HashSet<Vector> cycP      = new HashSet<Vector>() { new Vector(pDim) };
      TNum            baseCoord = Tools.One + step;
      for (int i = 1; i < amountOfPoints; i++) {
        TNum[] point      = new TNum[pDim];
        TNum   coordinate = baseCoord;
        TNum   multiplyer = coordinate;
        for (int t = 0; t < pDim; t++) { // (i, i^2, i^3 , ... , i^d)
          point[t]   =  coordinate;
          coordinate *= multiplyer;
        }
        cycP.Add(new Vector(point));
        baseCoord += step;
      }

      return AsVPolytop(cycP);
    }

    /// <summary>
    /// Makes a hD-sphere as VRep with given radius. The euclidean norm.
    /// </summary>
    /// <param name="dim">The dimension of the sphere. It is greater than 1.</param>
    /// <param name="thetaPartition">The number of partitions at zenith angle. Theta in [0, Pi].
    ///   thetaPoints should be greater than 2 for proper calculation.</param>
    /// <param name="phiPartition">The number of partitions at each azimuthal angle. Phi in [0, 2*Pi).</param>
    /// <param name="center">The center of a sphere.</param>
    /// <param name="radius">The radius of a sphere.</param>
    /// <returns>A convex polytop as VRep representing the sphere in hD.</returns>
    public static ConvexPolytop Sphere(int dim, int thetaPartition, int phiPartition, Vector center, TNum radius)
      => Ellipsoid(dim, thetaPartition, phiPartition, center, Vector.Ones(dim) * radius);

    /// <summary>
    /// Makes a hD-ellipsoid as VRep with given semi-axis.
    /// </summary>
    /// <param name="dim">The dimension of the ellipsoid. It is greater than 1.</param>
    /// <param name="thetaPartition">The number of partitions at zenith angle.</param>
    /// <param name="phiPartition">The number of partitions at each azimuthal angle.</param>
    /// <param name="center">The center of an ellipsoid.</param>
    /// <param name="semiAxis">The vector each coordinate of it represents the length of corresponding semi-axe.</param>
    /// <returns>A convex polytop as VRep representing the ellipsoid in hD.</returns>
    public static ConvexPolytop Ellipsoid(int dim, int thetaPartition, int phiPartition, Vector center, Vector semiAxis) {
      Debug.Assert(dim > 1, $"ConvexPolytop.Ellipsoid: The dimension of an ellipsoid must be 2 or greater. Found dim = {dim}.");
      Debug.Assert
        (
         center.Dim == dim
       , $"ConvexPolytop.Ellipsoid: the dimension of the center of an ellipsoid must be equal to dim = {dim}. Found center.Dim = {center.Dim}"
        );
      Debug.Assert
        (
         semiAxis.Dim == dim
       , $"ConvexPolytop.Ellipsoid: the dimension of the semiAxis-vector of an ellipsoid must be equal to dim = {dim}. Found semiAxis.Dim = {center.Dim}"
        );
#if DEBUG
      for (int i = 0; i < dim; i++) {
        Debug.Assert
          (
           Tools.GT(semiAxis[i])
         , $"ConvexPolytop.Ellipsoid: The value of semi-axis must be greater than 0! Found i = {i}, val = {semiAxis[i]}"
          );
      }
#endif

      // Phi in [0, 2*Pi)
      // Theta in [0, Pi]
      HashSet<Vector> Ps        = new HashSet<Vector>();
      int             N         = dim - 2;
      TNum            thetaStep = Tools.PI / TConv.FromInt(thetaPartition);
      TNum            phiStep   = Tools.PI2 / TConv.FromInt(phiPartition);

      List<TNum> thetaAll = new List<TNum>();
      for (int i = 0; i <= thetaPartition; i++) {
        thetaAll.Add(thetaStep * TConv.FromInt(i));
      }

      // цикл по переменной [0, 2*Pi)
      for (int i = 0; i < phiPartition; i++) {
        TNum phi = phiStep * TConv.FromInt(i);

        // соберём все наборы углов вида [Phi, t1, t2, t3, ..., t(n-2)]
        // где t_i принимают все возможные свои значения из theta_all
        List<List<TNum>> thetaAngles_prev = new List<List<TNum>>() { new List<TNum>() { phi } };
        List<List<TNum>> thetaAngles      = new List<List<TNum>>() { new List<TNum>() { phi } };
        // сколько раз нужно углы добавлять
        for (int k = 0; k < N; k++) {
          thetaAngles.Clear();
          // формируем наборы добавляя к каждому текущему набору всевозможные углы из theta all
          foreach (List<TNum> angle in thetaAngles_prev) {
            foreach (TNum theta in thetaAll) {
              thetaAngles.Add(new List<TNum>(angle) { theta });
            }
          }
          thetaAngles_prev = new List<List<TNum>>(thetaAngles);
        }

        foreach (List<TNum> s in thetaAngles) {
          List<TNum> point = new List<TNum>();
          // собрали 1 и 2 координаты
          TNum sinsN = Tools.One;
          for (int k = 1; k <= N; k++) { sinsN *= TNum.Sin(s[k]); }
          point.Add(semiAxis[0] * TNum.Cos(phi) * sinsN);
          point.Add(semiAxis[1] * TNum.Sin(phi) * sinsN);


          //добавляем серединные координаты
          if (dim >= 4) { // Их нет для 2Д и 3Д сфер
            TNum sinsJ = Tools.One;
            for (int j = 2; j <= N; j++) {
              sinsJ *= TNum.Sin(s[j - 1]);
              point.Add(semiAxis[j] * TNum.Cos(s[j]) * sinsJ);
            }
          }

          // последнюю координату
          if (dim >= 3) { // У 2Д сферы её нет
            point.Add(semiAxis[dim - 1] * TNum.Cos(s[1]));
          }

          // точка готова, добавляем в наш массив
          Ps.Add(center + new Vector(point.ToArray()));
        }
      }

      return AsVPolytop(Ps);
    }


    /// <summary>
    /// Makes the ball in 1-norm.
    /// </summary>
    /// <param name="dim">The dimension of the ball.</param>
    /// <param name="center">The center of the ball.</param>
    /// <param name="radius">The radius of the ball.</param>
    /// <returns>The ball in 1-norm.</returns>
    public static ConvexPolytop Ball_1(int dim, Vector center, TNum radius) {
      HashSet<Vector> ball = new HashSet<Vector>(2 * dim);
      for (int i = 1; i <= dim; i++) {
        Vector e = radius * Vector.MakeOrth(dim, i) + center;
        ball.Add(e);
        ball.Add(-e);
      }

      return AsVPolytop(ball);
    }

    /// <summary>
    /// Makes the ball in infinity norm.
    /// </summary>
    /// <param name="dim">The dimension of the ball.</param>
    /// <param name="center">The center of the ball.</param>
    /// <param name="radius">The radius of the ball.</param>
    /// <returns>The ball in infinity norm.</returns>
    public static ConvexPolytop Ball_oo(int dim, Vector center, TNum radius) {
      Vector one = Vector.Ones(dim) * radius;

      return RectParallel(-one + center, one + center);
    }

    /// <summary>
    /// Makes the convex polytop which represents the distance to the ball in dim-space.
    /// </summary>
    /// <param name="dim">The dimension of the sphere-ball. It is greater than 1.</param>
    /// <param name="thetaPartition">The number of partitions at zenith angle. Theta in [0, Pi].</param>
    /// <param name="phiPartition">The number of partitions at each azimuthal angle. Phi in [0, 2*Pi).</param>
    /// <param name="radius0">The radius of a initial sphere.</param>
    /// <param name="CMax">The value of the last coordinate of final sphere in dim+1 space.</param>
    /// <returns>The convex polytop which represents the distance to the ball in dim-space. </returns>
    public static ConvexPolytop DistanceToBall_2(int dim, int thetaPartition, int phiPartition, TNum radius0, TNum CMax) {
      ConvexPolytop ball0        = Sphere(dim, thetaPartition, phiPartition, Vector.Zero(dim), radius0);
      TNum          radiusF      = radius0 + CMax;
      ConvexPolytop ballF        = AsVPolytop(ball0.Vertices.Select(v => v * radiusF).ToHashSet());
      ConvexPolytop ball0_lifted = LiftUp(ball0, Tools.Zero);
      ConvexPolytop ballF_lifted = LiftUp(ballF, CMax);
      ball0_lifted.Vertices.UnionWith(ballF_lifted.Vertices); // Теперь тут лежат все точки

      return AsVPolytop(ball0_lifted.Vertices, true);
    }

    /// <summary>
    /// Makes the convex polytop which represents the distance to the given convex polytop in dim-space.
    /// </summary>
    /// <param name="P">Polytop the distance to which is constructed.</param>
    /// <param name="CMax">The value of the last coordinate of P in dim+1 space.</param>
    /// <param name="ballCreator">Function that makes balls.</param>
    /// <returns>A polytope representing the distance to the ball.</returns>
    private static ConvexPolytop DistanceToPolytopBall(
        ConvexPolytop                          P
      , TNum                                   CMax
      , Func<int, Vector, TNum, ConvexPolytop> ballCreator
      ) {
      // R (+) Ball(0, CMax)
      ConvexPolytop bigP = MinkowskiSum.BySandipDas(ballCreator(P.SpaceDim, Vector.Zero(P.SpaceDim), CMax), P);

      //{(R,0), (R (+) Ball(0, CMax),CMax)}
      ConvexPolytop toConv = LiftUp(bigP, CMax);
      toConv.Vertices.UnionWith(LiftUp(P, Tools.Zero).Vertices);

      //conv{...}
      return AsVPolytop(toConv.Vertices, true);
    }

    /// <summary>
    /// Makes the convex polytop which represents the distance in "_1"-norm to the given convex polytop in dim-space.
    /// </summary>
    /// <param name="P">Polytop the distance to which is constructed.</param>
    /// <param name="CMax">The value of the last coordinate of P in dim+1 space.</param>
    /// <returns>A polytope representing the distance to the polytop P in ball_1 norm.</returns>
    public static ConvexPolytop DistanceToPolytopBall_1(ConvexPolytop P, TNum CMax) => DistanceToPolytopBall(P, CMax, Ball_1);

    /// <summary>
    /// Makes the convex polytop which represents the distance in "infinity"-norm to the given convex polytop in dim-space.
    /// </summary>
    /// <param name="P">Polytop the distance to which is constructed.</param>
    /// <param name="CMax">The value of the last coordinate of P in dim+1 space.</param>
    /// <returns>A polytope representing the distance to the polytop P in ball_oo norm.</returns>
    public static ConvexPolytop DistanceToPolytopBall_oo(ConvexPolytop P, TNum CMax) => DistanceToPolytopBall(P, CMax, Ball_oo);

    /// <summary>
    /// Makes the convex polytop which represents the distance in "euclidean"-norm to the given convex polytop in dim-space.
    /// </summary>
    /// <param name="P">Polytop the distance to which is constructed.</param>
    /// <param name="thetaPartition">The number of partitions at zenith angle.</param>
    /// <param name="phiPartition">The number of partitions at each azimuthal angle.</param>
    /// <param name="CMax">The value of the last coordinate of P in dim+1 space.</param>
    /// <returns>A polytope representing the distance to the polytop P in ball_2 norm.</returns>
    public static ConvexPolytop DistanceToPolytopBall_2(ConvexPolytop P, int thetaPartition, int phiPartition, TNum CMax)
      => DistanceToPolytopBall(P, CMax, (dim, center, radius) => Sphere(dim, thetaPartition, phiPartition, center, radius));
#endregion


#region Functions
    /// <summary>
    /// Lifts the given convex polytop to the one above dimension, extends with the given value.
    /// </summary>
    /// <param name="P">The polytop to be lifted.</param>
    /// <param name="val">The value used in expansion.</param>
    /// <returns>The polytop in higher dimension.</returns>
    public static ConvexPolytop LiftUp(ConvexPolytop P, TNum val)
      => AsVPolytop(P.Vertices.Select(v => v.LiftUp(v.Dim + 1, val)).ToHashSet());

    /// <summary>
    /// Create a new convex polytop of (d-1)-dimension from the d-dim convex polytop P as section by given hyperplane.
    /// </summary>
    /// <param name="P">The given convex polytop P.</param>
    /// <param name="hp">The hyper plane to sectioning P.</param>
    /// <returns>The section of the polytop P.</returns>
    public static ConvexPolytop SectionByHyperPlane(ConvexPolytop P, HyperPlane hp) {
      HyperPlane       hp_  = new HyperPlane(-hp.Normal, hp.ConstantTerm);
      List<HyperPlane> hrep = new List<HyperPlane>(P.HRep) { hp, hp_ };

      return AsVPolytop(hp.ABasis.ProjectPoints(HRepToVRep_Naive(hrep)).ToHashSet(), true);
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
    /// <param name="filePath">The path to the file to write in without extension.</param>
    public void WriteTXT_3D(string filePath) {
      Debug.Assert
        (
         PolytopDim == 3
       , $"ConvexPolytop.WriteTXT_3D: The dimension of the polytop must be equal to 3! Found PDim = {PolytopDim}."
        );
      List<Vector> VList = Vertices.Order().ToList();
      Facet[]      FSet  = GW.Get2DFacets();
      using (StreamWriter writer = new StreamWriter(filePath + ".txt")) {
        writer.WriteLine($"PDim: {FL.Top.PolytopDim}");
        writer.WriteLine($"SDim: {SpaceDim}");
        writer.WriteLine();
        writer.WriteLine($"Vertices: {Vertices.Count}");
        writer.WriteLine(string.Join('\n', VList.Select(v => v.ToFileFormat())));
        writer.WriteLine();
        writer.WriteLine($"Faces: {FSet.Length}");
        foreach (Facet face in FSet.OrderBy(F => new Vector(F.Normal))) {
          writer.WriteLine($"N: {face.Normal.ToFileFormat()}");
          writer.WriteLine(string.Join(' ', face.Vertices.Select(v => VList.IndexOf(v))));
        }
      }
    }

    public void WriteTXT_3D_forDasha(string filePath) {
      List<Vector> VList = Vertices.Order().ToList();
      Facet[]      FSet  = GW.Get2DFacets();
      using (StreamWriter writer = new StreamWriter(filePath + ".txt")) {
        writer.Write("[");
        writer.Write(string.Join(',', VList.Select(v => v.ToStringDouble('[', ']'))));
        writer.Write("]");
        writer.WriteLine();
        writer.Write("[");
        foreach (Facet face in FSet) {
          writer.Write("[");
          writer.Write(string.Join(',', face.Vertices.Select(v => VList.IndexOf(v) + 1)));
          writer.Write("],");
        }
        writer.Write("]");
      }
    }

    /// <summary>
    /// Converts the H-representation of convex polytop to the V-representation by checking all possible d-tuples of the hyperplanes.
    /// </summary>
    /// <param name="H">The hyperplane arrangement.</param>
    /// <returns>The V-representation of the convex polytop.</returns>
    public static HashSet<Vector> HRepToVRep_Naive(List<HyperPlane> H) {
      int n = H.Count;
      int d = H.First().Normal.Dim;

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

      return new HashSet<Vector>(Vs);
    }

  }

}
