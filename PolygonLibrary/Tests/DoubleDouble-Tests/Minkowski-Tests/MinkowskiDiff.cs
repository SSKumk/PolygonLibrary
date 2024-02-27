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
           VPolytop G = new VPolytop([new Point(new ddouble[] { 0, 0, 0 }), new Point(new ddouble[] { 0, 0, value })]);
           bool isDiffNonEmpty = MinkowskiDiff.MinkDiff
             (
              F
            , G
            , out FaceLattice diffFL
            , MinkowskiDiff.FindExtrInCPOnVector_Naive
            , MinkowskiDiff.doSubtract
            , ConvexPolytop.HRepToVRep_Naive
            , GiftWrapping.WrapFaceLattice
             );
           switch (i) {
             case 0:
               Assert.That(isDiffNonEmpty);
               Assert.That(diffFL, Is.EqualTo(Cube3D_FL));
               diffFL.WriteTXTasCPolytop(path + "Cube_Seg0-0-0.txt");

               break;
             case 1:
               Assert.That(isDiffNonEmpty);
               diffFL.WriteTXTasCPolytop(path + "Cube_Seg0-0-0.5.txt");

               // тут параллелепипед
               break;
             case 2:
               Assert.That(isDiffNonEmpty);

               // тут квадрат
               break;
             case 3:
               Assert.That(isDiffNonEmpty, Is.EqualTo(false));

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
           ddouble  value = 0.5 * i;
           VPolytop G     = new VPolytop([new Point(new ddouble[] { 0, 0, 0 }), new Point(new ddouble[] { 0, 0, value })]);
           bool isDiffNonEmpty = MinkowskiDiff.MinkDiff
             (
              F
            , G
            , out FaceLattice diffFL
            , MinkowskiDiff.FindExtrInCPOnVector_Naive
            , MinkowskiDiff.doSubtract
            , ConvexPolytop.HRepToVRep_Naive
            , GiftWrapping.WrapFaceLattice
             );
           switch (i) {
             case 1:
               Assert.That(isDiffNonEmpty);
               diffFL.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-0.5.txt");

               break;
             case 2:
               Assert.That(isDiffNonEmpty);
               diffFL.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-1.txt");

               break;
             case 3:
               Assert.That(isDiffNonEmpty);
               diffFL.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-1.5.txt");

               break;
             case 4:
               Assert.That(isDiffNonEmpty);
               diffFL.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-2.txt");

               break;
             case 5:
               Assert.That(isDiffNonEmpty);
               diffFL.WriteTXTasCPolytop(path + "Sphere3-10-20-2_Seg0-0-2.5.txt");

               break;
           }
         }
       }
      );
  }

  [Test]
  public void Cyclic() {
    ConvexPolytop F = GiftWrapping.WrapPolytop(CyclicPolytop(3, 100, 0.01));
    bool isDiffNonEmpty = MinkowskiDiff.MinkDiff
      (
       F
     , new VPolytop(new List<Point>() { new Point(3) })
     , out FaceLattice diffFL
     , MinkowskiDiff.FindExtrInCPOnVector_Naive
     , MinkowskiDiff.doSubtract
     , ConvexPolytop.HRepToVRep_Naive
     , GiftWrapping.WrapFaceLattice
      );
    diffFL.WriteTXTasCPolytop(path + "Cyclic.txt");
  }

}
