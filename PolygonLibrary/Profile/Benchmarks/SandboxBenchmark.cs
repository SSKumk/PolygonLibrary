using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

// [ShortRunJob]
// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Sandbox {

  [Params(5)]
  public int spaceDim;

  [Params(3)]
  public int subSpaceDim;

  public LinearBasis lb = null!;

  public AffineBasis ab = null!;

  public Vector v = null!;

  public Matrix m = null!;

  [GlobalSetup]
  public void SetUp() {
    lb = LinearBasis.GenLinearBasis(spaceDim, subSpaceDim);
    ab = AffineBasis.GenAffineBasis(spaceDim, subSpaceDim);
    v  = Vector.GenVector(spaceDim);
    // v = Vector.GenVector(subSpaceDim);
    m = Matrix.GenMatrix(spaceDim, subSpaceDim, -5, -5);

    // if (ab.Contains(v) != ab.LinBasis.Contains(v - ab.Origin)) {
    //   throw new ArgumentException();
    // }
    if (ab.Contains(ab.LinBasis[0] + ab.LinBasis[2]) != ab.LinBasis.Contains(ab.LinBasis[0] + ab.LinBasis[2] - ab.Origin)) {
      throw new ArgumentException();
    }

  }


  [Benchmark(Baseline = true)]
  public void ContainsAff_naive() => _ = ab.LinBasis.Contains(v - ab.Origin);

  [Benchmark]
  public void ContainsAff_opt() => _ = ab.Contains(v);


  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<Sandbox>();
  //
  //     // Matrix a = Matrix.GenMatrixInt(10,4,-4,4, new GRandomLC(1));
  //     // Vector b = Vector.GenVector(4);
  //
  //     // Console.WriteLine($"{(a.Transpose() * b).Equals(a.MultiplyTransposedByVector(b))}");
  //   }
  //
  // }

}


/*


---------------------------------------------------------------------------------------------------------------------

*/
