using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class GaussBench {

  [Params(10, 100, 1000)]
  public int k;

  private ddouble[,] A;
  private ddouble[]  b;

  private Func<int, int, ddouble> AFunc;
  private Func<int, ddouble>      bFunc;
  private GaussSLE                gaussSLE;

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
  //
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
