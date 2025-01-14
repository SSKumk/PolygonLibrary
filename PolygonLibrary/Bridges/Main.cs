using System.Globalization;
using LDG;

// using DoubleDouble;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Bridges.DDConvertor>;

// using static CGLibrary.Geometry<double, Bridges.DConvertor>;

namespace Bridges;

class Program {

  static void Main(string[] args) {
    CGLibrary.Geometry<double, DConvertor>.Tools.Eps = 1e-8;


    // string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    string ldgDir = "E:\\Work\\LDG\\";

    BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, "SimpleMotion.Test1");
    bridgeCreator.Solve();
  }

}
