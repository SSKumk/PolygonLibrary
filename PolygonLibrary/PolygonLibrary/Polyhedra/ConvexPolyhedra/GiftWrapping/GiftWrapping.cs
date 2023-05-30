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
    var x = GW(Swarm.Select(s => new SubPoint(s, null, s)));

    //todo translate x --> BCP;
    BaseConvexPolyhedron P;

    return P;
  }

  public static BaseSubCP GW(IEnumerable<SubPoint> S) {
    if (S.First().Dim == 2) {
      // todo Как правильно произвести овыпукление S и сохранить связь Point2D <--> SubPoint

      return new TwoDimensional(); // todo Сделать SubTwoDimensional
    }


    //todo Проблема: Базис ребра должен состоять d-2 штук в d-пространстве. Но сейчас он в (d-1)-пространстве.
    (AffineBasis initBasis, SubPoint? notInPlane) = BuildInitialPlane(S);
    BaseSubCP initF = BuildInitialFace(S, initBasis, notInPlane);
    initF.Basis = initBasis; //todo Пока не знаю, как обойтись без setter-а для базиса

    HashSet<BaseSubCP> buildFaces     = new HashSet<BaseSubCP>() { initF };
    TempIncidenceInfo  buildIncidence = new TempIncidenceInfo();

    foreach (BaseSubCP edge in initF.Faces) {
      buildIncidence.Add(edge, (initF, null));
    }

    DFS_step(S, initF, ref buildFaces, ref buildIncidence);

    return new SubNonSimplex(buildFaces, buildIncidence);
  }

  public static void DFS_step(IEnumerable<SubPoint> S, BaseSubCP face, ref HashSet<BaseSubCP> buildFaces, ref TempIncidenceInfo buildIncidence) {
    foreach (BaseSubCP edge in face.Faces) {
      if (buildIncidence[edge].F2 is null) {
        BaseSubCP x = RollOverEdge(S, face, edge);
        buildFaces.Add(x);

        foreach (BaseSubCP rollEdge in x.Faces) { //todo нормально назвать ребро, которое нам сообщили после перекатывания
          if (buildIncidence.ContainsKey(rollEdge)) {
            buildIncidence[rollEdge] = (buildIncidence[rollEdge].F1, x);
          } else {
            buildIncidence.Add(rollEdge, (x, null));
          }
        }

        DFS_step(S, x, ref buildFaces, ref buildIncidence);
      }
    }
  }

  /// <summary>
  /// Procedure 
  /// </summary>
  /// <param name="S">The swarm of d-dimensional points possibly in non general positions.</param>
  /// <returns>
  /// A pair. First item is affine basis of the plane, the dimension of the basis is d-1, dimension of the vectors is d.
  /// The second item is aux, it needed for a establishing a right orientation of a normal to the plane. 
  /// </returns>
  public static (AffineBasis, SubPoint? notInPlane) BuildInitialPlane(IEnumerable<SubPoint> S) {
    Debug.Assert(S.Any(), "The swarm must has at least one point!");

    SubPoint           origin = S.Min(p => p)!;
    LinkedList<Vector> tempV  = new LinkedList<Vector>();
    AffineBasis        Basis  = new AffineBasis(origin);

    int dim = Basis.VecDim;

    for (int i = 1; i < dim; i++) {
      tempV.AddLast(Vector.CreateOrth(dim, i + 1));
    }

    HashSet<SubPoint> Viewed = new HashSet<SubPoint>() { origin };

    SubPoint? notInPlane = null;
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
          notInPlane = s;
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
        return (Basis, null);
      }

      Viewed.Add(sMin);
      tempV.RemoveFirst();
      Basis.AddVectorToBasis(r!, false);
    }

    return (Basis, notInPlane);
  }

  public static BaseSubCP BuildInitialFace(IEnumerable<SubPoint> S, AffineBasis aBasis, SubPoint? notInPlane) {
    if (aBasis.BasisDim < aBasis.VecDim - 1) {
      throw new NotImplementedException("The swarm do not form a convex hull of full dimension!");
    }

    Debug.Assert(notInPlane is not null, "notInPlane is not null");

    //Для ориентации нужно найти точку 's' роя НЕ лежащую в плоскости ==> 
    HyperPlane     hyperPlane = new HyperPlane(aBasis, (notInPlane, false));
    List<SubPoint> inPlane    = new List<SubPoint>();

    foreach (SubPoint s in S) {
      if (hyperPlane.Contains(s)) {
        inPlane.Add(new SubPoint(s.ProjectTo(aBasis), s, s.Original));
      }
    }

    return inPlane.Count == aBasis.VecDim ? new SubSimplex(inPlane) : GW(inPlane);
  }

  public static BaseSubCP RollOverEdge(IEnumerable<SubPoint> S, BaseSubCP face, BaseSubCP edge) {
    Point edgeOrinin = edge.Vertices.First();
    edge.Basis
  }

}
