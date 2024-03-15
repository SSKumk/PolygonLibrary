using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;


[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[ShortRunJob]
public class ConvexPolytopesBench {

/*
 Создаём только точки!
| Method     | dimCubes | Mean      | Error     | StdDev    | Gen0    | Gen1   | Allocated |
|----------- |--------- |----------:|----------:|----------:|--------:|-------:|----------:|
| MakeCube3D | 3        |  3.202 us | 0.0624 us | 0.1077 us |  0.9727 | 0.0038 |   5.96 KB |
| MakeCube3D | 4        |  7.034 us | 0.0813 us | 0.0634 us |  1.9608 | 0.0305 |  12.04 KB |
| MakeCube3D | 5        | 15.908 us | 0.3007 us | 0.2665 us |  4.4250 | 0.1526 |  27.22 KB |
| MakeCube3D | 6        | 36.688 us | 0.6076 us | 0.5386 us | 10.0708 | 0.7935 |  61.91 KB |
| MakeCube3D | 7        | 83.859 us | 1.6527 us | 2.4737 us | 22.7051 | 3.7842 | 139.56 KB |
*/

  // [Params(3, 4, 5, 6, 7)]
  // public int dimCubes;
  //
  // [Benchmark]
  // public void MakeCubes01() => ConvexPolytop.Cube01(dimCubes);

  /*

// todo узнать можно ли посмотреть пиковое потребление памяти ?
// todo не в микро секундах!

| Method      | dimSpheres | thetaPartition | phiPartition | Mean             | Error             | StdDev          | Gen0         | Gen1         | Gen2       | Allocated      |
|------------ |----------- |--------------- |------------- |-----------------:|------------------:|----------------:|-------------:|-------------:|-----------:|---------------:|
| MakeSpheres | 3          | 10             | 10           |         382.1 us |          15.78 us |         0.86 us |      15.1367 |       0.9766 |          - |       93.29 KB |
| MakeSpheres | 3          | 10             | 16           |         640.5 us |         357.74 us |        19.61 us |      22.4609 |       1.9531 |          - |       141.8 KB |
| MakeSpheres | 3          | 18             | 10           |         682.1 us |         238.23 us |        13.06 us |      24.4141 |       1.9531 |          - |       150.3 KB |
| MakeSpheres | 3          | 18             | 16           |       1,096.4 us |         138.19 us |         7.57 us |      39.0625 |       5.8594 |          - |      245.74 KB |
| MakeSpheres | 3          | 10             | 32           |       1,252.9 us |          83.78 us |         4.59 us |      44.9219 |       7.8125 |          - |      284.22 KB |
| MakeSpheres | 3          | 36             | 10           |       1,328.1 us |         101.12 us |         5.54 us |      46.8750 |       7.8125 |          - |      290.53 KB |
| MakeSpheres | 3          | 36             | 16           |       2,151.1 us |         277.17 us |        15.19 us |      74.2188 |      19.5313 |          - |      476.17 KB |
| MakeSpheres | 3          | 18             | 32           |       2,272.3 us |         291.85 us |        16.00 us |      78.1250 |      19.5313 |          - |      493.03 KB |
| MakeSpheres | 3          | 10             | 64           |       2,512.9 us |          33.42 us |         1.83 us |      89.8438 |      19.5313 |          - |       570.5 KB |
| MakeSpheres | 3          | 72             | 10           |       2,654.4 us |         471.13 us |        25.82 us |      89.8438 |      27.3438 |          - |       572.2 KB |
| MakeSpheres | 3          | 72             | 16           |       4,269.7 us |         362.26 us |        19.86 us |     148.4375 |      62.5000 |          - |       939.1 KB |
| MakeSpheres | 3          | 36             | 32           |       4,356.0 us |         359.77 us |        19.72 us |     148.4375 |      23.4375 |          - |      955.33 KB |
| MakeSpheres | 3          | 18             | 64           |       4,511.5 us |       1,105.74 us |        60.61 us |     156.2500 |      15.6250 |          - |      990.06 KB |
| MakeSpheres | 4          | 10             | 10           |       7,042.4 us |         114.58 us |         6.28 us |     164.0625 |      62.5000 |          - |     1038.72 KB |
| MakeSpheres | 3          | 36             | 64           |       8,874.6 us |         612.34 us |        33.56 us |     312.5000 |     171.8750 |          - |     1919.08 KB |
| MakeSpheres | 3          | 72             | 32           |       8,984.6 us |       7,735.93 us |       424.03 us |     296.8750 |     140.6250 |          - |     1884.61 KB |
| MakeSpheres | 4          | 10             | 16           |      11,678.3 us |       8,105.33 us |       444.28 us |     265.6250 |      15.6250 |          - |     1687.63 KB |
| MakeSpheres | 3          | 72             | 64           |      19,152.5 us |       1,841.45 us |       100.94 us |     562.5000 |     187.5000 |    93.7500 |     3784.48 KB |
| MakeSpheres | 4          | 18             | 10           |      21,563.6 us |       1,041.62 us |        57.09 us |     500.0000 |     375.0000 |          - |     3163.38 KB |
| MakeSpheres | 4          | 10             | 32           |      23,156.8 us |       2,352.36 us |       128.94 us |     531.2500 |     343.7500 |          - |     3385.14 KB |
| MakeSpheres | 4          | 18             | 16           |      38,139.8 us |      20,157.44 us |     1,104.90 us |     769.2308 |     307.6923 |   153.8462 |     5172.31 KB |
| MakeSpheres | 4          | 10             | 64           |      48,270.9 us |       2,247.38 us |       123.19 us |    1000.0000 |     363.6364 |   181.8182 |     6789.06 KB |
| MakeSpheres | 4          | 18             | 32           |      73,263.0 us |       7,542.78 us |       413.45 us |    1571.4286 |     571.4286 |   285.7143 |    10383.09 KB |
| MakeSpheres | 4          | 36             | 10           |      89,512.8 us |      53,588.93 us |     2,937.39 us |    1833.3333 |     833.3333 |   500.0000 |    12073.78 KB |
| MakeSpheres | 5          | 10             | 10           |     112,609.9 us |       8,424.55 us |       461.78 us |    2200.0000 |     800.0000 |   400.0000 |     14870.4 KB |
| MakeSpheres | 4          | 36             | 16           |     142,688.0 us |      23,162.18 us |     1,269.60 us |    2750.0000 |    1000.0000 |   750.0000 |    19794.48 KB |
| MakeSpheres | 4          | 18             | 64           |     153,314.8 us |      35,696.99 us |     1,956.67 us |    3000.0000 |    1000.0000 |   750.0000 |     20842.7 KB |
| MakeSpheres | 5          | 10             | 16           |     182,251.7 us |      16,696.91 us |       915.21 us |    3666.6667 |    1666.6667 |   666.6667 |    24023.98 KB |
| MakeSpheres | 4          | 36             | 32           |     290,265.5 us |     153,185.27 us |     8,396.60 us |    6000.0000 |    2000.0000 |  1000.0000 |    39746.08 KB |
| MakeSpheres | 4          | 72             | 10           |     365,444.2 us |     169,327.29 us |     9,281.40 us |    7000.0000 |    2000.0000 |  1000.0000 |    47336.55 KB |
| MakeSpheres | 5          | 10             | 32           |     368,650.1 us |      33,343.28 us |     1,827.66 us |    7000.0000 |    2000.0000 |  1000.0000 |     48124.9 KB |
| MakeSpheres | 4          | 72             | 16           |     591,076.3 us |     102,491.32 us |     5,617.90 us |   12000.0000 |    4000.0000 |  2000.0000 |    77784.62 KB |
| MakeSpheres | 4          | 36             | 64           |     598,737.8 us |      25,907.36 us |     1,420.07 us |   12000.0000 |    4000.0000 |  2000.0000 |    79811.54 KB |
| MakeSpheres | 5          | 18             | 10           |     637,464.6 us |     684,024.79 us |    37,493.71 us |   12000.0000 |    4000.0000 |  2000.0000 |    77548.75 KB |
| MakeSpheres | 5          | 10             | 64           |     744,283.0 us |     204,884.37 us |    11,230.40 us |   16000.0000 |    5000.0000 |  2000.0000 |    96408.95 KB |
| MakeSpheres | 5          | 18             | 16           |   1,035,894.5 us |     266,656.70 us |    14,616.35 us |   21000.0000 |    8000.0000 |  3000.0000 |   126131.13 KB |
| MakeSpheres | 4          | 72             | 32           |   1,195,211.7 us |     149,905.18 us |     8,216.81 us |   24000.0000 |    8000.0000 |  3000.0000 |   156232.35 KB |
| MakeSpheres | 5          | 18             | 32           |   2,032,126.4 us |     311,233.12 us |    17,059.74 us |   40000.0000 |   12000.0000 |  3000.0000 |   252918.56 KB |
| MakeSpheres | 4          | 72             | 64           |   2,445,702.4 us |     990,452.45 us |    54,290.04 us |   48000.0000 |   15000.0000 |  4000.0000 |   293890.38 KB |
| MakeSpheres | 5          | 18             | 64           |   4,116,135.4 us |     842,746.56 us |    46,193.78 us |   80000.0000 |   24000.0000 |  5000.0000 |   487271.51 KB |
| MakeSpheres | 5          | 36             | 10           |   4,884,558.1 us |     142,280.29 us |     7,798.86 us |   91000.0000 |   32000.0000 |  6000.0000 |   577613.34 KB |
| MakeSpheres | 5          | 36             | 16           |   8,032,787.9 us |   1,730,791.06 us |    94,870.50 us |  145000.0000 |   51000.0000 |  8000.0000 |   942423.38 KB |
| MakeSpheres | 5          | 36             | 32           |  16,187,444.6 us |   2,822,519.99 us |   154,711.84 us |  285000.0000 |   95000.0000 | 10000.0000 |   1804940.8 KB |
| MakeSpheres | 5          | 36             | 64           |  32,129,561.9 us |   3,294,149.09 us |   180,563.43 us |  561000.0000 |  184000.0000 | 12000.0000 |  3615753.62 KB |
| MakeSpheres | 5          | 72             | 10           |  38,424,358.9 us |   7,119,738.72 us |   390,256.90 us |  665000.0000 |  222000.0000 |  9000.0000 |  4477232.21 KB |
| MakeSpheres | 5          | 72             | 16           |  61,122,218.9 us |     500,425.66 us |    27,430.02 us | 1060000.0000 |  360000.0000 | 10000.0000 |  6957486.33 KB |
| MakeSpheres | 5          | 72             | 32           | 125,565,156.7 us |  31,070,416.48 us | 1,703,074.37 us | 2113000.0000 |  686000.0000 | 12000.0000 | 13922975.24 KB |
| MakeSpheres | 5          | 72             | 64           | 275,989,847.6 us | 110,431,568.47 us | 6,053,126.90 us | 4216000.0000 | 1365000.0000 | 14000.0000 | 27853990.65 KB |
   */

  // [Params(3, 4, 5)]
  // // ReSharper disable once UnassignedField.Global
  // public int dimSpheres;
  //
  // [Params( 10, 18, 36, 72)]
  // // ReSharper disable once UnassignedField.Global
  // public int thetaPartition;
  //
  // [Params(10, 16, 32, 64)]
  // // ReSharper disable once UnassignedField.Global
  // public int phiPartition;
  //
  // [Benchmark]
  // public void MakeSpheres() => ConvexPolytop.Sphere(dimSpheres, thetaPartition, phiPartition, Vector.Zero(dimSpheres), 1);

  /*
 Создаём только точки!

  */

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<ConvexPolytopesBench>();
  //   }
  //
  // }

}