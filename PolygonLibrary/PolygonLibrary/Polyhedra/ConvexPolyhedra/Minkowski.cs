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
  public static ConvexPolytop MinkSumCH(ConvexPolytop p1, ConvexPolytop p2) =>
    GiftWrapping.WrapPolytop(MinkSum(p1.Vertices, p2.Vertices));
  //  {
  //   HashSet<Point> toCH = new HashSet<Point>();
  //   foreach (Point v1 in p1.Vertices) {
  //     toCH.UnionWith(Shift(p2.Vertices, new Vector(v1)));
  //   }

  //   GiftWrapping gw = new GiftWrapping(toCH);
  //   return gw.CPolytop;
  // }




  // private int Dim(FLNode x, FLNode y) => new AffineBasis(x.Affine, y.Affine).SpaceDim;

  private int Dim(IEnumerable<Point> Ps) => new AffineBasis(Ps).SpaceDim;

  /// <summary>
  /// The Minkowski sum of two sets of points.
  /// </summary>
  /// <param name="A">The first set of points.</param>
  /// <param name="B">The second set of points.</param>
  /// <returns>The Minkowski sum of two sets of points.</returns>
  public static HashSet<Point> MinkSum(IEnumerable<Point> A, IEnumerable<Point> B) {
    HashSet<Point> AB = new HashSet<Point>();
    foreach (Point a in A) {
      AB.UnionWith(Shift(B, new Vector(a)));
    }
    return AB;
  }

  private Point CalcInnerPoint(FLNode x, FLNode y) => new Point(new Vector(x.InnerPoint) + new Vector(y.InnerPoint));
  // AddIncidence(FL[d + 1], ref node);
  // z --> (x,y)
  // node --> xi,yi
  // xi \in sub(x) && yi \in sub(y)
  // ==> AddSup / AddSuper
  private void AddIncidence(IEnumerable<FLNode> Nodes, FLNode node, Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy) {
    (FLNode xi, FLNode yi) = zTo_xy[node];

    foreach (FLNode z in Nodes) {
      (FLNode x, FLNode y) = zTo_xy[z];
      HashSet<FLNode> X = new HashSet<FLNode>(x.Sub!) { x };
      HashSet<FLNode> Y = new HashSet<FLNode>(y.Sub!) { y };
      if (X.Contains(xi) && Y.Contains(yi)) {
        node.AddSuper(z);
        z.AddSub(node);
      }
    }
  }
  public FaceLattice MinkowskiSDas(FaceLattice P, FaceLattice Q) {
    AffineBasis affinePQ = new AffineBasis(P.Top.Affine, Q.Top.Affine);
    int dim = affinePQ.SpaceDim;
    // if (dim < P.Top.InnerPoint.Dim) { // Пока полагаем, что dim(P _+_ Q) == d == Размерности пространства
    //   //todo Научиться проектировать FaceLattice в подпространство
    //   return MinkowskiSDas(P.ProjectTo(affinePQ), Q.ProjectTo(affinePQ));
    // }
    Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy = new Dictionary<FLNode, (FLNode x, FLNode y)>();
    Dictionary<(FLNode x, FLNode y), FLNode> xyToz = new Dictionary<(FLNode x, FLNode y), FLNode>();
    List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
    for (int i = 0; i < dim + 1; i++) {
      FL.Add(new HashSet<FLNode>());
    }

    //? Это точно внутренняя точка PQ ?!
    FLNode PQ = new FLNode(dim, CalcInnerPoint(P.Top, Q.Top), affinePQ);
    zTo_xy.Add(PQ, (P.Top, Q.Top));
    xyToz.Add((P.Top, Q.Top), PQ);
    FL[^1].Add(PQ);

    for (int d = dim - 1; d > -1; d--) {
      foreach (FLNode z in FL[d + 1]) {
        (FLNode x, FLNode y) = zTo_xy[z];
        List<FLNode> X = x.AllSub!.OrderBy(node => node.Dim).ToList();
        List<FLNode> Y = y.AllSub!.OrderBy(node => node.Dim).ToList();
        foreach (FLNode xi in X) {
          foreach (FLNode yi in Y) {

            HashSet<Point> candidate = MinkSum(xi.Vertices, yi.Vertices);
            AffineBasis candBasis = new AffineBasis(xi.Affine, yi.Affine);

            //? Кажется, что условия 0), 1), 2) независимы.
            //? А так как 0) ~d^3 может его не первым ставить?

            // 0) dim(xi _+_ yi) == dim(z) - 1
            if (candBasis.SpaceDim > z.Dim) { continue; }

            // first heuristic dim(xi _+_ yi) < dim(z) - 1 ==> нет смысла дальше перебирать
            if (candBasis.SpaceDim < z.Dim) { break; }

            // 1) Lemma 3
            {
              AffineBasis zSpace = new AffineBasis(z.Affine);
              var inPlane = zSpace.ProjectPoints(candidate);
              var innerInPlane = zSpace.ProjectPoint(z.InnerPoint);
              HyperPlane A = new HyperPlane(new AffineBasis(inPlane), (innerInPlane, false));

              HashSet<FLNode> xSuper = new HashSet<FLNode>(x.Super!) { x };
              HashSet<FLNode> ySuper = new HashSet<FLNode>(y.Super!) { y };

              Point xInPlane = zSpace.ProjectPoint(x.InnerPoint);
              Point yInPlane = zSpace.ProjectPoint(y.InnerPoint);

              //f' > f
              bool xCheck = true;
              foreach (FLNode xSup in xSuper) {
                xCheck = xCheck && A.ContainsNegative(new Point(
                 new Vector(zSpace.ProjectPoint(xSup.InnerPoint))
                  +
                    new Vector(yInPlane)));
              }

              //g' > g
              bool yCheck = true;
              foreach (FLNode ySup in ySuper) {
                yCheck = yCheck && A.ContainsNegative(new Point(
                  new Vector(zSpace.ProjectPoint(ySup.InnerPoint))
                   +
                    new Vector(xInPlane)));
              }


              if (!(xCheck && yCheck)) { break; }
            }

            // 2) Does not already exist in FL
            if (xyToz.ContainsKey((xi, yi))) { continue; }

            FLNode node = new FLNode(z.Dim - 1, candidate, CalcInnerPoint(xi, yi), candBasis);
            zTo_xy.Add(node, (xi, yi));
            xyToz.Add((xi, yi), node);


            AddIncidence(FL[d + 1], node, zTo_xy);
          }
        }
      }
    }
    PQ = new FLNode(dim, FL[0].Select(vertex => vertex.InnerPoint).ToHashSet(), CalcInnerPoint(P.Top, Q.Top), affinePQ);
    return new FaceLattice(PQ.Vertices, PQ);
  }

}
