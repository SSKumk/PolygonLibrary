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
  /// Move to the next point in list
  /// </summary>
  /// <param name="counter">Current counter</param>
  private static void Move(ref int counter) { counter++; }

  /// <summary>
  /// Add the point to the list
  /// </summary>
  /// <param name="list">The list of vertices of polygon to be build</param>
  /// <param name="point">The vertex to be added</param>
  /// <returns>
  /// True if polygon is built already and stop is needed
  /// False otherwise
  /// </returns>
  private static bool AddPoint(List<Point2D> list, Point2D point) {
    if (list.Count != 0 && point.Equals(list[0])) {
      return true;
    }
    list.Add(point);
    return false;
  }

  private static bool MoveGeneric(List<Point2D> list
                                , Point2D       p
                                , Point2D       q
                                , ref int       countP
                                , ref int       countQ
                                , int           skewPSign
                                , int           pHhat_q0
                                , int           qHhat_p0
                                , InsideType    inside) {
    if ((skewPSign >= 0 && qHhat_p0 > 0) || (skewPSign < 0 && pHhat_q0 <= 0)) {
      if (inside == InsideType.InP) {
        if (AddPoint(list, p)) { return true; }
      }
      Move(ref countP);
    } else {
      if (inside == InsideType.InQ) {
        if (AddPoint(list, q)) { return true; }
      }
      Move(ref countQ);
    }
    return false;
  }

  /// <summary>
  /// Intersection of two Convex polygons
  /// </summary>
  /// <param name="P">The first convex polygon</param>
  /// <param name="Q">The second convex polygon</param>
  /// <returns>The resultant convex polygon</returns>
  public static ConvexPolygon IntersectionPolygon(ConvexPolygon P, ConvexPolygon Q) {
    List<Point2D> R      = new List<Point2D>();
    InsideType    inside = InsideType.Unknown;
    int           lenP   = P.Vertices.Count;
    int           lenQ   = Q.Vertices.Count;
    int           countP = 1;
    int           countQ = 1;

    int  repeatCount = 0;
    bool noReturnYet = true;
    do {
      Point2D pred_p = P.Vertices.GetAtCyclic(countP - 1);
      Point2D pred_q = Q.Vertices.GetAtCyclic(countQ - 1);
      Point2D p      = P.Vertices.GetAtCyclic(countP);
      Point2D q      = Q.Vertices.GetAtCyclic(countQ);

      Segment  hat_p  = P.Contour.Edges.GetAtCyclic(countP - 1);
      Segment  hat_q  = Q.Contour.Edges.GetAtCyclic(countQ - 1);
      Vector2D hat_p0 = hat_p.directional.Normalize();
      Vector2D hat_q0 = hat_q.directional.Normalize();

      int skewPSign = Tools.CMP(hat_p0 ^ hat_q0);
      int pHhat_q0  = Tools.CMP(hat_q0 ^ (p - pred_q).NormalizeZero());
      int qHhat_p0  = Tools.CMP(hat_p0 ^ (q - pred_p).NormalizeZero());

      CrossInfo crossInfo = Segment.Intersect(hat_p, hat_q);

      if (crossInfo.crossType == CrossType.SinglePoint) {
        if (Tools.GT(pHhat_q0)) { inside = InsideType.InP; } else if (Tools.GT(qHhat_p0)) {
          inside = InsideType.InQ;
        } //Else keep 'inside'
        if (crossInfo.fTypeS1 == IntersectPointPos.Inner && crossInfo.fTypeS2 == IntersectPointPos.Inner) {
          if (AddPoint(R, crossInfo.fp)) {
            noReturnYet = false;
            continue;
          }
        }
      }

      //No 'meaty' intersection
      if (Tools.EQ(skewPSign) && Tools.LE(hat_p0 * hat_q0) && Tools.LE(pHhat_q0) && Tools.LE(qHhat_p0)) { return null; }

      switch (crossInfo.crossType) {
        case CrossType.NoCross:
          if (MoveGeneric(R, p, q, ref countP, ref countQ, skewPSign, pHhat_q0, qHhat_p0, inside)) {
            noReturnYet = false;
            continue;
          }
          break;
        case CrossType.SinglePoint:
          if (crossInfo.fTypeS1 != IntersectPointPos.End && crossInfo.fTypeS2 == IntersectPointPos.End) {
            if (AddPoint(R, crossInfo.fp)) {
              noReturnYet = false;
              continue;
            }
            Move(ref countQ);
            inside = InsideType.Unknown;
          } else if (crossInfo.fTypeS1 == IntersectPointPos.End && crossInfo.fTypeS2 != IntersectPointPos.End) {
            if (AddPoint(R, crossInfo.fp)) {
              noReturnYet = false;
              continue;
            }
            Move(ref countP);
            inside = InsideType.Unknown;
          } else if (crossInfo.fTypeS1 == IntersectPointPos.End && crossInfo.fTypeS2 == IntersectPointPos.End) {
            if (AddPoint(R, crossInfo.fp)) {
              noReturnYet = false;
              continue;
            }
            Move(ref countP);
            Move(ref countQ);
            inside = InsideType.Unknown;
          } else {
            if (MoveGeneric(R, p, q, ref countP, ref countQ, skewPSign, pHhat_q0, qHhat_p0, inside)) {
              noReturnYet = false;
              continue;
            }
          }
          break;
        case CrossType.Overlap:
          if (crossInfo.sTypeS1 == IntersectPointPos.End && crossInfo.sTypeS2 == IntersectPointPos.Inner) {
            if (AddPoint(R, p)) {
              noReturnYet = false;
              continue;
            }
            Move(ref countP);
            inside = InsideType.InP;
          } else if (crossInfo.sTypeS1 == IntersectPointPos.Inner && crossInfo.sTypeS2 == IntersectPointPos.End) {
            if (AddPoint(R, q)) {
              noReturnYet = false;
              continue;
            }
            Move(ref countQ);
            inside = InsideType.InQ;
          } else { // sTypeSP == E && sTypeSQ == E 
            if (AddPoint(R, crossInfo.sp)) {
              noReturnYet = false;
              continue;
            }
            Move(ref countP);
            Move(ref countQ);
            inside = InsideType.Unknown;
          }
          break;
      }
      repeatCount++;
    } while (noReturnYet && repeatCount < 2 * (lenP + lenQ));

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
