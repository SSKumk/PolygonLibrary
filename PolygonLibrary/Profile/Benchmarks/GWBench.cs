using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class GWBench {

  [Params(3, 4, 5, 6, 7)]
  // ReSharper disable once UnassignedField.Global
  public int dim;

  private ConvexPolytop? simplex;
  private ConvexPolytop? cube;
  private ConvexPolytop? cycle;

  [Params(8, 16, 32)]
  // ReSharper disable once UnassignedField.Global
  public int amountPointsOnMomentCurve;

  [GlobalCleanup]
  public void SetUp() {
    simplex = ConvexPolytop.SimplexRND(dim);
    cube    = ConvexPolytop.Cube01(dim);
    cycle = ConvexPolytop.CyclicPolytop
      (dim, amountPointsOnMomentCurve, 1 / (amountPointsOnMomentCurve * amountPointsOnMomentCurve));
  }

/*
  Заворачиваем случайные симплексы.

*/
  [Benchmark]
  public void GWSimplexRND() => GiftWrapping.WrapVRep(simplex!.VRep);

/*
  Заворачиваем кубы.

*/
  [Benchmark]
  public void GWCube() => GiftWrapping.WrapVRep(cube!.VRep);

/*
  Заворачиваем циклические многогранники. Количество точек было выбрано "из головы".

*/
  [Benchmark]
  public void GWCyclicPolytop() => GiftWrapping.WrapVRep(cycle!.VRep);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<GWBench>();
  //   }
  //
  // }

}
