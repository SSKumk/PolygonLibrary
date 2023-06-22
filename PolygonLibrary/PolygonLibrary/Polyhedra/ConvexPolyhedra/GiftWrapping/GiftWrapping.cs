using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    HashSet<Face> Fs = new HashSet<Face>(p.Faces!.Select(F => new Face(F.OriginalVertices, p.Basis)));
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
      (p.OriginalVertices, p.PolyhedronDim, Fs, Es, type, p.Basis, new IncidenceInfo(p.FaceIncidence!), new FansInfo(p.Faces!));
  }

  /*
   * Сделать типа фабрика
   *
   * Face ConstructConvexSwarm()
   *
   * Polyhedron ConstructPolyhedron()
   * 
   * PolyhedralComplex ConstructPolyhedralComplex()
   * 
   * И приватный конструктор (мб кидать исключение)
   *   Если было исключение, то перехватывать и что-то делать.
   *
   * хотелось бы иметь локальное хранилище глобальных переменных
   *
   * Сделать экземпляр класса:
   *  - bool удалось или нет
   *  - многогранник? = null
   *  - базис? = null
   */

  public static BaseSubCP ToConvex(IEnumerable<Point> Swarm) { return GW(Swarm.Select(s => new SubPoint(s, null, s))); }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points for some d.</param>
  /// <param name="FaceBasis">The basis of (d-1)-dimensional subspace in terms of d-space.</param>
  /// <param name="initEdge">The (d-2)-dimensional edge in terms of d-space.</param>
  /// <returns>
  /// The BaseSubCP: (d-1)-dimensional polyhedron complex expressed in terms of d-dimensional points.
  /// </returns>
  public static BaseSubCP BuildFace(IEnumerable<SubPoint> S, AffineBasis FaceBasis, BaseSubCP? initEdge = null) {
    if (initEdge is not null) {
      Debug.Assert(initEdge.PolyhedronDim == S.First().Dim - 2, "The dimension of the initial edge must equal to (d-2)!");
    }
    Debug.Assert(FaceBasis.SpaceDim == S.First().Dim - 1, "The basis must lie in (d-1)-dimensional space!");


    HyperPlane     hyperPlane = new HyperPlane(FaceBasis); //todo если хотим сохранить, то можно сформировать в вызывающей процедуре
    List<SubPoint> inPlane    = new List<SubPoint>();      //todo заметим, хранение разумно только на самом верхнем уровне

    foreach (SubPoint s in S) {
      if (hyperPlane.Contains(s)) {
        inPlane.Add(new SubPoint(s.ProjectTo(FaceBasis), s, s.Original));
      }
    }

    if (inPlane.Count == FaceBasis.VecDim) {
      return new SubSimplex(inPlane.Select(p => p.Parent!));
    } else {
      var x =  GW(inPlane, initEdge).ToPreviousSpace(); //todo
      return x;
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

      return new SubTwoDimensional(convexPolygon2D.Select(v => ((SubPoint2D)v).SubPoint).ToList());
    }

    if (initFace is null) {
      AffineBasis initBasis = BuildInitialPlane(S);

      if (initBasis.SpaceDim < initBasis.VecDim - 1) {
        throw new NotImplementedException(); //todo Может стоит передавать флаг, что делать если рой не полной размерности.
      }

      initFace = BuildFace(S, initBasis);
    }

    //todo По-хорошему бы выкинуть из роя точки, лежащие в плоскости начальной грани, но не являющиеся её вершинами
    // IEnumerable<SubPoint> S1 = S.Where(s => ... ); 


    Debug.Assert(initFace.SpaceDim == S.First().Dim, "The face must lie in d-dimensional space!");
    Debug.Assert(initFace.Faces!.All(F => F.SpaceDim == S.First().Dim), "All edges of the face must lie in d-dimensional space!");
    Debug.Assert(initFace.PolyhedronDim == S.First().Dim - 1, "The dimension of the face must equals to d-1!");
    Debug.Assert(initFace.Faces!.All(F => F.PolyhedronDim == S.First().Dim - 2), "The dimension of all edges must equals to d-2!");

    //todo Вписать Базис обычный. Подумать над SubBasis (скорее всего никуда не деться от него ...)
    HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>() { initFace };
    TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

    DFS_step(S, initFace, ref buildFaces, ref buildIncidence);

    SubIncidenceInfo info = new SubIncidenceInfo();

    foreach (KeyValuePair<BaseSubCP, (BaseSubCP F1, BaseSubCP? F2)> pair in buildIncidence) {
      info.Add(pair.Key, (pair.Value.F1, pair.Value.F2)!);
    }

    return new SubNonSimplex(buildFaces, info);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points.</param>
  /// <param name="face">The (d-1)-dimensional face in d-space.</param>
  /// <param name="buildFaces">Set of (d-1)-dimensional faces in d-space.</param>
  /// <param name="buildIncidence">Dictionary (d-2)-dimensional edge in d-space -->
  /// pair of (d-1)-dimensional faces in d-space.</param>
  public static void DFS_step(IEnumerable<SubPoint>  S
                            , BaseSubCP              face
                            , ref HashSet<BaseSubCP> buildFaces
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
        BaseSubCP nextFace = RollOverEdge(S, face, edge);

        buildFaces.Add(nextFace);
        DFS_step(S, nextFace, ref buildFaces, ref buildIncidence);
      }
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="S">Swarm in d-dimensional space.</param>
  /// <param name="face">(d-1)-dimensional face in d-dimensional space.</param>
  /// <param name="edge">(d-2)-dimensional edge in d-dimensional space.</param>
  /// <returns>(d-1)-dimensional face in d-dimensional space which incident to the face by the edge.</returns>
  public static BaseSubCP RollOverEdge(IEnumerable<SubPoint> S, BaseSubCP face, BaseSubCP edge) {
    Debug.Assert(face.SpaceDim == S.First().Dim, "The face must lie in d-dimensional space!");
    Debug.Assert(face.Faces!.All(F => F.SpaceDim == S.First().Dim), "All edges of the face must lie in d-dimensional space!");
    Debug.Assert(face.PolyhedronDim == S.First().Dim - 1, "The dimension of the face must equals to d-1!");
    Debug.Assert(edge.PolyhedronDim == S.First().Dim - 2, "The dimension of the edge must equals to d-2!");

    AffineBasis edgeBasis = new AffineBasis(edge.Vertices);
    AffineBasis basis_F   = new AffineBasis(edgeBasis);
    basis_F.AddPointToBasis(face.Vertices.First(p => !edge.Vertices.Contains(p)));


    Debug.Assert
      (basis_F.SpaceDim == face.PolyhedronDim, "The dimension of the basis F expressed in terms of edge must equals to F dimension!");

    double  minDot;
    Vector? r = null;

    minDot = double.MaxValue;

    foreach (SubPoint s in S) {
      Vector v = Vector.OrthonormalizeAgainstBasis(s - edgeBasis.Origin, edgeBasis.Basis);

      if (!v.IsZero) {
        double dot = v * basis_F.Basis.Last(); //По идее это должен быть нужный нам вектор, перпенд. E и лежащий в F 

        if (Tools.LT(dot, minDot)) {
          minDot = dot;
          r      = v;
        }
      }
    }

    AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
    newF_aBasis.AddVectorToBasis(r!, false);

    Debug.Assert(newF_aBasis.SpaceDim == face.PolyhedronDim, "The dimension of the basis of new F' must equals to F dimension!");


    return BuildFace(S, newF_aBasis); //todo Научиться проектировать ребро в базис плоскости будущей грани , edge.ProjectTo(basis_F)
  }

  /// <summary>
  /// Procedure 
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
  /// <returns>
  /// Affine basis of the plane, the dimension of the basis is less than d, dimension of the vectors is d.
  /// </returns>
  public static AffineBasis BuildInitialPlane(IEnumerable<SubPoint> S) {
    Debug.Assert(S.Any(), "The swarm must has at least one point!");

    SubPoint           origin = S.Min(p => p)!;
    LinkedList<Vector> tempV  = new LinkedList<Vector>();
    AffineBasis        Basis  = new AffineBasis(origin);

    int dim = Basis.VecDim;

    for (int i = 1; i < dim; i++) {
      tempV.AddLast(Vector.CreateOrth(dim, i + 1));
    }

    HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };

    double    minDot;
    Vector?   r = null;
    SubPoint? sMin;

    while (tempV.Any()) {
      minDot = double.MaxValue;
      sMin   = null;

      foreach (SubPoint s in S) {
        if (Viewed.Contains(s)) {
          continue;
        }

        Vector v = Vector.OrthonormalizeAgainstBasis(s - origin, Basis.Basis);

        if (v.IsZero) {
          Viewed.Add(s);
        } else {
          double dot = v * tempV.First();

          if (Tools.LT(dot, minDot)) {
            minDot = dot;
            r      = v;
            sMin   = s;
          }
        }
      }

      if (sMin is null) {
        return Basis;
      }

      Viewed.Add(sMin);
      tempV.RemoveFirst();
      Basis.AddVectorToBasis(r!, false);
    }

    // HyperPlane hyperPlane = new HyperPlane(Basis); // Мы гарантируем, что все точки лежат с одной стороны от этой плоскости
    // hyperPlane.OrientNormal(hyperPlane.FilterNotIn(S).First(),false);

    return Basis;
  }

}
