using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using MinkowskiDiffAlgorithm = CGLibrary.Geometry<double, Tests.DConvertor>.MinkowskiDiff;

namespace Tests.DoubleGeometry.Research.MinkowskiDiff;

[TestFixture]
public class MinkowskiDiffCyclicResearchTests {

  [Test]
  public void Cyclic3D_n15_step01_HrepToVrepGeometric_ReconstructsExpectedVertexCount() {
    // Исследовательская точка отсчёта для cyclic-polytopes в double.
    //
    // Наблюдения на 2026-04-05:
    // - HrepToVrep_Geometric(Cyclic(3, n, 0.01).Hrep) проходит для n = 4,5,6,8,10,12,20,25,30.
    // - Тот же прогон падает с "The set of inequalities is unbounded!" для n = 15,35,40,45,50.
    // - Для n = 15 чувствительность к step заметная:
    //   step = 0.5, 0.1, 0.05 -> проходит;
    //   step = 1.0, 0.01, 0.005 -> падает с assert про unbounded;
    //   step = 0.001 -> не падает, но даёт неверный vertex count (31 вместо 15).
    //
    // Этот тест специально фиксирует один проходящий параметр, близкий к зоне нестабильности:
    // Cyclic(3, 15, 0.1).
    ConvexPolytop cyclic = ConvexPolytop.Cyclic(3, 15, 0.1);

    SortedSet<Vector>? geometricVrep = ConvexPolytop.HrepToVrep_Geometric(cyclic.Hrep);

    Assert.Multiple(() => {
      Assert.That(geometricVrep, Is.Not.Null);
      Assert.That(geometricVrep!.Count, Is.EqualTo(cyclic.Vrep.Count));
      Assert.That(geometricVrep.SetEquals(cyclic.Vrep), Is.True);
    });
  }

  [Test]
  public void MinkowskiDiffGeometric_ForCyclic3D_n15_step01_MatchesNaiveOnOriginPoint() {
    // Для G = {0} разность Минковского не должна менять F.
    // Этот кейс нужен как исследовательский "зелёный" эталон рядом с падающими параметрами.
    //
    // Наблюдения на 2026-04-05:
    // - MinkowskiDiff.{Naive,Geometric}(Cyclic(3, 15, 0.1), {0}) -> обе ветки проходят и совпадают.
    // - При step = 0.01 тот же Geometric падает в HrepToVrep_Geometric.
    // - При ещё меньших шагах возможен уже и последующий сбой GiftWrapping на "Points too close".
    ConvexPolytop cyclic = ConvexPolytop.Cyclic(3, 15, 0.1);
    ConvexPolytop point = ConvexPolytop.CreateFromPoints([Vector.Zero(3)]);

    ConvexPolytop? naive = MinkowskiDiffAlgorithm.Naive(cyclic, point);
    ConvexPolytop? geometric = MinkowskiDiffAlgorithm.Geometric(cyclic, point);

    Assert.Multiple(() => {
      Assert.That(naive, Is.Not.Null);
      Assert.That(geometric, Is.Not.Null);
      Assert.That(geometric, Is.EqualTo(naive));
    });
  }

}
