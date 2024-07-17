using AVLUtils;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
[ShortRunJob]
public class SetBench {

  private List<Vector> Vs;
  private List<Vector> Vs_other;

  private HashSet<Vector>   hset;
  private SortedSet<Vector> sset;
  private AVLSet<Vector>    aset;

  private HashSet<Vector>   hset_UnionWith_List;
  private SortedSet<Vector> sset_UnionWith_List;
  private AVLSet<Vector>    aset_UnionWith_List;

  private HashSet<Vector>   hset_ForUW;
  private SortedSet<Vector> sset_ForUW;
  private AVLSet<Vector>    aset_ForUW;

  private Vector toAdd;

  [Params(3, 4, 5, 6)]
  // [Params(3)]
  public int dim;

  [Params(10, 100, 1000, 10000)]
  // [Params(10)]
  public int size;

  [GlobalSetup]
  public void GlobalSetup() {
    Tools.Eps = 1;

    Vs = new List<Vector>(size);
    for (int i = 0; i < size; i++) {
      Vector genVectorInt = Vector.GenVectorInt(dim, -1000, 1001);

      Vs.Add(genVectorInt);
    }
    Vs_other = new List<Vector>(size);
    for (int i = 0; i < size; i++) {
      Vector genVectorInt = Vector.GenVectorInt(dim, -1000, 1001);

      Vs_other.Add(genVectorInt);
    }

    hset = new HashSet<Vector>(Vs);
    sset = new SortedSet<Vector>(Vs);
    aset = new AVLSet<Vector>();
    foreach (Vector v in Vs) { aset.Add(v); }

    hset_ForUW = new HashSet<Vector>(Vs_other);
    sset_ForUW = new SortedSet<Vector>(Vs_other);
    aset_ForUW = new AVLSet<Vector>();
    foreach (Vector v in Vs_other) { aset_ForUW.Add(v); }

    hset_UnionWith_List = new HashSet<Vector>(Vs);
    sset_UnionWith_List = new SortedSet<Vector>(Vs);
    aset_UnionWith_List = new AVLSet<Vector>();
    foreach (Vector v in Vs) { aset_UnionWith_List.Add(v); }

    toAdd = Vector.GenVectorInt(dim, -1000, 1001);
  }

  [Benchmark]
  public void HashSet_Create() {
    HashSet<Vector> Hs = new HashSet<Vector>(Vs);
  }

  [Benchmark]
  public void SortedSet_Create() {
    SortedSet<Vector> Hs = new SortedSet<Vector>(Vs);
  }

  [Benchmark]
  public void HashSet_AddAll() {
    HashSet<Vector> Hs = new HashSet<Vector>();
    foreach (Vector v in Vs) { Hs.Add(v); }
  }

  [Benchmark]
  public void SortedSet_AddAll() {
    SortedSet<Vector> Ss = new SortedSet<Vector>();
    foreach (Vector v in Vs) { Ss.Add(v); }
  }

  [Benchmark]
  public void AVLSet_AddAll() {
    AVLSet<Vector> AVLs = new AVLSet<Vector>();
    foreach (Vector v in Vs) { AVLs.Add(v); }
  }


  [Benchmark]
  public void HashSet_Add() => hset.Add(toAdd);

  [Benchmark]
  public void SortedSet_Add() => sset.Add(toAdd);

  [Benchmark]
  public void AVLSetSet_Add() => aset.Add(toAdd);


  [Benchmark]
  public void HashSet_Contains() => hset.Contains(toAdd);

  [Benchmark]
  public void SortedSet_Contains() => sset.Contains(toAdd);

  [Benchmark]
  public void AVLSetSet_Contains() => aset.Contains(toAdd);


  [Benchmark]
  public void HashSet_Remove() => hset.Remove(toAdd);

  [Benchmark]
  public void SortedSet_Remove() => sset.Remove(toAdd);

  [Benchmark]
  public void AVLSetSet_Remove() => aset.Remove(toAdd);


  [Benchmark]
  public void HashSet_UnionWith_List() => hset_UnionWith_List.UnionWith(Vs_other);

  [Benchmark]
  public void SortedSet_UnionWith_List() => sset_UnionWith_List.UnionWith(Vs_other);

  [Benchmark]
  public void AVLSetSet_UnionWith_List() => aset_UnionWith_List.UnionWith(Vs_other);


  [Benchmark]
  public void HashSet_UnionWith() => hset_ForUW.UnionWith(hset);

  [Benchmark]
  public void SortedSet_UnionWith() => sset_ForUW.UnionWith(sset);

  [Benchmark]
  public void AVLSetSet_UnionWith() => aset_ForUW.UnionWith(aset);


  [Benchmark]
  public void HashSet_ExceptWith() => hset_ForUW.ExceptWith(hset);

  [Benchmark]
  public void SortedSet_ExceptWith() => sset_ForUW.ExceptWith(sset);

