using System.Diagnostics;
using System.Globalization;
using CGLibrary;
using NUnit.Framework;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using System.IO;
namespace Tests.OtherTests;

[TestFixture]
public class Sandbox {

  [Test]
  public void TXT() {
    string path = "E:\\test.txt";
    Cube3D.WriteTXT(path);
  }

}
