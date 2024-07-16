using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;
using AVLUtils;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
[ShortRunJob]
public class SetBench {

  private List<Vector> Vs;

  [Params(3, 4, 5, 6, 7)]
  // [Params(3)]
  public int dim;

  [Params(10, 100, 1000, 10000, 100000)]
  // [Params(10)]
  public int size;

  [GlobalSetup]
  public void GlobalSetup() {
    Tools.Eps = 1;

    Vs        = new List<Vector>(size);
    for (int i = 0; i < size; i++) {
      Vector genVectorInt = Vector.GenVectorInt(dim, -1000, 1001);

      Vs.Add(genVectorInt);
    }
  }

  [Benchmark]
  public void HashSet_Add() {
    HashSet<Vector> Hs = new HashSet<Vector>();
    foreach (Vector v in Vs) { Hs.Add(v); }
  }

  [Benchmark]
  public void SortedSet_Add() {
    SortedSet<Vector> Ss = new SortedSet<Vector>();
    foreach (Vector v in Vs) { Ss.Add(v); }

  }

  [Benchmark]
  public void AVLSet_Add() {
    AVLSet<Vector> AVLs = new AVLSet<Vector>();
    foreach (Vector v in Vs) { AVLs.Add(v); }

  }


  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<SetBench>();
    }

  }

}