  [Benchmark]
  public void AVLSetSet_ExceptWith() => aset_ForUW.ExceptWith(aset);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<SetBench>();
  //   }
  //
  // }

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









| Method                   | dim | size  | Mean            | Error            | StdDev         |
|------------------------- |---- |------ |----------------:|-----------------:|---------------:|
| HashSet_Create           | 3   | 10    |     2,568.61 ns |       706.247 ns |      38.712 ns |
| SortedSet_Create         | 3   | 10    |     1,231.70 ns |        75.297 ns |       4.127 ns |
| HashSet_AddAll           | 3   | 10    |     2,563.90 ns |       179.396 ns |       9.833 ns |
| SortedSet_AddAll         | 3   | 10    |       762.23 ns |       169.414 ns |       9.286 ns |
| AVLSet_AddAll            | 3   | 10    |       826.32 ns |        25.577 ns |       1.402 ns |
| HashSet_Add              | 3   | 10    |       270.91 ns |        50.078 ns |       2.745 ns |
| SortedSet_Add            | 3   | 10    |       120.63 ns |        30.194 ns |       1.655 ns |
| AVLSetSet_Add            | 3   | 10    |       147.26 ns |       124.961 ns |       6.850 ns |
| HashSet_Contains         | 3   | 10    |       227.21 ns |         8.659 ns |       0.475 ns |
| SortedSet_Contains       | 3   | 10    |        74.34 ns |         2.832 ns |       0.155 ns |
| AVLSetSet_Contains       | 3   | 10    |        99.74 ns |        11.657 ns |       0.639 ns |
| HashSet_Remove           | 3   | 10    |       228.30 ns |        18.260 ns |       1.001 ns |
| SortedSet_Remove         | 3   | 10    |        85.02 ns |        13.991 ns |       0.767 ns |
| AVLSetSet_Remove         | 3   | 10    |       122.63 ns |         8.048 ns |       0.441 ns |
| HashSet_UnionWith_List   | 3   | 10    |     2,814.57 ns |        56.987 ns |       3.124 ns |
| SortedSet_UnionWith_List | 3   | 10    |     1,533.97 ns |     1,037.823 ns |      56.887 ns |
| AVLSetSet_UnionWith_List | 3   | 10    |     1,474.24 ns |       225.776 ns |      12.376 ns |
| HashSet_UnionWith        | 3   | 10    |     2,804.95 ns |       978.843 ns |      53.654 ns |
| SortedSet_UnionWith      | 3   | 10    |     1,412.09 ns |       192.036 ns |      10.526 ns |
| AVLSetSet_UnionWith      | 3   | 10    |     1,651.66 ns |       182.938 ns |      10.027 ns |
| HashSet_ExceptWith       | 3   | 10    |     2,497.34 ns |       440.045 ns |      24.120 ns |
| SortedSet_ExceptWith     | 3   | 10    |     1,710.18 ns |       238.925 ns |      13.096 ns |
| AVLSetSet_ExceptWith     | 3   | 10    |     1,272.84 ns |       384.113 ns |      21.055 ns |
| HashSet_Create           | 3   | 100   |    24,632.38 ns |     3,149.861 ns |     172.655 ns |
| SortedSet_Create         | 3   | 100   |    20,787.43 ns |     3,204.575 ns |     175.654 ns |
| HashSet_AddAll           | 3   | 100   |    25,313.26 ns |       186.391 ns |      10.217 ns |
| SortedSet_AddAll         | 3   | 100   |    18,101.46 ns |     2,919.906 ns |     160.050 ns |
| AVLSet_AddAll            | 3   | 100   |    19,980.18 ns |     1,171.046 ns |      64.189 ns |
| HashSet_Add              | 3   | 100   |       266.06 ns |        39.894 ns |       2.187 ns |
| SortedSet_Add            | 3   | 100   |       195.58 ns |        19.091 ns |       1.046 ns |
| AVLSetSet_Add            | 3   | 100   |       245.07 ns |        12.504 ns |       0.685 ns |
| HashSet_Contains         | 3   | 100   |       221.56 ns |         5.850 ns |       0.321 ns |
| SortedSet_Contains       | 3   | 100   |       172.70 ns |        41.104 ns |       2.253 ns |
| AVLSetSet_Contains       | 3   | 100   |       171.16 ns |        12.383 ns |       0.679 ns |
| HashSet_Remove           | 3   | 100   |       222.46 ns |        29.069 ns |       1.593 ns |
| SortedSet_Remove         | 3   | 100   |       190.27 ns |        63.232 ns |       3.466 ns |
| AVLSetSet_Remove         | 3   | 100   |       214.53 ns |        10.112 ns |       0.554 ns |
| HashSet_UnionWith_List   | 3   | 100   |    29,319.95 ns |    10,081.193 ns |     552.584 ns |
| SortedSet_UnionWith_List | 3   | 100   |    25,385.17 ns |     2,553.987 ns |     139.993 ns |
| AVLSetSet_UnionWith_List | 3   | 100   |    28,347.83 ns |     2,694.682 ns |     147.705 ns |
| HashSet_UnionWith        | 3   | 100   |    27,932.11 ns |     1,725.266 ns |      94.568 ns |
| SortedSet_UnionWith      | 3   | 100   |    23,865.97 ns |     1,349.205 ns |      73.954 ns |
| AVLSetSet_UnionWith      | 3   | 100   |    28,268.84 ns |       309.868 ns |      16.985 ns |
| HashSet_ExceptWith       | 3   | 100   |    24,802.21 ns |       876.209 ns |      48.028 ns |
| SortedSet_ExceptWith     | 3   | 100   |    27,244.25 ns |     1,861.310 ns |     102.025 ns |
| AVLSetSet_ExceptWith     | 3   | 100   |    24,205.04 ns |     2,683.191 ns |     147.075 ns |
| HashSet_Create           | 3   | 1000  |   252,534.21 ns |    20,946.542 ns |   1,148.151 ns |
| SortedSet_Create         | 3   | 1000  |   378,454.75 ns |    17,722.770 ns |     971.445 ns |
| HashSet_AddAll           | 3   | 1000  |   254,861.44 ns |    31,775.031 ns |   1,741.697 ns |
| SortedSet_AddAll         | 3   | 1000  |   329,627.91 ns |    74,335.968 ns |   4,074.605 ns |
| AVLSet_AddAll            | 3   | 1000  |   350,644.94 ns |    40,515.737 ns |   2,220.804 ns |
| HashSet_Add              | 3   | 1000  |       298.40 ns |       454.243 ns |      24.899 ns |
| SortedSet_Add            | 3   | 1000  |       296.36 ns |        30.243 ns |       1.658 ns |
| AVLSetSet_Add            | 3   | 1000  |       287.65 ns |        20.479 ns |       1.123 ns |
| HashSet_Contains         | 3   | 1000  |       223.91 ns |        18.807 ns |       1.031 ns |
| SortedSet_Contains       | 3   | 1000  |       239.91 ns |         8.606 ns |       0.472 ns |
| AVLSetSet_Contains       | 3   | 1000  |       250.30 ns |        24.250 ns |       1.329 ns |
| HashSet_Remove           | 3   | 1000  |       230.54 ns |        12.358 ns |       0.677 ns |
| SortedSet_Remove         | 3   | 1000  |       286.72 ns |        30.694 ns |       1.682 ns |
| AVLSetSet_Remove         | 3   | 1000  |       345.06 ns |       124.445 ns |       6.821 ns |
| HashSet_UnionWith_List   | 3   | 1000  |   280,790.22 ns |    10,353.605 ns |     567.516 ns |
| SortedSet_UnionWith_List | 3   | 1000  |   416,133.61 ns |    17,649.789 ns |     967.444 ns |
| AVLSetSet_UnionWith_List | 3   | 1000  |   468,563.33 ns |    34,618.545 ns |   1,897.559 ns |
| HashSet_UnionWith        | 3   | 1000  |   281,066.52 ns |    73,670.960 ns |   4,038.154 ns |
| SortedSet_UnionWith      | 3   | 1000  |   364,355.53 ns |    13,320.291 ns |     730.130 ns |
| AVLSetSet_UnionWith      | 3   | 1000  |   450,937.09 ns |    62,316.852 ns |   3,415.797 ns |
| HashSet_ExceptWith       | 3   | 1000  |   247,633.42 ns |    14,086.259 ns |     772.115 ns |
| SortedSet_ExceptWith     | 3   | 1000  |   418,638.10 ns |    20,381.887 ns |   1,117.200 ns |
| AVLSetSet_ExceptWith     | 3   | 1000  |   392,448.89 ns |    50,770.959 ns |   2,782.928 ns |
| HashSet_Create           | 3   | 10000 | 2,549,433.53 ns |   134,180.501 ns |   7,354.886 ns |
| SortedSet_Create         | 3   | 10000 | 5,659,604.30 ns |   381,208.491 ns |  20,895.324 ns |
| HashSet_AddAll           | 3   | 10000 | 2,630,592.64 ns |    87,461.164 ns |   4,794.042 ns |
| SortedSet_AddAll         | 3   | 10000 | 5,856,465.62 ns | 3,521,587.322 ns | 193,030.084 ns |
| AVLSet_AddAll            | 3   | 10000 | 6,201,897.14 ns |   129,452.142 ns |   7,095.709 ns |
| HashSet_Add              | 3   | 10000 |       299.41 ns |         3.515 ns |       0.193 ns |
| SortedSet_Add            | 3   | 10000 |       381.84 ns |        36.138 ns |       1.981 ns |
| AVLSetSet_Add            | 3   | 10000 |       481.59 ns |        80.941 ns |       4.437 ns |
| HashSet_Contains         | 3   | 10000 |       224.33 ns |        21.748 ns |       1.192 ns |
| SortedSet_Contains       | 3   | 10000 |       360.08 ns |         7.374 ns |       0.404 ns |
| AVLSetSet_Contains       | 3   | 10000 |       360.75 ns |         8.662 ns |       0.475 ns |
| HashSet_Remove           | 3   | 10000 |       224.70 ns |         6.175 ns |       0.338 ns |
| SortedSet_Remove         | 3   | 10000 |       396.66 ns |        28.061 ns |       1.538 ns |
| AVLSetSet_Remove         | 3   | 10000 |       481.04 ns |        36.040 ns |       1.975 ns |
| HashSet_UnionWith_List   | 3   | 10000 | 2,867,331.77 ns |   571,763.787 ns |  31,340.302 ns |
| SortedSet_UnionWith_List | 3   | 10000 | 6,518,074.74 ns |   353,018.434 ns |  19,350.132 ns |
| AVLSetSet_UnionWith_List | 3   | 10000 | 7,670,906.12 ns |   691,425.804 ns |  37,899.381 ns |
| HashSet_UnionWith        | 3   | 10000 | 2,850,063.15 ns |   284,658.935 ns |  15,603.117 ns |
| SortedSet_UnionWith      | 3   | 10000 | 5,040,137.50 ns |   495,444.133 ns |  27,156.965 ns |
| AVLSetSet_UnionWith      | 3   | 10000 | 6,232,963.67 ns |   840,662.293 ns |  46,079.537 ns |
| HashSet_ExceptWith       | 3   | 10000 | 2,541,049.74 ns |   241,300.495 ns |  13,226.494 ns |
| SortedSet_ExceptWith     | 3   | 10000 | 5,609,860.68 ns |   537,322.931 ns |  29,452.483 ns |
| AVLSetSet_ExceptWith     | 3   | 10000 | 5,760,606.12 ns |   275,208.302 ns |  15,085.096 ns |
| HashSet_Create           | 4   | 10    |     3,175.90 ns |       434.168 ns |      23.798 ns |
| SortedSet_Create         | 4   | 10    |     1,143.63 ns |       735.401 ns |      40.310 ns |
| HashSet_AddAll           | 4   | 10    |     3,371.93 ns |     2,348.330 ns |     128.720 ns |
| SortedSet_AddAll         | 4   | 10    |       733.58 ns |        84.456 ns |       4.629 ns |
| AVLSet_AddAll            | 4   | 10    |       792.71 ns |       126.629 ns |       6.941 ns |
| HashSet_Add              | 4   | 10    |       344.62 ns |        35.890 ns |       1.967 ns |
| SortedSet_Add            | 4   | 10    |       131.50 ns |         5.985 ns |       0.328 ns |
| AVLSetSet_Add            | 4   | 10    |       119.08 ns |         2.059 ns |       0.113 ns |
| HashSet_Contains         | 4   | 10    |       300.86 ns |       203.428 ns |      11.151 ns |
| SortedSet_Contains       | 4   | 10    |        97.20 ns |        13.130 ns |       0.720 ns |
| AVLSetSet_Contains       | 4   | 10    |        99.84 ns |        12.151 ns |       0.666 ns |
| HashSet_Remove           | 4   | 10    |       297.91 ns |        93.761 ns |       5.139 ns |
| SortedSet_Remove         | 4   | 10    |       109.57 ns |         3.801 ns |       0.208 ns |
| AVLSetSet_Remove         | 4   | 10    |        89.77 ns |        15.217 ns |       0.834 ns |
| HashSet_UnionWith_List   | 4   | 10    |     3,574.16 ns |        38.985 ns |       2.137 ns |
| SortedSet_UnionWith_List | 4   | 10    |     1,719.33 ns |       518.581 ns |      28.425 ns |
| AVLSetSet_UnionWith_List | 4   | 10    |     1,675.61 ns |       370.048 ns |      20.284 ns |
| HashSet_UnionWith        | 4   | 10    |     3,611.13 ns |       480.411 ns |      26.333 ns |
| SortedSet_UnionWith      | 4   | 10    |     1,318.08 ns |       244.080 ns |      13.379 ns |
| AVLSetSet_UnionWith      | 4   | 10    |     1,852.39 ns |       308.325 ns |      16.900 ns |
| HashSet_ExceptWith       | 4   | 10    |     3,065.93 ns |       239.418 ns |      13.123 ns |
| SortedSet_ExceptWith     | 4   | 10    |     1,348.11 ns |       111.530 ns |       6.113 ns |
| AVLSetSet_ExceptWith     | 4   | 10    |     1,149.46 ns |       166.546 ns |       9.129 ns |
| HashSet_Create           | 4   | 100   |    31,362.82 ns |     3,936.222 ns |     215.758 ns |
| SortedSet_Create         | 4   | 100   |    21,422.88 ns |     2,697.386 ns |     147.853 ns |
| HashSet_AddAll           | 4   | 100   |    31,918.85 ns |     2,838.090 ns |     155.565 ns |
| SortedSet_AddAll         | 4   | 100   |    18,202.97 ns |     1,910.655 ns |     104.729 ns |
| AVLSet_AddAll            | 4   | 100   |    19,910.45 ns |     2,863.901 ns |     156.980 ns |
| HashSet_Add              | 4   | 100   |       343.63 ns |        21.056 ns |       1.154 ns |
| SortedSet_Add            | 4   | 100   |       221.26 ns |       227.993 ns |      12.497 ns |
| AVLSetSet_Add            | 4   | 100   |       268.91 ns |        14.967 ns |       0.820 ns |
| HashSet_Contains         | 4   | 100   |       289.99 ns |        21.725 ns |       1.191 ns |
| SortedSet_Contains       | 4   | 100   |       146.36 ns |        11.101 ns |       0.608 ns |
| AVLSetSet_Contains       | 4   | 100   |       172.07 ns |        24.213 ns |       1.327 ns |
| HashSet_Remove           | 4   | 100   |       290.75 ns |        63.782 ns |       3.496 ns |
| SortedSet_Remove         | 4   | 100   |       185.69 ns |        27.772 ns |       1.522 ns |
| AVLSetSet_Remove         | 4   | 100   |       212.16 ns |        17.600 ns |       0.965 ns |
| HashSet_UnionWith_List   | 4   | 100   |    35,725.69 ns |     6,046.365 ns |     331.422 ns |
| SortedSet_UnionWith_List | 4   | 100   |    26,590.41 ns |     3,882.419 ns |     212.808 ns |
| AVLSetSet_UnionWith_List | 4   | 100   |    29,061.78 ns |     2,819.558 ns |     154.549 ns |
| HashSet_UnionWith        | 4   | 100   |    35,620.76 ns |     2,756.754 ns |     151.107 ns |
| SortedSet_UnionWith      | 4   | 100   |    26,117.59 ns |     2,076.295 ns |     113.809 ns |
| AVLSetSet_UnionWith      | 4   | 100   |    30,045.83 ns |     4,675.857 ns |     256.300 ns |
| HashSet_ExceptWith       | 4   | 100   |    31,118.73 ns |     2,956.098 ns |     162.034 ns |
| SortedSet_ExceptWith     | 4   | 100   |    27,905.86 ns |     7,209.006 ns |     395.150 ns |
| AVLSetSet_ExceptWith     | 4   | 100   |    24,034.37 ns |     6,221.630 ns |     341.029 ns |
| HashSet_Create           | 4   | 1000  |   308,170.69 ns |    15,721.999 ns |     861.776 ns |
| SortedSet_Create         | 4   | 1000  |   346,259.28 ns |    28,816.740 ns |   1,579.543 ns |
| HashSet_AddAll           | 4   | 1000  |   316,710.29 ns |    24,153.438 ns |   1,323.931 ns |
| SortedSet_AddAll         | 4   | 1000  |   326,897.41 ns |    49,123.703 ns |   2,692.636 ns |
| AVLSet_AddAll            | 4   | 1000  |   363,332.13 ns |    12,707.859 ns |     696.561 ns |
| HashSet_Add              | 4   | 1000  |       340.76 ns |        33.336 ns |       1.827 ns |
| SortedSet_Add            | 4   | 1000  |       323.35 ns |        17.075 ns |       0.936 ns |
| AVLSetSet_Add            | 4   | 1000  |       360.73 ns |        23.669 ns |       1.297 ns |
| HashSet_Contains         | 4   | 1000  |       298.15 ns |        17.239 ns |       0.945 ns |
| SortedSet_Contains       | 4   | 1000  |       216.21 ns |        17.932 ns |       0.983 ns |
| AVLSetSet_Contains       | 4   | 1000  |       191.45 ns |        23.407 ns |       1.283 ns |
| HashSet_Remove           | 4   | 1000  |       290.00 ns |        50.129 ns |       2.748 ns |
| SortedSet_Remove         | 4   | 1000  |       264.05 ns |        55.073 ns |       3.019 ns |
| AVLSetSet_Remove         | 4   | 1000  |       309.53 ns |        40.247 ns |       2.206 ns |
| HashSet_UnionWith_List   | 4   | 1000  |   354,560.94 ns |    35,236.612 ns |   1,931.438 ns |
| SortedSet_UnionWith_List | 4   | 1000  |   430,630.22 ns |    13,192.564 ns |     723.129 ns |
| AVLSetSet_UnionWith_List | 4   | 1000  |   483,420.10 ns |    46,150.779 ns |   2,529.680 ns |
| HashSet_UnionWith        | 4   | 1000  |   355,489.68 ns |     8,981.797 ns |     492.323 ns |
| SortedSet_UnionWith      | 4   | 1000  |   375,757.50 ns |    79,240.509 ns |   4,343.440 ns |
| AVLSetSet_UnionWith      | 4   | 1000  |   455,649.12 ns |   193,494.040 ns |  10,606.061 ns |
| HashSet_ExceptWith       | 4   | 1000  |   313,639.57 ns |    84,303.582 ns |   4,620.964 ns |
| SortedSet_ExceptWith     | 4   | 1000  |   409,873.27 ns |    81,920.816 ns |   4,490.356 ns |
| AVLSetSet_ExceptWith     | 4   | 1000  |   402,676.76 ns |    27,736.939 ns |   1,520.355 ns |
| HashSet_Create           | 4   | 10000 | 3,207,287.96 ns |   144,217.568 ns |   7,905.052 ns |
| SortedSet_Create         | 4   | 10000 | 5,570,217.19 ns |   270,226.543 ns |  14,812.029 ns |
| HashSet_AddAll           | 4   | 10000 | 3,306,318.42 ns |   142,067.732 ns |   7,787.212 ns |
| SortedSet_AddAll         | 4   | 10000 | 5,682,517.19 ns | 1,186,426.702 ns |  65,032.051 ns |
| AVLSet_AddAll            | 4   | 10000 | 6,066,293.62 ns |   321,607.187 ns |  17,628.375 ns |
| HashSet_Add              | 4   | 10000 |       349.33 ns |         5.081 ns |       0.279 ns |
| SortedSet_Add            | 4   | 10000 |       383.30 ns |        10.446 ns |       0.573 ns |
| AVLSetSet_Add            | 4   | 10000 |       506.95 ns |        82.666 ns |       4.531 ns |
| HashSet_Contains         | 4   | 10000 |       289.97 ns |        13.794 ns |       0.756 ns |
| SortedSet_Contains       | 4   | 10000 |       390.17 ns |        46.480 ns |       2.548 ns |
| AVLSetSet_Contains       | 4   | 10000 |       336.89 ns |        20.465 ns |       1.122 ns |
| HashSet_Remove           | 4   | 10000 |       289.30 ns |        25.616 ns |       1.404 ns |
| SortedSet_Remove         | 4   | 10000 |       419.17 ns |        54.020 ns |       2.961 ns |
| AVLSetSet_Remove         | 4   | 10000 |       517.21 ns |        37.923 ns |       2.079 ns |
| HashSet_UnionWith_List   | 4   | 10000 | 3,617,220.18 ns |   650,894.363 ns |  35,677.716 ns |
| SortedSet_UnionWith_List | 4   | 10000 | 6,614,653.12 ns |   298,689.802 ns |  16,372.196 ns |
| AVLSetSet_UnionWith_List | 4   | 10000 | 7,822,954.17 ns | 1,271,846.508 ns |  69,714.199 ns |
| HashSet_UnionWith        | 4   | 10000 | 3,568,796.88 ns |   221,088.963 ns |  12,118.632 ns |
| SortedSet_UnionWith      | 4   | 10000 | 5,189,382.81 ns |    15,897.335 ns |     871.387 ns |
| AVLSetSet_UnionWith      | 4   | 10000 | 6,379,964.58 ns |   715,960.462 ns |  39,244.209 ns |
| HashSet_ExceptWith       | 4   | 10000 | 3,164,806.25 ns |   323,337.832 ns |  17,723.238 ns |
| SortedSet_ExceptWith     | 4   | 10000 | 5,886,199.22 ns |   298,447.961 ns |  16,358.940 ns |
| AVLSetSet_ExceptWith     | 4   | 10000 | 5,696,231.25 ns |   722,528.035 ns |  39,604.200 ns |
| HashSet_Create           | 5   | 10    |     3,822.41 ns |       351.598 ns |      19.272 ns |
| SortedSet_Create         | 5   | 10    |     1,338.62 ns |       176.729 ns |       9.687 ns |
| HashSet_AddAll           | 5   | 10    |     3,953.33 ns |       202.562 ns |      11.103 ns |
| SortedSet_AddAll         | 5   | 10    |       763.19 ns |        60.982 ns |       3.343 ns |
| AVLSet_AddAll            | 5   | 10    |       820.95 ns |        51.315 ns |       2.813 ns |
| HashSet_Add              | 5   | 10    |       417.20 ns |        60.870 ns |       3.336 ns |
| SortedSet_Add            | 5   | 10    |       142.85 ns |        19.803 ns |       1.085 ns |
| AVLSetSet_Add            | 5   | 10    |       156.80 ns |        12.225 ns |       0.670 ns |
| HashSet_Contains         | 5   | 10    |       358.92 ns |        46.362 ns |       2.541 ns |
| SortedSet_Contains       | 5   | 10    |       101.03 ns |         7.808 ns |       0.428 ns |
| AVLSetSet_Contains       | 5   | 10    |        96.78 ns |         6.363 ns |       0.349 ns |
| HashSet_Remove           | 5   | 10    |       353.44 ns |        22.682 ns |       1.243 ns |
| SortedSet_Remove         | 5   | 10    |       107.41 ns |        17.307 ns |       0.949 ns |
| AVLSetSet_Remove         | 5   | 10    |        88.26 ns |        13.238 ns |       0.726 ns |
| HashSet_UnionWith_List   | 5   | 10    |     4,321.75 ns |       549.071 ns |      30.096 ns |
| SortedSet_UnionWith_List | 5   | 10    |     1,643.85 ns |       109.244 ns |       5.988 ns |
| AVLSetSet_UnionWith_List | 5   | 10    |     1,744.94 ns |       147.991 ns |       8.112 ns |
| HashSet_UnionWith        | 5   | 10    |     4,272.52 ns |       240.478 ns |      13.181 ns |
| SortedSet_UnionWith      | 5   | 10    |     1,695.49 ns |       247.882 ns |      13.587 ns |
| AVLSetSet_UnionWith      | 5   | 10    |     1,793.42 ns |       340.832 ns |      18.682 ns |
| HashSet_ExceptWith       | 5   | 10    |     3,803.23 ns |        88.590 ns |       4.856 ns |
| SortedSet_ExceptWith     | 5   | 10    |     1,080.36 ns |        76.221 ns |       4.178 ns |
| AVLSetSet_ExceptWith     | 5   | 10    |     1,239.82 ns |       149.487 ns |       8.194 ns |
| HashSet_Create           | 5   | 100   |    37,894.74 ns |     1,750.434 ns |      95.947 ns |
| SortedSet_Create         | 5   | 100   |    23,391.21 ns |     4,051.302 ns |     222.066 ns |
| HashSet_AddAll           | 5   | 100   |    39,014.03 ns |     9,550.114 ns |     523.474 ns |
| SortedSet_AddAll         | 5   | 100   |    18,550.70 ns |     4,537.997 ns |     248.743 ns |
| AVLSet_AddAll            | 5   | 100   |    19,513.35 ns |     1,368.399 ns |      75.007 ns |
| HashSet_Add              | 5   | 100   |       420.75 ns |        75.480 ns |       4.137 ns |
| SortedSet_Add            | 5   | 100   |       259.31 ns |       271.792 ns |      14.898 ns |
| AVLSetSet_Add            | 5   | 100   |       243.11 ns |        28.084 ns |       1.539 ns |
| HashSet_Contains         | 5   | 100   |       356.54 ns |        50.367 ns |       2.761 ns |
| SortedSet_Contains       | 5   | 100   |       166.82 ns |        22.255 ns |       1.220 ns |
| AVLSetSet_Contains       | 5   | 100   |       190.81 ns |         5.631 ns |       0.309 ns |
| HashSet_Remove           | 5   | 100   |       353.57 ns |        30.246 ns |       1.658 ns |
| SortedSet_Remove         | 5   | 100   |       188.44 ns |        10.991 ns |       0.602 ns |
| AVLSetSet_Remove         | 5   | 100   |       206.25 ns |         9.826 ns |       0.539 ns |
| HashSet_UnionWith_List   | 5   | 100   |    42,743.50 ns |     4,101.259 ns |     224.804 ns |
| SortedSet_UnionWith_List | 5   | 100   |    28,240.12 ns |     1,266.612 ns |      69.427 ns |
| AVLSetSet_UnionWith_List | 5   | 100   |    31,131.62 ns |     5,991.346 ns |     328.406 ns |
| HashSet_UnionWith        | 5   | 100   |    43,072.94 ns |     1,855.540 ns |     101.708 ns |
| SortedSet_UnionWith      | 5   | 100   |    26,960.24 ns |       713.031 ns |      39.084 ns |
| AVLSetSet_UnionWith      | 5   | 100   |    31,108.72 ns |       741.881 ns |      40.665 ns |
| HashSet_ExceptWith       | 5   | 100   |    38,189.44 ns |     2,704.167 ns |     148.224 ns |
| SortedSet_ExceptWith     | 5   | 100   |    26,658.48 ns |     4,237.931 ns |     232.295 ns |
| AVLSetSet_ExceptWith     | 5   | 100   |    23,210.14 ns |     2,129.391 ns |     116.719 ns |
| HashSet_Create           | 5   | 1000  |   372,572.80 ns |     9,107.362 ns |     499.205 ns |
| SortedSet_Create         | 5   | 1000  |   343,829.74 ns |    43,622.415 ns |   2,391.092 ns |
| HashSet_AddAll           | 5   | 1000  |   382,247.14 ns |    43,059.542 ns |   2,360.239 ns |
| SortedSet_AddAll         | 5   | 1000  |   329,872.64 ns |    40,215.986 ns |   2,204.374 ns |
| AVLSet_AddAll            | 5   | 1000  |   355,072.27 ns |    79,956.402 ns |   4,382.680 ns |
| HashSet_Add              | 5   | 1000  |       424.53 ns |         3.285 ns |       0.180 ns |
| SortedSet_Add            | 5   | 1000  |       322.13 ns |        17.041 ns |       0.934 ns |
| AVLSetSet_Add            | 5   | 1000  |       435.48 ns |        84.030 ns |       4.606 ns |
| HashSet_Contains         | 5   | 1000  |       351.81 ns |        25.374 ns |       1.391 ns |
| SortedSet_Contains       | 5   | 1000  |       240.39 ns |        10.288 ns |       0.564 ns |
| AVLSetSet_Contains       | 5   | 1000  |       241.05 ns |        34.861 ns |       1.911 ns |
| HashSet_Remove           | 5   | 1000  |       361.81 ns |        19.517 ns |       1.070 ns |
| SortedSet_Remove         | 5   | 1000  |       277.71 ns |        16.865 ns |       0.924 ns |
| AVLSetSet_Remove         | 5   | 1000  |       295.80 ns |       148.464 ns |       8.138 ns |
| HashSet_UnionWith_List   | 5   | 1000  |   433,973.99 ns |    72,906.636 ns |   3,996.259 ns |
| SortedSet_UnionWith_List | 5   | 1000  |   436,630.78 ns |    66,715.689 ns |   3,656.912 ns |
| AVLSetSet_UnionWith_List | 5   | 1000  |   497,898.18 ns |    51,678.749 ns |   2,832.687 ns |
| HashSet_UnionWith        | 5   | 1000  |   434,017.19 ns |    70,876.479 ns |   3,884.979 ns |
| SortedSet_UnionWith      | 5   | 1000  |   387,251.97 ns |    22,128.171 ns |   1,212.920 ns |
| AVLSetSet_UnionWith      | 5   | 1000  |   467,093.33 ns |    66,971.406 ns |   3,670.929 ns |
| HashSet_ExceptWith       | 5   | 1000  |   375,896.31 ns |    17,094.166 ns |     936.989 ns |
| SortedSet_ExceptWith     | 5   | 1000  |   414,873.68 ns |    14,594.215 ns |     799.958 ns |
| AVLSetSet_ExceptWith     | 5   | 1000  |   387,824.20 ns |    28,716.008 ns |   1,574.021 ns |
| HashSet_Create           | 5   | 10000 | 3,898,226.82 ns |   773,060.163 ns |  42,374.036 ns |
| SortedSet_Create         | 5   | 10000 | 5,615,680.47 ns |   610,502.661 ns |  33,463.711 ns |
| HashSet_AddAll           | 5   | 10000 | 3,995,825.91 ns |   338,603.175 ns |  18,559.983 ns |
| SortedSet_AddAll         | 5   | 10000 | 5,608,322.14 ns |   575,887.691 ns |  31,566.348 ns |
| AVLSet_AddAll            | 5   | 10000 | 6,137,633.07 ns |   160,757.132 ns |   8,811.641 ns |
| HashSet_Add              | 5   | 10000 |       422.03 ns |         7.009 ns |       0.384 ns |
| SortedSet_Add            | 5   | 10000 |       379.35 ns |        41.944 ns |       2.299 ns |
| AVLSetSet_Add            | 5   | 10000 |       497.11 ns |        46.690 ns |       2.559 ns |
| HashSet_Contains         | 5   | 10000 |       353.21 ns |        21.775 ns |       1.194 ns |
| SortedSet_Contains       | 5   | 10000 |       333.36 ns |        37.343 ns |       2.047 ns |
| AVLSetSet_Contains       | 5   | 10000 |       365.01 ns |        24.538 ns |       1.345 ns |
| HashSet_Remove           | 5   | 10000 |       354.40 ns |        19.577 ns |       1.073 ns |
| SortedSet_Remove         | 5   | 10000 |       393.08 ns |        11.461 ns |       0.628 ns |
| AVLSetSet_Remove         | 5   | 10000 |       526.89 ns |        76.040 ns |       4.168 ns |
| HashSet_UnionWith_List   | 5   | 10000 | 4,391,096.88 ns |   119,947.558 ns |   6,574.730 ns |
| SortedSet_UnionWith_List | 5   | 10000 | 6,819,330.99 ns |   472,717.409 ns |  25,911.236 ns |
| AVLSetSet_UnionWith_List | 5   | 10000 | 7,778,782.29 ns |   489,362.172 ns |  26,823.592 ns |
| HashSet_UnionWith        | 5   | 10000 | 4,328,180.21 ns |   559,019.459 ns |  30,641.743 ns |
| SortedSet_UnionWith      | 5   | 10000 | 5,514,431.90 ns | 5,096,593.464 ns | 279,361.485 ns |
| AVLSetSet_UnionWith      | 5   | 10000 | 6,480,256.12 ns | 1,366,699.605 ns |  74,913.417 ns |
| HashSet_ExceptWith       | 5   | 10000 | 3,835,009.90 ns |   458,944.499 ns |  25,156.297 ns |
| SortedSet_ExceptWith     | 5   | 10000 | 5,577,549.61 ns |   720,520.222 ns |  39,494.145 ns |
| AVLSetSet_ExceptWith     | 5   | 10000 | 5,734,741.41 ns |   710,046.042 ns |  38,920.020 ns |
| HashSet_Create           | 6   | 10    |     4,641.17 ns |       314.689 ns |      17.249 ns |
| SortedSet_Create         | 6   | 10    |     1,086.67 ns |       137.116 ns |       7.516 ns |
| HashSet_AddAll           | 6   | 10    |     4,606.20 ns |       361.445 ns |      19.812 ns |
| SortedSet_AddAll         | 6   | 10    |       669.09 ns |        46.252 ns |       2.535 ns |
| AVLSet_AddAll            | 6   | 10    |       794.13 ns |       105.775 ns |       5.798 ns |
| HashSet_Add              | 6   | 10    |       489.18 ns |        27.894 ns |       1.529 ns |
| SortedSet_Add            | 6   | 10    |       157.93 ns |        95.337 ns |       5.226 ns |
| AVLSetSet_Add            | 6   | 10    |       176.13 ns |        24.893 ns |       1.364 ns |
| HashSet_Contains         | 6   | 10    |       423.21 ns |        13.984 ns |       0.767 ns |
| SortedSet_Contains       | 6   | 10    |        75.00 ns |         1.840 ns |       0.101 ns |
| AVLSetSet_Contains       | 6   | 10    |        74.91 ns |        21.492 ns |       1.178 ns |
| HashSet_Remove           | 6   | 10    |       416.34 ns |        13.545 ns |       0.742 ns |
| SortedSet_Remove         | 6   | 10    |        80.79 ns |         5.486 ns |       0.301 ns |
| AVLSetSet_Remove         | 6   | 10    |       123.23 ns |        26.590 ns |       1.457 ns |
| HashSet_UnionWith_List   | 6   | 10    |     5,026.95 ns |     1,625.347 ns |      89.091 ns |
| SortedSet_UnionWith_List | 6   | 10    |     1,780.14 ns |       361.729 ns |      19.828 ns |
| AVLSetSet_UnionWith_List | 6   | 10    |     1,946.46 ns |        84.773 ns |       4.647 ns |
| HashSet_UnionWith        | 6   | 10    |     5,077.58 ns |     1,005.749 ns |      55.128 ns |
| SortedSet_UnionWith      | 6   | 10    |     1,835.77 ns |       272.149 ns |      14.917 ns |
| AVLSetSet_UnionWith      | 6   | 10    |     2,008.55 ns |        12.667 ns |       0.694 ns |
| HashSet_ExceptWith       | 6   | 10    |     4,489.48 ns |       255.803 ns |      14.021 ns |
| SortedSet_ExceptWith     | 6   | 10    |     1,126.15 ns |       379.144 ns |      20.782 ns |
| AVLSetSet_ExceptWith     | 6   | 10    |     1,291.95 ns |       113.721 ns |       6.233 ns |
| HashSet_Create           | 6   | 100   |    44,905.05 ns |     1,477.711 ns |      80.998 ns |
| SortedSet_Create         | 6   | 100   |    21,707.23 ns |    17,039.810 ns |     934.009 ns |
| HashSet_AddAll           | 6   | 100   |    45,051.40 ns |     4,875.763 ns |     267.257 ns |
| SortedSet_AddAll         | 6   | 100   |    17,785.19 ns |     2,915.902 ns |     159.830 ns |
| AVLSet_AddAll            | 6   | 100   |    19,849.19 ns |     1,531.518 ns |      83.948 ns |
| HashSet_Add              | 6   | 100   |       496.51 ns |        80.406 ns |       4.407 ns |
| SortedSet_Add            | 6   | 100   |       231.58 ns |       118.340 ns |       6.487 ns |
| AVLSetSet_Add            | 6   | 100   |       267.96 ns |        41.157 ns |       2.256 ns |
| HashSet_Contains         | 6   | 100   |       417.73 ns |        66.905 ns |       3.667 ns |
| SortedSet_Contains       | 6   | 100   |       169.04 ns |         6.501 ns |       0.356 ns |
| AVLSetSet_Contains       | 6   | 100   |       172.01 ns |        33.237 ns |       1.822 ns |
| HashSet_Remove           | 6   | 100   |       425.05 ns |        47.844 ns |       2.622 ns |
| SortedSet_Remove         | 6   | 100   |       189.60 ns |        41.949 ns |       2.299 ns |
| AVLSetSet_Remove         | 6   | 100   |       203.48 ns |         9.149 ns |       0.501 ns |
| HashSet_UnionWith_List   | 6   | 100   |    50,368.01 ns |     3,886.651 ns |     213.040 ns |
| SortedSet_UnionWith_List | 6   | 100   |    29,129.89 ns |     1,058.770 ns |      58.035 ns |
| AVLSetSet_UnionWith_List | 6   | 100   |    32,020.65 ns |     5,178.748 ns |     283.865 ns |
| HashSet_UnionWith        | 6   | 100   |    50,568.55 ns |    11,549.873 ns |     633.087 ns |
| SortedSet_UnionWith      | 6   | 100   |    28,204.80 ns |     1,091.670 ns |      59.838 ns |
| AVLSetSet_UnionWith      | 6   | 100   |    34,059.88 ns |    42,249.430 ns |   2,315.834 ns |
| HashSet_ExceptWith       | 6   | 100   |    44,005.22 ns |    11,078.746 ns |     607.263 ns |
| SortedSet_ExceptWith     | 6   | 100   |    27,130.48 ns |     1,446.146 ns |      79.268 ns |
| AVLSetSet_ExceptWith     | 6   | 100   |    24,130.33 ns |     1,782.273 ns |      97.692 ns |
| HashSet_Create           | 6   | 1000  |   464,850.70 ns |    30,147.952 ns |   1,652.511 ns |
| SortedSet_Create         | 6   | 1000  |   352,725.55 ns |    70,123.019 ns |   3,843.679 ns |
| HashSet_AddAll           | 6   | 1000  |   447,743.91 ns |    31,248.081 ns |   1,712.813 ns |
| SortedSet_AddAll         | 6   | 1000  |   335,252.15 ns |    12,918.343 ns |     708.098 ns |
| AVLSet_AddAll            | 6   | 1000  |   359,912.76 ns |   184,005.289 ns |  10,085.951 ns |
| HashSet_Add              | 6   | 1000  |       488.83 ns |        42.425 ns |       2.325 ns |
| SortedSet_Add            | 6   | 1000  |       323.87 ns |         4.169 ns |       0.229 ns |
| AVLSetSet_Add            | 6   | 1000  |       410.44 ns |        61.575 ns |       3.375 ns |
| HashSet_Contains         | 6   | 1000  |       425.50 ns |        53.010 ns |       2.906 ns |
| SortedSet_Contains       | 6   | 1000  |       247.22 ns |        35.368 ns |       1.939 ns |
| AVLSetSet_Contains       | 6   | 1000  |       271.75 ns |        37.223 ns |       2.040 ns |
| HashSet_Remove           | 6   | 1000  |       429.81 ns |         8.701 ns |       0.477 ns |
| SortedSet_Remove         | 6   | 1000  |       282.53 ns |        70.855 ns |       3.884 ns |
| AVLSetSet_Remove         | 6   | 1000  |       307.07 ns |        29.929 ns |       1.641 ns |
| HashSet_UnionWith_List   | 6   | 1000  |   510,132.55 ns |    21,802.570 ns |   1,195.072 ns |
| SortedSet_UnionWith_List | 6   | 1000  |   459,688.22 ns |    41,376.872 ns |   2,268.006 ns |
| AVLSetSet_UnionWith_List | 6   | 1000  |   519,639.10 ns |    45,912.381 ns |   2,516.613 ns |
| HashSet_UnionWith        | 6   | 1000  |   508,232.98 ns |    80,374.739 ns |   4,405.611 ns |
| SortedSet_UnionWith      | 6   | 1000  |   402,983.38 ns |    31,067.090 ns |   1,702.892 ns |
| AVLSetSet_UnionWith      | 6   | 1000  |   484,978.15 ns |    36,764.373 ns |   2,015.179 ns |
| HashSet_ExceptWith       | 6   | 1000  |   447,880.19 ns |    31,319.793 ns |   1,716.744 ns |
| SortedSet_ExceptWith     | 6   | 1000  |   414,683.39 ns |    43,186.457 ns |   2,367.195 ns |
| AVLSetSet_ExceptWith     | 6   | 1000  |   392,736.26 ns |    39,610.797 ns |   2,171.201 ns |
| HashSet_Create           | 6   | 10000 | 4,546,902.34 ns |   540,826.612 ns |  29,644.532 ns |
| SortedSet_Create         | 6   | 10000 | 5,697,660.42 ns |   497,153.683 ns |  27,250.671 ns |
| HashSet_AddAll           | 6   | 10000 | 4,729,034.90 ns |   215,200.507 ns |  11,795.866 ns |
| SortedSet_AddAll         | 6   | 10000 | 5,633,969.01 ns |   524,070.455 ns |  28,726.070 ns |
| AVLSet_AddAll            | 6   | 10000 | 6,095,358.59 ns |   220,211.502 ns |  12,070.536 ns |
| HashSet_Add              | 6   | 10000 |       499.14 ns |        93.770 ns |       5.140 ns |
| SortedSet_Add            | 6   | 10000 |       402.39 ns |         9.728 ns |       0.533 ns |
| AVLSetSet_Add            | 6   | 10000 |       572.45 ns |       206.421 ns |      11.315 ns |
| HashSet_Contains         | 6   | 10000 |       428.01 ns |         9.778 ns |       0.536 ns |
| SortedSet_Contains       | 6   | 10000 |       329.31 ns |         1.503 ns |       0.082 ns |
| AVLSetSet_Contains       | 6   | 10000 |       362.02 ns |        20.012 ns |       1.097 ns |
| HashSet_Remove           | 6   | 10000 |       421.04 ns |         2.822 ns |       0.155 ns |
| SortedSet_Remove         | 6   | 10000 |       385.30 ns |        25.444 ns |       1.395 ns |
| AVLSetSet_Remove         | 6   | 10000 |       493.15 ns |       138.999 ns |       7.619 ns |
| HashSet_UnionWith_List   | 6   | 10000 | 5,188,626.04 ns |   193,633.826 ns |  10,613.723 ns |
| SortedSet_UnionWith_List | 6   | 10000 | 6,916,207.29 ns |   389,069.769 ns |  21,326.227 ns |
| AVLSetSet_UnionWith_List | 6   | 10000 | 8,086,523.18 ns | 1,179,905.741 ns |  64,674.615 ns |
| HashSet_UnionWith        | 6   | 10000 | 5,196,982.03 ns |   153,528.603 ns |   8,415.421 ns |
| SortedSet_UnionWith      | 6   | 10000 | 5,470,583.98 ns |   642,712.847 ns |  35,229.260 ns |
| AVLSetSet_UnionWith      | 6   | 10000 | 6,600,878.12 ns |   321,426.431 ns |  17,618.467 ns |
| HashSet_ExceptWith       | 6   | 10000 | 4,611,313.15 ns |   118,877.771 ns |   6,516.092 ns |
| SortedSet_ExceptWith     | 6   | 10000 | 5,551,414.45 ns | 1,180,915.339 ns |  64,729.955 ns |
| AVLSetSet_ExceptWith     | 6   | 10000 | 5,823,119.40 ns |   573,854.958 ns |  31,454.927 ns |


 */
