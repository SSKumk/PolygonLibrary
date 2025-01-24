using System.Globalization;
using LDG;

using DoubleDouble;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Bridges.DDConvertor>;

// using static CGLibrary.Geometry<double, Bridges.DConvertor>;

namespace Bridges;

class Program {

  static void Main(string[] args) {
    // CGLibrary.Geometry<double, DConvertor>.Tools.Eps   = 1e-8;
    CGLibrary.Geometry<ddouble, DDConvertor>.Tools.Eps = 1e-16;


    string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgDir = "E:\\Work\\LDG\\_Out\\";

    // BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, "Oscillator");
    BridgeCreator<ddouble, DDConvertor> bridgeCreator = new BridgeCreator<ddouble, DDConvertor>(ldgDir, "Oscillator");
    bridgeCreator.Solve();
  }

}
