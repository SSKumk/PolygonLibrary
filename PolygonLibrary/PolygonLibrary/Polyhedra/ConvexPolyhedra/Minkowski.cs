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
    AffineBasis affinePQ = new AffineBasis(P.Top.Affine.Origin + Q.Top.Affine.Origin
                                      , P.Top.Affine.Basis.Concat(Q.Top.Affine.Basis));

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
    AffineBasis affinePQ = new AffineBasis(P.Affine.Origin + Q.Affine.Origin
                                  , P.Affine.Basis.Concat(Q.Affine.Basis));
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
        AffineBasis zSpace = new AffineBasis(z.Affine);
        Point innerInAffine_z = zSpace.ProjectPoint(z.InnerPoint);

        //? Теперь я не уверен нужно ли сортировать
        // Собираем все подграни в соответствующих решётках. (Среди них будем искать грани для z)
        // List<FLNode> X = x.GetAllNonStrictSub().ToList();
        // List<FLNode> Y = y.GetAllNonStrictSub().ToList();
        List<FLNode> X = x.GetAllNonStrictSub().OrderByDescending(node => node.Dim).ToList();
        List<FLNode> Y = y.GetAllNonStrictSub().OrderByDescending(node => node.Dim).ToList();

        foreach (FLNode xi in X) {
          foreach (FLNode yi in Y) {

            // -2) Если мы обрабатывали эти пары (или их награни), то идём дальше
            if (xyToz.ContainsKey((xi, yi))) { continue; }

            // -1) Смотрим потенциально набираем ли мы нужную размерность
            if (xi.Affine.SpaceDim + yi.Affine.SpaceDim < z.Dim - 1) { continue; }

            // Берём очередного кандидата.
            HashSet<Point> candidate = MinkSum(xi.Vertices, yi.Vertices);
            AffineBasis candBasis = new AffineBasis(xi.Affine.Origin + yi.Affine.Origin
                                          , xi.Affine.Basis.Concat(yi.Affine.Basis));

            // 0) dim(xi (+) yi) == dim(z) - 1
            if (candBasis.SpaceDim != z.Dim - 1) { continue; }

            // 1) Lemma 3.
            // Живём в пространстве x (+) y == z, а потенциальная грань xi (+) yi имеет на 1 размерность меньше.
            var candidateInAffine_z = zSpace.ProjectPoints(candidate);
            // Строим гиперплоскость. Нужна для проверки валидности получившийся подграни.
            HyperPlane A = new HyperPlane(new AffineBasis(candidateInAffine_z));
            // Технический if. Невозможно ориентировать, если внутренняя точка попала в гиперплоскость. 
            if (!A.Contains(innerInAffine_z)) { // Если внутренняя точка суммы попала на кандидата
              A.OrientNormal(innerInAffine_z, false); // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из z
            } else { continue; }

            // Собираем все надграни xi и yi, которые лежат в подрешётках x и y соответственно.
            // НО, достаточно только непосредственных надграней!
            HashSet<FLNode> xiAbove = FLNode.GetFromBottomToTop(xi, x, true);
            HashSet<FLNode> yiAbove = FLNode.GetFromBottomToTop(yi, y, true);

            // F = x >= f' > f = xi
            // InnerPoint(f') + InnerPoint(g) \in A^-
            bool xCheck = true;
            foreach (var x_InnerPoint in xiAbove.Select(n => n.InnerPoint)) {
              xCheck = xCheck && A.ContainsNegative(zSpace.ProjectPoint(x_InnerPoint + yi.InnerPoint));
            }

            // G = y >= g' > g = yi
            // InnerPoint(g') + InnerPoint(f) \in A^-
            bool yCheck = true;
            foreach (var y_InnerPoint in yiAbove.Select(n => n.InnerPoint)) {
              yCheck = yCheck && A.ContainsNegative(zSpace.ProjectPoint(y_InnerPoint + xi.InnerPoint));
            }

            if (!(xCheck && yCheck)) { continue; }

            // Узел является валидным. Создаём и добавляем везде куда надо.
            FLNode node = new FLNode(candidate, xi.InnerPoint + yi.InnerPoint, candBasis);
            FL[node.Dim].Add(node);
            zTo_xy.Add(node, (xi, yi));
            xyToz.Add((xi, yi), node);

            // ? Возможно нужно всем парам (Sub(xi), yi) и (xi, Sub(yi)) в качестве узла указать 'node' (По лемме 4)
            // ! но в таком виде НЕ работает!
            // var xiSub = xi.GetAllSub();
            // var yiSub = yi.GetAllSub();
            // foreach (var xisub in xiSub) {
            //   xyToz.Add((xisub, yi), node);
            // }
            // foreach (var yisub in yiSub) {
            //   xyToz.Add((xi, yisub), node);
            // }

            // Устанавливаем связи с другими гранями из предыдущего слоя по Лемме 6.
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

    // Убираем лишние точки из многогранников снизу-вверх.
    foreach (HashSet<FLNode> level in FL) {
      foreach (FLNode node in level) {
        node.ReconstructPolytop();
      }
    }
    return new FaceLattice(FL);
  }

}
