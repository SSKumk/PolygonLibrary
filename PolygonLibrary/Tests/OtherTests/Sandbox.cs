using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using System.IO;

namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  [Test]
  public void MakePointsOnSphereTest() {
    ConvexPolytop
     .DistanceToBall_2
        (
         2
       , 40
       , 40
       , 1
       , 3
        )
     .WriteTXT_3D("F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/Other/DistanceToBall_2-sphere");
    // .WriteTXT_3D_forDasha("F:/Works/IMM/Аспирантура/_PolygonLibrary/PolygonLibrary/Tests/OtherTests/LDG_computations/Other/Cube3D");
  }

}
