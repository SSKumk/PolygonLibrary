using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using System.IO;
using DoubleDouble;
using static CGLibrary.Geometry<DoubleDouble.ddouble, Tests.DDConvertor>;

// using static CGLibrary.Geometry<double, Tests.DConvertor>;


namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  private static readonly string pathData =
    "F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/Other/";

  [Test]
  public void Sandboxx() {
    // HyperPlane hp = new HyperPlane(Vector.Ones(4), 0.5);
    // var        x  = ConvexPolytop.SectionByHyperPlane(ConvexPolytop.RectParallel(Vector.Zero(4), Vector.Ones(4)), hp);
    // .Ellipsoid(3, 10, 20, Vector.Zero(3), new Vector(new ddouble[] { 4.5, 6, 3 }))
    // .WriteTXT_3D(pathData + "Ellipsoid-4.5-6-3");
    // x.WriteTXT_3D(pathData + "Cube4D-Section");
    // ConvexPolytop
    //  .DistanceToBall_2
    //     (
    //      2
    //    , 40
    //    , 40
    //    , 1
    //    , 3
    //     )
    //  .WriteTXT_3D("F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/Other/DistanceToBall_2-sphere");
    // .WriteTXT_3D_forDasha("F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/Other/Cube3D");
  }

}
