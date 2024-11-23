using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;

namespace Profile.Benchmarks;

using static Geometry<ddouble, Tests.DDConvertor>;

[SimpleJob(iterationCount: 30)]
// [ShortRunJob]
public class Sandbox {

  private static readonly string pathData =
    // "E:\\Work\\PolygonLibrary\\PolygonLibrary\\Tests\\OtherTests\\LDG_Computations\\";
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/";

  [Params(3)]
  public int spaceDim;

  // [Params(3, 7)]
  // public int subSpaceDim;

  public LinearBasis? lb = null;

  public AffineBasis? ab = null;

  public Vector? v = null;

  public Matrix? m = null;

  public List<HyperPlane> HPs;

  public ConvexPolytop? p = null;

  // [Params("5.10")]
  // public string t;


  [GlobalSetup]
  public void SetUp() {
    // SolverLDG solverLdg = new SolverLDG(pathData, "MassDot");
    // string    t         = "5.10";
    // string eps   = "1e-016";
    // ParamReader prR = new ParamReader
    // ($"{solverLdg.WorkDir}{solverLdg.gd.TaskDirToWriteInto}/ddouble/Geometric/{eps}/{t}){solverLdg.FileName}.cpolytop");
    // p = ConvexPolytop.CreateFromReader(prR);

    // p = MinkowskiSum.BySandipDas(ConvexPolytop.Cube01_VRep(spaceDim).RotateRND(), ConvexPolytop.SimplexRND(spaceDim));

    var _  = p.Hrep;
    var __ = p.Vrep;
  }


  // [Benchmark(Baseline = true)]
  // public void NearestPoint() => p.NearestPoint(Vector.Ones(spaceDim));

  // [Benchmark]
  // public void NearestPointOpt_SortedSet() => p.NearestPointOpt(Vector.Ones(spaceDim));

  // public class Program {
  //
  //   public static void Main(string[] args) {
  //     var summary = BenchmarkRunner.Run<Sandbox>();
  //
  //     // int           dim = 5;
  //     // ConvexPolytop s   = ConvexPolytop.SimplexRND(dim);
  //     // for (int i = 0; i < 4; i++) {
  //     //   Console.WriteLine($"i = {i}. |V| = {s.Vrep.Count}. |H| = {s.Hrep.Count}");
  //     //   ConvexPolytop n = ConvexPolytop.SimplexRND(dim);
  //     //   s = MinkowskiSum.BySandipDas(s, n);
  //     // }
  //   }
  //
  // }

}


/*




*/
