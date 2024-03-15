using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using CGLibrary;
using DoubleDouble;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[ShortRunJob]
public class MinkowskiSumBench {

  [Params(3, 4, 5, 6, 7)]
  // ReSharper disable once UnassignedField.Global
  public int dimCubes;

  public ConvexPolytop cubeP;

  public ConvexPolytop cubeQ;

  [GlobalSetup]
  public void SetUp() {
    Matrix rotate1 = Matrix.GenONMatrix(dimCubes);
    // cubeP = ConvexPolytop.Cube01(dimCubes).VRep;
  }

}
