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

    double eps = double.Parse("1e-4");
    // ddouble eps = ddouble.Parse("1e-15");
    // Rational eps = Rational.Parse("1/10000000000000000");

    // string problem = "Oscillator2D-square"; // double: (+)45'291, (-)43'672, (*)56'252, (/)6'189,

    // string problem = "Oscillator3D"; // double: (+), (-), (*), (/),
    // string problem = "Oscillator"; // double: (+), (-), (*), (/),


    string problem = "Oscillator-pyramid";
    // string problem = "Oscillator-mass";
    // string problem = "MassDot-mass";

    BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, problem, eps);
    // BridgeCreator<ddouble, DDConvertor> bridgeCreator = new BridgeCreator<ddouble, DDConvertor>(ldgDir, problem, eps);
    // BridgeCreator<Rational, RConvertor> bridgeCreator = new BridgeCreator<Rational, RConvertor>(ldgDir, problem, (Rational)eps);

    bridgeCreator.Solve();
  }

}
