using BenchmarkDotNet.Order;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[ShortRunJob]
public class GaussBench {

  [Params(3, 4, 5, 6, 7)]
  public int k;

  private ddouble[,] A = null!;
  private ddouble[]  b = null!;

  private Func<int, int, ddouble> AFunc    = null!;
  private Func<int, ddouble>      bFunc    = null!;
  private GaussSLE                gaussSLE = null!;

  [GlobalSetup]
  public void GlobalSetup() {
    A     = Matrix.GenNonSingular(k, -100, 100);
    b     = GenArray(k, -100, 100);
    AFunc = (r, l) => A[r, l];
    bFunc = r => b[r];

    gaussSLE = new GaussSLE(AFunc, bFunc, k, k);
  }

  [Benchmark]
  public void GaussNoChoice() {
    gaussSLE.SetGaussChoice(GaussSLE.GaussChoice.No);
    gaussSLE.Solve();
  }

  [Benchmark]
  public void GaussRowChoice() {
    gaussSLE.SetGaussChoice(GaussSLE.GaussChoice.RowWise);
    gaussSLE.Solve();
  }

  [Benchmark]
  public void GaussColChoice() {
    gaussSLE.SetGaussChoice(GaussSLE.GaussChoice.ColWise);
    gaussSLE.Solve();
  }

  [Benchmark]
  public void GaussAllChoice() {
    gaussSLE.SetGaussChoice(GaussSLE.GaussChoice.All);
    gaussSLE.Solve();
  }

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<GaussBench>();
  //   }
  //
  // }

}
/*
// * Summary *

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3296/23H2/2023Update/SunValley3)
Intel Core i5-10400F CPU 2.90GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.101
  [Host]     : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.1 (8.0.123.58001), X64 RyuJIT AVX2

| Method         | k | Mean       | Error       | StdDev    |
|--------------- |-- |-----------:|------------:|----------:|
| GaussRowChoice | 3 |   678.1 ns |    84.38 ns |   4.63 ns |
| GaussAllChoice | 3 |   789.5 ns |    57.15 ns |   3.13 ns |
| GaussColChoice | 3 |   824.7 ns |    67.90 ns |   3.72 ns |
| GaussNoChoice  | 3 |   964.2 ns |    45.43 ns |   2.49 ns |
| GaussNoChoice  | 4 | 1,130.9 ns |    30.10 ns |   1.65 ns |
| GaussAllChoice | 4 | 1,428.5 ns |   133.02 ns |   7.29 ns |
| GaussRowChoice | 4 | 1,512.5 ns |    10.11 ns |   0.55 ns |
| GaussColChoice | 4 | 1,650.3 ns |    99.81 ns |   5.47 ns |
| GaussColChoice | 5 | 2,452.6 ns |     3.23 ns |   0.18 ns |
| GaussAllChoice | 5 | 2,523.1 ns |   426.69 ns |  23.39 ns |
| GaussRowChoice | 5 | 2,746.2 ns |   111.00 ns |   6.08 ns |
| GaussNoChoice  | 5 | 2,848.8 ns |   547.87 ns |  30.03 ns |
| GaussColChoice | 6 | 3,108.5 ns |   105.64 ns |   5.79 ns |
| GaussNoChoice  | 6 | 3,397.4 ns |    82.94 ns |   4.55 ns |
| GaussRowChoice | 6 | 3,439.8 ns |   423.17 ns |  23.20 ns |
| GaussAllChoice | 6 | 4,368.4 ns |   338.68 ns |  18.56 ns |
| GaussColChoice | 7 | 4,409.4 ns |   483.53 ns |  26.50 ns |
| GaussNoChoice  | 7 | 4,917.8 ns |   200.89 ns |  11.01 ns |
| GaussRowChoice | 7 | 5,654.4 ns |   590.91 ns |  32.39 ns |
| GaussAllChoice | 7 | 6,957.1 ns | 2,102.24 ns | 115.23 ns |




| Method         | k    | Mean             | Error          | StdDev         | Allocated |
|--------------- |----- |-----------------:|---------------:|---------------:|----------:|
| GaussRowChoice | 10   |         11.22 us |       0.196 us |       0.201 us |         - |
| GaussColChoice | 10   |         11.77 us |       0.148 us |       0.132 us |         - |
| GaussNoChoice  | 10   |         11.98 us |       0.165 us |       0.146 us |         - |
| GaussAllChoice | 10   |         16.81 us |       0.116 us |       0.103 us |         - |
| GaussColChoice | 100  |      4,722.29 us |      50.670 us |      42.312 us |       3 B |
| GaussRowChoice | 100  |      5,437.28 us |      98.626 us |      82.357 us |       3 B |
| GaussNoChoice  | 100  |      9,672.65 us |     141.840 us |     118.443 us |       6 B |
| GaussAllChoice | 100  |     12,709.49 us |     179.103 us |     149.559 us |       6 B |
| GaussRowChoice | 1000 |  6,265,092.79 us | 124,775.351 us | 157,800.868 us |     400 B |
| GaussColChoice | 1000 |  8,531,047.33 us | 143,838.033 us | 280,545.037 us |     400 B |
| GaussNoChoice  | 1000 | 12,622,569.28 us | 246,655.643 us | 649,789.623 us |     400 B |
| GaussAllChoice | 1000 | 15,938,538.93 us | 318,318.237 us | 402,570.649 us |     400 B |


// * Hints *
Outliers
  GaussBench.GaussRowChoice: Default -> 1 outlier  was  removed (11.73 us)
  GaussBench.GaussColChoice: Default -> 1 outlier  was  removed (12.27 us)
  GaussBench.GaussNoChoice: Default  -> 1 outlier  was  removed (12.53 us)
  GaussBench.GaussAllChoice: Default -> 1 outlier  was  removed (17.32 us)
  GaussBench.GaussColChoice: Default -> 2 outliers were removed (4.93 ms, 5.09 ms)
  GaussBench.GaussRowChoice: Default -> 2 outliers were removed (5.77 ms, 5.79 ms)
  GaussBench.GaussNoChoice: Default  -> 2 outliers were removed (10.58 ms, 10.73 ms)
  GaussBench.GaussAllChoice: Default -> 2 outliers were removed (13.58 ms, 13.62 ms)
  GaussBench.GaussRowChoice: Default -> 4 outliers were removed (6.91 s..60.20 s)
  GaussBench.GaussColChoice: Default -> 4 outliers were removed (9.52 s..61.04 s)
  GaussBench.GaussNoChoice: Default  -> 7 outliers were removed, 8 outliers were detected (7.08 s, 13.23 s..84.28 s)
  GaussBench.GaussAllChoice: Default -> 2 outliers were removed, 3 outliers were detected (15.08 s, 17.31 s, 17.37 s)

// * Legends *
  k         : Value of the 'k' parameter
  Mean      : Arithmetic mean of all measurements
  Error     : Half of 99.9% confidence interval
  StdDev    : Standard deviation of all measurements
  Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 us      : 1 Microsecond (0.000001 sec)

// * Diagnostic Output - MemoryDiagnoser *


// ***** BenchmarkRunner: End *****
Run time: 00:49:12 (2952.68 sec), executed benchmarks: 12

Global total time: 00:49:19 (2959.85 sec), executed benchmarks: 12
*/
