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
  public class MinkowskiSum { }

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
  /// <param name="P">The first polytope represented as a list.</param>
  /// <param name="Q">The second polytope represented as a list.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  internal static FaceLattice MinkSumCH(FLNode P, FLNode Q) => GiftWrapping.WrapFaceLattice
    (MinkSumPoints(P.Vertices, Q.Vertices));

  /// <summary>
  /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
  /// </summary>
  /// <param name="P">The first polytope represented as a ConvexPolytop.</param>
  /// <param name="Q">The second polytope represented as a ConvexPolytop.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumCH(ConvexPolytop P, ConvexPolytop Q) => GiftWrapping.WrapFaceLattice
    (MinkSumPoints(P.Vertices, Q.Vertices));

  /// <summary>
  /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
  /// </summary>
  /// <param name="P">The first polytope represented as a face lattice.</param>
  /// <param name="Q">The second polytope represented as a face lattice.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumCH(FaceLattice P, FaceLattice Q) => MinkSumCH(P.Top, Q.Top);

  /// <summary>
  /// The Minkowski sum of two sets of points. S1 (+) S2 = {p | p = P + Q, ∀P ∈ S1 and ∀Q ∈ S2}
  /// </summary>
  /// <param name="A">The first set of points.</param>
  /// <param name="B">The second set of points.</param>
  /// <returns>The set of points represented the Minkowski sum of two sets of points.</returns>
  public static HashSet<Point> MinkSumPoints(IEnumerable<Point> A, IEnumerable<Point> B) {
    HashSet<Point> AB = new HashSet<Point>();
    foreach (Point a in A) {
      AB.UnionWith(Shift(B, new Vector(a)));
    }

    return AB;
  }

  /// <summary>
  /// Computes the Minkowski sum of two polytopes via face lattice algorithm.
  /// Основной алгоритм суммы Минковского через вычисления решётки. 
  /// См Sandip Das A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
  /// </summary>
  /// <param name="P">The first polytope represented as a face lattice.</param>
  /// <param name="Q">The second polytope represented as a face lattice.</param>
  /// <returns>
  /// Returns a face lattice of the sum.
  /// </returns>
  public static FaceLattice MinkSumSDas(FaceLattice P, FaceLattice Q) {
    // Вычисляю аффинное пространство суммы P и Q
    // Начало координат складываю как точки. А вектора поочерёдно добавляем в базис (если можем).
    AffineBasis affinePQ = new AffineBasis
      (P.Top.AffBasis.Origin + Q.Top.AffBasis.Origin, P.Top.AffBasis.Basis.Concat(Q.Top.AffBasis.Basis));
    int dim = affinePQ.SpaceDim;

    if (dim == 0) { // Случай точки обработаем отдельно
      return new FaceLattice(P.Top.InnerPoint + Q.Top.InnerPoint);
    }

    // z --> (x \in P, y \in Q) Словарь отображающий z \in P (+) Q в пару (x,y)
    Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy = new Dictionary<FLNode, (FLNode x, FLNode y)>();
    // Словарь (x \in P, y \in Q) --> z \in P (+) Q. (Нужен для уменьшения перебора в процессе движения по решёткам)
    Dictionary<(FLNode x, FLNode y), FLNode> xyToz = new Dictionary<(FLNode x, FLNode y), FLNode>();
    // Сама решётка. Где на i-ом уровне находится множество всех граней соответствующей размерности.
    List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
    for (int i = 0; i <= dim; i++) {
      FL.Add(new HashSet<FLNode>());
    }

    // Заполняем максимальный элемент
    // Нас пока не волнует, что вершин может получится больше чем нужно (потом это исправим)
    FLNode PQ = new FLNode(MinkSumPoints(P.Top.Vertices, Q.Top.Vertices), P.Top.InnerPoint + Q.Top.InnerPoint, affinePQ);
    zTo_xy.Add(PQ, (P.Top, Q.Top));
    xyToz.Add((P.Top, Q.Top), PQ);
    FL[^1].Add(PQ);

    for (int d = dim - 1; d >= 0; d--) {
      foreach (FLNode z in FL[d + 1]) {
        // Будем описывать подграни по очереди для каждой грани с предыдущего уровня.
        (FLNode x, FLNode y) = zTo_xy[z];
        // Аффинное пространство грани z (F(+)G в терминах Лемм)
        // AffineBasis zSpace = new AffineBasis(z.AffBasis);
        AffineBasis zSpace          = z.AffBasis;
        Point       innerInAffine_z = zSpace.ProjectPoint(z.InnerPoint);

        // Собираем все подграни в соответствующих решётках, 
        // сортируя по убыванию размерности для удобства перебора. 
        // Среди них будем искать подграни, которые при суммировании дают d-грани z
        IEnumerable<FLNode> X = x.AllNonStrictSub.OrderByDescending(node => node.Dim);
        IEnumerable<FLNode> Y = y.AllNonStrictSub.OrderByDescending(node => node.Dim);

        foreach (FLNode xi in X) {
          foreach (FLNode yj in Y) {
            // -2) Если мы обрабатывали эти пары (или их награни), то идём дальше
            //  { continue; }

            // -1) Смотрим потенциально набираем ли мы нужную размерность
            if (xi.AffBasis.SpaceDim + yj.AffBasis.SpaceDim < z.Dim - 1) { break; }

            // Берём очередного кандидата.
            HashSet<Point> candidate = MinkSumPoints(xi.Vertices, yj.Vertices);
            AffineBasis candBasis = new AffineBasis
              (xi.AffBasis.Origin + yj.AffBasis.Origin, xi.AffBasis.Basis.Concat(yj.AffBasis.Basis));

            // 0) dim(xi (+) yj) == dim(z) - 1
            if (candBasis.SpaceDim != z.Dim - 1) { continue; }

            // 1) Lemma 3.
            // Живём в пространстве x (+) y == z, а потенциальная грань xi (+) yj имеет на 1 размерность меньше.
            IEnumerable<Point> candidateInAffine_z = zSpace.ProjectPoints(candidate);
            // Строим гиперплоскость. Нужна для проверки валидности получившийся подграни.
            HyperPlane A = new HyperPlane(new AffineBasis(candidateInAffine_z));

            // Технический if. Невозможно ориентировать, если внутренняя точка попала в гиперплоскость. 
            if (A.Contains(innerInAffine_z)) { continue; }
            // Если внутренняя точка суммы попала на кандидата
            // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из z

            // Ориентируем нормаль гиперплоскости суммы xi и yj
            A.OrientNormal(innerInAffine_z, false);

            // Согласно лемме 3 берём надграни xi и yj, которые лежат в подрешётках x и y соответственно
            IEnumerable<FLNode> xiSuper = xi.Super.Intersect(x.GetLevelBelowNonStrict(xi.AffBasis.SpaceDim + 1));
            IEnumerable<FLNode> yjSuper = yj.Super.Intersect(y.GetLevelBelowNonStrict(yj.AffBasis.SpaceDim + 1));

            // F = x >= f' > f = xi
            // InnerPoint(f') + InnerPoint(g) \in A^-
            bool xCheck = true;
            foreach (Point? x_InnerPoint in xiSuper.Select(n => n.InnerPoint)) {
              xCheck = xCheck && A.ContainsNegative(zSpace.ProjectPoint(x_InnerPoint + yj.InnerPoint));
            }

            // G = y >= g' > g = yj
            // InnerPoint(g') + InnerPoint(f) \in A^-
            bool yCheck = true;
            foreach (Point? y_InnerPoint in yjSuper.Select(n => n.InnerPoint)) {
              yCheck = yCheck && A.ContainsNegative(zSpace.ProjectPoint(y_InnerPoint + xi.InnerPoint));
            }

            // Если условие Леммы 3 не выполняется, то xi+yj не может дать d-грань z
            if (!(xCheck && yCheck)) { continue; }

            // И условие Леммы 3 выполнилось, значит, xi+yj есть валидная d-грань z. 
            // Если такого узла ещё не было создано, то создаём его.

            if (!xyToz.TryGetValue((xi, yj), out FLNode? node_xiyj)) {
              FLNode node = new FLNode(candidate, xi.InnerPoint + yj.InnerPoint, candBasis);
              FL[node.Dim].Add(node); // Добавляем узел в решётку
              // Добавляем информацию о связи суммы и слагаемых в соответствующие словари
              zTo_xy.Add(node, (xi, yj));
              xyToz.Add((xi, yj), node);
              // Устанавливаем связи
              z.AddSub(node);
              node.AddSuper(z);
            } else {
              // Устанавливаем связи
              z.AddSub(node_xiyj);
              node_xiyj.AddSuper(z);
            }


            // Устанавливаем связи построенной d-грани с другими (d+1)-гранями по Лемме 6.
            // Это очень медленно!
            // foreach (FLNode prevNode in FL[d + 1]) {
            //    (FLNode xp, FLNode yq) = zTo_xy[prevNode];
            //
            //   HashSet<FLNode> Xp = xp.AllNonStrictSub.ToHashSet();
            //   HashSet<FLNode> Yq = yq.AllNonStrictSub.ToHashSet();
            //
            //   if (Xp.Contains(xi) && Yq.Contains(yj)) {
            //     node.AddSuper(prevNode);
            //     prevNode.AddSub(node);
            //   }
            // }

            // ToDo: Нужно ли реализовать конец абзаца перед Теоремой 2 на стр. 190 (стр. 12 в файле статьи) ?
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
