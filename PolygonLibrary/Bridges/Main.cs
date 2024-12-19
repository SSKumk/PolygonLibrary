using System.Globalization;
using DoubleDouble;
// using static CGLibrary.Geometry<DoubleDouble.ddouble, Bridges.DDConvertor>;

// using static CGLibrary.Geometry<double, Bridges.DConvertor>;
// using static Bridges.BridgeCreator<double, Bridges.DConvertor>;

namespace Bridges;

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    // string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    string ldgDir = "E:\\Work\\LDG\\";

    Geometry<double, DConvertor>.ParamReader pr = new Geometry<double, DConvertor>.ParamReader(ldgDir + "test.txt");

    Console.WriteLine(pr.ReadVector("V"));
    Console.WriteLine(pr.ReadString("Str"));
    _ = pr.ReadNumberLine(4);
    // SetUpDirectories(ldgDir);

    // BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, "SimpleMotion.Test9");
    // bridgeCreator.Solve();
  }

}