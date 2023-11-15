using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class MinkowskiSum {

  }

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
  public static FaceLattice MinkSumCH(FLNode P, FLNode Q) =>
    GiftWrapping.WrapFaceLattice(MinkSum(P.Vertices, Q.Vertices));

  public static FaceLattice MinkSumCH(FaceLattice P, FaceLattice Q) => MinkSumCH(P.Top, Q.Top);



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

  private static Point AddPoints(Point p1, Point p2) => new Point(new Vector(p1) + new Vector(p2));

  private static Point CalcInnerPoint(FLNode x, FLNode y) => AddPoints(x.InnerPoint, y.InnerPoint);
  // AddIncidence(FL[d + 1], candidate, zTo_xy);
  // candidate --> xi,yi
  // for all z \in FL[d+1] --> (x,y)
  // xi \in sub(x) && yi \in sub(y)
  // ==> AddSup / AddAbove

  /// <summary>
  /// 
  /// </summary>
  /// <param name="Nodes"></param>
  /// <param name="candidate"></param>
  /// <param name="x"></param>
  // /// <param name="zTo_xy"></param>
  // private static void AddIncidence(
  //         IEnumerable<FLNode> Nodes
  //       , FLNode candidate
  //       , Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy) {


  // }

  public static FaceLattice MinkSumSDas(FaceLattice P, FaceLattice Q) =>
        MinkSumSDas(P.Top, Q.Top);

  public static FaceLattice MinkSumSDas(FLNode P, FLNode Q) {
    AffineBasis affinePQ = new AffineBasis(AddPoints(P.Affine.Origin, Q.Affine.Origin)
                                          , P.Affine.Basis.Concat(Q.Affine.Basis));

    int dim = affinePQ.SpaceDim;
    if (dim == 0) {
      Point s = new Point(new Vector(P.InnerPoint) + new Vector(Q.InnerPoint));
      return new FaceLattice(s);
    }

    if (dim < P.InnerPoint.Dim) { // Пока полагаем, что dim(P (+) Q) == d == Размерности пространства
      throw new NotImplementedException();
      // var x = Q.ProjectTo(affinePQ);
      // FaceLattice lowDim = MinkSumSDas(P.ProjectTo(affinePQ), Q.ProjectTo(affinePQ));
      // return lowDim.TranslateToOriginal(affinePQ);
    }

    Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy = new Dictionary<FLNode, (FLNode x, FLNode y)>();
    Dictionary<(FLNode x, FLNode y), FLNode> xyToz = new Dictionary<(FLNode x, FLNode y), FLNode>();
    List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
    for (int i = 0; i < dim + 1; i++) {
      FL.Add(new HashSet<FLNode>());
    }

    FLNode PQ = new FLNode(MinkSum(P.Vertices, Q.Vertices), CalcInnerPoint(P, Q), affinePQ);
    zTo_xy.Add(PQ, (P, Q));
    xyToz.Add((P, Q), PQ);
    FL[^1].Add(PQ);

    for (int d = dim - 1; d > -1; d--) {
      foreach (FLNode z in FL[d + 1]) {
        (FLNode x, FLNode y) = zTo_xy[z];
        AffineBasis zSpace = new AffineBasis(z.Affine);
        Point innerInAffine_z = zSpace.ProjectPoint(z.InnerPoint);

        //? Теперь я не уверен нужно ли сортировать
        List<FLNode> X = x.GetAllNonStrictSub().OrderByDescending(node => node.Dim).ToList();
        List<FLNode> Y = y.GetAllNonStrictSub().OrderByDescending(node => node.Dim).ToList();

        foreach (FLNode xi in X) {
          foreach (FLNode yi in Y) {

            HashSet<Point> candidate = MinkSum(xi.Vertices, yi.Vertices);
            AffineBasis candBasis = new AffineBasis(AddPoints(xi.Affine.Origin, yi.Affine.Origin)
                                          , xi.Affine.Basis.Concat(yi.Affine.Basis));

            // 0) dim(xi (+) yi) == dim(z) - 1
            if (candBasis.SpaceDim != z.Dim - 1) { continue; }


            // first heuristic dim(xi (+) yi) < dim(z) - 1 ==> нет смысла дальше перебирать
            // if (candBasis.SpaceDim < z.Dim - 1) { break; }


            // 1) Lemma 3
            var candidateInAffine_z = zSpace.ProjectPoints(candidate);
            HyperPlane A = new HyperPlane(new AffineBasis(candidateInAffine_z));
            if (!A.Contains(innerInAffine_z)) { // Если внутренняя точка суммы попала на кандидата
              A.OrientNormal(innerInAffine_z, false); // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из z
            } // else {continue;}

            HashSet<FLNode> xAbove = FLNode.GetFromBottomToTop(xi, x, true);
            HashSet<FLNode> yAbove = FLNode.GetFromBottomToTop(yi, y, true);



            // F = x >= f' > f = xi
            // InnerPoint(f') + InnerPoint(g) \in A^-
            bool xCheck = true;
            foreach (var x_InnerPoint in xAbove.Select(n => n.InnerPoint)) {
              xCheck = xCheck && A.ContainsNegative(zSpace.ProjectPoint(AddPoints(x_InnerPoint, yi.InnerPoint)));
            }

            // G = y >= g' > g = yi
            // InnerPoint(g') + InnerPoint(f) \in A^-
            bool yCheck = true;
            foreach (var y_InnerPoint in yAbove.Select(n => n.InnerPoint)) {
              yCheck = yCheck && A.ContainsNegative(zSpace.ProjectPoint(AddPoints(y_InnerPoint, xi.InnerPoint)));
            }

            // ? Нет смысла дальше перебирать подрешётки xi и yi одновременно, НО как это реализовать?!
            if (!(xCheck && yCheck)) { continue; } //  candidate.Count == 3


            // 2) Does not already exist in FL //! теперь я перестал понимать этот пункт!
            // Если брать (xi, yi), то не понятно зачем. Так как разным xi, yi обязательно соответствуют разные грани (возможно невалидные)
            // то есть в этом случае это выражение не имеет смысла. (всегда ложно).
            // Если брать (x, y), то тоже получается ерунда, так как z = x (+) y, то есть это выражение всегда истинно.
            if (xyToz.ContainsKey((xi, yi))) { continue; }

            FLNode node = new FLNode(candidate, CalcInnerPoint(xi, yi), candBasis);
            FL[node.Dim].Add(node);
            zTo_xy.Add(node, (xi, yi));
            xyToz.Add((xi, yi), node);

            // ? Возможно нужно всем парам (Sub(xi), yi) и (xi, Sub(yi)) в качестве узла указать 'node' (По лемме 4)
            foreach (FLNode prevNode in FL[d + 1]) {
              (FLNode xp, FLNode yq) = zTo_xy[prevNode];

              HashSet<FLNode> Xp = xp.GetAllNonStrictSub();
              HashSet<FLNode> Yq = yq.GetAllNonStrictSub();

              if (Xp.Contains(xi) && Yq.Contains(yi)) {
                node.AddAbove(prevNode);
                prevNode.AddSub(node);
              }
            }
          }
        }
      }
    }
    Debug.Assert(PQ.Sub is not null, "There are NO face lattice!");
    Debug.Assert(FL[0].Count != 0, "There are NO vertices in face lattice!");

    //? Пересобираем Lattice чтобы убрать лишние точки! Можно ли так или лучше всё-таки отдельный класс?
    foreach (HashSet<FLNode> level in FL) {
      foreach (FLNode node in level) {
        node.ResetPolytop();
      }
    }
    return new FaceLattice(FL);
  }

}
