using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<DoubleDouble.ddouble, Tests.DDConvertor>;
using static Tests.ToolsTests.TestsBase<DoubleDouble.ddouble, Tests.DDConvertor>;
using System.IO;
using DoubleDouble;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;


// using static CGLibrary.Geometry<double, Tests.DConvertor>;


namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/Other/";

  private static readonly string pathPresentationFig =
    "F:\\Works\\Конференции и семинары\\2024\\04. Молодёжный семинар\\Figures\\";

  [Test]
  public void Sandboxx() {

    // for (int i = 3; i <= 7; i++) {
    //   ConvexPolytop P = ConvexPolytop.SimplexRND(i,true);
    //   ConvexPolytop Q = ConvexPolytop.SimplexRND(i,true);
    //   Console.WriteLine($"{i} | {string.Join(' ', MinkowskiSum.BySandipDas(P,Q).FVector)}");
    // }
    Console.WriteLine($"6 | {string.Join(' ', ConvexPolytop.Cyclic(6,64,0.01).FVector)}");

    // ConvexPolytop ellips = ConvexPolytop.Ellipsoid(3, 20, 100, Vector.Zero(3), new Vector(new ddouble[] { 1, 2, 3 }));
    // ConvexPolytop cube   = ConvexPolytop.AsVPolytop(Cube(3, out _, new int[] { 1, 2, 3 }, 20).ToHashSet());

    // ConvexPolytop cube  = ConvexPolytop.RectParallel(-Vector.Ones(3), Vector.Ones(3));
    // ConvexPolytop candy = ConvexPolytop.Sphere(3, 2, 4, Vector.Zero(3), 1);

    // cube.WriteTXT_3D(pathPresentationFig + "Cube01");
    // candy.WriteTXT_3D(pathPresentationFig + "Octahedron");

    // ConvexPolytop sum = MinkowskiSum.BySandipDas(cube, candy);
    // sum.WriteTXT_3D(pathPresentationFig + "Cube01_plus_Octahedron");
    // using (StreamWriter sr = new StreamWriter(pathPresentationFig + "GeneralPos.obj")) {
      // PrVsObj(sr, ConvexPolytop.Cube01(3));
    // }
    // conv.WriteTXT_3D(pathPresentationFig + "GW_example_conv");
  }

  public static void PrVsObj(StreamWriter sr, ConvexPolytop P) {
    foreach (Vector v in P.Vertices) {
      sr.WriteLine($"v {v.ToStrSepBySpace()}");
    }
  }

}
