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

  public static Polyhedron WrapPolyhedron(IEnumerable<Point> SwarmOrig) {
    HashSet<SubPoint> Swarm = new HashSet<SubPoint>(SwarmOrig.Select(s => new SubPoint(s, null, s)));
    BaseSubCP         p     = GW(Swarm);

    if (p.PolyhedronDim == 2) {
      throw new ArgumentException("P is TwoDimensional! Use ArcHull instead.");
    } //todo Может TwoDimensional сделать у многогранника?

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

    return new Polyhedron
      (p.OriginalVertices, p.PolyhedronDim, Fs, Es, type, new IncidenceInfo(p.FaceIncidence!), new FansInfo(p.Faces!));
  }

  /// <summary>
  /// Builds next face of the polyhedron.
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points for some d.</param>
  /// <param name="FaceBasis">The basis of (d-1)-dimensional subspace in terms of d-space.</param>
  /// <param name="n"></param>
  /// <param name="r"></param>
  /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space.</param>
  /// <returns>
  /// The BaseSubCP: (d-1)-dimensional polyhedron complex expressed in terms of d-dimensional points.
  /// </returns>
  public static BaseSubCP BuildFace(ref HashSet<SubPoint> S
                                  , AffineBasis           FaceBasis
                                  , Vector                n
                                  , Vector?               r        = null
                                  , BaseSubCP?            initEdge = null) {
#if DEBUG
    int SDim = S.First().Dim;
#endif

    if (initEdge is not null) {
      Debug.Assert(initEdge.PolyhedronDim == SDim - 2, $"BuildFace (dim = {S.First().Dim}): The dimension of the initial edge must equal to (d-2)!");
      initEdge.Normal = r;
    }
    Debug.Assert(FaceBasis.SpaceDim == SDim - 1, $"BuildFace (dim = {S.First().Dim}): The basis must lie in (d-1)-dimensional space!");


    var        x          = S.Select(FaceBasis.Contains);
    HyperPlane hp = new HyperPlane(FaceBasis.Origin, n);
    HashSet<SubPoint> inPlane = S.Where(s => hp.Contains(s))
                                 .Select(s => new SubPoint(s.ProjectTo(FaceBasis), s, s.Original))
                                 .ToHashSet();
    var xx = S.Select(hp.Eval);


    Debug.Assert(inPlane.Count >= SDim, $"BuildFace (dim = {S.First().Dim}): In plane must be at least d points!");

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
  /// Procedure performing convexification of a swarm of d-dimensional points for some d.
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points to be convexified.</param>
  /// <param name="initFace">
  /// If not null, then this is some (d-1)-dimensional face in terms of d-dimensional space of the convex hull to be constructed.
  /// All (d-2)-dimensional edges also expressed in terms of d-space
  /// </param>
  /// <returns>d-dimensional convex sub polyhedron, which is the convex hull of the given swarm.</returns>
  /// <exception cref="NotImplementedException">Is thrown if the swarm is not of the full dimension.</exception>
  public static BaseSubCP GW(HashSet<SubPoint> S, BaseSubCP? initFace = null) {
    if (S.First().Dim == 2) {
      List<Point2D> convexPolygon2D = Convexification.GrahamHull(S.Select(s => new SubPoint2D(s)));

      if (convexPolygon2D.Count == 3) {
        return new SubSimplex(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
      }

      return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
    }

    if (initFace is null) {
      // AffineBasis initBasis = BuildInitialPlane(S, out Vector n);
      // Debug.Assert(initBasis.SpaceDim + 1 == S.First().Dim, "GW: The dimension of the initial plane must be equal to d-1");
      // HyperPlane  hp        = new HyperPlane(initBasis, (initBasis.Origin + n, true));

      AffineBasis initBasis = BuildInitialPlane(S);
      Debug.Assert(initBasis.SpaceDim + 1 == S.First().Dim, $"GW (dim = {S.First().Dim}): The dimension of the initial plane must be equal to d-1");
      HyperPlane hp = new HyperPlane(initBasis);
      Vector     n  = hp.Normal;
      OrientNormal(S, ref n, initBasis.Origin);
      hp.OrientNormal(initBasis.Origin + n, true);


#if DEBUG //Контролировать НАСКОЛЬКО далеко точки вылетели из плоскости.
      Dictionary<SubPoint, double> badPoints = new Dictionary<SubPoint, double>();
      foreach (SubPoint s in S) {
        double d = hp.Eval(s);
        if (Tools.GT(d)) {
          badPoints[s] = d;
        }
      }
      if (badPoints.Any()) {
        throw new ArgumentException($"GW (dim = {S.First().Dim}): Some points outside the initial plane!");
      }
#endif

      if (initBasis.SpaceDim < initBasis.VecDim - 1) {
        throw new NotImplementedException(); //todo Может стоит передавать флаг, что делать если рой не полной размерности.
      }

      //todo ИДЕЯ. Может при возврате из подпространства убирать точки из 'S'? Грань мы знаем, точки в плоскости грани там тоже знаем.
      initFace        = BuildFace(ref S, initBasis, n);
      initFace.Normal = n;
    }
    Debug.Assert(initFace.Normal is not null, "initFace.Normal is null.");
    Debug.Assert(!initFace.Normal.IsZero, "initFace.Normal has zero length.");
    Debug.Assert(initFace.SpaceDim == S.First().Dim, "The initFace must lie in d-dimensional space!");
    Debug.Assert
      (initFace.Faces!.All(F => F.SpaceDim == S.First().Dim), "All edges of the initFace must lie in d-dimensional space!");
    Debug.Assert(initFace.PolyhedronDim == S.First().Dim - 1, "The dimension of the initFace must equals to d-1!");
    Debug.Assert
      (
       initFace.Faces!.All(F => F.PolyhedronDim == S.First().Dim - 2)
     , "The dimension of all edges of initFace must equal to d-2!"
      );


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
  public static void DFS_step(HashSet<SubPoint>      S
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
  public static BaseSubCP RollOverEdge(HashSet<SubPoint> S, BaseSubCP face, BaseSubCP edge, out Vector n) {
    Debug.Assert(face.SpaceDim == S.First().Dim, "RollOverEdge: The face must lie in d-dimensional space!");
    Debug.Assert
      (face.Faces!.All(F => F.SpaceDim == S.First().Dim), "RollOverEdge: All edges of the face must lie in d-dimensional space!");
    Debug.Assert(face.PolyhedronDim == S.First().Dim - 1, "RollOverEdge: The dimension of the face must equal to d-1!");
    Debug.Assert(edge.PolyhedronDim == S.First().Dim - 2, "RollOverEdge: The dimension of the edge must equal to d-2!");

    AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
    SubPoint    f         = face.Vertices.First(p => !edge.Vertices.Contains(p));
    Vector      v         = Vector.OrthonormalizeAgainstBasis(f - edgeBasis.Origin, edgeBasis.Basis);

    Vector? r        = null;
    SubPoint?  sStar    = null;
    double  maxAngle = double.MinValue;

    Debug.Assert(face.Normal is not null, $"RollOverEdge (dim = {S.First().Dim}): face.Normal is null");
    Debug.Assert(!face.Normal.IsZero, $"RollOverEdge (dim = {S.First().Dim}): face.Normal has zero length");

    foreach (SubPoint s in S) {
      Vector so = s - edgeBasis.Origin;
      Vector u  = (so * v) * v + (so * face.Normal) * face.Normal;

      if (!u.IsZero) {
        u = u.Normalize();
        double angle = Math.Acos(v * u);
        // double dot = v * u;

        if (Tools.GT(angle, maxAngle)) {
          maxAngle = angle;
          r        = u;
          sStar    = s;
        }
      }
    }

    AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
    newF_aBasis.AddVectorToBasis(r!, false);

    Debug.Assert
      (
       newF_aBasis.SpaceDim == face.PolyhedronDim
     , $"RollOverEdge (dim = {S.First().Dim}): The dimension of the basis of new F' must equals to F dimension!"
      );

    List<SubPoint> newPlane = new List<SubPoint>(edge.Vertices) { sStar! };
    newF_aBasis = new AffineBasis(newPlane);
    HyperPlane hp = new HyperPlane(newF_aBasis);
    n  = hp.Normal;
    OrientNormal(S, ref n, newF_aBasis.Origin);
    hp.OrientNormal(newF_aBasis.Origin + n, true);

    // n = (r! * face.Normal) * v - (r! * v) * face.Normal;

    Debug.Assert(Tools.EQ(n.Length, 1), $"RollOverEdge (dim = {S.First().Dim}): New normal is not of length 1.");

    OrientNormal(S, ref n, edgeBasis.Origin);

    HyperPlane hpHelp = new HyperPlane(edgeBasis.Origin, n);
    List<double>  j  = S.Select(s => hpHelp.Eval(s)).ToList();

    var x  = new AffineBasis(new List<Point>() { S.ToList()[1], S.ToList()[3] });
    var xx = Vector.OrthonormalizeAgainstBasis(S.ToList()[2] - S.ToList()[1], x.Basis);

    return BuildFace(ref S, newF_aBasis, n, r, edge);
  }


  /// <summary>
  /// Procedure builds initial (d-1)-plane in d-space, which holds at least d points of S
  /// and all other points lies for a one side from it.
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
  /// <returns>
  /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
  /// </returns>
  public static AffineBasis BuildInitialPlane(HashSet<SubPoint> S) {
    Debug.Assert(S.Any(), "The swarm must has at least one point!");

    SubPoint           origin = S.Min(p => p)!;
    LinkedList<Vector> tempV  = new LinkedList<Vector>();
    AffineBasis        FinalV = new AffineBasis(origin);

    int dim = FinalV.VecDim;

    for (int i = 1; i < dim; i++) {
      tempV.AddLast(Vector.CreateOrth(dim, i + 1));
    }

    HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };

    double maxAngle;
    // double    minDot;
    SubPoint? sExtr;

    while (tempV.Any()) {
      Vector t = tempV.First();
      tempV.RemoveFirst();
      maxAngle = double.MinValue;
      // minDot = double.MaxValue;
      sExtr = null;

      foreach (SubPoint s in S) {
        if (Viewed.Contains(s)) {
          continue;
        }

        Vector n0 = Vector.OrthonormalizeAgainstBasis(s - origin, FinalV.Basis);
        if (n0.IsZero) {
          Viewed.Add(s);

          continue;
        }

        Vector n = Vector.OrthonormalizeAgainstBasis(n0, tempV);
        if (n.IsZero) {
          continue;
        }

        double angle = Math.Acos(n * t);
        if (Tools.GT(angle, maxAngle)) {
          maxAngle = angle;
          sExtr    = s;
        }
      }

      if (sExtr is null) {
        return FinalV;
      }

      Viewed.Add(sExtr);
      bool isAdded = FinalV.AddVectorToBasis(sExtr - origin);
      Debug.Assert(isAdded, "BuildInitialPlane: Vector was not added to FinalV!");
      tempV = new LinkedList<Vector>(Vector.OrthonormalizeAgainstBasis(tempV, FinalV.Basis));
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
  // public static AffineBasis BuildInitialPlane(HashSet<SubPoint> S, out Vector n) {
  //   Debug.Assert(S.Any(), "BuildInitialPlane: The swarm must has at least one point!");
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
  //   while (TempV.Any()) {
  //     Vector t = TempV.First();
  //     TempV.RemoveFirst();
  //     double    maxAngle = double.MinValue;
  //     SubPoint? sExtr    = null;
  //
  //     Vector  v = Vector.OrthonormalizeAgainstBasis(t, FinalV.Basis, new[] { n });
  //     Vector? r = null;
  //
  //     foreach (SubPoint s in S) {
  //       Vector u = ((s - origin) * v) * v + ((s - origin) * n) * n;
  //
  //       if (!u.IsZero) {
  //         u = u.Normalize();
  //         double angle = Math.Acos(v * u);
  //
  //         if (Tools.GT(angle, maxAngle)) {
  //           maxAngle = angle;
  //           sExtr    = s;
  //           r        = u;
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
  //     Debug.Assert(isAdded, "BuildInitialPlane: The new vector of FinalV is linear combination of FinalV vectors!");
  //
  //     n = (r! * n) * v - (r! * v) * n;
  //
  //     Debug.Assert(!n.IsZero, "BuildInitialPlane: Normal is zero!");
  //
  //
  //     OrientNormal(S, ref n, origin);
  //   }
  //
  //   return FinalV;
  // }

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

}
