using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[ShortRunJob]
// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Sandbox {

  [Params(4)]
  public int spaceDim;

  [Params(1, 2, 3, 4)]
  public int subSpaceDim;

  public Vector      v  = null!;
  public LinearBasis lb = null!;


  [GlobalSetup]
  public void SetUp() {
    lb = LinearBasis.GenLinearBasis(spaceDim, subSpaceDim);
    v  = Vector.GenVector(spaceDim);
  }

  [Benchmark]
  public void Prj() {
    var x = lb.Basis.ProjectOntoSubspace(v);
  }


  public class Program {

    public static void Main(string[] args) {
      var summary = BenchmarkRunner.Run<Sandbox>();
      // LinearBasis lb = LinearBasis.GenLinearBasis(4, 2);
      // Vector v  = Vector.GenVector(4);

      // var x = lb.Basis.ProjectOntoSubspace(v);
      // var y = lb.Basis.ProjectOntoSubspace_Lin(v);
      // var n = lb.Basis * (lb.Basis.Transpose() * v);
      //
      // var b = x.Equals(y);
      //
      // Console.WriteLine($"{y.Equals(n)}");
    }
  }

}


/*
 *
| Method       | dim | Mean      | Error      | StdDev   |
|------------- |---- |----------:|-----------:|---------:|
| Transpose    | 3   |  33.39 ns |   4.969 ns | 0.272 ns |
| TransposeLin | 3   |  30.88 ns |   9.681 ns | 0.531 ns |
| Transpose    | 4   |  49.50 ns |  18.858 ns | 1.034 ns |
| TransposeLin | 4   |  49.87 ns |  51.770 ns | 2.838 ns |
| Transpose    | 5   |  67.61 ns |   5.082 ns | 0.279 ns |
| TransposeLin | 5   |  65.68 ns |  61.029 ns | 3.345 ns |
| Transpose    | 6   |  93.33 ns |  52.905 ns | 2.900 ns |
| TransposeLin | 6   |  93.53 ns |  84.300 ns | 4.621 ns |
| Transpose    | 7   | 123.66 ns |  95.639 ns | 5.242 ns |
| TransposeLin | 7   | 112.79 ns |  49.784 ns | 2.729 ns |
| Transpose    | 8   | 165.71 ns | 172.105 ns | 9.434 ns |
| TransposeLin | 8   | 149.08 ns | 155.910 ns | 8.546 ns |
| Transpose    | 9   | 193.06 ns | 110.823 ns | 6.075 ns |
| TransposeLin | 9   | 172.90 ns |  14.470 ns | 0.793 ns |


  private static Vector PrjWiki_iterator(Vector v, LinearBasis lb) {
    Vector res = Vector.Zero(lb.SpaceDim);
    foreach (Vector bvec in lb) {
      res += bvec * (bvec * v);
    }

    return res;
  }

    private static Vector PrjCol(Vector v, LinearBasis lb) {
    ddouble[] proj = new ddouble[lb.SpaceDim];
    for (int j = 0; j < lb.SubSpaceDim; j++) {
      ddouble dotProduct = 0;
      for (int i = 0; i < lb.SpaceDim; i++) { // Вычисляем скалярное произведение v * bvec
        dotProduct += v[i] * lb.Basis![i, j];
      }
      for (int i = 0; i < lb.SpaceDim; i++) { // Добавляем проекцию на bvec
        proj[i] += dotProduct * lb.Basis![i, j];
      }
    }

    return new Vector(proj, false);
  }

    public void PrjNaive() { _ = lb.Basis * (lb.Basis.Transpose() * v); }


| Method           | spaceDim | subSpaceDim | Mean       | Error     | StdDev   |
|----------------- |--------- |------------ |-----------:|----------:|---------:|
| PrjCol_indexer   | 4        | 1           |   155.9 ns |  59.48 ns |  3.26 ns |
| PrjNaive         | 4        | 1           |   197.6 ns | 104.62 ns |  5.73 ns |
| PrjWiki_iterator | 4        | 1           |   244.1 ns |  47.77 ns |  2.62 ns |
| PrjCol_indexer   | 4        | 2           |   291.1 ns |  21.15 ns |  1.16 ns |
| PrjNaive         | 4        | 2           |   330.8 ns |  18.52 ns |  1.01 ns |
| PrjWiki_iterator | 4        | 2           |   427.2 ns |  85.75 ns |  4.70 ns |
| PrjCol_indexer   | 4        | 3           |   429.6 ns |  40.48 ns |  2.22 ns |
| PrjNaive         | 4        | 3           |   469.3 ns |  88.47 ns |  4.85 ns |
| PrjCol_indexer   | 4        | 4           |   560.8 ns |  48.29 ns |  2.65 ns |
| PrjNaive         | 4        | 4           |   619.6 ns | 370.71 ns | 20.32 ns |
| PrjWiki_iterator | 4        | 3           |   654.6 ns | 857.32 ns | 46.99 ns |
| PrjWiki_iterator | 4        | 4           |   811.9 ns | 131.60 ns |  7.21 ns |

| Method           | spaceDim | subSpaceDim | Mean     | Error     | StdDev   |
|----------------- |--------- |------------ |---------:|----------:|---------:|
| PrjWiki_iterator | 4        | 1           | 246.6 ns | 152.92 ns |  8.38 ns |
| PrjCol_indexer   | 4        | 1           | 156.5 ns |  22.86 ns |  1.25 ns |
| PrjNaive         | 4        | 1           | 204.0 ns | 126.85 ns |  6.95 ns |
| PrjWiki_iterator | 4        | 2           | 428.2 ns | 111.56 ns |  6.12 ns |
| PrjCol_indexer   | 4        | 2           | 290.7 ns |  12.40 ns |  0.68 ns |
| PrjNaive         | 4        | 2           | 340.3 ns | 240.69 ns | 13.19 ns |
| PrjWiki_iterator | 4        | 3           | 627.7 ns | 181.85 ns |  9.97 ns |
| PrjCol_indexer   | 4        | 3           | 438.6 ns | 296.15 ns | 16.23 ns |
| PrjNaive         | 4        | 3           | 481.1 ns |   9.29 ns |  0.51 ns |
| PrjWiki_iterator | 4        | 4           | 828.0 ns |  49.96 ns |  2.74 ns |
| PrjCol_indexer   | 4        | 4           | 557.7 ns |  25.15 ns |  1.38 ns |
| PrjNaive         | 4        | 4           | 612.3 ns |  43.95 ns |  2.41 ns |


| Method  | spaceDim | subSpaceDim | Mean     | Error     | StdDev   |
|-------- |--------- |------------ |---------:|----------:|---------:|
| Prj     | 4        | 1           | 169.8 ns |  62.75 ns |  3.44 ns |
| Prj_Lin | 4        | 1           | 158.6 ns |  66.97 ns |  3.67 ns |
| Prj     | 4        | 2           | 300.5 ns |  48.97 ns |  2.68 ns |
| Prj_Lin | 4        | 2           | 289.5 ns | 139.87 ns |  7.67 ns |
| Prj     | 4        | 3           | 448.6 ns | 217.93 ns | 11.95 ns |
| Prj_Lin | 4        | 3           | 423.5 ns |  64.20 ns |  3.52 ns |
| Prj     | 4        | 4           | 579.3 ns | 219.18 ns | 12.01 ns |
| Prj_Lin | 4        | 4           | 560.5 ns |  61.89 ns |  3.39 ns |

 *
 */
