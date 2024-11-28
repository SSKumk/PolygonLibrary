using System.Globalization;
using System.Numerics;
using CGLibrary;

namespace Trajectories;

public class TrajMain<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string BridgeDir;
  public readonly string ConfigDir;
  public readonly string OutputDir;

  public readonly TNum tMin;

  public TrajMain(string bridgeDir, string configDir, string outputDir) {
    BridgeDir = bridgeDir;
    ConfigDir = configDir;
    OutputDir = outputDir;

    Geometry<TNum, TConv>.ParamReader pr = new Geometry<TNum, TConv>.ParamReader(bridgeDir + "tMin.txt");
    tMin = pr.ReadNumber<TNum>("tMin");
  }



}

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string bridgeDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\Bridges\\Simple Motion_T=10\\First TS_01\\System.Double\\1e-008\\";
    string configDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\Trajectories\\Configs\\";
    string outputDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\Trajectories\\";

    TrajMain<double, DConvertor> trajCalc = new TrajMain<double, DConvertor>(bridgeDir, configDir, outputDir);
  }

}
