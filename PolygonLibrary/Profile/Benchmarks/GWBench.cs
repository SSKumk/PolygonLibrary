using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;




[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[ShortRunJob]
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

  [GlobalSetup]
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


  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<GWBench>();
    }

  }

  /*
| Method          | dim | amountPoints| Mean              | Error             | StdDev          |
|---------------- |---- |-------------|------------------:|------------------:|----------------:|
| GWCyclicPolytop | 3   | 32          |          3.330 us |         0.3395 us |       0.0186 us |
| GWCyclicPolytop | 3   | 8           |          3.454 us |         1.0647 us |       0.0584 us |
| GWCyclicPolytop | 3   | 16          |          3.505 us |         0.4793 us |       0.0263 us |
| GWCyclicPolytop | 4   | 16          |          3.665 us |         0.3413 us |       0.0187 us |
| GWCyclicPolytop | 4   | 8           |          3.711 us |         0.6794 us |       0.0372 us |
| GWCyclicPolytop | 4   | 32          |          3.768 us |         1.0054 us |       0.0551 us |
| GWCyclicPolytop | 5   | 16          |          4.134 us |         0.6574 us |       0.0360 us |
| GWCyclicPolytop | 5   | 8           |          4.145 us |         1.3051 us |       0.0715 us |
| GWCyclicPolytop | 5   | 32          |          4.164 us |         0.3630 us |       0.0199 us |
| GWCyclicPolytop | 6   | 8           |          4.491 us |         0.2307 us |       0.0126 us |
| GWCyclicPolytop | 6   | 32          |          4.512 us |         0.5697 us |       0.0312 us |
| GWCyclicPolytop | 6   | 16          |          4.526 us |         1.0227 us |       0.0561 us |
| GWCyclicPolytop | 7   | 32          |          4.794 us |         0.1508 us |       0.0083 us |
| GWCyclicPolytop | 7   | 8           |          4.812 us |         0.4420 us |       0.0242 us |
| GWCyclicPolytop | 7   | 16          |          5.333 us |         5.2783 us |       0.2893 us |
| GWSimplexRND    | 3   | 32          |          6.213 us |         0.4272 us |       0.0234 us |
| GWSimplexRND    | 3   | 8           |          6.321 us |         0.6806 us |       0.0373 us |
| GWSimplexRND    | 3   | 16          |          6.678 us |         2.0040 us |       0.1098 us |
| GWSimplexRND    | 4   | 16          |          9.727 us |         0.9667 us |       0.0530 us |
| GWSimplexRND    | 4   | 8           |          9.738 us |         1.5193 us |       0.0833 us |
| GWSimplexRND    | 4   | 32          |          9.763 us |         1.7990 us |       0.0986 us |
| GWSimplexRND    | 5   | 8           |         13.642 us |         0.5159 us |       0.0283 us |
| GWSimplexRND    | 5   | 16          |         13.963 us |         0.7472 us |       0.0410 us |
| GWSimplexRND    | 5   | 32          |         13.984 us |         5.1714 us |       0.2835 us |
| GWSimplexRND    | 6   | 8           |         19.143 us |         2.0046 us |       0.1099 us |
| GWSimplexRND    | 6   | 32          |         19.241 us |         2.4472 us |       0.1341 us |
| GWSimplexRND    | 6   | 16          |         19.675 us |         8.3220 us |       0.4562 us |
| GWSimplexRND    | 7   | 16          |         25.997 us |         8.3994 us |       0.4604 us |
| GWSimplexRND    | 7   | 8           |         26.122 us |         5.2370 us |       0.2871 us |
| GWSimplexRND    | 7   | 32          |         26.402 us |         7.6916 us |       0.4216 us |
| GWCube          | 3   | 8           |        294.554 us |        68.5771 us |       3.7589 us |
| GWCube          | 3   | 32          |        312.823 us |        14.2745 us |       0.7824 us |
| GWCube          | 3   | 16          |        319.741 us |        41.3934 us |       2.2689 us |
| GWCube          | 4   | 32          |      3,267.144 us |       450.3712 us |      24.6864 us |
| GWCube          | 4   | 16          |      3,309.474 us |       138.6029 us |       7.5973 us |
| GWCube          | 4   | 8           |      3,659.407 us |       822.3518 us |      45.0759 us |
| GWCube          | 5   | 32          |     47,463.033 us |    37,508.9614 us |   2,055.9927 us |
| GWCube          | 5   | 16          |     47,481.288 us |    19,443.3759 us |   1,065.7570 us |
| GWCube          | 5   | 8           |     51,846.373 us |    22,021.3610 us |   1,207.0651 us |
| GWCube          | 6   | 16          |    742,726.533 us |    85,545.5056 us |   4,689.0378 us |
| GWCube          | 6   | 8           |    788,482.033 us |   986,509.3213 us |  54,073.9047 us |
| GWCube          | 6   | 32          |    815,578.900 us |    82,953.3734 us |   4,546.9543 us |
| GWCube          | 7   | 16          | 14,261,765.000 us | 1,072,172.3855 us |  58,769.3863 us |
| GWCube          | 7   | 8           | 14,308,697.233 us |   549,699.8933 us |  30,130.9060 us |
| GWCube          | 7   | 32          | 14,945,955.967 us | 4,871,682.2905 us | 267,033.3451 us |

  */
}
