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
    ConvexPolytop ellips = ConvexPolytop.Ellipsoid(3, 20, 100, Vector.Zero(3), new Vector(new ddouble[] { 1, 2, 3 }));
    ConvexPolytop cube   = ConvexPolytop.AsVPolytop(Cube(3, out _, new int[] { 1, 2, 3 }, 20).ToHashSet());

    using (StreamWriter sr = new StreamWriter(pathPresentationFig + "GW_example.obj")) {
      PrVsObj(sr, cube);
      PrVsObj(sr, ellips);
    }
    HashSet<Vector> S    = ellips.Vertices.Union(cube.Vertices).ToHashSet();
    ConvexPolytop   conv = ConvexPolytop.AsVPolytop(S, true);
    conv.WriteTXT_3D(pathPresentationFig + "GW_example_conv");
  }

  public static void PrVsObj(StreamWriter sr, ConvexPolytop P) {
    foreach (Vector v in P.Vertices) {
      sr.WriteLine($"v {v.ToStrSepBySpace()}");
    }
  }

}
