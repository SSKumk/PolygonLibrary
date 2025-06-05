using System.Globalization;
using CGLibrary;
using LDG;
using DoubleDouble;
using Rationals;


namespace Bridges;

class Program {

  static void Main(string[] args) {
    string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    // string ldgDir = "E:\\Work\\LDG\\";

    // double eps = double.Parse("1e-8");
    ddouble eps = ddouble.Parse("1e-15");
    // Rational eps = Rational.Parse("1/10000000000000000");


    string problem = "Oscillator-cone6";
    // string problem = "Oscillator-mass-cone6";
    // string problem = "Oscillator3D-mass-cone6";

    // BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, problem, eps);
    BridgeCreator<ddouble, DDConvertor> bridgeCreator = new BridgeCreator<ddouble, DDConvertor>(ldgDir, problem, eps);
    // BridgeCreator<Rational, RConvertor> bridgeCreator = new BridgeCreator<Rational, RConvertor>(ldgDir, problem, (Rational)eps);

    bridgeCreator.Solve();
  }

}
