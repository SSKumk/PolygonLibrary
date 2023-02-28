using System;
using System.Collections.Generic;
using System.Linq;
using PolygonLibrary.Basics;
using PolygonLibrary.Segments;
using PolygonLibrary.Toolkit;

namespace PolygonLibrary.Polygons.ConvexPolygons;

public partial class ConvexPolygon {

  #region Convex polygon utilities

  /// <summary>
  /// Flags for IntersectionPolygon procedure
  /// </summary>
  private enum InsideType {
    Unknown
  , InP
  , InQ
  }

  /// <summary>
  /// Smart adding a point to a list
  /// </summary>
  /// <param name="P">The list whereto the point should be added</param>
  /// <param name="k">The point to be added</param>
  private static void AddPoint(List<Point2D> P, Point2D k) {
    if (P.Count == 0 || k != P.Last()) {
      P.Add(k);
    }
  }

  private static void Move(List<Point2D> R, Point2D vertex, ref int countVertex, bool needAdd) {
    if (needAdd) {
      R.Add(vertex);
    }

    countVertex++;
  }

  /// <summary>
  /// Intersection of two Convex polygons
  /// </summary>
  /// <param name="P">The first convex polygon</param>
  /// <param name="Q">The second convex polygon</param>
  /// <returns>The resultant convex polgon</returns>
  public ConvexPolygon IntersectionPolygon(ConvexPolygon P, ConvexPolygon Q) {
    List<Point2D> R      = new List<Point2D>();
    InsideType    inside = InsideType.Unknown;

    int     lenP   = P.Vertices.Count;
    int     lenQ   = Q.Vertices.Count;
    int     countP = 1;
    int     countQ = 1;
    Point2D p      = P.Vertices[0];
    Point2D q      = Q.Vertices[0];
    Point2D pred_p;
    Point2D pred_q;
    do {
      pred_p = p;
      pred_q = q;
      p      = P.Vertices.GetAtCyclic(countP);
      q      = Q.Vertices.GetAtCyclic(countQ);

      Segment   hat_p     = P.Contour.Edges.GetAtCyclic(countP - 1);
      Segment   hat_q     = Q.Contour.Edges.GetAtCyclic(countQ - 1);
      CrossInfo crossInfo = Segment.Intersect(hat_p, hat_q);
      Vector2D  hat_p0    = hat_p.directional.Normalize();
      Vector2D  hat_q0    = hat_q.directional.Normalize();

      int qInHp = Tools.CMP((q - pred_p).Normalize() ^ hat_p0);
      int pInHq = Tools.CMP((p - pred_q).Normalize() ^ hat_q0);
      if (crossInfo.crossType == CrossType.SinglePoint) {
        if (R.Count != 0) {
          if (crossInfo.p.Equals(R.First())) {
            return new ConvexPolygon(R);
          }
        }

        if (pInHq > 0) {
          inside = InsideType.InP;
        }
        else if (qInHp > 0) {
          inside = InsideType.InQ;
        }

        //Else keep 'inside'
        AddPoint(R, p);
      }

      int cross = Tools.CMP(hat_p.directional ^ hat_q.directional);
      switch (crossInfo.crossType) {
        case CrossType.Overlap when hat_p.directional * hat_q.directional < 0:
          return null; //todo И страдать правильным образом
        case CrossType.Overlap:
          Move(R, crossInfo.p, ref countP, false);

          break;
        default: {
          if (Tools.GE(cross, 0)) {
            if (qInHp > 0) {
              Move(R, crossInfo.p, ref countP, inside == InsideType.InP);
            }
            else {
              Move(R, crossInfo.p, ref countQ, inside == InsideType.InQ);
            }
          }
          else {
            if (pInHq > 0) {
              Move(R, crossInfo.p, ref countQ, inside == InsideType.InQ);
            }
            else {
              Move(R, crossInfo.p, ref countP, inside == InsideType.InP);
            }
          }

          break;
        }
      }
    } while ((countP < lenP || countQ < lenQ) && countP < 2 * lenP && countQ < 2 * lenQ);

    if (R.Count != 0) {
      return new ConvexPolygon(R);
    }

    if (Q.Contains(P.Vertices.First())) {
      return P;
    }

    if (P.Contains(Q.Vertices.First())) {
      return Q;
    }

    return null;
  }

  #endregion

}