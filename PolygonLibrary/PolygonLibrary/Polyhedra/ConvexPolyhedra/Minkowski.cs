using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  /// <summary>
  /// Shift given swarm by given vector.
  /// </summary>
  /// <param name="S">S to be shifted.</param>
  /// <param name="shift">Vector to shift.</param>
  /// <returns>Shifted swarm.</returns>
  public static List<Point> Shift(IEnumerable<Point> S, Vector shift) { return S.Select(s => new Point(s + shift)).ToList(); }


  /// <summary>
  /// Computes the Minkowski sum of two polytopes.
  /// </summary>
  /// <param name="p1">The first polytop represented as a list.</param>
  /// <param name="p2">The second polytop represented as a list.</param>
  /// <returns>
  /// Returns a list of vertices that make up the Minkowski sum of the two input polytopes.
  /// </returns>
  public static ConvexPolytop MinkSumCH(ConvexPolytop p1, ConvexPolytop p2) {
    HashSet<Point> toCH = new HashSet<Point>();
    foreach (Point v1 in p1.Vertices) {
      toCH.UnionWith(Shift(p2.Vertices, new Vector(v1)));
    }

    GiftWrapping gw = new GiftWrapping(toCH);
    return gw.ConvexPolytop;
  }

}
