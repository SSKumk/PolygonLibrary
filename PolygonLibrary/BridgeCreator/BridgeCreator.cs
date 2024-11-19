using System.Globalization;
using System.Numerics;
using CGLibrary;


namespace BridgeCreator;
// todo:  1. Счёт мостов. Файл настроек Игры, файл настроек набора Мостов
//       (задано одно терминальное множество, задана функция платы и набор С, надграфик выпуклой функции)

class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public Geometry<TNum, TConv>.SolverLDG sl;

  public BridgeCreator(string workDir, string taskDir, string taskName, string bridgeConfigDir) {

  }

}


class Program {

  static void Main(string[] args) {
    var x = new BridgeCreator<double, DConvertor>();
  }

}