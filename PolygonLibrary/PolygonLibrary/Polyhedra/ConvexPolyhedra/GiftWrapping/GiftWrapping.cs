using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AVLUtils;
using PolygonLibrary.Basics;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polyhedra.ConvexPolyhedra.GiftWrapping;

public class GiftWrapping {

  public static BaseConvexPolyhedron ToConvex(IEnumerable<Point> Swarm) {
    BaseSubCP x = GW(Swarm.Select(s => new SubPoint(s, null, s)));

    //todo translate x --> BCP;
    BaseConvexPolyhedron P;

    return P;
  }

  /// <summary>
  /// Procedure performing convexification of a swarm of d-dimensional points.
  /// </summary>
  /// <param name="S">The swarm to be convexified.</param>
  /// <param name="initFace">
  /// If not null, then this is some face of the convex hull to be constructed.
  /// The face consists of d-dimensional sub-points and has its (d-1)-dimensional affine basis,
  /// which includes d-1 d-dimensional vectors of the original space.</param>
  /// <returns>d-dimensional convex sub polyhedron, which is the convex hull of the given swarm.</returns>
  /// <exception cref="NotImplementedException">Is thrown if the swarm is not of the full dimension.</exception>
  public static BaseSubCP GW(IEnumerable<SubPoint> S, BaseSubCP? initFace = null) {
    if (S.First().Dim == 2) {
      // todo Как правильно произвести овыпукление S и сохранить связь Point2D <--> SubPoint

      /*
       * Сделать SubPoint2D : Point2D
       * Заслать в ArcHull2D(), так как он не создаёт новых точек, то наши SubPoint-ы будут жить!
       * Каждый Point2D --> SubPoint2D --> SubPoint
       *
       * Создаём грань
       */

      return new SubTwoDimensional(); // todo Сделать SubTwoDimensional
    }

    //todo Проблема: Базис ребра должен состоять d-2 штук в d-пространстве. Но сейчас он в (d-1)-пространстве.
    if (initFace is null) {
      AffineBasis initBasis = BuildInitialPlane(S);

      if (initBasis.BasisDim < initBasis.VecDim - 1) {
        throw new NotImplementedException(); //todo Может стоит передавать флаг, что делать если рой не полной размерности.
      }

      initFace = BuildFace(S, initBasis);
    }

    //todo По-хорошему бы выкинуть из роя точки, лежащие в плоскости начальной грани, но не являющиеся её вершинами
    // IEnumerable<SubPoint> S1 = S.Where(s => ... ); 

    HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>() { initFace };
    TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

    DFS_step(S, initFace, ref buildFaces, ref buildIncidence);

    return new SubNonSimplex(buildFaces, buildIncidence);
  }

  public static void DFS_step(IEnumerable<SubPoint>  S
                            , BaseSubCP              face
                            , ref HashSet<BaseSubCP> buildFaces
                            , ref TempIncidenceInfo  buildIncidence) {
    foreach (BaseSubCP edge in face.Faces) {
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

          if (dot < minDot) {
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

  public static BaseSubCP BuildFace(IEnumerable<SubPoint> S, AffineBasis aBasis) {
    //todo Для ориентации нужно найти точку 's' роя НЕ лежащую в плоскости. Надо ли??? 
    HyperPlane     hyperPlane = new HyperPlane(aBasis); //todo если хотим сохранить, то можно сформировать в вызывающей процедуре
    List<SubPoint> inPlane    = new List<SubPoint>();   //todo заметим, хранение разумно только на самом верхнем уровне

    foreach (SubPoint s in S) {
      if (hyperPlane.Contains(s)) {
        inPlane.Add(new SubPoint(s.ProjectTo(aBasis), s, s.Original));
      }
    }

    return inPlane.Count == aBasis.VecDim ? new SubSimplex(inPlane) : GW(inPlane);
  }

  public static BaseSubCP RollOverEdge(IEnumerable<SubPoint> S, BaseSubCP face, BaseSubCP edge) {
    AffineBasis edgeBasis = BuildEdgeBasis(S, edge); //todo Где его взять?? (d-2)-штук d-мерных

    AffineBasis basisF_ = new AffineBasis(edgeBasis);
    basisF_.AddPointToBasis(face.Vertices.First(p => !edge.Vertices.Contains(p)));


    Debug.Assert(basisF_.BasisDim == face.Basis.BasisDim, "The dimension of the new basis F' must equals to F dimension!");

    double  minDot;
    Vector? r = null;

    minDot = double.MaxValue;

    foreach (SubPoint s in S) {
      Vector v = Vector.OrthonormalizeAgainstBasis(s - edgeBasis.Origin, edgeBasis.Basis);

      if (!v.IsZero) {
        double dot = v * basisF_.Basis.Last(); //По идее это должен быть нужный нам вектор, перпенд. E и лежащий в F 

        if (dot < minDot) {
          minDot = dot;
          r      = v;
        }
      }
    }

    AffineBasis newF_aBasis = new AffineBasis(edgeBasis);
    newF_aBasis.AddVectorToBasis(r!, false);

    return ?????;
  }

}
