using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;


namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class GiftWrapping {

    private readonly BaseSubCP      BuiltPolytop;
    private          HashSet<Point> SOrig;

    private Polytop? _polytop;
    public  Polytop  Polytop => _polytop ??= GetPolytop();

    public HashSet<Point> Vertices => Polytop.Vertices;
    // public HashSet<Point> Vertices => BuiltPolytop.OriginalVertices;

    private List<HyperPlane>? _HRepr;

    public List<HyperPlane> HRepresentation {
      get
        {
          if (_HRepr is null) {
            List<HyperPlane> res = new List<HyperPlane>();
            foreach (Face facet in Polytop.Faces) {
              res.Add(new HyperPlane(facet.Vertices.First(), facet.Normal));
            }
            _HRepr = res;
          }

          return _HRepr;
        }
    }

    private List<Point>? _VRepr;
    public  List<Point>  VerticesList => _VRepr ??= BuiltPolytop.OriginalVertices.ToList();


    //todo public SOME_CLASS PolytopComplex() => throw new NotImplementedException();
    public GiftWrapping(IEnumerable<Point> Swarm) {
      SOrig = new HashSet<Point>(Swarm);
      GiftWrappingMain x = new GiftWrappingMain(new HashSet<SubPoint>(Swarm.Select(s => new SubPoint(s, null, s))));
      BuiltPolytop = x.BuiltPolytop;
    }

    private Polytop GetPolytop() {
      Debug.Assert(BuiltPolytop is not null, "GiftWrapping.GetPolytop(): built polytop is null!");
      HashSet<Face> Fs = new HashSet<Face>(BuiltPolytop.Faces!.Select(F => new Face(F.OriginalVertices, F.Normal!)));
      HashSet<Edge> Es = new HashSet<Edge>();

      foreach (BaseSubCP face in BuiltPolytop.Faces!) {
        Es.UnionWith(face.Faces!.Select(F => new Edge(F.OriginalVertices)));
      }

      ConvexPolyhedronType type;

      switch (BuiltPolytop.Type) {
        case SubCPType.Simplex:
          type = ConvexPolyhedronType.Simplex;

          break;
        case SubCPType.NonSimplex:
          type = ConvexPolyhedronType.NonSimplex;

          break;
        default: throw new NotImplementedException();
      }

      return new Polytop
        (
         BuiltPolytop.OriginalVertices
       , BuiltPolytop.PolytopDim
       , Fs
       , Es
       , type
       , new IncidenceInfo(BuiltPolytop.FaceIncidence!)
       , new FansInfo(BuiltPolytop.Faces!)
        );
    }


    private sealed class GiftWrappingMain {

#region Internal fields for GW algorithm
      private readonly HashSet<SubPoint> S;
      private readonly int               spaceDim;
      private          BaseSubCP?        initFace;

      private readonly HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>();
      private readonly HashSet<SubPoint>  buildPoints    = new HashSet<SubPoint>();
      private readonly TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

      public readonly BaseSubCP BuiltPolytop;
#endregion

#region Constructors
      public GiftWrappingMain(HashSet<SubPoint> Swarm, BaseSubCP? initFace = null) {
        S             = Swarm;
        spaceDim      = S.First().Dim;
        this.initFace = initFace;

        BuiltPolytop = GW(); // todo 4Дэ КУБ НЕ ЗАВОРАЧИВАЕТСЯ!!!
      }
#endregion

#region Main functions
      private BaseSubCP GW() {
        if (spaceDim == 2) {
          List<Point2D> convexPolygon2D = Convexification.GrahamHull(S.Select(s => new SubPoint2D(s)));

          if (convexPolygon2D.Count == 3) {
            return new SubSimplex(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
          }

          return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
        }


        initFace ??= BuildFace(BuildInitialPlane(out Vector normal), normal);
        buildFaces.Add(initFace);
        buildPoints.UnionWith(initFace.Vertices);

#region Debug
        Debug.Assert(initFace is not null, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet is null!");
        Debug.Assert(initFace.Normal is not null, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet is null.");
        Debug.Assert(!initFace.Normal.IsZero, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet has zero length.");
        Debug.Assert
          (
           initFace.SpaceDim == spaceDim
         , $"GiftWrapping.GW (space dim = {spaceDim}): The initFace must lie in d-dimensional space!"
          );
        Debug.Assert
          (
           initFace.Faces!.All(F => F.SpaceDim == spaceDim)
         , $"GiftWrapping.GW (space dim = {spaceDim}): All edges of the initFace must lie in d-dimensional space!"
          );
        Debug.Assert
          (
           initFace.PolytopDim == spaceDim - 1
         , $"GiftWrapping.GW (space dim = {spaceDim}): The dimension of the initFace must equals to d-1!"
          );
        Debug.Assert
          (
           initFace.Faces!.All(F => F.PolytopDim == spaceDim - 2)
         , $"GiftWrapping.GW (space dim = {spaceDim}): The dimension of all edges of initFace must equal to d-2!"
          );
#endregion

        Queue<BaseSubCP> toTreat = new Queue<BaseSubCP>();
        toTreat.Enqueue(initFace);
        foreach (BaseSubCP edge in initFace.Faces!) {
          buildIncidence.Add(edge, (initFace, null));
        }

        do {
          BaseSubCP face = toTreat.Dequeue();

          foreach (BaseSubCP edge in face.Faces!.Where(edge => buildIncidence[edge].F2 is null)) {
            BaseSubCP nextFace = RollOverEdge(face, edge, out Vector n);
            nextFace.Normal = n;

            buildFaces.Add(nextFace);
            buildPoints.UnionWith(nextFace.Vertices);

            bool hasFreeEdges = false;
            foreach (BaseSubCP newEdge in nextFace.Faces!) {
              if (buildIncidence.TryGetValue(newEdge, out (BaseSubCP F1, BaseSubCP? F2) E)) {
                buildIncidence[newEdge] = E.F1.GetHashCode() <= nextFace.GetHashCode() ? (E.F1, nextFace) : (nextFace, E.F1);
              } else {
                buildIncidence.Add(newEdge, (nextFace, null));
                hasFreeEdges = true;
              }
            }
            if (hasFreeEdges) {
              toTreat.Enqueue(nextFace);
            }
          }
        } while (toTreat.Count != 0);

        // Подготавливаем и собираем многогранник из накопленных граней
        SubIncidenceInfo incidence = new SubIncidenceInfo();
        foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
          incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
        }
        if (buildFaces.Count == spaceDim + 1 && buildFaces.All(F => F.Type == SubCPType.Simplex)) {
          return new SubSimplex(buildPoints, buildFaces, incidence);
        }

        return new SubNonSimplex(buildFaces, incidence, buildPoints);
      }

      /// <summary>
      /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
      /// and all other points lies for a one side from it.
      /// </summary>
      /// <param name="normal">The outward normal to the initial plane.</param>
      /// <returns>
      /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
      /// </returns>
      public AffineBasis BuildInitialPlane(out Vector normal) {
        Debug.Assert(S.Any(), $"BuildInitialPlaneSwart (dim = {spaceDim}): The swarm must has at least one point!");

        SubPoint    origin = S.Min(p => p)!;
        AffineBasis FinalV = new AffineBasis(origin);

        Vector n = -Vector.CreateOrth(spaceDim, 1);

        while (FinalV.SpaceDim < spaceDim - 1) {
          TNum      maxAngle = -Tools.Six; // "Большое отрицательное число."
          SubPoint? sExtr    = null;

          Vector   e;
          int      i      = 0;
          Vector[] nBasis = new[] { n };
          do {
            i++;
            e = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis, nBasis);
          } while (e.IsZero && i <= spaceDim);
          Debug.Assert
            (i <= spaceDim, $"BuildInitialPlaneSwart (dim = {spaceDim}): Can't find vector e! That orthogonal to FinalV and n.");

          Vector? r = null;
          foreach (SubPoint s in S) {
            Vector u = ((s - origin) * e) * e + ((s - origin) * n) * n;

            if (!u.IsZero) {
              TNum angle = Vector.Angle(e, u);

              if (Tools.GT(angle, maxAngle)) {
                maxAngle = angle;
                sExtr    = s;
                r        = u.Normalize();
              }
            }
          }

          if (sExtr is null) { // Что делать если выпуклая оболочка не полной размерности?
            throw new ArgumentException($"GW.BIP (dim = {spaceDim}): All points of S lies in initial plane!");

            // normal = n;

            // return FinalV;
          }

          bool isAdded = FinalV.AddVectorToBasis(sExtr - origin);

          Debug.Assert
            (
             isAdded
           , $"BuildInitialPlaneSwart (dim = {spaceDim}): The new vector of FinalV is linear combination of FinalV vectors!"
            );

          // НАЧАЛЬНАЯ Нормаль Наша (точная)
          i = 0;
          do {
            i++;
            n = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis);
          } while (n.IsZero && i <= spaceDim);

          //НАЧАЛЬНАЯ Нормаль по Сварту
          // n = (r! * n) * e - (r! * e) * n;

          OrientNormal(ref n, origin);
          if (S.All(s => new HyperPlane(origin, n).Contains(s))) {
            throw new ArgumentException
              (
               $"BuildInitialPlaneSwart (dim = {spaceDim}): All points from S lies in initial plane! There are no convex hull of full dimension."
              );
          }

          Debug.Assert(!n.IsZero, $"BuildInitialPlaneSwart (dim = {spaceDim}): Normal is zero!");
        }

        normal = n;

        return FinalV;
      }

      /// <summary>
      /// Builds next face of a polytop.
      /// </summary>
      /// <param name="FaceBasis">The basis of (d-1)-dimensional subspace in terms of d-space.</param>
      /// <param name="n"></param>
      /// <param name="r"></param>
      /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space.</param>
      /// <returns>
      /// The BaseSubCP: (d-1)-dimensional polytop complex expressed in terms of d-dimensional points.
      /// </returns>
      public BaseSubCP BuildFace(AffineBasis FaceBasis, Vector n, Vector? r = null, BaseSubCP? initEdge = null) {
        Debug.Assert
          (FaceBasis.SpaceDim == spaceDim - 1, $"BuildFace (dim = {spaceDim}): The basis must lie in (d-1)-dimensional space!");


        if (initEdge is not null) {
          Debug.Assert
            (
             initEdge.PolytopDim == spaceDim - 2
           , $"BuildFace (dim = {S.First().Dim}): The dimension of the initial edge must equal to (d-2)!"
            );

          initEdge.Normal = r;
        }


        HyperPlane hp = new HyperPlane(FaceBasis.Origin, n);
        HashSet<SubPoint> inPlane = S.Where(s => hp.Contains(s))
                                     .Select(s => new SubPoint(s.ProjectTo(FaceBasis), s, s.Original))
                                     .ToHashSet();

        Debug.Assert(inPlane.Count >= spaceDim, $"BuildFace (dim = {spaceDim}): In plane must be at least d points!");

        if (inPlane.Count == FaceBasis.VecDim) {
          BaseSubCP buildedSimplex = new SubSimplex(inPlane.Select(p => p.Parent!));
          buildedSimplex.Normal = n;

          return buildedSimplex;
        }

        BaseSubCP? prj = initEdge?.ProjectTo(FaceBasis);
        if (prj is not null) { //
          prj.Normal = Vector.CreateOrth(FaceBasis.SpaceDim, FaceBasis.SpaceDim);
        }

        BaseSubCP buildedFace = new GiftWrappingMain(inPlane, prj).BuiltPolytop.ToPreviousSpace();
        buildedFace.Normal = n;

        HashSet<SubPoint> toRemove = new HashSet<SubPoint>(inPlane.Select(s => s.Parent).ToHashSet()!);
        toRemove.ExceptWith(buildedFace.Vertices);
        S.ExceptWith(toRemove);

        return buildedFace;
      }

      /// <summary>
      /// Rolls through a given edge from a given face to a new face and returns its normal vector.
      /// </summary>
      /// <param name="face">(d-1)-dimensional face in d-dimensional space.</param>
      /// <param name="edge">(d-2)-dimensional edge in d-dimensional space.</param>
      /// <param name="n"></param>
      /// <returns>(d-1)-dimensional face in d-dimensional space which incident to the face by the edge.</returns>
      public BaseSubCP RollOverEdge(BaseSubCP face, BaseSubCP edge, out Vector n) {
        Debug.Assert(face.SpaceDim == spaceDim, $"RollOverEdge (dim = {spaceDim}): The face must lie in d-dimensional space!");
        Debug.Assert
          (
           face.Faces!.All(F => F.SpaceDim == spaceDim)
         , $"RollOverEdge (dim = {spaceDim}): All edges of the face must lie in d-dimensional space!"
          );
        Debug.Assert
          (face.PolytopDim == spaceDim - 1, $"RollOverEdge (dim = {spaceDim}): The dimension of the face must equal to d-1!");
        Debug.Assert
          (edge.PolytopDim == spaceDim - 2, $"RollOverEdge (dim = {spaceDim}): The dimension of the edge must equal to d-2!");
        Debug.Assert(face.Normal is not null, $"RollOverEdge (dim = {spaceDim}): face.Normal is null");
        Debug.Assert(!face.Normal.IsZero, $"RollOverEdge (dim = {spaceDim}): face.Normal has zero length");

        AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
        SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
        Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);

        Vector?   r        = null;
        SubPoint? sStar    = null;
        TNum      maxAngle = -Tools.Six; // Something small

        foreach (SubPoint s in S) {
          Vector so = s - edgeBasis.Origin;
          Vector u  = (so * v) * v + (so * face.Normal) * face.Normal;

          if (!u.IsZero) {
            TNum angle = Vector.Angle(v, u);

            if (Tools.GT(angle, maxAngle)) {
              maxAngle = angle;
              r        = u.Normalize();
              sStar    = s;
            }
          }
        }

        AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
        newF_aBasis.AddVectorToBasis(r!, false);

        Debug.Assert
          (
           newF_aBasis.SpaceDim == face.PolytopDim
         , $"RollOverEdge (dim = {spaceDim}): The dimension of the basis of new F' must equals to F dimension!"
          );


        List<SubPoint> newPlane = new List<SubPoint>(edge.Vertices) { sStar! };
        newF_aBasis = new AffineBasis(newPlane);

        //ПЕРЕКАТ точно ЛУЧШЕ точно но в DOUBLE
        n = CalcOuterNormal(newF_aBasis);

        //ПЕРЕКАТ Сварт ЧЕМ "быстро" но в DDOUBLE!!!
        // n = (r! * face.Normal) * v - (r! * v) * face.Normal;
        Debug.Assert(Tools.EQ(n.Length, Tools.One), $"GW.RollOverEdge (dim = {spaceDim}): New normal is not of length 1.");


        return BuildFace(newF_aBasis, n, r, edge);
      }
#endregion

#region Auxiliary functions
      /// <summary>
      /// Calculates the outer normal vector of a given set of points.
      /// </summary>
      /// <param name="planeBasis">The basis of the plane.</param>
      /// <returns>The outer normal vector.</returns>
      private Vector CalcOuterNormal(AffineBasis planeBasis) {
        HyperPlane hp = new HyperPlane(planeBasis);
        Vector     n  = hp.Normal;
        OrientNormal(ref n, planeBasis.Origin);

#if DEBUG
        if (S.All(s => hp.Contains(s))) {
          throw new ArgumentException
            ("CalcOuterNormal: All points from S lies in initial plane! There are no convex hull of full dimension.");
        }
#endif

        return n;
      }

      /// <summary>
      /// Orients the normal outward to the given swarm.
      /// </summary>
      /// <param name="normal">The normal to orient.</param>
      /// <param name="origin">A point from S.</param>
      private void OrientNormal(ref Vector normal, Point origin) {
        foreach (SubPoint s in S) {
          TNum dot = (s - origin) * normal;

          if (Tools.LT(dot)) {
            break;
          }

          if (Tools.GT(dot)) {
            normal = -normal;

            break;
          }
        }
      }
#endregion

    }

    public static Polytop WrapPolytop(IEnumerable<Point> SwarmOrig) {
      HashSet<SubPoint> Swarm = new HashSet<SubPoint>(SwarmOrig.Select(s => new SubPoint(s, null, s)));
      BaseSubCP         p     = GW(Swarm);

      if (p.PolytopDim == 2) {
        throw new ArgumentException("WrapPolytop: P is TwoDimensional! Use ArcHull instead.");
      }

      HashSet<Face> Fs = new HashSet<Face>(p.Faces!.Select(F => new Face(F.OriginalVertices, F.Normal!)));
      HashSet<Edge> Es = new HashSet<Edge>();

      foreach (BaseSubCP face in p.Faces!) {
        Es.UnionWith(face.Faces!.Select(F => new Edge(F.OriginalVertices)));
      }

      ConvexPolyhedronType type;

      switch (p.Type) {
        case SubCPType.Simplex:
          type = ConvexPolyhedronType.Simplex;

          break;
        case SubCPType.NonSimplex:
          type = ConvexPolyhedronType.NonSimplex;

          break;
        default: throw new NotImplementedException();
      }

      return new Polytop
        (p.OriginalVertices, p.PolytopDim, Fs, Es, type, new IncidenceInfo(p.FaceIncidence!), new FansInfo(p.Faces!));
    }

    /// <summary>
    /// Procedure performing convexification of a swarm of d-dimensional points for some d.
    /// </summary>
    /// <param name="S">The swarm of d-dimensional points to be convexified.</param>
    /// <param name="initFace">
    /// If not null, then this is some (d-1)-dimensional face in terms of d-dimensional space of the convex hull to be constructed.
    /// All (d-2)-dimensional edges also expressed in terms of d-space
    /// </param>
    /// <returns>d-dimensional convex sub polytop, which is the convex hull of the given swarm.</returns>
    /// <exception cref="NotImplementedException">Is thrown if the swarm is not of the full dimension.</exception>
    public static BaseSubCP GW(HashSet<SubPoint> S, BaseSubCP? initFace = null) {
      int spaceDim = S.First().Dim;

      if (spaceDim == 2) {
        List<Point2D> convexPolygon2D = Convexification.GrahamHull(S.Select(s => new SubPoint2D(s)));

        if (convexPolygon2D.Count == 3) {
          return new SubSimplex(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
        }

        return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
      }

      if (initFace is null) {
        AffineBasis initBasis = BuildInitialPlaneSwart(S, out Vector n);

        if (n is null) {
          throw new ArgumentException
            ($"GW (dim = {spaceDim}): Swarm is flat! Use GW algorithm in suitable space. (dim S = {initBasis.SpaceDim})");
        }
        Debug.Assert
          (initBasis.SpaceDim + 1 == spaceDim, $"GW (dim = {spaceDim}): The dimension of the initial plane must be equal to d-1");


#if DEBUG //Контролировать НАСКОЛЬКО далеко точки вылетели из плоскости.
        HyperPlane                 hp        = new HyperPlane(initBasis.Origin, n);
        Dictionary<SubPoint, TNum> badPoints = new Dictionary<SubPoint, TNum>();
        foreach (SubPoint s in S) {
          TNum d = hp.Eval(s);
          if (Tools.GT(d)) {
            badPoints[s] = d;
          }
        }
        if (badPoints.Any()) {
          throw new ArgumentException($"GW (dim = {spaceDim}): Some points outside the initial plane!");
        }
#endif

        if (initBasis.SpaceDim < initBasis.VecDim - 1) {
          throw new NotImplementedException(); //todo Может стоит передавать флаг, что делать если рой не полной размерности.
        }

        initFace        = BuildFace(ref S, initBasis, n);
        initFace.Normal = n;
      }

#region Debug
      Debug.Assert(initFace.Normal is not null, "initFace.Normal is null.");
      Debug.Assert(!initFace.Normal.IsZero, "initFace.Normal has zero length.");
      Debug.Assert(initFace.SpaceDim == spaceDim, "The initFace must lie in d-dimensional space!");
      Debug.Assert
        (initFace.Faces!.All(F => F.SpaceDim == spaceDim), "All edges of the initFace must lie in d-dimensional space!");
      Debug.Assert(initFace.PolytopDim == spaceDim - 1, "The dimension of the initFace must equals to d-1!");
      Debug.Assert
        (initFace.Faces!.All(F => F.PolytopDim == spaceDim - 2), "The dimension of all edges of initFace must equal to d-2!");
#endregion

      HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>() { initFace };
      HashSet<SubPoint>  buildPoints    = new HashSet<SubPoint>(initFace.Vertices);
      TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();
      Queue<BaseSubCP>   toTreat        = new Queue<BaseSubCP>();
      toTreat.Enqueue(initFace);
      foreach (BaseSubCP edge in initFace.Faces!) {
        buildIncidence.Add(edge, (initFace, null));
      }


      do {
        BaseSubCP face = toTreat.Dequeue();


        foreach (BaseSubCP edge in face.Faces!.Where(edge => buildIncidence[edge].F2 is null)) {
          BaseSubCP nextFace = RollOverEdge(S, face, edge, out Vector n);
          nextFace.Normal = n;

          bool hasFreeEdges = false;
          foreach (BaseSubCP newEdge in nextFace.Faces!) {
            if (buildIncidence.TryGetValue(newEdge, out (BaseSubCP F1, BaseSubCP? F2) E)) {
              buildIncidence[newEdge] = E.F1.GetHashCode() <= nextFace.GetHashCode() ? (E.F1, nextFace) : (nextFace, E.F1);
            } else {
              buildIncidence.Add(newEdge, (nextFace, null));
              hasFreeEdges = true;
            }
          }

          buildFaces.Add(nextFace);
          buildPoints.UnionWith(nextFace.Vertices);
          if (hasFreeEdges) {
            toTreat.Enqueue(nextFace);
          }
        }
      } while (toTreat.Count != 0);

      SubIncidenceInfo incidence = new SubIncidenceInfo();

      foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
        incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
      }

      if (buildFaces.Count == spaceDim + 1 && buildFaces.All(F => F.Type == SubCPType.Simplex)) {
        return new SubSimplex(buildPoints, buildFaces, incidence);
      }

      return new SubNonSimplex(buildFaces, incidence, buildPoints);
    }

    // /// <summary>
    // /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
    // /// and all other points lies for a one side from it. Also finds outward normal.
    // /// If the dimension of the plane less than d-1 than the normal is null.
    // /// </summary>
    // /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
    // /// <param name="n">The outward normal to the initial plane. Null if the dimension of the plane less than d-1.</param>
    // /// <returns>
    // /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
    // /// </returns>
    // public static AffineBasis BuildInitialPlaneUs(HashSet<SubPoint> S, out Vector? n) {
    //   Debug.Assert(S.Any(), $"GW.BuildInitialPlaneUs (dim = {S.First().Dim}): The swarm must has at least one point!");
    //
    //   SubPoint           origin = S.Min(p => p)!;
    //   LinkedList<Vector> tempV  = new LinkedList<Vector>();
    //   AffineBasis        FinalV = new AffineBasis(origin);
    //
    //   int dim = FinalV.VecDim;
    //
    //   for (int i = 1; i < dim; i++) {
    //     tempV.AddLast(Vector.CreateOrth(dim, i + 1));
    //   }
    //
    //   HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };
    //
    //   TNum      maxAngle;
    //   SubPoint? sExtr;
    //
    //   while (tempV.Any()) {
    //     Vector t = tempV.First();
    //     tempV.RemoveFirst();
    //
    //     maxAngle = -Tools.Six;
    //     sExtr    = null;
    //
    //     foreach (SubPoint s in S) {
    //       if (Viewed.Contains(s)) {
    //         continue;
    //       }
    //
    //       Vector u0 = Vector.OrthonormalizeAgainstBasis(s - origin, FinalV.Basis);
    //       if (u0.IsZero) {
    //         Viewed.Add(s);
    //
    //         continue;
    //       }
    //
    //       Vector u = Vector.OrthonormalizeAgainstBasis(u0, tempV);
    //       if (u.IsZero) {
    //         continue;
    //       }
    //
    //       // TNum angle = TNum.Acos(n * t);
    //       TNum angle = Vector.Angle(u, t);
    //       if (Tools.GT(angle, maxAngle)) {
    //         maxAngle = angle;
    //         sExtr    = s;
    //       }
    //     }
    //
    //     if (sExtr is null) {
    //       n = null;
    //
    //       return FinalV;
    //     }
    //
    //     Viewed.Add(sExtr);
    //     bool isAdded = FinalV.AddVectorToBasis(sExtr - origin);
    //     Debug.Assert(isAdded, $"GW.BuildInitialPlaneUs (dim = {S.First().Dim}): Vector was not added to FinalV!");
    //     tempV = new LinkedList<Vector>(Vector.OrthonormalizeAgainstBasis(tempV, FinalV.Basis));
    //   }
    //
    //   n = CalcOuterNormal(S, FinalV);
    //
    //   return FinalV;
    // }

    /// <summary>
    /// Builds next face of the polytop.
    /// </summary>
    /// <param name="S">The swarm of d-dimensional points for some d.</param>
    /// <param name="FaceBasis">The basis of (d-1)-dimensional subspace in terms of d-space.</param>
    /// <param name="n"></param>
    /// <param name="r"></param>
    /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space.</param>
    /// <returns>
    /// The BaseSubCP: (d-1)-dimensional polytop complex expressed in terms of d-dimensional points.
    /// </returns>
    public static BaseSubCP BuildFace(ref HashSet<SubPoint> S
                                    , AffineBasis           FaceBasis
                                    , Vector                n
                                    , Vector?               r        = null
                                    , BaseSubCP?            initEdge = null) {
#if DEBUG
      int SDim = S.First().Dim;
      Debug.Assert
        (FaceBasis.SpaceDim == SDim - 1, $"BuildFace (dim = {S.First().Dim}): The basis must lie in (d-1)-dimensional space!");

#endif

      if (initEdge is not null) {
#if DEBUG
        Debug.Assert
          (
           initEdge.PolytopDim == SDim - 2
         , $"BuildFace (dim = {S.First().Dim}): The dimension of the initial edge must equal to (d-2)!"
          );
#endif

        initEdge.Normal = r;
      }


      HyperPlane hp = new HyperPlane(FaceBasis.Origin, n);
      HashSet<SubPoint> inPlane = S.Where(s => hp.Contains(s))
                                   .Select(s => new SubPoint(s.ProjectTo(FaceBasis), s, s.Original))
                                   .ToHashSet();

#if DEBUG
      Debug.Assert(inPlane.Count >= SDim, $"BuildFace (dim = {S.First().Dim}): In plane must be at least d points!");
#endif

      if (inPlane.Count == FaceBasis.VecDim) {
        return new SubSimplex(inPlane.Select(p => p.Parent!));
      } else {
        BaseSubCP? prj = initEdge?.ProjectTo(FaceBasis);

        if (prj is not null) { //
          prj.Normal = Vector.CreateOrth(FaceBasis.SpaceDim, FaceBasis.SpaceDim);
        }

        BaseSubCP         buildedFace = GW(inPlane, prj).ToPreviousSpace();
        HashSet<SubPoint> toRemove    = new HashSet<SubPoint>(inPlane.Select(s => s.Parent).ToHashSet()!);
        toRemove.ExceptWith(buildedFace.Vertices);
        S.ExceptWith(toRemove);

        return buildedFace;
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="S">The swarm of d-dimensional points.</param>
    /// <param name="face">The (d-1)-dimensional face in d-space.</param>
    /// <param name="buildFaces">Set of (d-1)-dimensional faces in d-space.</param>
    /// <param name="buildPoints"></param>
    /// <param name="buildIncidence">Dictionary (d-2)-dimensional edge in d-space -->
    ///   pair of (d-1)-dimensional faces in d-space.</param>
    public static void DFS_step(HashSet<SubPoint>      S
                              , BaseSubCP              face
                              , ref HashSet<BaseSubCP> buildFaces
                              , ref HashSet<SubPoint>  buildPoints
                              , ref TempIncidenceInfo  buildIncidence) {
      foreach (BaseSubCP edge in face.Faces!) {
        if (buildIncidence.TryGetValue(edge, out (BaseSubCP F1, BaseSubCP? F2) E)) {
          buildIncidence[edge] = E.F1.GetHashCode() <= face.GetHashCode() ? (E.F1, face) : (face, E.F1);
        } else {
          buildIncidence.Add(edge, (face, null));
        }
      }

      foreach (BaseSubCP edge in face.Faces) {
        if (buildIncidence[edge].F2 is null) {
          BaseSubCP nextFace = RollOverEdge(S, face, edge, out Vector n);
          nextFace.Normal = n;
          buildFaces.Add(nextFace);
          buildPoints.UnionWith(nextFace.Vertices);
          DFS_step(S, nextFace, ref buildFaces, ref buildPoints, ref buildIncidence);
        }
      }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="S">Swarm in d-dimensional space.</param>
    /// <param name="face">(d-1)-dimensional face in d-dimensional space.</param>
    /// <param name="edge">(d-2)-dimensional edge in d-dimensional space.</param>
    /// <param name="n"></param>
    /// <returns>(d-1)-dimensional face in d-dimensional space which incident to the face by the edge.</returns>
    public static BaseSubCP RollOverEdge(HashSet<SubPoint> S, BaseSubCP face, BaseSubCP edge, out Vector n) {
#if DEBUG
      int spaceDim = S.First().Dim;

      Debug.Assert(face.SpaceDim == spaceDim, $"RollOverEdge (dim = {spaceDim}): The face must lie in d-dimensional space!");
      Debug.Assert
        (
         face.Faces!.All(F => F.SpaceDim == spaceDim)
       , $"RollOverEdge (dim = {spaceDim}): All edges of the face must lie in d-dimensional space!"
        );
      Debug.Assert
        (face.PolytopDim == spaceDim - 1, $"RollOverEdge (dim = {spaceDim}): The dimension of the face must equal to d-1!");
      Debug.Assert
        (edge.PolytopDim == spaceDim - 2, $"RollOverEdge (dim = {spaceDim}): The dimension of the edge must equal to d-2!");
      Debug.Assert(face.Normal is not null, $"RollOverEdge (dim = {spaceDim}): face.Normal is null");
      Debug.Assert(!face.Normal.IsZero, $"RollOverEdge (dim = {spaceDim}): face.Normal has zero length");
#endif
      AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
      SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
      Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);

      Vector?   r        = null;
      SubPoint? sStar    = null;
      TNum      maxAngle = -Tools.Six; // Something small

      foreach (SubPoint s in S) {
        Vector so = s - edgeBasis.Origin;
        Vector u  = (so * v) * v + (so * face.Normal) * face.Normal;

        if (!u.IsZero) {
          TNum angle = Vector.Angle(v, u);

          if (Tools.GT(angle, maxAngle)) {
            maxAngle = angle;
            r        = u.Normalize();
            sStar    = s;
          }
        }
      }

      AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
      newF_aBasis.AddVectorToBasis(r!, false);

#if DEBUG
      Debug.Assert
        (
         newF_aBasis.SpaceDim == face.PolytopDim
       , $"RollOverEdge (dim = {spaceDim}): The dimension of the basis of new F' must equals to F dimension!"
        );
#endif


      List<SubPoint> newPlane = new List<SubPoint>(edge.Vertices) { sStar! };
      newF_aBasis = new AffineBasis(newPlane);

      //ПЕРЕКАТ точно ЛУЧШЕ точно но в DOUBLE
      n = CalcOuterNormal(S, newF_aBasis);

      //ПЕРЕКАТ Сварт ЧЕМ "быстро" но в DDOUBLE!!!
      // n = (r! * face.Normal) * v - (r! * v) * face.Normal;
#if DEBUG
      Debug.Assert(Tools.EQ(n.Length, Tools.One), $"GW.RollOverEdge (dim = {spaceDim}): New normal is not of length 1.");
#endif


      return BuildFace(ref S, newF_aBasis, n, r, edge);
    }


    /// <summary>
    /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
    /// and all other points lies for a one side from it.
    /// </summary>
    /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
    /// <param name="normal">The outward normal to the initial plane.</param>
    /// <returns>
    /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
    /// </returns>
    public static AffineBasis BuildInitialPlaneSwart(HashSet<SubPoint> S, out Vector normal) {
      int spaceDim = S.First().Dim;
      Debug.Assert(S.Any(), $"BuildInitialPlaneSwart (dim = {spaceDim}): The swarm must has at least one point!");

      SubPoint    origin = S.Min(p => p)!;
      AffineBasis FinalV = new AffineBasis(origin);

      Vector n = -Vector.CreateOrth(spaceDim, 1);

      while (FinalV.SpaceDim < spaceDim - 1) {
        TNum      maxAngle = -Tools.Six; // "Большое отрицательное число."
        SubPoint? sExtr    = null;

        Vector   e;
        int      i      = 0;
        Vector[] nBasis = new[] { n };
        do {
          i++;
          e = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis, nBasis);
        } while (e.IsZero && i <= spaceDim);
        Debug.Assert
          (i <= spaceDim, $"BuildInitialPlaneSwart (dim = {spaceDim}): Can't find vector e! That orthogonal to FinalV and n.");

        Vector? r = null;
        foreach (SubPoint s in S) {
          Vector u = ((s - origin) * e) * e + ((s - origin) * n) * n;

          if (!u.IsZero) {
            TNum angle = Vector.Angle(e, u);

            if (Tools.GT(angle, maxAngle)) {
              maxAngle = angle;
              sExtr    = s;
              r        = u.Normalize();
            }
          }
        }

        if (sExtr is null) {
          normal = n;

          return FinalV;
        }

        bool isAdded = FinalV.AddVectorToBasis(sExtr - origin);

        Debug.Assert
          (
           isAdded
         , $"BuildInitialPlaneSwart (dim = {spaceDim}): The new vector of FinalV is linear combination of FinalV vectors!"
          );

        // НАЧАЛЬНАЯ Нормаль Наша
        i = 0;
        do {
          i++;
          n = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis);
        } while (n.IsZero && i <= spaceDim);

        //НАЧАЛЬНАЯ Нормаль по Сварту
        // n = (r! * n) * e - (r! * e) * n;

        OrientNormal(S, ref n, origin);
        if (S.All(s => new HyperPlane(origin, n).Contains(s))) {
          throw new ArgumentException
            (
             $"BuildInitialPlaneSwart (dim = {spaceDim}): All points from S lies in initial plane! There are no convex hull of full dimension."
            );
        }

        Debug.Assert(!n.IsZero, $"BuildInitialPlaneSwart (dim = {spaceDim}): Normal is zero!");
      }

      normal = n;

      return FinalV;
    }

    /// <summary>
    /// Calculates the outer normal vector of a given set of points.
    /// </summary>
    /// <param name="S">The set of points.</param>
    /// <param name="planeBasis">The basis of the plane.</param>
    /// <returns>The outer normal vector.</returns>
    private static Vector CalcOuterNormal(IEnumerable<SubPoint> S, AffineBasis planeBasis) {
      HyperPlane hp = new HyperPlane(planeBasis);
      Vector     n  = hp.Normal;
      OrientNormal(S, ref n, planeBasis.Origin);

      if (S.All(s => hp.Contains(s))) {
        throw new ArgumentException
          ("BIP.CalcOuterNormal: All points from S lies in initial plane! There are no convex hull of full dimension.");
      }

      return n;
    }

    /// <summary>
    /// Orients the normal outward to the given swarm.
    /// </summary>
    /// <param name="S">The swarm to orient on.</param>
    /// <param name="n">The normal to orient.</param>
    /// <param name="origin">A point from S.</param>
    private static void OrientNormal(IEnumerable<Point> S, ref Vector n, Point origin) {
      foreach (Point s in S) {
        TNum dot = (s - origin) * n;

        if (Tools.LT(dot)) {
          break;
        }

        if (Tools.GT(dot)) {
          n = -n;

          break;
        }
      }
    }

  }

}
