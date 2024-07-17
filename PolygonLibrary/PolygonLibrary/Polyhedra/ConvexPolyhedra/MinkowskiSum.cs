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
    public static HashSet<Vector> AlgSumPoints(IEnumerable<Vector> A, IEnumerable<Vector> B) {
      HashSet<Vector> AB = new HashSet<Vector>();
      foreach (Vector a in A) {
        AB.UnionWith(Shift(B, a));
      }

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
      => ConvexPolytop.AsFLPolytop(AlgSumPoints(P.Vertices, Q.Vertices));

    // /// <summary>
    // /// Computes the Minkowski sum of two polytopes via convex hull algorithm.
    // /// </summary>
    // /// <param name="P">The first polytope represented as a VPolytop.</param>
    // /// <param name="Q">The second polytope represented as a VPolytop..</param>
    // /// <returns>
    // /// Returns a face lattice of the sum.
    // /// </returns>
    // public static ConvexPolytop ByConvexHull(HashSet<Vector> P, HashSet<Vector> Q)
    //   => ConvexPolytop.AsFLPolytop(AlgSumPoints(P, Q));

    // /// <summary>
    // /// Computes the Minkowski sum of two polytopes via face lattice algorithm.
    // /// If FL does not exist than both for P and Q will be compute before starting a sum algorithm.
    // /// <para>
    // /// См. 2021. Sandip Das and Subhadeep Ranjan Dev.
    // /// A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
    // /// </para>
    // /// </summary>
    // /// <param name="P">The first convex polytope.</param>
    // /// <param name="Q">The second convex polytope.</param>
    // /// <returns>
    // /// Returns a convex polytop of sum.
    // /// </returns>
    // public static ConvexPolytop BySandipDas(ConvexPolytop P, ConvexPolytop Q)
    //   => ConvexPolytop.AsFLPolytop(BySandipDas(P.FL, Q.FL));

    // /// <summary>
    // /// Computes the Minkowski sum of two polytopes via face lattice algorithm.
    // /// <para>
    // /// См. 2021г. Sandip Das and Subhadeep Ranjan Dev.
    // /// A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
    // /// </para>
    // /// </summary>
    // /// <param name="P">The first polytope represented as a face lattice.</param>
    // /// <param name="Q">The second polytope represented as a face lattice.</param>
    // /// <returns>
    // /// Returns a face lattice of the sum.
    // /// </returns>
    // public static FaceLattice BySandipDas(FaceLattice P, FaceLattice Q) {
    //   // Вычисляю аффинное пространство суммы P и Q
    //   // Начало координат складываю как точки. А вектора поочерёдно добавляем в базис (если можем).
    //   AffineBasis affinePQ = AffineBasis.AsVectors
    //     (P.Top.AffBasis.Origin + Q.Top.AffBasis.Origin, P.Top.AffBasis.Basis.Concat(Q.Top.AffBasis.Basis));
    //   int dim = affinePQ.SubSpaceDim;
    //
    //   if (dim == 0) { // Случай точки обработаем отдельно
    //     return new FaceLattice(P.Top.InnerPoint + Q.Top.InnerPoint);
    //   }
    //
    //   // z --> (x \in P, y \in Q) Словарь отображающий z \in P (+) Q в пару (x,y)
    //   Dictionary<FLNode, (FLNode x, FLNode y)> zTo_xy = new Dictionary<FLNode, (FLNode x, FLNode y)>();
    //   // Словарь (x \in P, y \in Q) --> z \in P (+) Q. (Нужен для уменьшения перебора в процессе движения по решёткам)
    //   Dictionary<(FLNode x, FLNode y), FLNode> xyToz = new Dictionary<(FLNode x, FLNode y), FLNode>();
    //   // Сама решётка. Где на i-ом уровне находится множество всех граней соответствующей размерности.
    //   List<HashSet<FLNode>> FL = new List<HashSet<FLNode>>();
    //   for (int i = 0; i <= dim; i++) {
    //     FL.Add(new HashSet<FLNode>());
    //   }
    //
    //   // Заполняем максимальный элемент
    //   // Нас пока не волнует, что вершины не те, что нам нужны (потом это исправим)
    //   Vector innerPQ = P.Top.InnerPoint + Q.Top.InnerPoint;
    //   FLNode PQ      = new FLNode(new VectorHashSet { innerPQ }, innerPQ, affinePQ);
    //   zTo_xy.Add(PQ, (P.Top, Q.Top));
    //   xyToz.Add((P.Top, Q.Top), PQ);
    //   FL[^1].Add(PQ);
    //
    //   for (int d = dim - 1; d >= 0; d--) {
    //     foreach (FLNode z in FL[d + 1]) {
    //       // Будем описывать подграни по очереди для каждой грани с предыдущего уровня.
    //       (FLNode x, FLNode y) = zTo_xy[z];
    //       // Аффинное пространство грани z (F(+)G в терминах Лемм)
    //       AffineBasis zSpace          = z.AffBasis;
    //       Vector      innerInAffine_z = zSpace.ProjectVector(z.InnerPoint);
    //
    //       // Собираем все подграни в соответствующих решётках,
    //       // сортируя по убыванию размерности для удобства перебора.
    //       // Среди них будем искать подграни, которые при суммировании дают d-грани z
    //       IEnumerable<FLNode> X = x.AllNonStrictSub.OrderByDescending(node => node.PolytopDim);
    //       IEnumerable<FLNode> Y = y.AllNonStrictSub.OrderByDescending(node => node.PolytopDim);
    //
    //       foreach (FLNode xi in X) {
    //         foreach (FLNode yj in Y) {
    //           // -1) Смотрим потенциально набираем ли мы нужную размерность
    //           if (xi.AffBasis.SubSpaceDim + yj.AffBasis.SubSpaceDim < z.PolytopDim - 1) { break; }
    //
    //           // Берём очередного кандидата.
    //           AffineBasis candBasis = AffineBasis.AsVectors
    //             (xi.AffBasis.Origin + yj.AffBasis.Origin, xi.AffBasis.Basis.Concat(yj.AffBasis.Basis));
    //
    //           // 0) dim(xi (+) yj) == dim(z) - 1
    //           if (candBasis.SubSpaceDim != d) { continue; }
    //
    //           { // 0+) Если такая пара граней уже встречалась, то строить её не надо, а надо установить связи
    //             if (xyToz.TryGetValue((xi, yj), out FLNode? node_xiyj)) {
    //               // Устанавливаем связи
    //               z.AddSub(node_xiyj);
    //               node_xiyj.AddSuper(z);
    //
    //               continue; // Подумать, надо ли писать else вместо continue ...
    //             }
    //           }
    //
    //
    //           // 1) Lemma 3.
    //           // Живём в пространстве x (+) y == z, а потенциальная грань xi (+) yj имеет на 1 размерность меньше.
    //
    //           // Строим гиперплоскость. Нужна для проверки валидности получившийся подграни.
    //           HyperPlane A = new HyperPlane(ReCalcAffineBasis(candBasis, zSpace));
    //
    //           // Технический if. Невозможно ориентировать, если внутренняя точка попала в гиперплоскость.
    //           if (A.Contains(innerInAffine_z)) { continue; }
    //           // Если внутренняя точка суммы попала на кандидата
    //           // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из z
    //
    //           // Ориентируем нормаль гиперплоскости суммы xi и yj
    //           A.OrientNormal(innerInAffine_z, false);
    //
    //           // Согласно лемме 3 берём надграни xi и yj, которые лежат в подрешётках x и y соответственно
    //           IEnumerable<FLNode> xiSuper = xi.Super.Intersect(x.GetLevelBelowNonStrict(xi.AffBasis.SubSpaceDim + 1));
    //           IEnumerable<FLNode> yjSuper = yj.Super.Intersect(y.GetLevelBelowNonStrict(yj.AffBasis.SubSpaceDim + 1));
    //
    //           // F = x >= f' > f = xi
    //           // InnerPoint(f') + InnerPoint(g) \in A^-
    //           bool xCheck = true;
    //           foreach (Vector? x_InnerPoint in xiSuper.Select(n => n.InnerPoint)) {
    //             xCheck = xCheck && A.ContainsNegative(zSpace.ProjectVector(x_InnerPoint + yj.InnerPoint));
    //           }
    //
    //           // G = y >= g' > g = yj
    //           // InnerPoint(g') + InnerPoint(f) \in A^-
    //           bool yCheck = true;
    //           foreach (Vector? y_InnerPoint in yjSuper.Select(n => n.InnerPoint)) {
    //             yCheck = yCheck && A.ContainsNegative(zSpace.ProjectVector(y_InnerPoint + xi.InnerPoint));
    //           }
    //
    //           // Если условие Леммы 3 не выполняется, то xi+yj не может дать d-грань z
    //           if (!(xCheck && yCheck)) { continue; }
    //
    //           // И условие Леммы 3 выполнилось, значит, xi+yj есть валидная d-грань z.
    //           // Если такого узла ещё не было создано, то создаём его.
    //
    //           Vector newInner = xi.InnerPoint + yj.InnerPoint;
    //           // newInner в качестве Polytop для FLNode это "костыль", чтобы правильно считался хеш и, притом, быстро.
    //           FLNode node = new FLNode(new VectorHashSet { newInner }, newInner, candBasis);
    //
    //           // FLNode node = new FLNode(candidate, xi.InnerPoint + yj.InnerPoint, candBasis);
    //           FL[d].Add(node); // Добавляем узел в решётку
    //           // Добавляем информацию о связи суммы и слагаемых в соответствующие словари
    //           zTo_xy.Add(node, (xi, yj));
    //           xyToz.Add((xi, yj), node);
    //           // Устанавливаем связи
    //           z.AddSub(node);
    //           node.AddSuper(z);
    //
    //           // ToDo: Нужно ли реализовать конец абзаца перед Теоремой 2 на стр. 190 (стр. 12 в файле статьи) ?
    //         }
    //       }
    //     }
    //   }
    //   Debug.Assert(PQ.Sub is not null, "There are NO face lattice!");
    //   Debug.Assert(FL[0].Count != 0, "There are NO vertices in face lattice!");
    //
    //
    //   // Наполняем все k-грани точками снизу-вверх.
    //   return new FaceLattice(FL).LinearVertexTransform(x => x);
    // }

    // /// <summary>
    // /// Computes the Minkowski sum of two polytopes via face lattice algorithm. It stops then calculate the facet-layer.
    // /// <para>
    // /// См. 2021. Sandip Das and Subhadeep Ranjan Dev.
    // /// A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
    // /// </para>
    // /// </summary>
    // /// <param name="P">The first convex polytope.</param>
    // /// <param name="Q">The second convex polytope.</param>
    // /// <returns>
    // /// Returns a HRep of the sum.
    // /// </returns>
    // public static ConvexPolytop BySandipDasCutted(ConvexPolytop P, ConvexPolytop Q)
    //   => ConvexPolytop.AsHPolytop(BySandipDasCutted(P.FL, Q.FL));
    //
    // /// <summary>
    // /// Computes the Minkowski sum of two polytopes via face lattice algorithm. It stops then calculate the facet-layer.
    // /// <para>
    // /// См. 2021. Sandip Das and Subhadeep Ranjan Dev.
    // /// A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
    // /// </para>
    // /// </summary>
    // /// <param name="P">The first polytope represented as a face lattice.</param>
    // /// <param name="Q">The second polytope represented as a face lattice.</param>
    // /// <returns>
    // /// Returns a HRep of the sum.
    // /// </returns>
    // public static List<HyperPlane> BySandipDasCutted(FaceLattice P, FaceLattice Q) {
    //   // Вычисляю аффинное пространство суммы P и Q
    //   // Начало координат складываю как точки. А вектора поочерёдно добавляем в базис (если можем).
    //   AffineBasis affinePQ = AffineBasis.AsVectors
    //     (P.Top.AffBasis.Origin + Q.Top.AffBasis.Origin, P.Top.AffBasis.Basis.Concat(Q.Top.AffBasis.Basis));
    //   int dim = affinePQ.SubSpaceDim;
    //
    //   if (dim == 0) { // Случай точки обработаем отдельно
    //     List<HyperPlane> HPs = new List<HyperPlane>();
    //
    //     int vecDim = affinePQ.Origin.Dim;
    //     for (int i = 0; i < vecDim; i++) {
    //       HPs.Add(new HyperPlane(Vector.MakeOrth(vecDim, i + 1), affinePQ.Origin[i]));
    //     }
    //
    //     return HPs;
    //   }
    //
    //   List<HyperPlane> HRep    = new List<HyperPlane>();
    //   Vector           innerPQ = P.Top.InnerPoint + Q.Top.InnerPoint;
    //   FLNode           PQ      = new FLNode(new VectorHashSet { innerPQ }, innerPQ, affinePQ);
    //
    //   FLNode      x               = P.Top;
    //   FLNode      y               = Q.Top;
    //   AffineBasis zSpace          = PQ.AffBasis;
    //   Vector      innerInAffine_z = zSpace.ProjectVector(PQ.InnerPoint);
    //
    //   // Собираем все подграни в соответствующих решётках,
    //   // сортируя по убыванию размерности для удобства перебора.
    //   // Среди них будем искать подграни, которые при суммировании дают гиперграни PQ
    //   IEnumerable<FLNode> X = x.AllNonStrictSub.OrderByDescending(node => node.PolytopDim);
    //   IEnumerable<FLNode> Y = y.AllNonStrictSub.OrderByDescending(node => node.PolytopDim);
    //
    //   foreach (FLNode xi in X) {
    //     foreach (FLNode yj in Y) {
    //       // -1) Смотрим потенциально набираем ли мы нужную размерность
    //       if (xi.AffBasis.SubSpaceDim + yj.AffBasis.SubSpaceDim < PQ.PolytopDim - 1) { break; }
    //
    //       // Берём очередного кандидата.
    //       AffineBasis candBasis = AffineBasis.AsVectors
    //         (xi.AffBasis.Origin + yj.AffBasis.Origin, xi.AffBasis.Basis.Concat(yj.AffBasis.Basis));
    //
    //       // 0) dim(xi (+) yj) == dim(PQ) - 1
    //       if (candBasis.SubSpaceDim != PQ.AffBasis.SubSpaceDim - 1) { continue; }
    //
    //       // 1) Lemma 3.
    //       // Живём в пространстве x (+) y == PQ, а потенциальная гипергрань xi (+) yj имеет на 1 размерность меньше.
    //
    //       // Строим гиперплоскость. Нужна для проверки валидности получившийся подграни.
    //       HyperPlane A_hp = new HyperPlane(ReCalcAffineBasis(candBasis, zSpace));
    //
    //       // Технический if. Невозможно ориентировать, если внутренняя точка попала в гиперплоскость.
    //       if (A_hp.Contains(innerInAffine_z)) { continue; }
    //       // Если внутренняя точка суммы попала на кандидата,
    //       // то гарантировано он плохой. И не важно куда смотрит нормаль, там будут точки из PQ
    //
    //       // Ориентируем нормаль гиперплоскости суммы xi и yj
    //       A_hp.OrientNormal(innerInAffine_z, false);
    //
    //       // Согласно лемме 3 берём надграни xi и yj, которые лежат в подрешётках x и y соответственно.
    //       IEnumerable<FLNode> xiSuper = xi.Super.Intersect(x.GetLevelBelowNonStrict(xi.AffBasis.SubSpaceDim + 1));
    //       IEnumerable<FLNode> yjSuper = yj.Super.Intersect(y.GetLevelBelowNonStrict(yj.AffBasis.SubSpaceDim + 1));
    //
    //       // F = x >= f' > f = xi
    //       // InnerPoint(f') + InnerPoint(g) \in A^-
    //       bool xCheck = true;
    //       foreach (Vector? x_InnerPoint in xiSuper.Select(n => n.InnerPoint)) {
    //         xCheck = xCheck && A_hp.ContainsNegative(zSpace.ProjectVector(x_InnerPoint + yj.InnerPoint));
    //       }
    //
    //       // G = y >= g' > g = yj
    //       // InnerPoint(g') + InnerPoint(f) \in A^-
    //       bool yCheck = true;
    //       foreach (Vector? y_InnerPoint in yjSuper.Select(n => n.InnerPoint)) {
    //         yCheck = yCheck && A_hp.ContainsNegative(zSpace.ProjectVector(y_InnerPoint + xi.InnerPoint));
    //       }
    //
    //       // Если условие Леммы 3 не выполняется, то xi+yj не может дать гипер-грань PQ.
    //       if (!(xCheck && yCheck)) { continue; }
    //
    //       // И условие Леммы 3 выполнилось, значит, xi+yj есть валидная гипер-грань PQ.
    //       HRep.Add(new HyperPlane(candBasis, (PQ.InnerPoint, false)));
    //     }
    //   }
    //
    //   return HRep;
    // }


    /// <summary>
    /// Computes the Minkowski sum of two polytopes via face lattice algorithm.
    /// <para>
    /// См. 2021г. Sandip Das and Subhadeep Ranjan Dev.
    /// A Worst-Case Optimal Algorithm to Compute the Minkowski Sum of Convex Polytopes.
    /// </para>
    /// </summary>
    /// <param name="P">The first polytope represented as a face lattice.</param>
    /// <param name="Q">The second polytope represented as a face lattice.</param>
    /// <param name="isOnlyHRep">If the flag is set then only affine basis of facets will be computed and HRep be produced.</param>
    /// <returns>
    /// Returns a convex polytop defined by face lattice or HRep.
    /// </returns>
    public static ConvexPolytop BySandipDas(ConvexPolytop P, ConvexPolytop Q, bool isOnlyHRep = false) {
      // Вычисляю аффинное пространство суммы P и Q
      // Начало координат складываю как точки. А вектора поочерёдно добавляем в базис (если можем).

      LinearBasis linBasisPQ = new LinearBasis(P.FL.Top.AffBasis.LinBasis, Q.FL.Top.AffBasis.LinBasis);
      AffineBasis affinePQ   = new AffineBasis(P.FL.Top.AffBasis.Origin + Q.FL.Top.AffBasis.Origin, linBasisPQ);
      int         dim        = affinePQ.SubSpaceDim;

      if (dim == 0) { // Случай точки обработаем отдельно
        if (isOnlyHRep) {
          List<HyperPlane> HPs = new List<HyperPlane>();

          int vecDim = affinePQ.Origin.Dim;
          for (int i = 0; i < vecDim; i++) {
            Vector makeOrth = Vector.MakeOrth(vecDim, i + 1);
            HPs.Add(new HyperPlane(makeOrth, affinePQ.Origin[i]));
            HPs.Add(new HyperPlane(-makeOrth, -affinePQ.Origin[i]));
          }

          return ConvexPolytop.AsHPolytop(HPs);
        }

        return ConvexPolytop.AsFLPolytop(new List<Vector>() { P.FL.Top.InnerPoint + Q.FL.Top.InnerPoint });
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
      // Нас пока не волнует, что вершины не те, что нам нужны (потом это исправим)
      Vector innerPQ = P.FL.Top.InnerPoint + Q.FL.Top.InnerPoint;
      FLNode PQ      = new FLNode(new VectorHashSet { innerPQ }, innerPQ, affinePQ);
      zTo_xy.Add(PQ, (P.FL.Top, Q.FL.Top));
      xyToz.Add((P.FL.Top, Q.FL.Top), PQ);
      FL[^1].Add(PQ);

      bool doNext = true;
      for (int d = dim - 1; d >= 0 && doNext; d--) {
        foreach (FLNode z in FL[d + 1]) {
          // Будем описывать подграни по очереди для каждой грани с предыдущего уровня.
          (FLNode x, FLNode y) = zTo_xy[z];
          // Аффинное пространство грани z (F(+)G в терминах Лемм)

          AffineBasis zSpace          = z.AffBasis;
          Vector      innerInAffine_z = zSpace.ProjectVector(z.InnerPoint);

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
              AffineBasis candAffBasis = new AffineBasis(xi.AffBasis.Origin + yj.AffBasis.Origin, candLinBasis);

              // 0) dim(xi (+) yj) == dim(z) - 1
              if (candAffBasis.SubSpaceDim != d) { continue; }

              { // 0+) Если такая пара граней уже встречалась, то строить её не надо, а надо установить связи
                if (xyToz.TryGetValue((xi, yj), out FLNode? node_xiyj)) {
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
              IEnumerable<FLNode> xiSuper = xi.Super.Intersect(x.GetLevelBelowNonStrict(xi.AffBasis.SubSpaceDim + 1));
              IEnumerable<FLNode> yjSuper = yj.Super.Intersect(y.GetLevelBelowNonStrict(yj.AffBasis.SubSpaceDim + 1));

              // F = x >= f' > f = xi
              // InnerPoint(f') + InnerPoint(g) \in A^-
              bool xCheck = true;
              foreach (Vector? x_InnerPoint in xiSuper.Select(n => n.InnerPoint)) {
                xCheck = xCheck && A.ContainsNegative(zSpace.ProjectVector(x_InnerPoint + yj.InnerPoint));
              }

              // G = y >= g' > g = yj
              // InnerPoint(g') + InnerPoint(f) \in A^-
              bool yCheck = true;
              foreach (Vector? y_InnerPoint in yjSuper.Select(n => n.InnerPoint)) {
                yCheck = yCheck && A.ContainsNegative(zSpace.ProjectVector(y_InnerPoint + xi.InnerPoint));
              }

              // Если условие Леммы 3 не выполняется, то xi+yj не может дать d-грань z
              if (!(xCheck && yCheck)) { continue; }

              // И условие Леммы 3 выполнилось, значит, xi+yj есть валидная d-грань z.
              // Если такого узла ещё не было создано, то создаём его.

              Vector newInner = xi.InnerPoint + yj.InnerPoint;
              // newInner в качестве Polytop для FLNode это "костыль", чтобы правильно считался хеш и, притом, быстро.
              FLNode node = new FLNode(new VectorHashSet { newInner }, newInner, candAffBasis);

              // FLNode node = new FLNode(candidate, xi.InnerPoint + yj.InnerPoint, candBasis);
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

        if (isOnlyHRep) { doNext = false; }
      }

      if (isOnlyHRep) {
        return ConvexPolytop.AsHPolytop
          (FL[dim - 1].Select(facet => new HyperPlane(facet.AffBasis, (PQ.InnerPoint, false))).ToList());
      }

      Debug.Assert(PQ.Sub is not null, "There are NO face lattice!");
      Debug.Assert(FL[0].Count != 0, "There are NO vertices in face lattice!");

      // Наполняем все k-грани точками снизу-вверх.
      return ConvexPolytop.AsFLPolytop(new FaceLattice(FL).LinearVertexTransform(x => x));
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
    Vector      newO  = to.ProjectVector(from.Origin);
    LinearBasis newLB = new LinearBasis(newO.Dim, to.LinBasis.ProjectVectorsToSubSpace(from.LinBasis.ToList()), false);

    return new AffineBasis(newO, newLB);
  }

}
