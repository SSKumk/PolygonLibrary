using System.Globalization;
using CGLibrary;
using LDG;
using DoubleDouble;
using Rationals;


namespace Bridges;

class Program {

  static void Main(string[] args) {
    string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgDir = "E:\\Work\\LDG\\_Out\\";



    // BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, "Oscillator", 1e-08);
    // BridgeCreator<ddouble, DDConvertor> bridgeCreator = new BridgeCreator<ddouble, DDConvertor>(ldgDir, "MassDot", 1e-15);
    // BridgeCreator<ddouble, DDConvertor> bridgeCreator = new BridgeCreator<ddouble, DDConvertor>(ldgDir, "Oscillator2D", 1e-15);
    BridgeCreator<Rational, RConvertor> bridgeCreator = new BridgeCreator<Rational, RConvertor>(ldgDir, "Oscillator2D", (Rational)1e-30);
    bridgeCreator.Solve();
  }

}
