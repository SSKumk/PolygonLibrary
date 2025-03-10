using System.Globalization;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using static CGLibrary.Geometry<double, Tests.DConvertor>;

namespace Tests.Double_Tests.Minkowski_Tests;

[TestFixture]
public class MinkowskiDiff3D
{

  private string path;

  [OneTimeSetUp]
  public void SetUp()
  {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    path = Directory.GetCurrentDirectory() + "/MinkDiff/";
  }


  [Test]
  public void Cube_Seg0_0_z()
  {
    ConvexPolytop F = Cube3D;
    Assert.Multiple
      (
       () =>
       {
         double value = 0;
         for (int i = 0; i <= 3; i++)
         {
           ConvexPolytop G = ConvexPolytop.CreateFromPoints
             ([new Vector(new double[] { 0, 0, 0 }), new Vector(new double[] { 0, 0, value })]);
           ConvexPolytop? diff = MinkowskiDiff.Naive(F, G);
           switch (i)
           {
             case 0:
               Assert.That(diff is not null);
               Assert.That(diff!.FLrep, Is.EqualTo(Cube3D.FLrep));

               break;
             case 1:
               Assert.That(diff is not null);

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
  public void Sphere_Seg0_0_z()
  {
    ConvexPolytop F = ConvexPolytop.Sphere(Vector.Zero(3), 2, 10, 20);
    Assert.Multiple
      (
       () =>
       {
         for (int i = 1; i <= 5; i++)
         {
           double value = 0.5 * i;
           ConvexPolytop G = ConvexPolytop.CreateFromPoints
             ([new Vector(new double[] { 0, 0, 0 }), new Vector(new double[] { 0, 0, value })]);
           ConvexPolytop? diff = MinkowskiDiff.Naive(F, G);
           switch (i)
           {
             case 1:
               Assert.That(diff is not null);

               break;
             case 2:
               Assert.That(diff is not null);

               break;
             case 3:
               Assert.That(diff is not null);

               break;
             case 4:
               Assert.That(diff is not null);

               break;
             case 5:
               Assert.That(diff is not null);

               break;
           }
         }
       }
      );
  }

  // [Test]
  // public void Cyclic()
  // {
  //   ConvexPolytop F = ConvexPolytop.Cyclic(3, 100, 0.01);
  //   ConvexPolytop? diff = MinkowskiDiff.Naive(F, ConvexPolytop.CreateFromPoints(new SortedSet<Vector>() { new Vector(3) }));
  // }

}
