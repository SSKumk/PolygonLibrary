using System.Globalization;
using DoubleDouble;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

namespace Tests.DoubleDouble_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiDiff3D
{

  // private string path;

  [OneTimeSetUp]
  public void SetUp()
  {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    // path                       = Directory.GetCurrentDirectory() + "/MinkDiff/";
    // if (!Directory.Exists(path)) {
    //   Directory.CreateDirectory(path);
    // }
  }


  [Test]
  public void Cube_Seg0_0_z()
  {
    ConvexPolytop F = Cube3D;
    Assert.Multiple
      (
       () =>
       {
         ddouble value = 0;
         for (int i = 0; i <= 3; i++)
         {
           ConvexPolytop G = ConvexPolytop.CreateFromPoints
             ([new Vector(new ddouble[] { 0, 0, 0 }), new Vector(new ddouble[] { 0, 0, value })]);
           ConvexPolytop? diff_naive = MinkowskiDiff.Naive(F, G);
           ConvexPolytop? diff_geometric = MinkowskiDiff.Geometric(F, G);
           switch (i)
           {
             case 0:
               Assert.That(diff_naive is not null);
               Assert.That(diff_geometric is not null);
               Assert.That(diff_naive.FLrep, Is.EqualTo(Cube3D_FL));
               Assert.That(diff_geometric.FLrep, Is.EqualTo(Cube3D_FL));
               Assert.That(diff_naive, Is.EqualTo(diff_geometric));
               // diff_naive.WriteTXT_3D(path + "Cube_Seg0-0-0");

               break;
             case 1:
               Assert.That(diff_naive is not null);
               Assert.That(diff_naive, Is.EqualTo(diff_geometric));

               // diff_naive.WriteTXT_3D(path + "Cube_Seg0-0-0.5");

               // тут параллелепипед
               break;
             case 2:
               Assert.That(diff_naive is not null);
               Assert.That(diff_naive, Is.EqualTo(diff_geometric));

               // тут квадрат
               break;
             case 3:
               Assert.That(diff_naive is null);
               Assert.That(diff_geometric is null);
               Assert.That(diff_naive, Is.EqualTo(diff_geometric));

               // а тут пусто
               break;
           }
           Assert.That(diff_naive, Is.EqualTo(diff_geometric));
           value += 0.5;
         }
       }
      );
  }

  [Test]
  public void Sphere_Seg0_0_z()
  {
    ConvexPolytop F = ConvexPolytop.Sphere(3, 10, 20, Vector.Zero(3), 2);
    Assert.Multiple
      (
       () =>
       {
         for (int i = 1; i <= 5; i++)
         {
           ddouble value = 0.5 * i;
           ConvexPolytop G = ConvexPolytop.CreateFromPoints
             ([new Vector(new ddouble[] { 0, 0, 0 }), new Vector(new ddouble[] { 0, 0, value })]);
           ConvexPolytop? diff_naive = MinkowskiDiff.Naive(F, G);
           ConvexPolytop? diff_geometric = MinkowskiDiff.Geometric(F, G);
           // switch (i) {
           //   case 1:
           //     Assert.That(diff_naive is not null);
           //     // diff.WriteTXT_3D(path + "Sphere3-10-20-2_Seg0-0-0.5");
           //
           //     break;
           //   case 2:
           //     Assert.That(diff_naive is not null);
           //     // diff.WriteTXT_3D(path + "Sphere3-10-20-2_Seg0-0-1");
           //
           //     break;
           //   case 3:
           //     Assert.That(diff_naive is not null);
           //     // diff.WriteTXT_3D(path + "Sphere3-10-20-2_Seg0-0-1.5");
           //
           //     break;
           //   case 4:
           //     Assert.That(diff_naive is not null);
           //     // diff.WriteTXT_3D(path + "Sphere3-10-20-2_Seg0-0-2");
           //
           //     break;
           //   case 5:
           //     Assert.That(diff_naive is not null);
           //     // diff.WriteTXT_3D(path + "Sphere3-10-20-2_Seg0-0-2.5");
           //
           //     break;
           // }
           Assert.That(diff_naive, Is.EqualTo(diff_geometric));
         }
       }
      );
  }

  [Test]
  public void Cyclic()
  {
    ConvexPolytop F = ConvexPolytop.Cyclic(3, 100, 0.01);
    ConvexPolytop point = ConvexPolytop.CreateFromPoints(new SortedSet<Vector>() { new Vector(3) });

    ConvexPolytop? diff_naive = MinkowskiDiff.Naive(F, point);
    ConvexPolytop? diff_geometric = MinkowskiDiff.Geometric(F, point);
    Assert.That(diff_naive, Is.EqualTo(diff_geometric));

    // diff.WriteTXT_3D(path + "Cyclic");
  }

}
