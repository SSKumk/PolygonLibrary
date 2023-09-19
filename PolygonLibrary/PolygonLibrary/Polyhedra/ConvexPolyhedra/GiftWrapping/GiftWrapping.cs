using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

// /// <param name="curFacet">The (d-1)-dimensional face in d-space.</param>
// /// <param name="buildFaces">Set of (d-1)-dimensional faces in d-space.</param>
// /// <param name="buildPoints"></param>
// /// <param name="buildIncidence">Dictionary (d-2)-dimensional edge in d-space -->
// ///   pair of (d-1)-dimensional faces in d-space.</param>

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>
  where TConv : INumConvertor<TNum> {

  public abstract class GiftWrapping {

#region Internal fields for GW algorithm
    protected HashSet<Point>    SOrig;
    protected HashSet<SubPoint> S;
    protected int               spaceDim;

    protected BaseSubCP? initFacet;
    protected int?       swarmDim;

    protected HashSet<BaseSubCP> buildFacets    = new HashSet<BaseSubCP>();
    protected HashSet<SubPoint>  buildPoints    = new HashSet<SubPoint>();
    protected TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

    protected BaseSubCP? BuiltPolytop;
#endregion

#region Other fields and properties
    private Polytop? _polytop;
    public  Polytop  Polytop => _polytop ??= GetPolytop();

    private List<HyperPlane>? _HRepr;

    public List<HyperPlane> HRepresentation {
      get
        {
          if (_HRepr is null) {
            List<HyperPlane> res = new List<HyperPlane>();
            foreach (Facet facet in Polytop.Facets) {
              res.Add(new HyperPlane(facet.Vertices.First(), facet.Normal));
            }
            _HRepr = res;
          }

          return _HRepr;
        }
    }

    private List<Point>? _VRepr;

    public List<Point> VRepresentation => _VRepr ??= Polytop.Vertices.ToList();


    //todo public SOME_CLASS PolytopComplex() => throw new NotImplementedException();
#endregion

#region Constructors
    protected GiftWrapping(IEnumerable<SubPoint> Swarm) {
      SOrig    = new HashSet<Point>(Swarm.Select(s => s.Original)); // не уверен, что нужно в промежуточных вычислениях
      S        = new HashSet<SubPoint>(Swarm.Select(s => new SubPoint(s, s.Parent, s)));
      spaceDim = S.First().Dim;

      GW();
    }
#endregion

#region Abstracts methods
    public abstract AffineBasis BuildInitialPlane(out Vector initNormal);

    protected abstract BaseSubCP     RollOverEdge(BaseSubCP           facet, BaseSubCP edge, out Vector nNew);
    protected abstract List<Point2D> Convexify2D(HashSet<SubPoint>    S);
    protected abstract BaseSubCP     ConvexifyFacet(HashSet<SubPoint> inPlane, BaseSubCP? normalPrj = null);

    // public abstract BaseSubCP   BuildFacet(AffineBasis       FacetBasis, Vector curNormal, Vector? r = null, BaseSubCP? initEdge = null);
#endregion

#region Common methods
    public void BuildInitialFacet() {
      initFacet = BuildFacet(BuildInitialPlane(out Vector normal), normal);
      buildFacets.Add(initFacet);
      buildPoints.UnionWith(initFacet.Vertices);
    }

    //todo Должна устанавливать внешнюю нормаль у возвращаемой грани!
    protected BaseSubCP BuildFacet(AffineBasis FacetBasis, Vector curNormal, BaseSubCP? initEdge = null) {
      HashSet<SubPoint> inPlane = S.Where(FacetBasis.Contains)
                                   .Select(s => new SubPoint(s.ProjectTo(FacetBasis), s, s.Original))
                                   .ToHashSet();

      // if (inPlane.Count == FacetBasis.VecDim) { ИЗМЕНИЛ!!!
      if (inPlane.Count == FacetBasis.SpaceDim + 1) {
        return new SubSimplex(inPlane.Select(p => p.Parent!));
      } else {
        BaseSubCP? prj = initEdge?.ProjectTo(FacetBasis);

        if (prj is not null) {
          prj.Normal = Vector.CreateOrth(FacetBasis.SpaceDim, FacetBasis.SpaceDim);
        }

        // todo в каком пространстве мы получили результат ???
        BaseSubCP buildedFacet = ConvexifyFacet(inPlane, prj); //.ToPreviousSpace();

        //todo Понять в каком пространстве нам вернулись точки
        // Небольшая эвристика, точки которые лежат внутри выпуклой оболочки можно удалить из роя
        // HashSet<SubPoint> toRemove     = new HashSet<SubPoint>(inPlane.Select(s => s.Parent).ToHashSet()!);
        // toRemove.ExceptWith(buildedFacet.Vertices);
        // S.ExceptWith(toRemove);


        buildedFacet.Normal = curNormal; //todo А она всегда есть ???

        return buildedFacet;
      }
    }


    protected void GW() {
      if (spaceDim == 2 || swarmDim == 2) {
        List<Point2D> convexPolygon2D = Convexify2D(S);

        if (convexPolygon2D.Count == 3) {
          BuiltPolytop = new SubSimplex(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
        }

        BuiltPolytop = new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
      }

      if (initFacet is null) {
        BuildInitialFacet();
      }

#region Debug
      Debug.Assert(initFacet is not null, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet is null!");
      Debug.Assert(initFacet.Normal is not null, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet is null.");
      Debug.Assert(!initFacet.Normal.IsZero, $"GiftWrapping.GW (space dim = {spaceDim}): initial facet has zero length.");
      Debug.Assert
        (
         initFacet.SpaceDim == spaceDim
       , $"GiftWrapping.GW (space dim = {spaceDim}): The initFace must lie in d-dimensional space!"
        );
      Debug.Assert
        (
         initFacet.Faces!.All(F => F.SpaceDim == spaceDim)
       , $"GiftWrapping.GW (space dim = {spaceDim}): All edges of the initFace must lie in d-dimensional space!"
        );
      // Debug.Assert
      //   (
      //    initFacet.PolytopDim == spaceDim - 1
      //  , $"GiftWrapping.GW (space dim = {spaceDim}): The dimension of the initFace must equals to d-1!"
      //   );
      // Debug.Assert
      //   (
      //    initFacet.Faces!.All(F => F.PolytopDim == spaceDim - 2)
      //  , $"GiftWrapping.GW (space dim = {spaceDim}): The dimension of all edges of initFace must equal to d-2!"
      //   );
#endregion

      DFS(initFacet);

      // Подготавливаем и собираем многогранник из накопленных граней
      SubIncidenceInfo incidence = new SubIncidenceInfo();
      foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
        incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
      }
      if (buildFacets.Count == spaceDim + 1 && buildFacets.All(F => F.Type == SubCPType.Simplex)) {
        BuiltPolytop = new SubSimplex(buildPoints, buildFacets, incidence);
      }

      BuiltPolytop = new SubNonSimplex(buildFacets, incidence, buildPoints);
    }

    public void DFS(BaseSubCP curFacet) {
      foreach (BaseSubCP edge in curFacet.Faces!) {
        if (buildIncidence.TryGetValue(edge, out (BaseSubCP F1, BaseSubCP? F2) E)) {
          buildIncidence[edge] = E.F1.GetHashCode() <= curFacet.GetHashCode() ? (E.F1, curFacet) : (curFacet, E.F1);
        } else {
          buildIncidence.Add(edge, (curFacet, null));
        }
      }

      foreach (BaseSubCP edgeToRollOver in curFacet.Faces) {
        if (buildIncidence[edgeToRollOver].F2 is null) {
          BaseSubCP nextFace = RollOverEdge(curFacet, edgeToRollOver, out Vector nNext);
          nextFace.Normal = nNext;
          buildFacets.Add(nextFace);
          buildPoints.UnionWith(nextFace.Vertices);
          DFS(nextFace);
        }
      }
    }
#endregion


    /// <summary>
    /// Calculates the outer normal vector of a given set of points.
    /// </summary>
    /// <param name="S">The set of points.</param>
    /// <param name="planeBasis">The basis of the plane.</param>
    /// <returns>The outer normal vector.</returns>
    protected static Vector CalcOuterNormal(IEnumerable<SubPoint> S, AffineBasis planeBasis) {
      HyperPlane hp = new HyperPlane(planeBasis);
      Vector     n  = hp.Normal;
      OrientNormal(S, ref n, planeBasis.Origin);

      return n;
    }


    /// <summary>
    /// Orients the normal outward to the given swarm.
    /// </summary>
    /// <param name="S">The swarm to orient on.</param>
    /// <param name="normal">The normal to orient.</param>
    /// <param name="origin">A point from S.</param>
    protected static void OrientNormal(IEnumerable<Point> S, ref Vector normal, Point origin) {
      foreach (Point s in S) {
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

    public Polytop GetPolytop() {
      Debug.Assert(BuiltPolytop is not null, "GiftWrapping.GetPolytop(): built polytop is null!");
      HashSet<Facet> Fs = new HashSet<Facet>(BuiltPolytop.Faces!.Select(F => new Facet(F.OriginalVertices, F.Normal!)));
      HashSet<Edge>  Es = new HashSet<Edge>();

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

  }

  // public abstract class GW_SwartInitPlane : GiftWrapping { }
  public abstract class GW_OurInitPlane : GiftWrapping {

    protected GW_OurInitPlane(IEnumerable<SubPoint> Swarm) : base(Swarm) { }

    protected override List<Point2D> Convexify2D(HashSet<SubPoint> S) {
      return Convexification.GrahamHull(S.Select(s => new SubPoint2D(s)));
    }

    public override AffineBasis BuildInitialPlane(out Vector initNormal) {
      Debug.Assert(S.Any(), $"GW_OurInitPlane.BuildInitialPlane (dim = {spaceDim}): The swarm must has at least one point!");

      SubPoint           origin    = S.Min(p => p)!;
      LinkedList<Vector> tempVecs  = new LinkedList<Vector>();
      AffineBasis        FinalVecs = new AffineBasis(origin);

      int dim = FinalVecs.VecDim;

      for (int i = 1; i < dim; i++) {
        tempVecs.AddLast(Vector.CreateOrth(dim, i + 1));
      }

      HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };

      TNum      maxAngle;
      SubPoint? sExtr;

      while (tempVecs.Any()) {
        Vector t = tempVecs.First();
        tempVecs.RemoveFirst();

        maxAngle = -Tools.Six;
        sExtr    = null;

        foreach (SubPoint s in S) {
          if (Viewed.Contains(s)) {
            continue;
          }

          Vector u0 = Vector.OrthonormalizeAgainstBasis(s - origin, FinalVecs.Basis);
          if (u0.IsZero) {
            Viewed.Add(s);

            continue;
          }

          Vector u = Vector.OrthonormalizeAgainstBasis(u0, tempVecs);
          if (u.IsZero) {
            continue;
          }

          TNum angle = Vector.Angle(u, t);
          if (Tools.GT(angle, maxAngle)) {
            maxAngle = angle;
            sExtr    = s;
          }
        }

        if (sExtr is null) {
          /* todo Как в Нашей начальной плоскости определить нормаль в случае, когда рой не полной размерности ???
           * надо перейти в подпространство начальной плоскости
           * в нём определить нормаль ()
           */
          initNormal = Vector.CreateOrth(spaceDim, 1);

          return FinalVecs;
        }

        Viewed.Add(sExtr);
        bool isAdded = FinalVecs.AddVectorToBasis(sExtr - origin);
        Debug.Assert(isAdded, $"GW_OurInitPlane.BuildInitialPlane (dim = {spaceDim}): Vector was not added to FinalVecs!");
        tempVecs = new LinkedList<Vector>(Vector.OrthonormalizeAgainstBasis(tempVecs, FinalVecs.Basis));
      }

      initNormal = CalcOuterNormal(S, FinalVecs);

      return FinalVecs;
    }

  }

  // public class GW_SwartInitPlane_SwartOverEdge : GW_SwartInitPlane { }
  //
  // public class GW_SwartInitPlane_OurOverEdge : GW_SwartInitPlane { }
  //
  // public class GW_OurInitPlane_SwartOverEdge : GW_OurInitPlane { }
  //
  public class GW_OurInitPlane_OurOverEdge : GW_OurInitPlane {

    public GW_OurInitPlane_OurOverEdge(IEnumerable<SubPoint> Swarm) : base(Swarm) { }

    protected override BaseSubCP RollOverEdge(BaseSubCP facet, BaseSubCP edge, out Vector nNew) {
      throw new NotImplementedException();
    }

    protected override BaseSubCP ConvexifyFacet(HashSet<SubPoint> inPlane, BaseSubCP? normalPrj = null) {
      return new GW_OurInitPlane_OurOverEdge(inPlane).BuiltPolytop!;
    }

  }
  //
  // public class GW_Swart : GiftWrapping {
  //
  //   public GW_Swart(IEnumerable<Point> Swarm) : base(Swarm) { }
  //
  //
  //   public override AffineBasis BuildInitialPlane(out Vector n) { throw new NotImplementedException(); }
  //
  //   public override BaseSubCP BuildFacet(AffineBasis FacetBasis, Vector n, Vector? r = null, BaseSubCP? initEdge = null) {
  //     throw new NotImplementedException();
  //   }
  //
  //   public override void RollOverEdge(BaseSubCP facet, BaseSubCP edge, out Vector nNew) { throw new NotImplementedException(); }
  //
  // }

  public class GiftWrappingOld {

//     /// <summary>
//     /// Procedure performing convexification of a swarm of d-dimensional points for some d.
//     /// </summary>
//     /// <param name="S">The swarm of d-dimensional points to be convexified.</param>
//     /// <param name="initFace">
//     /// If not null, then this is some (d-1)-dimensional face in terms of d-dimensional space of the convex hull to be constructed.
//     /// All (d-2)-dimensional edges also expressed in terms of d-space
//     /// </param>
//     /// <returns>d-dimensional convex sub polytop, which is the convex hull of the given swarm.</returns>
//     /// <exception cref="NotImplementedException">Is thrown if the swarm is not of the full dimension.</exception>
//     public static BaseSubCP GW(HashSet<SubPoint> S, BaseSubCP? initFace = null) {
//
//       if (initFace is null) {
//         //Uncomment to include Garret Swart Initial Plane.
//         AffineBasis initBasis = BuildInitialPlaneSwart(S, out Vector n);
//
//         //Uncomment to include Ours Initial Plane.
//         // AffineBasis initBasis = BuildInitialPlaneUs(S, out Vector? n);
//
//         if (n is null) {
//           throw new ArgumentException
//             ($"GW (dim = {spaceDim}): Swarm is flat! Use GW algorithm in suitable space. (dim S = {initBasis.SpaceDim})");
//         }
//         Debug.Assert
//           (initBasis.SpaceDim + 1 == spaceDim, $"GW (dim = {spaceDim}): The dimension of the initial plane must be equal to d-1");
//
//
// #if DEBUG //Контролировать НАСКОЛЬКО далеко точки вылетели из плоскости.
//         HyperPlane                 hp        = new HyperPlane(initBasis.Origin, n);
//         Dictionary<SubPoint, TNum> badPoints = new Dictionary<SubPoint, TNum>();
//         foreach (SubPoint s in S) {
//           TNum d = hp.Eval(s);
//           if (Tools.GT(d)) {
//             badPoints[s] = d;
//           }
//         }
//         if (badPoints.Any()) {
//           throw new ArgumentException($"GW (dim = {spaceDim}): Some points outside the initial plane!");
//         }
// #endif
//
//         if (initBasis.SpaceDim < initBasis.VecDim - 1) {
//           throw new NotImplementedException(); //todo Может стоит передавать флаг, что делать если рой не полной размерности.
//         }
//
//         initFace        = BuildFace(ref S, initBasis, n);
//         initFace.Normal = n;
//       }

//     }


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


//     /// <summary>
//     ///
//     /// </summary>
//     /// <param name="S">Swarm in d-dimensional space.</param>
//     /// <param name="face">(d-1)-dimensional face in d-dimensional space.</param>
//     /// <param name="edge">(d-2)-dimensional edge in d-dimensional space.</param>
//     /// <param name="n"></param>
//     /// <returns>(d-1)-dimensional face in d-dimensional space which incident to the face by the edge.</returns>
//     public static BaseSubCP RollOverEdge(HashSet<SubPoint> S, BaseSubCP face, BaseSubCP edge, out Vector n) {
// #if DEBUG
//       int spaceDim = S.First().Dim;
// #endif
//       Debug.Assert(face.SpaceDim == spaceDim, $"RollOverEdge (dim = {spaceDim}): The face must lie in d-dimensional space!");
//       Debug.Assert
//         (
//          face.Faces!.All(F => F.SpaceDim == spaceDim)
//        , $"RollOverEdge (dim = {spaceDim}): All edges of the face must lie in d-dimensional space!"
//         );
//       Debug.Assert
//         (face.PolytopDim == spaceDim - 1, $"RollOverEdge (dim = {spaceDim}): The dimension of the face must equal to d-1!");
//       Debug.Assert
//         (edge.PolytopDim == spaceDim - 2, $"RollOverEdge (dim = {spaceDim}): The dimension of the edge must equal to d-2!");
//
//       AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
//       SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
//       Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);
//
//       Vector?   r        = null;
//       SubPoint? sStar    = null;
//       TNum      maxAngle = -Tools.Six; // Something small
//
//       Debug.Assert(face.Normal is not null, $"RollOverEdge (dim = {spaceDim}): face.Normal is null");
//       Debug.Assert(!face.Normal.IsZero, $"RollOverEdge (dim = {spaceDim}): face.Normal has zero length");
//
//       foreach (SubPoint s in S) {
//         Vector so = s - edgeBasis.Origin;
//         Vector u  = (so * v) * v + (so * face.Normal) * face.Normal;
//
//         if (!u.IsZero) {
//           TNum angle = Vector.Angle(v, u);
//
//           if (Tools.GT(angle, maxAngle)) {
//             maxAngle = angle;
//             r        = u.Normalize();
//             sStar    = s;
//           }
//         }
//       }
//
//       AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
//       newF_aBasis.AddVectorToBasis(r!, false);
//
//       Debug.Assert
//         (
//          newF_aBasis.SpaceDim == face.PolytopDim
//        , $"RollOverEdge (dim = {spaceDim}): The dimension of the basis of new F' must equals to F dimension!"
//         );
//
//       List<SubPoint> newPlane = new List<SubPoint>(edge.Vertices) { sStar! };
//       newF_aBasis = new AffineBasis(newPlane);
//
//       n = CalcOuterNormal(S, newF_aBasis);
//       Debug.Assert(Tools.EQ(n.Length, Tools.One), $"GW.RollOverEdge (dim = {spaceDim}): New normal is not of length 1.");
//
//
//       return BuildFace(ref S, newF_aBasis, n, r, edge);
//     }


    // /// <summary>
    // /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
    // /// and all other points lies for a one side from it.
    // /// </summary>
    // /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
    // /// <param name="n">The outward normal to the initial plane.</param>
    // /// <returns>
    // /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
    // /// </returns>
    // public static AffineBasis BuildInitialPlaneSwart(HashSet<SubPoint> S, out Vector n) {
    //   int spaceDim = S.First().Dim;
    //   Debug.Assert(S.Any(), $"BuildInitialPlaneSwart (dim = {spaceDim}): The swarm must has at least one point!");
    //
    //   SubPoint    origin = S.Min(p => p)!;
    //   AffineBasis FinalV = new AffineBasis(origin);
    //
    //   n = -Vector.CreateOrth(spaceDim, 1);
    //
    //   while (FinalV.SpaceDim < spaceDim - 1) {
    //     TNum      maxAngle = -Tools.Six; // "Большое отрицательное число."
    //     SubPoint? sExtr    = null;
    //
    //     Vector e;
    //     int    i = 0;
    //     do {
    //       i++;
    //       e = Vector.OrthonormalizeAgainstBasis(Vector.CreateOrth(spaceDim, i), FinalV.Basis, new[] { n });
    //     } while (e.IsZero && i <= spaceDim);
    //     Debug.Assert
    //       (i <= spaceDim, $"BuildInitialPlaneSwart (dim = {spaceDim}): Can't find vector e! That orthogonal to FinalV and n.");
    //
    //     Vector? r = null;
    //     foreach (SubPoint s in S) {
    //       Vector u = ((s - origin) * e) * e + ((s - origin) * n) * n;
    //
    //       if (!u.IsZero) {
    //         TNum angle = Vector.Angle(e, u);
    //
    //         if (Tools.GT(angle, maxAngle)) {
    //           maxAngle = angle;
    //           sExtr    = s;
    //           r        = u.Normalize();
    //         }
    //       }
    //     }
    //
    //     if (sExtr is null) {
    //       return FinalV;
    //     }
    //
    //     bool isAdded = FinalV.AddVectorToBasis(sExtr - origin);
    //
    //     Debug.Assert
    //       (
    //        isAdded
    //      , $"BuildInitialPlaneSwart (dim = {spaceDim}): The new vector of FinalV is linear combination of FinalV vectors!"
    //       );
    //
    //     n = (r! * n) * e - (r! * e) * n;
    //     OrientNormal(S, ref n, origin);
    //     Vector normal = n;
    //     if (S.All(s => new HyperPlane(origin, normal).Contains(s))) {
    //       throw new ArgumentException
    //         (
    //          $"BuildInitialPlaneSwart (dim = {spaceDim}): All points from S lies in initial plane! There are no convex hull of full dimension."
    //         );
    //     }
    //
    //     Debug.Assert(!n.IsZero, $"BuildInitialPlaneSwart (dim = {spaceDim}): Normal is zero!");
    //   }
    //
    //   return FinalV;
    // }

  }

}
