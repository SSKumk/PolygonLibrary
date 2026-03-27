using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.DoubleGeometry.Basics;

public abstract class ConvexPolygonIntersectionTestBase {

  protected readonly LinkedList<Vector2D> SquareList = new LinkedList<Vector2D>
    (
     new List<Vector2D>
       {
         new Vector2D(0, 0)
       , new Vector2D(6, 0)
       , new Vector2D(6, 6)
       , new Vector2D(0, 6)
       }
    );

  protected void CyclicListComparison(List<Vector2D> l1, List<Vector2D> l2, string mes) {
    Assert.That(l1.Count, Is.EqualTo(l2.Count), mes + ": lengths of the lists are different");
    int i2 = l2.IndexOf(l1[0]);
    Assert.That(i2, Is.GreaterThanOrEqualTo(0), mes + ": the second list does not contain the point " + l1[0]);
    for (int i1 = 0; i1 < l1.Count; i1++, i2 = (i2 + 1) % l2.Count) {
      Assert.That(
         l1[i1].CompareTo(l2[i2]), Is.EqualTo(0),
         mes + ": point #" + i1 + " " + l1[i1] + " of the 1st list is not equal to point #" + i2 + " " + l2[i2] +
         " of the 2nd list"
      );
    }
  }

  protected void DoIntersectionTest(string mes, LinkedList<Vector2D> P_List, LinkedList<Vector2D> Q_List, List<Vector2D> answer) {
    for (int p = 0; p < P_List.Count; p++) {
      ConvexPolygon P = new ConvexPolygon(P_List);
      for (int q = 0; q < Q_List.Count; q++) {
        ConvexPolygon Q = new ConvexPolygon(Q_List);
        ConvexPolygon? resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
        ConvexPolygon? resQP = ConvexPolygon.IntersectionPolygon(Q, P);

        Assert.That(resPQ, Is.Not.Null, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]} is Null");
        Assert.That(resQP, Is.Not.Null, $"Intersection{mes}: Q-P : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]} is Null");

        CyclicListComparison(answer, resPQ.Vertices, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        CyclicListComparison(answer, resQP.Vertices, $"Intersection{mes}: Q-P : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Q_List.CyclicShift();
      }
      P_List.CyclicShift();
    }
  }

  protected void NullIntersectionTest(string mes, LinkedList<Vector2D> P_List, LinkedList<Vector2D> Q_List) {
    for (int p = 0; p < P_List.Count; p++) {
      ConvexPolygon P = new ConvexPolygon(P_List);
      for (int q = 0; q < Q_List.Count; q++) {
        ConvexPolygon Q = new ConvexPolygon(Q_List);
        ConvexPolygon? resPQ = ConvexPolygon.IntersectionPolygon(P, Q);
        ConvexPolygon? resQP = ConvexPolygon.IntersectionPolygon(Q, P);
        Assert.That(resPQ, Is.Null, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Assert.That(resQP, Is.Null, $"Intersection{mes}: P-Q : P starts at {P.Vertices[0]}, Q starts at {Q.Vertices[0]}");
        Q_List.CyclicShift();
      }
      P_List.CyclicShift();
    }
  }

}
