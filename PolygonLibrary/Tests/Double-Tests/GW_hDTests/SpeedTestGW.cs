using System.Diagnostics;
using CGLibrary;
using NUnit.Framework;
using static CGLibrary.Geometry<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsBase<double, Tests.DConvertor>;
using static Tests.ToolsTests.TestsPolytopes<double, Tests.DConvertor>;


namespace Tests.Double_Tests.GW_hDTests;

[TestFixture]
public class SpeedTestGW {

  [Test]
  public void DoGWBench() {
    // 2Д случай:

    using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "_some.txt")) {
      writer.WriteLine("Hello!");
    }
  }

}

