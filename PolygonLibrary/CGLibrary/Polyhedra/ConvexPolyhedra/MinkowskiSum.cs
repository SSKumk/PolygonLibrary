namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class MinkowskiSum {

    /// <summary>
    /// The Minkowski sum of two sets of points. S1 (+) S2 = {p | p = P + Q, ∀P ∈ S1 and ∀Q ∈ S2}
    /// </summary>
    /// <param name="A">The first set of points.</param>
    /// <param name="B">The second set of points.</param>
    /// <returns>The set of points represented the Minkowski sum of two sets of points.</returns>
    public static SortedSet<Vector> AlgSumPoints(IEnumerable<Vector> A, IEnumerable<Vector> B) {
      SortedSet<Vector> AB = new SortedSet<Vector>();
      foreach (Vector a in A) {
        AB.UnionWith(Shift(B, a));
      }

#if DEBUG
      List<Vector> ab = new List<Vector>();
      foreach (Vector a in A) {
        foreach (Vector b in Shift(B, a)) {
          ab.Add(b);
        }
      }

      Debug.Assert(AB.SetEquals(ab), $"AlgSumEatSomePoints!");
#endif
      return AB;
    }

    /// <summary>
    /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
    /// </summary>
    /// <param name="P">The first polytope represented as a ConvexPolytop.</param>
    /// <param name="Q">The second polytope represented as a ConvexPolytop.</param>
    /// <returns>
    /// Returns a face lattice of the sum.
    /// </returns>
    public static ConvexPolytop ByConvexHull(ConvexPolytop P, ConvexPolytop Q)
      => ConvexPolytop.CreateFromPoints(AlgSumPoints(P.Vrep, Q.Vrep));

    /// <summary>
    /// Computes the Minkowski sum of two polytopes via face lattice algorithm.
    /// <para>
    /// См. 2021г. Sandip Das and Subhadeep Ranjan Dev.
    /// A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
    /// </para>
    /// </summary>
    /// <param name="P">The first polytope represented as a face lattice.</param>
    /// <param name="Q">The second polytope represented as a face lattice.</param>
    /// <param name="onlyHrep">If the flag is set then only affine basis of facets will be computed and Hrep be produced.</param>
    /// <returns>
    /// Returns a convex polytope defined by face lattice or Hrep.
    /// </returns>
    public static ConvexPolytop BySandipDas(ConvexPolytop P, ConvexPolytop Q, bool onlyHrep = false) {
      // Вычисляю аффинное пространство суммы P и Q
      // Начало координат складываю как точки. А вектора поочерёдно добавляем в базис (если можем).

      LinearBasis linBasisPQ = new LinearBasis(P.FLrep.Top.AffBasis.LinBasis, Q.FLrep.Top.AffBasis.LinBasis);
      AffineBasis affinePQ   = new AffineBasis(P.FLrep.Top.AffBasis.Origin + Q.FLrep.Top.AffBasis.Origin, linBasisPQ, false);
      int         dim        = affinePQ.SubSpaceDim;

      if (dim == 0) { // Случай точки обработаем отдельно
        if (onlyHrep) {
          List<HyperPlane> HPs = new List<HyperPlane>();

          int vecDim = affinePQ.Origin.SpaceDim;
          for (int i = 0; i < vecDim; i++) {
            Vector makeOrth = Vector.MakeOrth(vecDim, i + 1);
            HPs.Add(new HyperPlane(makeOrth, affinePQ.Origin[i]));
            HPs.Add(new HyperPlane(-makeOrth, -affinePQ.Origin[i]));
          }

          return ConvexPolytop.CreateFromHalfSpaces(HPs);
        }

        return ConvexPolytop.CreateFromPoints(new List<Vector>() { P.FLrep.Top.InnerPoint + Q.FLrep.Top.InnerPoint });
      }


      // z --> (x \in P, y \in Q) Словарь отображающий z \in P (+) Q в пару (x,y)
      SortedDictionary<FLNodeSum, (FLNode x, FLNode y)> zTo_xy = new SortedDictionary<FLNodeSum, (FLNode x, FLNode y)>();
      // Словарь (x \in P, y \in Q) --> z \in P (+) Q. (Нужен для уменьшения перебора в процессе движения по решёткам)
      SortedDictionary<(FLNode x, FLNode y), FLNodeSum> xyToz = new SortedDictionary<(FLNode x, FLNode y), FLNodeSum>();
      // Сама решётка. Где на i-ом уровне находится множество всех граней соответствующей размерности.
      List<SortedSet<FLNodeSum>> FL = new List<SortedSet<FLNodeSum>>();
      for (int i = 0; i <= dim; i++) {
        FL.Add(new SortedSet<FLNodeSum>());
      }

      // Заполняем максимальный элемент
      // Нас пока не волнует, что вершины не те, что нам нужны (потом это исправим)
      Vector    innerPQ = P.FLrep.Top.InnerPoint + Q.FLrep.Top.InnerPoint;
      FLNodeSum PQ      = new FLNodeSum(innerPQ, affinePQ);
      zTo_xy.Add(PQ, (P.FLrep.Top, Q.FLrep.Top));
      xyToz.Add((P.FLrep.Top, Q.FLrep.Top), PQ);
      FL[^1].Add(PQ);

      bool doNext = true;
      for (int d = dim - 1; d >= 0 && doNext; d--) {
        foreach (FLNodeSum z in FL[d + 1]) {
          // Будем описывать подграни по очереди для каждой грани с предыдущего уровня.
          (FLNode x, FLNode y) = zTo_xy[z];

          // Аффинное пространство грани z (F(+)G в терминах Лемм)
          AffineBasis zSpace          = z.AffBasis;
          Vector      innerInAffine_z = zSpace.ProjectPointToSubSpace(z.InnerPoint);

          // Собираем все подграни в соответствующих решётках,
          // сортируя по убыванию размерности для удобства перебора.
          // Среди них будем искать подграни, которые при суммировании дают d-грани z
          IEnumerable<FLNode> X = x.AllNonStrictSub.OrderByDescending(node => node.PolytopDim);
          IEnumerable<FLNode> Y = y.AllNonStrictSub.OrderByDescending(node => node.PolytopDim);

          foreach (FLNode xi in X) {
            foreach (FLNode yj in Y) {
              // -1) Смотрим потенциально набираем ли мы нужную размерность
              if (xi.AffBasis.SubSpaceDim + yj.AffBasis.SubSpaceDim < z.PolytopDim - 1) { break; }

              // Берём очередного кандидата.
              LinearBasis candLinBasis = new LinearBasis(xi.AffBasis.LinBasis, yj.AffBasis.LinBasis);
              AffineBasis candAffBasis = new AffineBasis(xi.AffBasis.Origin + yj.AffBasis.Origin, candLinBasis, false);

              // 0) dim(xi (+) yj) == dim(z) - 1
              if (candAffBasis.SubSpaceDim != d) { continue; }

              { // 0+) Если такая пара граней уже встречалась, то строить её не надо, а надо установить связи
                if (xyToz.TryGetValue((xi, yj), out FLNodeSum? node_xiyj)) {
                  // Устанавливаем связи
                  z.AddSub(node_xiyj);
                  node_xiyj.AddSuper(z);

                  continue; // Подумать, надо ли писать else вместо continue ...
                }
              }


              // 1) Lemma 3.
              // Живём в пространстве x (+) y == z, а потенциальная грань xi (+) yj имеет на 1 размерность меньше.

              // Строим гиперплоскость. Нужна для проверки валидности получившийся подграни.
              HyperPlane A = new HyperPlane(ReCalcAffineBasis(candAffBasis, zSpace));

              // Технический if. Невозможно ориентировать, если внутренняя точка попала в гиперплоскость.
              if (A.Contains(innerInAffine_z)) { continue; }
              // Если внутренняя точка суммы попала на кандидата
              // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из z

              // Ориентируем нормаль гиперплоскости суммы xi и yj
              A.OrientNormal(innerInAffine_z, false);

              // Согласно лемме 3 берём надграни xi и yj, которые лежат в подрешётках x и y соответственно
              SortedSet<FLNode> xiSuper_clone = new SortedSet<FLNode>(xi.Super);
              xiSuper_clone.IntersectWith(x.GetLevelBelowNonStrict(xi.AffBasis.SubSpaceDim + 1));
              SortedSet<FLNode> yjSuper_clone = new SortedSet<FLNode>(yj.Super);
              yjSuper_clone.IntersectWith(y.GetLevelBelowNonStrict(yj.AffBasis.SubSpaceDim + 1));

              // IEnumerable<FLNode> xiSuper = xi.Super.Intersect(x.GetLevelBelowNonStrict(xi.AffBasis.SubSpaceDim + 1));
              // IEnumerable<FLNode> yjSuper = yj.Super.Intersect(y.GetLevelBelowNonStrict(yj.AffBasis.SubSpaceDim + 1));

              // F = x >= f' > f = xi
              // InnerPoint(f') + InnerPoint(g) \in A^-
              bool xCheck = true;
              foreach (Vector? x_InnerPoint in xiSuper_clone.Select(n => n.InnerPoint)) {
                if (!A.ContainsNegative(zSpace.ProjectPointToSubSpace(x_InnerPoint + yj.InnerPoint))) {
                  xCheck = false;

                  break;
                }
              }
              bool yCheck = true;
              if (xCheck) {
                // G = y >= g' > g = yj
                // InnerPoint(g') + InnerPoint(f) \in A^-
                foreach (Vector? y_InnerPoint in yjSuper_clone.Select(n => n.InnerPoint)) {
                  if (!A.ContainsNegative(zSpace.ProjectPointToSubSpace(y_InnerPoint + xi.InnerPoint))) {
                    yCheck = false;

                    break;
                  }
                }
              }

              // Если условие Леммы 3 не выполняется, то xi+yj не может дать d-грань z
              if (!(xCheck && yCheck)) { continue; }

              // И условие Леммы 3 выполнилось, значит, xi+yj есть валидная d-грань z.
              // Если такого узла ещё не было создано, то создаём его.

              Vector    newInner = xi.InnerPoint + yj.InnerPoint;
              FLNodeSum node     = new FLNodeSum(newInner, candAffBasis);
              FL[d].Add(node); // Добавляем узел в решётку
              // Добавляем информацию о связи суммы и слагаемых в соответствующие словари
              zTo_xy.Add(node, (xi, yj));
              xyToz.Add((xi, yj), node);
              // Устанавливаем связи
              z.AddSub(node);
              node.AddSuper(z);

              // ToDo: Нужно ли реализовать конец абзаца перед Теоремой 2 на стр. 190 (стр. 12 в файле статьи) ?
            }
          }
        }

        if (onlyHrep) { doNext = false; }
      }

      if (onlyHrep) {
        return ConvexPolytop.CreateFromHalfSpaces
          (FL[dim - 1].Select(facet => new HyperPlane(facet.AffBasis, (PQ.InnerPoint, false))).ToList());
      }

      Debug.Assert(PQ.Sub is not null, "There are NO face lattice!");
      Debug.Assert(FL[0].Count != 0, "There are NO vertices in face lattice!");

      // Наполняем все k-грани точками снизу-вверх.
      return ConvexPolytop.CreateFromFaceLattice(FaceLattice.ConstructFromFLNodeSum(FL), false);
    }

  }


  /// <summary>
  /// Shift given swarm by given vector.
  /// </summary>
  /// <param name="S">S to be shifted.</param>
  /// <param name="shift">Vector to shift.</param>
  /// <returns>Shifted swarm.</returns>
  public static List<Vector> Shift(IEnumerable<Vector> S, Vector shift) { return S.Select(s => s + shift).ToList(); }

  /// <summary>
  /// This function recalculates the basis 'from' to the basis 'to'. 'From' must lie in space of 'to' basis.
  /// </summary>
  /// <param name="from">Basis to recalculate.</param>
  /// <param name="to">Basis to which 'from' should be recalculated.</param>
  /// <returns>'From' basis in terms of 'to' basis.</returns>
  private static AffineBasis ReCalcAffineBasis(AffineBasis from, AffineBasis to) {
    Vector      newO  = to.ProjectPointToSubSpace(from.Origin);
    LinearBasis newLB = new LinearBasis(newO.SpaceDim, to.LinBasis.ProjectVectorsToSubSpace(from.LinBasis));

    return new AffineBasis(newO, newLB, false);
  }

}

