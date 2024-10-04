namespace CGLibrary;

public partial class Geometry<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public class HrepToFLrep {

    public static FaceLattice HrepToFLrep_Geometric(List<HyperPlane> HPs, int PolytopDim) {
      SortedDictionary<FLNode, List<HyperPlane>> vToHPs = new SortedDictionary<FLNode, List<HyperPlane>>();
      List<SortedDictionary<Vector, FLNode>>     FL     = new List<SortedDictionary<Vector, FLNode>>(); // 0 and 1 lvls
      for (int i = 0; i < PolytopDim; i++) {
        FL.Add(new SortedDictionary<Vector, FLNode>());
      }
      
      int                                        m      = HPs.Count;
      int                                        d      = HPs.First().Normal.SpaceDim;

      // Этап 1. Поиск какой-либо вершины и определение гиперплоскостей, которым она принадлежит
      // Наивная реализация
      Vector? firstPoint = FindInitialVertex_Naive(HPs, m, d);
      if (firstPoint is null) {
        throw new ArgumentException($"This is not a polytope!");
      }
      FL[0].Add(firstPoint, new FLNode(firstPoint));

      // Этап 2. Поиск всех остальных вершин
      Queue<(FLNode, List<HyperPlane>)> process = new Queue<(FLNode, List<HyperPlane>)>();
      process.Enqueue((FL[0].First().Value, HPs.Where(hp => hp.Contains(FL[0].First().Key)).ToList()));

      // Обход в ширину
      while (process.TryDequeue(out (FLNode, List<HyperPlane>) elem)) {
        (FLNode z, List<HyperPlane> Hz) = elem;
        vToHPs.Add(z, Hz);

        Combination J = new Combination(Hz.Count, d - 1);
        do // перебираем все 1-грани, то есть рёбра.
        {
          List<HyperPlane> edgeLine = new List<HyperPlane>(d - 1);
          for (int j = 0; j < d - 1; j++) {
            edgeLine.Add(Hz[J[j]]);
          }
          LinearBasis edge_coSpace = new LinearBasis(edgeLine.Select(hp => hp.Normal));
          bool        isEdge       = true;


          // ищем направляющий вектор прямой, перпендикулярный линейному пространству edge
          Vector v = edge_coSpace.FindOrthonormalVector();

          // проверяем вектор v
          bool firstNonZeroProduct = true;
          foreach (HyperPlane hp in Hz) {
            TNum dotProduct = v * hp.Normal;

            // Если скалярное произведение равно нулю, игнорируем
            if (!Tools.EQ(dotProduct)) {
              // Первое ненулевое скалярное произведение определяет ориентацию вектора v
              if (firstNonZeroProduct) {
                if (Tools.GT(dotProduct)) { v = -v; }
                firstNonZeroProduct = false;
              } else { // Для всех последующих не нулевых произведений требуется, чтобы они были отрицательные
                if (Tools.GT(dotProduct)) {
                  isEdge = false;

                  break;
                }
              }
            }
          }
          if (isEdge) {
            // Теперь v определяет луч, на котором лежит ребро
            List<HyperPlane> zNewHPs = new List<HyperPlane>();
            TNum             tMin    = Tools.PositiveInfinity;
            Vector           zNew    = Vector.Zero(d);
            foreach (HyperPlane hp in HPs) {
              TNum denominator = hp.Normal * v;

              // если ноль, то эта гиперплоскость формирует ребро, вдоль которого мы движемся
              if (Tools.NE(denominator)) {
                TNum ti = (hp.ConstantTerm - hp.Normal * z.InnerPoint) / denominator;

                if (Tools.EQ(ti, tMin)) {
                  zNewHPs.Add(hp);
                }
                if (Tools.GT(ti) && Tools.LT(ti, tMin)) {
                  tMin = ti;
                  zNewHPs.Clear();
                  zNewHPs.Add(hp);
                  zNew = Vector.MulByNumAndAdd(v, tMin, z.InnerPoint); // z + tMin * v; // v*tMin + z
                }
              }
            }
            FLNode zNew_node = new FLNode(zNew);
            FL[1].Add((z.InnerPoint+zNew_node.InnerPoint)/Tools.Two,new FLNode(new List<FLNode>()
              {
                z, zNew_node
              }));
            if (!FL[0].TryGetValue(zNew, out FLNode? _)) {
              FL[0].Add(zNew, zNew_node);
              edgeLine.AddRange(zNewHPs);
              process.Enqueue((zNew_node, edgeLine));
            }
          }
        } while (J.Next());
      }

      // Теперь собираем всю оставшуюся решётку
      foreach (FLNode vertex in FL[0].Values) {
        List<HyperPlane> vHP = vToHPs[vertex];
        foreach (HyperPlane hp in vHP) {             // Цикл по гиперплоскостям, на которых строим k-грани
          for (int i = 1; i < PolytopDim - 1; i++) { // Цикл по размерности узлов, по которым будем строить решётку
            // строим узел размерности i+1
            var set = FL[i].Where(node => hp.Contains(node.Value.InnerPoint)).ToList();
            // перебираем по две штуки
            for (int fst = 0; fst < set.Count - 1; fst++) {
              for (int snd = fst + 1; snd < set.Count; snd++) {
                Vector innerPoint = (set[fst].Value.InnerPoint + set[snd].Value.InnerPoint) / Tools.Two;

                int iP_belong = vHP.Count(vhp => vhp.Contains(innerPoint));
                if (iP_belong + i + 1 == PolytopDim) { // Кажется, что это поможет отфильтровать лишние грани
                  if (FL[i+1].TryGetValue(innerPoint, out FLNode? supper)) { // если узел уже есть, то устанавливаем связи
                    FLNode.Connect(set[fst].Value, supper);
                    FLNode.Connect(set[snd].Value, supper);
                  } else {
                    // todo
                  }
                  var x = new FLNode([set[fst], set[snd]]);

                }
              }
            }
          }
          // FL[PolytopDim] = ...
        }
      }


      return new FaceLattice(FL);
    }

    /// <summary>
    /// Finds an initial vertex of the convex polytop using a naive approach by checking combinations of hyperplanes.
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
