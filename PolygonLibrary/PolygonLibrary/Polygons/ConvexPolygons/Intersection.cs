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
  private enum InsideType { Unknown, InP, InQ }

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

  /// <summary>
  /// Moves to next vertex and if needed current vertex added to list 
  /// </summary>
  /// <param name="R">The list of vertices of polygon to be build</param>
  /// <param name="vertex">The vertex to be added</param>
  /// <param name="countVertex">Current counter</param>
  /// <param name="needAdd">The flag determines will vertex be added or not</param>
  private static void Move(List<Point2D> R, Point2D vertex, ref int countVertex, bool needAdd) {
    if (needAdd) {
      AddPoint(R, vertex);
    }

    countVertex++;
  }

  /// <summary>
  /// Intersection of two Convex polygons
  /// </summary>
  /// <param name="P">The first convex polygon</param>
  /// <param name="Q">The second convex polygon</param>
  /// <returns>The resultant convex polgon</returns>
  public static ConvexPolygon IntersectionPolygon(ConvexPolygon P, ConvexPolygon Q) {
    List<Point2D> R      = new List<Point2D>();
    InsideType    inside = InsideType.Unknown;

    int lenP   = P.Vertices.Count;
    int lenQ   = Q.Vertices.Count;
    int countP = 1;
    int countQ = 1;

    int repeatCount = 0;
    do {
      Point2D pred_p = P.Vertices.GetAtCyclic(countP - 1);
      Point2D pred_q = Q.Vertices.GetAtCyclic(countQ - 1);
      Point2D p      = P.Vertices.GetAtCyclic(countP);
      Point2D q      = Q.Vertices.GetAtCyclic(countQ);

      Segment  hat_p  = P.Contour.Edges.GetAtCyclic(countP - 1);
      Segment  hat_q  = Q.Contour.Edges.GetAtCyclic(countQ - 1);
      Vector2D hat_p0 = hat_p.directional.Normalize();
      Vector2D hat_q0 = hat_q.directional.Normalize();

      int cross    = Tools.CMP(hat_p0 ^ hat_q0);
      int pHhat_q0 = Tools.CMP(hat_q0 ^ (p - pred_q).NormalizeZero());
      int qHhat_p0 = Tools.CMP(hat_p0 ^ (q - pred_p).NormalizeZero());

      CrossInfo crossInfo = Segment.Intersect(hat_p, hat_q);

      if (crossInfo.crossType == CrossType.SinglePoint) {
        if (R.Count > 2) {
          if (crossInfo.p.Equals(R.First())) {
            return new ConvexPolygon(R);
          }
        }

        if (pHhat_q0 > 0) {
          inside = InsideType.InP;
        } else if (qHhat_p0 > 0) {
          inside = InsideType.InQ;
        } //Else keep 'inside'

        AddPoint(R, crossInfo.p);
      }

      switch (crossInfo.crossType) {
        case CrossType.Overlap when hat_p0 * hat_q0 < 0: return null; //todo И страдать правильным образом
        case CrossType.Overlap:
          if (inside == InsideType.InP) {
            Move(R, crossInfo.p, ref countQ, false);
          } else {
            Move(R, crossInfo.p, ref countP, false);
          }
          break;
        case CrossType.NoCross:
        case CrossType.SinglePoint: {
          if (Tools.GE(cross, 0)) {
            if (qHhat_p0 > 0) {
              Move(R, p, ref countP, inside == InsideType.InP);
            } else {
              Move(R, q, ref countQ, inside == InsideType.InQ);
            }
          } else {
            if (pHhat_q0 > 0) {
              Move(R, q, ref countQ, inside == InsideType.InQ);
            } else {
              Move(R, p, ref countP, inside == InsideType.InP);
            }
          }
          //todo Костыль!
          if (R.Count > 2 && R.Last().Equals(R.First())) {
            R.RemoveAt(R.Count - 1);
            return new ConvexPolygon(R);
          }
          break;
        }
      }
      repeatCount++;
    } while (repeatCount < 2 * (lenP + lenQ));

    
    //If the intersection is a point or a segment, then we assume that the intersection is empty  
    if (R.Count == 1 || R.Count == 2) {
      return null;
    }

    if (R.Count > 2) {
      return new ConvexPolygon(R);
    }

    if (Q.ContainsInside(P.Vertices.First())) {
      return P;
    }

    if (P.ContainsInside(Q.Vertices.First())) {
      return Q;
    }

    return null;
  }
#endregion

}
