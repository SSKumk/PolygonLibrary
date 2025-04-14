namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // !Не работает! Don't work!
  public class HrepToFLrep {

    public static FaceLattice? HrepToFLrep_Geometric(List<HyperPlane> HPs, int PolytopDim) {
      SortedDictionary<FLNode, List<HyperPlane>> vToHPs = new SortedDictionary<FLNode, List<HyperPlane>>();
      List<SortedSet<FLNode>>                    FL     = new List<SortedSet<FLNode>>(); // 0 and 1 lvls
      for (int i = 0; i <= PolytopDim; i++) {
        FL.Add(new SortedSet<FLNode>());
      }

      int d = HPs.First().Normal.SpaceDim;

      // Этап 1. Поиск какой-либо вершины и определение гиперплоскостей, которым она принадлежит
      // Наивная реализация
      Vector? firstPoint = ConvexPolytop.FindInitialVertex_Simplex(HPs, out List<HyperPlane>? activeHPs);
      if (firstPoint is null || activeHPs is null) { return null; }
      FL[0].Add(new FLNode(firstPoint)); // первую вершину определили

      Queue<(FLNode vertex, List<HyperPlane> HPs)> process = new Queue<(FLNode, List<HyperPlane>)>();
      process.Enqueue((FL[0].First(), activeHPs));
      vToHPs.Add(process.Peek().vertex, process.Peek().HPs);

      // Поиск всех остальных вершин
      while (process.TryDequeue(out (FLNode, List<HyperPlane>) elem)) {
        (FLNode z, List<HyperPlane> Hz) = elem;

        Combination J = new Combination(Hz.Count, d - 1);
        do {
          List<HyperPlane> edge = new List<HyperPlane>(d - 1);
          for (int j = 0; j < d - 1; j++) {
            edge.Add(Hz[J[j]]);
          }
          LinearBasis coEdgeLinSpace = new LinearBasis(edge.Select(hp => hp.Normal));
          if (coEdgeLinSpace.SubSpaceDim != d - 1) {
            continue;
          } // Несколько гиперплоскостей "наложились", ребро не получим

          Vector v = coEdgeLinSpace.FindOrthonormalVector();

          // проверяем вектор v
          bool firstNonZeroProduct = true;
          bool isEdge              = true;
          foreach (HyperPlane hp in Hz) {
            TNum dotProduct = v * hp.Normal;

            // Если скалярное произведение равно нулю, игнорируем
            if (!Tools.EQ(dotProduct)) {
              // Первое ненулевое скалярное произведение определяет ориентацию вектора v
              if (firstNonZeroProduct) {
                if (Tools.GT(dotProduct)) { v = -v; }
                firstNonZeroProduct = false;
              }
              else { // Для всех последующих не нулевых произведений требуется, чтобы они были отрицательные
                if (Tools.GT(dotProduct)) {
                  isEdge = false;

                  break;
                }
              }
            }
          }
          if (isEdge) {
            // Теперь v определяет луч, на котором лежит ребро
            List<HyperPlane> zNewHPs       = new List<HyperPlane>();
            List<HyperPlane> orthToEdgeHPs = new List<HyperPlane>();
            TNum             tMin          = Tools.Zero;
            bool             isInf         = true;
            Vector           zNew          = Vector.Zero(d);
            bool             foundPrev     = false;
            FLNode?          zNew_node     = null;
            foreach (HyperPlane hp in HPs) {
              TNum denominator = hp.Normal * v;

              if (Tools.EQ(denominator)) {
                if (hp.Contains(z.InnerPoint)) {
                  orthToEdgeHPs.Add(hp);
                }
              }
              else {
                TNum ti = (hp.ConstantTerm - hp.Normal * z.InnerPoint) / denominator;

                if (Tools.GT(ti)) {                  // учитываем только положительные ti
                  if (isInf || Tools.LT(ti, tMin)) { // первый кандидат или найден новый минимум
                    tMin  = ti;
                    isInf = false;
                    zNewHPs.Clear();
                    zNewHPs.Add(hp);
                    zNew      = Vector.MulByNumAndAdd(v, tMin, z.InnerPoint); // вычисляем новую точку: v*tMin + z
                    zNew_node = new FLNode(zNew);
                    if (FL[0].Contains(zNew_node)) { // todo: потенциальная проблема Теперь мы сравниваем не векторы, но узлы
                      foundPrev = true;

                      break;
                    }
                  }
                  else if (Tools.EQ(ti, tMin)) { // если ti равен текущему минимуму, добавляем гиперплоскость
                    zNewHPs.Add(hp);
                  }
                }
              }
            }
            Debug.Assert(!isInf, $"HrepToFLrep_Geometric: The set of inequalities is unbounded!");
            Debug.Assert(zNew_node is not null, "HrepToFLrep_Geometric new node is null!");

            FL[1]
             .Add
                (
                 new FLNode(new List<FLNode>() { z, zNew_node }, new AffineBasis(z.InnerPoint, new LinearBasis(v)))
                ); // todo: а в любом случае стоит собирать ребро?

            if (!foundPrev) {
              FL[0].Add(zNew_node);
              orthToEdgeHPs.AddRange(zNewHPs);
              process.Enqueue((zNew_node, orthToEdgeHPs));
              vToHPs.Add(zNew_node, orthToEdgeHPs);
            }
          }
        } while (J.Next());
        // Console.WriteLine($"QSize = {process.Count}");
      }

      // Теперь собираем всю оставшуюся решётку
      foreach (FLNode vertex in FL[0]) {
        List<HyperPlane> vHP = vToHPs[vertex];
        foreach (HyperPlane hp in vHP) {             // Цикл по гиперплоскостям, на которых строим k-грани
          for (int i = 1; i < PolytopDim - 1; i++) { // Цикл по размерности узлов, по которым будем строить решётку
            // строим узел размерности i+1

            // собрали все грани размерности i, которые лежат в данной гиперплоскости -- они будут формировать (i+1) грань
            var set = FL[i].Where(node => hp.Contains(node.InnerPoint)).ToList();
            // перебираем по две штуки
            for (int fst = 0; fst < set.Count - 1; fst++) {
              for (int snd = fst + 1; snd < set.Count; snd++) {
                Vector innerPoint = (set[fst].InnerPoint + set[snd].InnerPoint) / Tools.Two;

                int iP_belong = vHP.Count(vhp => vhp.Contains(innerPoint));
                if (iP_belong + i + 1 == PolytopDim) { // Кажется, что это поможет отфильтровать лишние грани

                  bool found = false;
                  foreach (FLNode supper in FL[i + 1]) {
                    // Если узел уже есть, то устанавливаем связи. Смотрим на аффинные пространства узлов!
                    if (supper.AffBasis.Contains(innerPoint)) {
                      FLNode.Connect(set[fst], supper, false);
                      FLNode.Connect(set[snd], supper, false);
                      found = true;

                      break;
                    }
                  }
                  if (!found) { // иначе создаём узел и устанавливаем связи
                    FL[i + 1].Add(new FLNode(new List<FLNode>() { set[fst], set[snd] }));
                  }
                }
              }
            }
          }
        }
      }
      FL[PolytopDim].Add(new FLNode(FL[^2]));


      return new FaceLattice(FL, true);
    }

    /// <summary>
    /// Finds an initial vertex of the convex polytope using a naive approach by checking combinations of hyperplanes.
    /// </summary>
    /// <param name="HPs">List of hyperplanes defining the Hrep.</param>
    /// <param name="m">Total number of hyperplanes.</param>
    /// <param name="d">Dimension of the polytop.</param>
    /// <returns>A vertex of the polytop, or null if no vertex is found.</returns>
    private static Vector? FindInitialVertex_Naive(List<HyperPlane> HPs, int m, int d) {
      Combination          combination = new Combination(m, d);
      Func<int, int, TNum> AFunc       = (r, l) => HPs[combination[r]].Normal[l];
      Func<int, TNum>      bFunc       = r => HPs[combination[r]].ConstantTerm;
      GaussSLE             gaussSLE    = new GaussSLE(d, d);
      bool                 belongs;
      bool                 goNext     = true;
      List<Vector>         firstPoint = new List<Vector>();
      do { // Перебираем все сочетания из d элементов из набора гиперплоскостей в поиске любой вершины.
        gaussSLE.SetSystem(AFunc, bFunc, d, d, GaussSLE.GaussChoice.All);
        gaussSLE.Solve();
        if (gaussSLE.GetSolution(out Vector point)) { // Ищем точку пересечения
          belongs = true;
          foreach (HyperPlane hp in HPs) {
            if (hp.ContainsPositive(point)) {
              belongs = false;

              break;
            }
          }
          if (belongs) {
            firstPoint.Add(point);
            goNext = false;
          }
        }
      } while (goNext && combination.Next());

      return firstPoint.Count != 0 ? firstPoint.First() : null;
    }

  }

}
