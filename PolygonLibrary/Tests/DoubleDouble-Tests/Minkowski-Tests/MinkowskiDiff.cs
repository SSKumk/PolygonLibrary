using System.Globalization;
using DoubleDouble;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiDiff3D {

  [OneTimeSetUp]
  public void SetUp() { CultureInfo.CurrentCulture = CultureInfo.InvariantCulture; }

  [Test]
  public void Cube_Seg() {
    FaceLattice F = Cube3D_FL;
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

               break;
             case 1:
               Assert.That(isDiffNonEmpty);
               // тут прямоугольник
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
  public void Cube_Cube() {
    var x = CyclicPolytop(3, 8,0.1);
  }



}