/*
| Method           | dim | size | Mean     | Error    | StdDev  |
|----------------- |---- |----- |---------:|---------:|--------:|
| HashSet_Create   | 3   | 1000 | 251.2 us | 21.57 us | 1.18 us |
| SortedSet_Create | 3   | 1000 | 358.7 us | 39.42 us | 2.16 us |


| Method        | dim | size   | Mean             | Error             | StdDev           |
|-------------- |---- |------- |-----------------:|------------------:|-----------------:|
| SortedSet_Add | 5   | 10     |         735.7 ns |          86.41 ns |          4.74 ns |
| SortedSet_Add | 4   | 10     |         745.8 ns |          93.44 ns |          5.12 ns |
| SortedSet_Add | 7   | 10     |         778.0 ns |          10.94 ns |          0.60 ns |
| SortedSet_Add | 6   | 10     |         787.2 ns |         317.84 ns |         17.42 ns |
| AVLSet_Add    | 3   | 10     |         815.5 ns |           8.32 ns |          0.46 ns |
| AVLSet_Add    | 5   | 10     |         833.9 ns |          29.04 ns |          1.59 ns |
| SortedSet_Add | 3   | 10     |         837.5 ns |          85.65 ns |          4.69 ns |
| AVLSet_Add    | 6   | 10     |         862.5 ns |         200.14 ns |         10.97 ns |
| AVLSet_Add    | 7   | 10     |         890.7 ns |         127.13 ns |          6.97 ns |
| AVLSet_Add    | 4   | 10     |         972.0 ns |          26.33 ns |          1.44 ns |
| HashSet_Add   | 3   | 10     |       2,607.9 ns |         526.08 ns |         28.84 ns |
| HashSet_Add   | 4   | 10     |       3,199.5 ns |         199.45 ns |         10.93 ns |
| HashSet_Add   | 5   | 10     |       4,072.5 ns |          23.84 ns |          1.31 ns |
| HashSet_Add   | 6   | 10     |       4,713.7 ns |         163.90 ns |          8.98 ns |
| HashSet_Add   | 7   | 10     |       5,424.9 ns |         463.75 ns |         25.42 ns |
| SortedSet_Add | 6   | 100    |      18,350.1 ns |       2,260.83 ns |        123.92 ns |
| SortedSet_Add | 3   | 100    |      18,666.5 ns |       5,675.78 ns |        311.11 ns |
| SortedSet_Add | 4   | 100    |      18,721.1 ns |       2,036.84 ns |        111.65 ns |
| SortedSet_Add | 5   | 100    |      19,366.8 ns |       1,360.59 ns |         74.58 ns |
| SortedSet_Add | 7   | 100    |      19,474.8 ns |      25,548.77 ns |      1,400.41 ns |
| AVLSet_Add    | 5   | 100    |      20,259.7 ns |       5,094.82 ns |        279.26 ns |
| AVLSet_Add    | 7   | 100    |      20,308.0 ns |       2,849.16 ns |        156.17 ns |
| AVLSet_Add    | 6   | 100    |      20,576.9 ns |         669.23 ns |         36.68 ns |
| AVLSet_Add    | 3   | 100    |      20,644.5 ns |       4,740.62 ns |        259.85 ns |
| AVLSet_Add    | 4   | 100    |      20,949.1 ns |       3,605.12 ns |        197.61 ns |
| HashSet_Add   | 3   | 100    |      26,746.1 ns |         623.12 ns |         34.16 ns |
| HashSet_Add   | 4   | 100    |      32,550.6 ns |       4,278.78 ns |        234.53 ns |
| HashSet_Add   | 5   | 100    |      40,637.6 ns |      12,977.22 ns |        711.33 ns |
| HashSet_Add   | 6   | 100    |      47,127.6 ns |       5,403.79 ns |        296.20 ns |
| HashSet_Add   | 7   | 100    |      55,193.4 ns |      19,352.61 ns |      1,060.78 ns |
| HashSet_Add   | 3   | 1000   |     267,494.7 ns |      36,643.90 ns |      2,008.58 ns |
| HashSet_Add   | 4   | 1000   |     329,744.8 ns |      26,311.79 ns |      1,442.24 ns |
| SortedSet_Add | 4   | 1000   |     340,918.9 ns |      45,813.52 ns |      2,511.19 ns |
| SortedSet_Add | 3   | 1000   |     344,505.0 ns |      24,632.10 ns |      1,350.17 ns |
| SortedSet_Add | 7   | 1000   |     348,255.2 ns |      17,736.35 ns |        972.19 ns |
| SortedSet_Add | 6   | 1000   |     357,880.0 ns |       6,280.94 ns |        344.28 ns |
| AVLSet_Add    | 5   | 1000   |     362,838.4 ns |       6,829.19 ns |        374.33 ns |
| SortedSet_Add | 5   | 1000   |     366,222.3 ns |     453,634.21 ns |     24,865.22 ns |
| AVLSet_Add    | 7   | 1000   |     369,316.9 ns |      76,504.56 ns |      4,193.47 ns |
| AVLSet_Add    | 4   | 1000   |     374,960.4 ns |      22,959.29 ns |      1,258.48 ns |
| AVLSet_Add    | 6   | 1000   |     377,788.3 ns |       3,147.62 ns |        172.53 ns |
| AVLSet_Add    | 3   | 1000   |     378,589.8 ns |     130,777.73 ns |      7,168.37 ns |
| HashSet_Add   | 5   | 1000   |     404,582.2 ns |      58,411.94 ns |      3,201.76 ns |
| HashSet_Add   | 6   | 1000   |     470,132.5 ns |       7,203.38 ns |        394.84 ns |
| HashSet_Add   | 7   | 1000   |     534,959.7 ns |     131,890.95 ns |      7,229.39 ns |
| HashSet_Add   | 3   | 10000  |   2,901,197.5 ns |      60,721.17 ns |      3,328.33 ns |
| HashSet_Add   | 4   | 10000  |   3,563,106.0 ns |   1,115,570.81 ns |     61,148.20 ns |
| HashSet_Add   | 5   | 10000  |   4,171,542.2 ns |     660,897.93 ns |     36,226.05 ns |
| HashSet_Add   | 6   | 10000  |   4,919,005.2 ns |   1,014,661.84 ns |     55,617.04 ns |
| HashSet_Add   | 7   | 10000  |   5,637,730.5 ns |     421,695.08 ns |     23,114.53 ns |
| SortedSet_Add | 4   | 10000  |   5,865,718.4 ns |   3,218,162.27 ns |    176,398.33 ns |
| SortedSet_Add | 5   | 10000  |   5,907,221.5 ns |   1,867,617.16 ns |    102,370.40 ns |
| SortedSet_Add | 6   | 10000  |   5,918,474.0 ns |     668,376.79 ns |     36,635.99 ns |
| SortedSet_Add | 7   | 10000  |   5,994,900.8 ns |   2,726,424.63 ns |    149,444.53 ns |
| SortedSet_Add | 3   | 10000  |   6,081,687.5 ns |     841,200.48 ns |     46,109.04 ns |
| AVLSet_Add    | 4   | 10000  |   6,413,809.4 ns |     618,604.33 ns |     33,907.79 ns |
| AVLSet_Add    | 7   | 10000  |   6,519,599.9 ns |   2,032,491.77 ns |    111,407.73 ns |
| AVLSet_Add    | 6   | 10000  |   6,736,755.7 ns |     500,705.88 ns |     27,445.38 ns |
| AVLSet_Add    | 3   | 10000  |   6,752,274.2 ns |   1,011,990.57 ns |     55,470.62 ns |
| AVLSet_Add    | 5   | 10000  |   6,914,136.5 ns |   2,388,220.47 ns |    130,906.42 ns |
| HashSet_Add   | 3   | 100000 |  28,583,072.9 ns |   5,179,441.15 ns |    283,902.65 ns |
| HashSet_Add   | 4   | 100000 |  37,412,823.8 ns |  27,059,224.19 ns |  1,483,207.39 ns |
| HashSet_Add   | 5   | 100000 |  42,306,850.0 ns |     841,616.78 ns |     46,131.86 ns |
| HashSet_Add   | 6   | 100000 |  50,100,442.4 ns |   9,946,427.65 ns |    545,197.26 ns |
| HashSet_Add   | 7   | 100000 |  61,773,185.2 ns |  61,064,249.78 ns |  3,347,137.58 ns |
| SortedSet_Add | 5   | 100000 | 116,932,993.3 ns |  13,180,416.62 ns |    722,463.11 ns |
| SortedSet_Add | 3   | 100000 | 120,456,086.7 ns | 167,935,488.16 ns |  9,205,110.78 ns |
| AVLSet_Add    | 5   | 100000 | 130,778,516.7 ns |  45,385,679.86 ns |  2,487,742.26 ns |
| AVLSet_Add    | 3   | 100000 | 134,387,400.0 ns | 215,272,673.04 ns | 11,799,821.61 ns |
| SortedSet_Add | 6   | 100000 | 140,404,183.3 ns |  53,114,048.45 ns |  2,911,360.22 ns |
| AVLSet_Add    | 4   | 100000 | 142,983,166.7 ns | 130,211,737.79 ns |  7,137,344.73 ns |
| AVLSet_Add    | 6   | 100000 | 152,068,608.3 ns |  56,624,776.23 ns |  3,103,795.06 ns |
| AVLSet_Add    | 7   | 100000 | 152,097,458.3 ns |  75,732,664.51 ns |  4,151,162.89 ns |
| SortedSet_Add | 7   | 100000 | 189,630,566.7 ns | 589,370,578.26 ns | 32,305,390.14 ns |
| SortedSet_Add | 4   | 100000 | 221,198,333.3 ns | 297,379,297.96 ns | 16,300,362.78 ns |



 */