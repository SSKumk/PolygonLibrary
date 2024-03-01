using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using DoubleDouble;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using System.Numerics;
using Tests.ToolsTests;

namespace Tests.SpeedTests;

[TestFixture]
public class SpeedTestMinkSum {

  private static FaceLattice MinkSumCH(FaceLattice F, FaceLattice G)
    => MinkowskiSum.ByConvexHull(new ConvexPolytop(F.Vertices), new ConvexPolytop(G.Vertices)).FL;
  
  [Test]
  public void CubesSum() {
    StreamWriter      writer  = new StreamWriter(Directory.GetCurrentDirectory() + "/SpeedBench/MinkSum/Cubes.txt");
    const int         N       = 10;
    List<FaceLattice> CubesFL = new List<FaceLattice>();
    for (int dim = 2; dim <= 6; dim++) {
      CubesFL.Add(CubeFL(dim));
    }
    Stopwatch    timer = new Stopwatch();
    FaceLattice? Sum   = null;
    for (int i = 0; i < CubesFL.Count; i++) {
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkSumCH(CubesFL[i], CubesFL[i]); }
      timer.Stop();
      writer.Write($"{CubesFL[i].Top.Dim} & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkSumCH(CubesFL[i], CubesFL[i]); }
      timer.Stop();
      writer.WriteLine($" & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");

      writer.Flush();
    }
    Sum!.Equals(null);
  }

  [Test]
  public void CubeCubeRotatedRNDSum() {
    StreamWriter      writer = new StreamWriter(Directory.GetCurrentDirectory() + "/SpeedBench/MinkSum/CubeCubeRotatedRND.txt");
    const int         N = 10;
    List<FaceLattice> CubesFL = new List<FaceLattice>();
    List<FaceLattice> CubesRotatedFL = new List<FaceLattice>();
    for (int dim = 2; dim <= 6; dim++) {
      CubesFL.Add(CubeFL(dim));
      CubesRotatedFL.Add(CubeRotatedRND_FL(dim));
    }
    Stopwatch    timer = new Stopwatch();
    FaceLattice? Sum   = null;
    for (int i = 0; i < CubesFL.Count; i++) {
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkowskiSum.BySandipDas(CubesFL[i], CubesRotatedFL[i]); }
      timer.Stop();
      writer.Write($"{CubesFL[i].Top.Dim} & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkSumCH(CubesFL[i], CubesRotatedFL[i]); }
      timer.Stop();
      writer.WriteLine($" & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");

      writer.Flush();
    }
    Sum!.Equals(null);
  }

  [Test]
  public void CubesSimplicesRNDSum() {
    StreamWriter      writer = new StreamWriter(Directory.GetCurrentDirectory() + "/SpeedBench/MinkSum/CubesSimplicesRND.txt");
    const int         N = 10;
    List<FaceLattice> CubesFL = new List<FaceLattice>();
    List<FaceLattice> SimplicesFL = new List<FaceLattice>();
    for (int dim = 2; dim <= 6; dim++) {
      CubesFL.Add(CubeFL(dim));
      SimplicesFL.Add(SimplexFL(dim));
    }
    Stopwatch    timer = new Stopwatch();
    FaceLattice? Sum   = null;
    for (int i = 0; i < CubesFL.Count; i++) {
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkowskiSum.BySandipDas(CubesFL[i], SimplicesFL[i]); }
      timer.Stop();
      writer.Write($"{CubesFL[i].Top.Dim} & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkSumCH(CubesFL[i], SimplicesFL[i]); }
      timer.Stop();
      writer.WriteLine($" & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");

      writer.Flush();
    }
    Sum!.Equals(null);
  }

  [Test]
  public void DiamondDiamondRotatedRNDSum() {
    const int theta = 3;
    const int phi = 10;
    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + $"/SpeedBench/MinkSum/DiamondDiamondRotatedRND-{theta}-{phi}-1.txt");
    const int N = 10;
    List<FaceLattice> FirstSphere = new List<FaceLattice>();
    List<FaceLattice> SecondSphere = new List<FaceLattice>();
    for (int dim = 2; dim <= 5; dim++) {
      var Sphere = Sphere_list(dim, theta, phi, 1);
      FirstSphere.Add(GiftWrapping.WrapFaceLattice(Sphere));
      SecondSphere.Add(GiftWrapping.WrapFaceLattice(RotateRND(Sphere)));
    }
    Stopwatch    timer = new Stopwatch();
    FaceLattice? Sum   = null;
    for (int i = 0; i < FirstSphere.Count; i++) {
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkowskiSum.BySandipDas(FirstSphere[i], SecondSphere[i]); }
      timer.Stop();
      writer.Write($"{FirstSphere[i].NonZeroKFacesAmount} & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");
      writer.Flush();
      timer.Restart();
      for (int k = 0; k < N; k++) { Sum = MinkSumCH(FirstSphere[i], SecondSphere[i]); }
      timer.Stop();
      writer.WriteLine($" & {(timer.Elapsed.TotalSeconds / N).ToString("F5", CultureInfo.InvariantCulture)}");

      writer.Flush();
    }
    Sum!.Equals(null);
  }

}
