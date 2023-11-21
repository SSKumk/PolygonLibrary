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

  //todo Когда Сумма Минковского будет готова, перенести всё в этот класс
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
  /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
  /// </summary>
  /// <param name="p1">The first polytop represented as a list.</param>
  /// <param name="p2">The second polytop represented as a list.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumCH(FLNode P, FLNode Q) =>
    GiftWrapping.WrapFaceLattice(MinkSum(P.Vertices, Q.Vertices));

  /// <summary>
  /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
  /// </summary>
  /// <param name="p1">The first polytop represented as a ConvexPolytop.</param>
  /// <param name="p2">The second polytop represented as a ConvexPolytop.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumCH(ConvexPolytop P, ConvexPolytop Q) =>
    GiftWrapping.WrapFaceLattice(MinkSum(P.Vertices, Q.Vertices));

  /// <summary>
  /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
  /// </summary>
  /// <param name="p1">The first polytop represented as a face lattice.</param>
  /// <param name="p2">The second polytop represented as a face lattice.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumCH(FaceLattice P, FaceLattice Q) => MinkSumCH(P.Top, Q.Top);

  /// <summary>
  /// The Minkowski sum of two sets of points. S1 (+) S2 = {p | p = p1 + p2, ∀p1 ∈ S1 and ∀p2 ∈ S2}
  /// </summary>
  /// <param name="A">The first set of points.</param>
  /// <param name="B">The second set of points.</param>
  /// <returns>The set of points represented the Minkowski sum of two sets of points.</returns>
  public static HashSet<Point> MinkSum(IEnumerable<Point> A, IEnumerable<Point> B) {
    HashSet<Point> AB = new HashSet<Point>();
    foreach (Point a in A) {
      AB.UnionWith(Shift(B, new Vector(a)));
    }
    return AB;
  }

  /// <summary>
  /// Computes the Minkowski sum of two polytopes via face lattice algorithm.
  /// </summary>
  /// <param name="p1">The first polytop represented as a face lattice.</param>
  /// <param name="p2">The second polytop represented as a face lattice.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumSDas(FaceLattice P, FaceLattice Q) {
    // Вычисляю аффинное пространство суммы P и Q
    // Начало координат складываю как точки. А вектора поочерёдно добавляем в базис (если можем).
    AffineBasis affinePQ = new AffineBasis(P.Top.AffBasis.Origin + Q.Top.AffBasis.Origin
                                      , P.Top.AffBasis.Basis.Concat(Q.Top.AffBasis.Basis));

    int dim = affinePQ.SpaceDim;
    if (dim == 0) { // Случай точки обработаем отдельно
      Point s = new Point(new Vector(P.Top.InnerPoint) + new Vector(Q.Top.InnerPoint));
      return new FaceLattice(s);
    }

    if (dim < P.Top.InnerPoint.Dim) { // Уходим в подпространство
      // FaceLattice lowDim = MinkSumSDas(P.ProjectTo(affinePQ).Top, Q.ProjectTo(affinePQ).Top);
      // return lowDim.TranslateToOriginal(affinePQ);
      throw new NotImplementedException();
    }

    return MinkSumSDas(P.Top, Q.Top);
  }

  // Основной алгоритм суммы Минковского через вычисления решётки. См Sandip Das A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
  private static FaceLattice MinkSumSDas(FLNode P, FLNode Q) {
    AffineBasis affinePQ = new AffineBasis(P.AffBasis.Origin + Q.AffBasis.Origin
                                  , P.AffBasis.Basis.Concat(Q.AffBasis.Basis));
    int dim = affinePQ.SpaceDim;
    // z --> (x \in P, y \in Q) Словарь отображающий z \in P (+) Q в пару (x,y)
    Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy = new Dictionary<FLNode, (FLNode x, FLNode y)>();
    // Словарь (x \in P, y \in Q) --> z \in P (+) Q. (Нужен для уменьшения перебора в процессе движения по решёткам)
    Dictionary<(FLNode x, FLNode y), FLNode> xyToz = new Dictionary<(FLNode x, FLNode y), FLNode>();
    // Сама решётка. Где на i-ом уровне находится множество всех граней соответствующей размерности.
    List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
    for (int i = 0; i < dim + 1; i++) {
      FL.Add(new HashSet<FLNode>());
    }

    // Заполняем максимальный элемент
    // Нас пока не волнует, что вершин может получится больше чем нужно (потом это исправим)
    FLNode PQ = new FLNode(MinkSum(P.Vertices, Q.Vertices), P.InnerPoint + Q.InnerPoint, affinePQ);
    zTo_xy.Add(PQ, (P, Q));
    xyToz.Add((P, Q), PQ);
    FL[^1].Add(PQ);

    for (int d = dim - 1; d > -1; d--) {
      foreach (FLNode z in FL[d + 1]) {
        // Будем описывать под-грани по-очереди для каждой грани с предыдущего уровня.
        (FLNode x, FLNode y) = zTo_xy[z];
        // Аффинное пространство грани z (F(+)G в терминах Лемм)
        AffineBasis zSpace = new AffineBasis(z.AffBasis);
        Point innerInAffine_z = zSpace.ProjectPoint(z.InnerPoint);

        //? Теперь я не уверен нужно ли сортировать
        // Собираем все подграни в соответствующих решётках. (Среди них будем искать грани для z)
        // List<FLNode> X = x.GetAllNonStrictSub().ToList();
        // List<FLNode> Y = y.GetAllNonStrictSub().ToList();
        List<FLNode> X = x.GetAllNonStrictSub().OrderByDescending(node => node.Dim).ToList();
        List<FLNode> Y = y.GetAllNonStrictSub().OrderByDescending(node => node.Dim).ToList();

        foreach (FLNode xi in X) {
          foreach (FLNode yj in Y) {

            // -2) Если мы обрабатывали эти пары (или их награни), то идём дальше
            if (xyToz.ContainsKey((xi, yj))) {
              System.Console.WriteLine("Skip!");
              continue;
            }

            // -1) Смотрим потенциально набираем ли мы нужную размерность
            if (xi.AffBasis.SpaceDim + yj.AffBasis.SpaceDim < z.Dim - 1) { break; }

            // Берём очередного кандидата.
            HashSet<Point> candidate = MinkSum(xi.Vertices, yj.Vertices);
            AffineBasis candBasis = new AffineBasis(xi.AffBasis.Origin + yj.AffBasis.Origin
                                          , xi.AffBasis.Basis.Concat(yj.AffBasis.Basis));

            // 0) dim(xi (+) yj) == dim(z) - 1
            if (candBasis.SpaceDim != z.Dim - 1) { continue; }

            // 1) Lemma 3.
            // Живём в пространстве x (+) y == z, а потенциальная грань xi (+) yj имеет на 1 размерность меньше.
            var candidateInAffine_z = zSpace.ProjectPoints(candidate);
            // Строим гиперплоскость. Нужна для проверки валидности получившийся подграни.
            HyperPlane A = new HyperPlane(new AffineBasis(candidateInAffine_z));
            // Технический if. Невозможно ориентировать, если внутренняя точка попала в гиперплоскость. 
            if (!A.Contains(innerInAffine_z)) { // Если внутренняя точка суммы попала на кандидата
              A.OrientNormal(innerInAffine_z, false); // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из z
            } else { continue; }

            // Собираем все надграни xi и yj, которые лежат в подрешётках x и y соответственно.
            // todo НО, достаточно только непосредственных надграней! 
            // И более того, может сможем взять подграни x размерности dim+1
            HashSet<FLNode> xiSuper = x.GetLevel(d + 1);
            if (xi.Super is not null) {
              xiSuper.IntersectWith(xi.Super);
            }
            HashSet<FLNode> yjSuper = y.GetLevel(d + 1);
            if (yj.Super is not null) {
              yjSuper.IntersectWith(yj.Super);
            }

            // F = x >= f' > f = xi
            // InnerPoint(f') + InnerPoint(g) \in A^-
            bool xCheck = true;
            foreach (var x_InnerPoint in xiSuper.Select(n => n.InnerPoint)) {
              xCheck = xCheck && A.ContainsNegative(zSpace.ProjectPoint(x_InnerPoint + yj.InnerPoint));
            }

            // G = y >= g' > g = yj
            // InnerPoint(g') + InnerPoint(f) \in A^-
            bool yCheck = true;
            foreach (var y_InnerPoint in yjSuper.Select(n => n.InnerPoint)) {
              yCheck = yCheck && A.ContainsNegative(zSpace.ProjectPoint(y_InnerPoint + xi.InnerPoint));
            }

            if (!(xCheck && yCheck)) { continue; }

            // Узел является валидным. Создаём и добавляем везде куда надо.
            FLNode node = new FLNode(candidate, xi.InnerPoint + yj.InnerPoint, candBasis);
            FL[node.Dim].Add(node);
            zTo_xy.Add(node, (xi, yj));
            xyToz.Add((xi, yj), node);

            // ? Возможно нужно всем парам (Sub(xi), yj) и (xi, Sub(yj)) в качестве узла указать 'node' (По лемме 4)
            // ! но в таком виде НЕ работает!
            // var xiSub = xi.GetAllSub();
            // var yjSub = yj.GetAllSub();
            // foreach (var xisub in xiSub) {
            //   xyToz.Add((xisub, yj), node);
            // }
            // foreach (var yjsub in yjSub) {
            //   xyToz.Add((xi, yjsub), node);
            // }

            // Устанавливаем связи с другими гранями из предыдущего слоя по Лемме 6.
            foreach (FLNode prevNode in FL[d + 1]) {
              (FLNode xp, FLNode yq) = zTo_xy[prevNode];

              HashSet<FLNode> Xp = xp.GetAllNonStrictSub();
              HashSet<FLNode> Yq = yq.GetAllNonStrictSub();

              if (Xp.Contains(xi) && Yq.Contains(yj)) {
                node.AddSuper(prevNode);
                prevNode.AddSub(node);
              }
            }
          }
        }
      }
    }
    Debug.Assert(PQ.Sub is not null, "There are NO face lattice!");
    Debug.Assert(FL[0].Count != 0, "There are NO vertices in face lattice!");

    // Убираем лишние точки из многогранников снизу-вверх.
    foreach (HashSet<FLNode> level in FL) {
      foreach (FLNode node in level) {
        node.ReconstructPolytop();
      }
    }
    return new FaceLattice(FL);
  }

}
