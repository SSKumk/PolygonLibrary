using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AVLUtils;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class GiftWrapping {

  public static Polyhedron WrapPolyhedron(IEnumerable<Point> Swarm) {
    BaseSubCP p = GW(Swarm.Select(s => new SubPoint(s, null, s)));

    if (p.PolyhedronDim == 2) {
      throw new ArgumentException("P is TwoDimensional! Use ArcHull instead.");
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

    return new Polyhedron(p.OriginalVertices, p.PolyhedronDim, Fs, Es, type, new IncidenceInfo(p.FaceIncidence!), new FansInfo(p.Faces!));
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points for some d.</param>
  /// <param name="FaceBasis">The basis of (d-1)-dimensional subspace in terms of d-space.</param>
  /// <param name="n"></param>
  /// <param name="r"></param>
  /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space.</param>
  /// <returns>
  /// The BaseSubCP: (d-1)-dimensional polyhedron complex expressed in terms of d-dimensional points.
  /// </returns>
  public static BaseSubCP BuildFace(IEnumerable<SubPoint> S
                                  , AffineBasis           FaceBasis
                                  , Vector                n
                                  , Vector?               r        = null
                                  , BaseSubCP?            initEdge = null) {
    if (initEdge is not null) {
      Debug.Assert(initEdge.PolyhedronDim == S.First().Dim - 2, "The dimension of the initial edge must equal to (d-2)!");
      initEdge.Normal = r;
    }
    Debug.Assert(FaceBasis.SpaceDim == S.First().Dim - 1, "The basis must lie in (d-1)-dimensional space!");


    HyperPlane     hyperPlane = new HyperPlane(FaceBasis.Origin, n);
    List<SubPoint> inPlane    = new List<SubPoint>();

    foreach (SubPoint s in S) {
      if (hyperPlane.Contains(s)) {
        inPlane.Add(new SubPoint(s.ProjectTo(FaceBasis), s, s.Original));
      }
    }

    if (inPlane.Count == FaceBasis.VecDim) {
      return new SubSimplex(inPlane.Select(p => p.Parent!));
    } else {
      BaseSubCP? prj = initEdge?.ProjectTo(FaceBasis);

      if (prj is not null) { // 
        prj.Normal = Vector.CreateOrth(FaceBasis.SpaceDim, FaceBasis.SpaceDim);
      }

      return GW(inPlane, prj).ToPreviousSpace();
    }
  }

  /// <summary>
  /// Procedure performing convexification of a swarm of d-dimensional points for some d.
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points to be convexified.</param>
  /// <param name="initFace">
  /// If not null, then this is some (d-1)-dimensional face in terms of d-dimensional space of the convex hull to be constructed.
  /// All (d-2)-dimensional edges also expressed in terms of d-space
  /// </param>
  /// <returns>d-dimensional convex sub polyhedron, which is the convex hull of the given swarm.</returns>
  /// <exception cref="NotImplementedException">Is thrown if the swarm is not of the full dimension.</exception>
  public static BaseSubCP GW(IEnumerable<SubPoint> S, BaseSubCP? initFace = null) {
    if (S.First().Dim == 2) {
      List<Point2D> convexPolygon2D = Convexification.ArcHull2D(S.Select(s => new SubPoint2D(s)));

      if (convexPolygon2D.Count == 3) {
        return new SubSimplex(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
      }

      return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
    }

    if (initFace is null) {
      AffineBasis initBasis = BuildInitialPlane(S);
      // AffineBasis initBasis = BuildInitialPlane(S, out n);
      HyperPlane hp = new HyperPlane(initBasis);
      Vector     n  = hp.Normal;
      OrientNormal(S, ref n, initBasis.Origin);

      if (initBasis.SpaceDim < initBasis.VecDim - 1) {
        throw new NotImplementedException(); //todo Может стоит передавать флаг, что делать если рой не полной размерности.
      }

      initFace        = BuildFace(S, initBasis, n);
      initFace.Normal = n;
    }


    //todo По-хорошему бы выкинуть из роя точки, лежащие в плоскости начальной грани, но не являющиеся её вершинами
    // IEnumerable<SubPoint> S1 = S.Where(s => ... ); 

    Debug.Assert(initFace.Normal is not null, "n is not null");

    Debug.Assert(initFace.SpaceDim == S.First().Dim, "The face must lie in d-dimensional space!");
    Debug.Assert(initFace.Faces!.All(F => F.SpaceDim == S.First().Dim), "All edges of the face must lie in d-dimensional space!");
    Debug.Assert(initFace.PolyhedronDim == S.First().Dim - 1, "The dimension of the face must equals to d-1!");
    Debug.Assert(initFace.Faces!.All(F => F.PolyhedronDim == S.First().Dim - 2), "The dimension of all edges must equals to d-2!");


    HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>() { initFace };
    HashSet<SubPoint>  buildPoints    = new HashSet<SubPoint>(initFace.Vertices);
    TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

    DFS_step(S, initFace, ref buildFaces, ref buildPoints, ref buildIncidence);

    SubIncidenceInfo incidence = new SubIncidenceInfo();

    foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
      incidence.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
    }

    if (buildFaces.Count == S.First().Dim + 1 && buildFaces.All(F => F.Type == SubCPType.Simplex)) {
      return new SubSimplex(buildPoints, buildFaces, incidence);
    }

    return new SubNonSimplex(buildFaces, incidence, buildPoints);
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
  public static void DFS_step(IEnumerable<SubPoint>  S
                            , BaseSubCP              face
                            , ref HashSet<BaseSubCP> buildFaces
                            , ref HashSet<SubPoint>  buildPoints
                            , ref TempIncidenceInfo  buildIncidence) {
    foreach (BaseSubCP edge in face.Faces!) {
      if (buildIncidence.ContainsKey(edge)) {
        if (buildIncidence[edge].F1.GetHashCode() <= face.GetHashCode()) {
          buildIncidence[edge] = (buildIncidence[edge].F1, face);
        } else {
          buildIncidence[edge] = (face, buildIncidence[edge].F1);
        }
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
  public static BaseSubCP RollOverEdge(IEnumerable<SubPoint> S, BaseSubCP face, BaseSubCP edge, out Vector n) {
    Debug.Assert(face.SpaceDim == S.First().Dim, "The face must lie in d-dimensional space!");
    Debug.Assert(face.Faces!.All(F => F.SpaceDim == S.First().Dim), "All edges of the face must lie in d-dimensional space!");
    Debug.Assert(face.PolyhedronDim == S.First().Dim - 1, "The dimension of the face must equals to d-1!");
    Debug.Assert(edge.PolyhedronDim == S.First().Dim - 2, "The dimension of the edge must equals to d-2!");

    AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
    SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
    Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);

    double  minDot;
    Vector? r = null;
    minDot = double.MaxValue;

    Debug.Assert(face.Normal is not null, "face.Normal != null");

    foreach (SubPoint s in S) {
      Vector so = s - edgeBasis.Origin;
      Vector u  = (so * v) * v + (so * face.Normal) * face.Normal;
      u = u.NormalizeZero();

      if (!u.IsZero) {
        double dot = v * u;

        if (Tools.LT(dot, minDot)) {
          minDot = dot;
          r      = u;
        }
      }
    }

    AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
    newF_aBasis.AddVectorToBasis(r!, false);

    Debug.Assert(newF_aBasis.SpaceDim == face.PolyhedronDim, "The dimension of the basis of new F' must equals to F dimension!");

    n = (r! * face.Normal) * v - (r! * v) * face.Normal;
    n = n.Normalize();
    OrientNormal(S, ref n, edgeBasis.Origin);


    return BuildFace(S, newF_aBasis, n, r, edge);
  }


  /// <summary>
  /// Orients the normal outward to the given swarm.
  /// </summary>
  /// <param name="S">The swarm to orient on.</param>
  /// <param name="n">The normal to orient.</param>
  /// <param name="origin">A point from S.</param>
  private static void OrientNormal(IEnumerable<Point> S, ref Vector n, Point origin) {
    foreach (Point s in S) {
      double dot = (s - origin) * n;

      if (Tools.LT(dot)) {
        break;
      }

      if (Tools.GT(dot)) {
        n = -n;

        break;
      }
    }
  }

  /// <summary>
  /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
  /// and all other points lies for a one side from it.
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
  /// <param name="n">The outward normal to the initial plane.</param>
  /// <returns>
  /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
  /// </returns>
  public static AffineBasis BuildInitialPlane(IEnumerable<SubPoint> S) {
    Debug.Assert(S.Any(), "The swarm must has at least one point!");

    SubPoint           origin = S.Min(p => p)!;
    LinkedList<Vector> tempV  = new LinkedList<Vector>();
    AffineBasis        FinalV = new AffineBasis(origin);

    int dim = FinalV.VecDim;

    for (int i = 1; i < dim; i++) {
      tempV.AddLast(Vector.CreateOrth(dim, i + 1));
    }

    HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };

    double    minDot;
    SubPoint? sExtr;

    while (tempV.Any()) {
      Vector t = tempV.First();
      tempV.RemoveFirst();
      minDot = double.MaxValue;
      sExtr  = null;

      foreach (SubPoint s in S) {
        if (Viewed.Contains(s)) {
          continue;
        }

        Vector n = Vector.OrthonormalizeAgainstBasis(s - origin, FinalV.Basis, tempV);

        if (n.IsZero) {
          Viewed.Add(s);
        } else {
          double dot = n * t;

          if (Tools.LT(dot, minDot)) {
            minDot = dot;
            sExtr  = s;
          }
        }
      }

      if (sExtr is null) {
        return FinalV;
      }

      Viewed.Add(sExtr);
      FinalV.AddVectorToBasis(sExtr - origin);
      tempV = new LinkedList<Vector>(Vector.OrthonormalizeAgainstBasis(tempV, FinalV.Basis)); //todo ??????????
    }

    return FinalV;
  }

  // /// <summary>
  // /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
  // /// and all other points lies for a one side from it.
  // /// </summary>
  // /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
  // /// <param name="n">The outward normal to the initial plane.</param>
  // /// <returns>
  // /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
  // /// </returns>
  // public static AffineBasis BuildInitialPlane(IEnumerable<SubPoint> S, out Vector n) {
  //   Debug.Assert(S.Any(), "The swarm must has at least one point!");
  //
  //   SubPoint           origin = S.Min(p => p)!;
  //   LinkedList<Vector> TempV  = new LinkedList<Vector>();
  //   AffineBasis        FinalV = new AffineBasis(origin);
  //
  //   int dim = FinalV.VecDim;
  //
  //   for (int i = 1; i < dim; i++) {
  //     TempV.AddLast(Vector.CreateOrth(dim, i + 1));
  //   }
  //
  //   double[] n_arr = new double[dim];
  //   n_arr[0] = -1;
  //
  //   for (int i = 1; i < dim; i++) {
  //     n_arr[i] = 0;
  //   }
  //   n = new Vector(n_arr);
  //
  //   HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };
  //
  //   double    minDot;
  //   SubPoint? sExtr;
  //
  //   while (TempV.Any()) {
  //     Vector t = TempV.First();
  //     TempV.RemoveFirst();
  //     minDot = double.MaxValue;
  //     sExtr  = null;
  //
  //     Vector v = Vector.OrthonormalizeAgainstBasis(t, TempV);
  //     Vector? u = null;
  //
  //     foreach (SubPoint s in S) {
  //       if (Viewed.Contains(s)) {
  //         continue;
  //       }
  //
  //       u = ((s - origin) * v) * v + ((s - origin) * n) * n;
  //
  //       if (u.IsZero) {
  //         Viewed.Add(s);
  //       } else {
  //         double dot = v * u;
  //
  //         if (Tools.LT(dot, minDot)) {
  //           minDot = dot;
  //           sExtr  = s;
  //         }
  //       }
  //     }
  //
  //     if (sExtr is null) {
  //       return FinalV;
  //     }
  //
  //     Viewed.Add(sExtr);
  //     FinalV.AddVectorToBasis(u!, false);
  //
  //     n = (u! * n) * v + (u! * v) * n;
  //
  //     foreach (SubPoint s in S) {
  //       double dot = new Vector(s) * n;
  //
  //       if (dot < 0) {
  //         break;
  //       }
  //
  //       if (dot > 0) {
  //         n = -n;
  //
  //         break;
  //       }
  //     }
  //   }
  //
  //   return FinalV;
  // }

}
