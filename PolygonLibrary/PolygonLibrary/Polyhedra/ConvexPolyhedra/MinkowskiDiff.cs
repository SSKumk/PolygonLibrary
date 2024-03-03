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

  public class MinkowskiDiff {

    public static ConvexPolytop? Naive(ConvexPolytop F, ConvexPolytop G) {
      return MinkDiff
               (
                F.HRep
              , G.VRep
              , out ConvexPolytop diffFG
              , FindExtrInCPOnVector_Naive
              , doSubtract
              , ConvexPolytop.HRepToVRep_Naive
              , GiftWrapping.WrapFaceLattice
               )
               ? diffFG
               : null;
    }

    /* 1.
     * Варианты первого пункта!
     * 1) Для каждого l тупо решить LP-задачу на G. O(|N(F)| * |N(G)|*d^3).
     *
     * 2) Есть надежда, что, если l' и l'' - соседние векторы в N(F), то v(l') и v(l'') расположены недалеко в 1-рёберном графе G.
     *    Для какого-то вектора l1 \in N(F) честно решаем задачу LP1. В конце не забываем таблицу симплекс-метода.
     *    Для какого-то соседнего вектора l2 пересчитываем в таблице строку, связанную с целевой функцией, и продолжаем работу симплекс-метода.
     */

    /*
     * Для каждого l \in N(F) перебрать все v \in V(G) и найти максимум l*v. Omega(|N(F)| * |V(G)|*d). \
     * Если многогранник плохой, то мы выиграем у симплекс-метода за счёт d вместо d^3.
     * Но в среднем мы будем проигрывать, так как симплекс-метод не идёт по всем вершинам и множитель |V(G)| - грубая оценка сверху.
     */

    public static Vector FindExtrInCPOnVector_Naive(VPolytop P, Vector l) {
      Vector extr    = new Vector(P.Vertices.First());
      TNum   extrVal = l * extr;

      foreach (Vector vertex in P.Vertices) {
        Vector vec = new Vector(vertex);
        TNum   val = l * vec;
        if (val > extrVal) {
          extrVal = val;
          extr    = vec;
        }
      }

      return extr;
    }


    /* 2.
     * Вроде ничего другого тут и нет
     */

    public static HyperPlane doSubtract(HyperPlane minuend, Vector subtrahend) {
      return new HyperPlane(minuend.Normal, minuend.ConstantTerm - minuend.Normal * subtrahend);
    }

    /* 3. ???
     *
     */

    /* 4.
     * HRepToVRep_Naive Реализована в классе ConvexPolytop
     *
     *
     * Возможно нужно перебирать не все кортежи, а те, которые получаются при путешествии по 1-рёберному графу искомого многогранника.
     * Но у нас нет такого графа. Можно ли его получить из H-представления многогранника?
     *
     * Кроме того, эту оптимизацию можно проводить, используя информацию о соседстве векторов в Г. Эта информация есть, если мы её получили ранее и не проводили пункт 3).
     * O(d^3 * |V(F-G)|) в худшем случае d-симплекса это в точности предыдущая сложность, но наши многогранники, скорее всего , не симплексы.
     */


    /* 5.
     * FaceLattice можно получить из класса GiftWrapping
     */


    public static bool MinkDiff(
        HPolytop                                  F
      , VPolytop                                  G
      , out ConvexPolytop                         diffFG
      , Func<VPolytop, Vector, Vector>            findExtrInG_on_lFromNF
      , Func<HyperPlane, Vector, HyperPlane>      doSubtract // <-- todo Как назвать?
      , Func<HPolytop, VPolytop>                  HRepToVRep
      , Func<IEnumerable<Vector>, FaceLattice>    produceFL
      , Func<List<HyperPlane>, List<HyperPlane>>? doHRedundancy = null
      ) {
      List<HyperPlane> gamma = new List<HyperPlane>();
      foreach (HyperPlane hpF in F.Faces) {
        // 1) Для каждого l \in N(F) найти v(l) \in V(G), экстремальную на l.
        Vector extrOn_l = findExtrInG_on_lFromNF(G, hpF.Normal);


        // 2) Для каждого l \in N(F) построить пару (l,C'(l)) = (l, C(l) - l*v(l)) - построить функцию \gamma. C - свободный член гиперграни.
        gamma.Add(doSubtract(hpF, extrOn_l));
      }


      // 3? Провести H-redundancy на наборе gamma = {(l,C'(l))}.


      // 4) Построить V - representation V(F - G) набора Г = { (l, C'(l)) }
      VPolytop VRepFminusG = HRepToVRep(new HPolytop(gamma));


      // 5) Построить FL роя V(F-G)
      if (VRepFminusG.Vertices.Count < 3) {
        diffFG = new ConvexPolytop(new List<Vector>());

        return false;
      }

      diffFG = new ConvexPolytop(produceFL(VRepFminusG.Vertices));

      return true;
    }

  }

}
