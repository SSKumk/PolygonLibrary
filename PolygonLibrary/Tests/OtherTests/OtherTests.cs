using System.Numerics;
using CGLibrary;
using DoubleDouble;
using NUnit.Framework;
using ddG = Tests.ToolsTests. TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using dG = Tests.ToolsTests.  TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;


namespace Tests.OtherTests;

[TestFixture]
public class OtherTests {



  // [Test]
  // public void Atan2Test() {
  //   var x = ddouble.Atan2(-1e-15, -1);
  //   var y = double.Atan2(-1e-15, -1);
  //
  //   Console.WriteLine(x);
  //   Console.WriteLine(y);
  // }
  //
  // [Test]
  // public void AcosTest() {
  //   var x = ddouble.Acos(1);
  //   var y = double.Acos(1);
  //   Console.WriteLine(x);
  //   Console.WriteLine(y);
  // }

  private List<dG.Point> ToDPoints(List<ddG.Point> from) {
    return from.Select
                (
                 p => {
                   double[] pDD = new double[p.Dim];
                   for (int i = 0; i < p.Dim; i++) {
                     pDD[i] = (double)p[i];
                   }

                   return new dG.Point(pDD);
                 }
                )
               .ToList();
  }


  [Test]
  public void FindEtalon_3D() {
    int        cubeDim = 3;
    ddG.Matrix x1x2    = MakeRotationMatrix(cubeDim, 1, 2, ddG.Tools.PI / 4);
    ddG.Matrix x1x3    = MakeRotationMatrix(cubeDim, 1, 3, ddG.Tools.PI / 4);

    ddG.Matrix x1x2x1x3 = x1x2 * x1x3;

    List<ddG.Point> cube_DD             = ddG.Cube(cubeDim, out _, new[] { 1, 2, 3 }, 5, 0, true);
    var             cube_DD_rotated     = Rotate(cube_DD, x1x2x1x3);
    List<dG.Point>  cube_double_rotated = ToDPoints(cube_DD_rotated);


    var P1 = dG.GiftWrapping.WrapPolytop(cube_double_rotated);
    var P2 = ddG.GiftWrapping.WrapPolytop(cube_DD_rotated);

    Console.WriteLine($"{P1.Faces.First().Normal}");
    Console.WriteLine(P2.Faces.First().Normal);
  }

  [Test]
  public void FindEtalon_4D() {
    int        cubeDim = 4;
    ddG.Matrix x1x2    = MakeRotationMatrix(cubeDim, 1, 2, ddG.Tools.PI / 4);
    ddG.Matrix x1x3    = MakeRotationMatrix(cubeDim, 1, 3, ddG.Tools.PI / 4);
    ddG.Matrix x1x4    = MakeRotationMatrix(cubeDim, 1, 4, ddG.Tools.PI / 4);

    ddG.Matrix x1x2x1x3x1x4 = x1x2 * x1x3 * x1x4;

    List<ddG.Point> cube_DD             = ddG.Cube(cubeDim, out List<ddG.Point> _, new[] { 1, 2, 3, 4 }, 5, 0, true);
    var             cube_DD_rotated     = Rotate(cube_DD, x1x2x1x3x1x4);
    List<dG.Point>  cube_double_rotated = ToDPoints(cube_DD_rotated);


    var P1 = dG.GiftWrapping.WrapPolytop(cube_double_rotated);
    var P2 = ddG.GiftWrapping.WrapPolytop(cube_DD_rotated);

    Console.WriteLine($"{P1.Faces.First().Normal}");
    Console.WriteLine(P2.Faces.First().Normal);
  }

  [Test]
  public void FindEtalon_8D() {
    int        cubeDim = 8;
    ddG.Matrix x1x2    = MakeRotationMatrix(cubeDim, 1, 2, ddG.Tools.PI / 4);
    ddG.Matrix x1x3    = MakeRotationMatrix(cubeDim, 1, 3, ddG.Tools.PI / 4);
    ddG.Matrix x1x4    = MakeRotationMatrix(cubeDim, 1, 4, ddG.Tools.PI / 4);
    ddG.Matrix x1x5    = MakeRotationMatrix(cubeDim, 1, 5, ddG.Tools.PI / 4);
    ddG.Matrix x1x6    = MakeRotationMatrix(cubeDim, 1, 6, ddG.Tools.PI / 4);
    ddG.Matrix x1x7    = MakeRotationMatrix(cubeDim, 1, 7, ddG.Tools.PI / 4);
    ddG.Matrix x1x8    = MakeRotationMatrix(cubeDim, 1, 8, ddG.Tools.PI / 4);

    ddG.Matrix x1_x8 = x1x2 * x1x3 * x1x4 * x1x5 * x1x6 * x1x7 * x1x8;

    List<ddG.Point> cube_DD             = ddG.Cube(cubeDim, out List<ddG.Point> _, new[] { 1, 2, 3, 4 }, 0, 0, true);
    var             cube_DD_rotated     = Rotate(cube_DD, x1_x8);
    List<dG.Point>  cube_double_rotated = ToDPoints(cube_DD_rotated);


    var P1 = dG.GiftWrapping.WrapPolytop(cube_double_rotated);
    var P2 = ddG.GiftWrapping.WrapPolytop(cube_DD_rotated);

    Console.WriteLine($"{P1.Faces.First().Normal}");
    Console.WriteLine(P2.Faces.First().Normal);
  }

}
