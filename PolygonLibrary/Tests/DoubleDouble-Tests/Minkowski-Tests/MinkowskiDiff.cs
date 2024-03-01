using System.Globalization;
using DoubleDouble;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiDiff3D {

  private string path;

  [OneTimeSetUp]
  public void SetUp() {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    path                       = Directory.GetCurrentDirectory() + "/MinkDiff/";
  }

  [Test]
  public void Cube_Seg0_0_z() {
    ConvexPolytop F = Cube3D;
    Assert.Multiple
      (
       () => {
         ddouble value = 0;
         for (int i = 0; i <= 3; i++) {
           ConvexPolytop G = new ConvexPolytop
             ([new Vector(new ddouble[] { 0, 0, 0 }), new Vector(new ddouble[] { 0, 0, value })]);
           ConvexPolytop? diff = MinkowskiDiff.Naive(F, G);
           switch (i) {
             case 0:
               Assert.That(diff is not null);
               Assert.That(diff.FL, Is.EqualTo(Cube3D_FL));
               diff.WriteTXTasCPolytop(path + "Cube_Seg0-0-0.txt");

               break;
             case 1:
               Assert.That(diff is not null);
               diff.WriteTXTasCPolytop(path + "Cube_Seg0-0-0.5.txt");

               // тут параллелепипед
               break;
             case 2:
               Assert.That(diff is not null);

               // тут квадрат
               break;
             case 3:
               Assert.That(diff is null);

               // а тут пусто
               break;
           }
           value += 0.5;
         }
       }
      );
  }

  [Test]
  public void Sphere_Seg0_0_z() {
    ConvexPolytop F = Sphere(3, 10, 20, 2).CPolytop;
    Assert.Multiple
      (
       () => {
         for (int i = 1; i <= 5; i++) {
           ddouble value = 0.5 * i;
           ConvexPolytop G = new ConvexPolytop
             ([new Vector(new ddouble[] { 0, 0, 0 }), new Vector(new ddouble[] { 0, 0, value })]);
           ConvexPolytop? diff = MinkowskiDiff.Naive(F, G);
           switch (i) {
             case 1:
               Assert.That(diff is not null);
               diff.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-0.5.txt");

               break;
             case 2:
               Assert.That(diff is not null);
               diff.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-1.txt");

               break;
             case 3:
               Assert.That(diff is not null);
               diff.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-1.5.txt");

               break;
             case 4:
               Assert.That(diff is not null);
               diff.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-2.txt");

               break;
             case 5:
               Assert.That(diff is not null);
               diff.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-2.5.txt");

               break;
           }
         }
       }
      );
  }

  [Test]
  public void Cyclic() {
    ConvexPolytop  F    = GiftWrapping.WrapPolytop(CyclicPolytop(3, 100, 0.01));
    ConvexPolytop? diff = MinkowskiDiff.Naive(F, new ConvexPolytop(new List<Vector>() { new Vector(3) }));
    diff.WriteTXTasCPolytop(path + "Cyclic.txt");
  }

}
