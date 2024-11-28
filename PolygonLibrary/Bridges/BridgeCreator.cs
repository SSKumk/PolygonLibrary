using System.Globalization;
using System.Numerics;
using CGLibrary;


namespace Bridges;
// todo:  1. Счёт мостов. Файл настроек Игры, файл настроек набора Мостов
//       (задано одно терминальное множество, задана функция платы и набор С, надграфик выпуклой функции)

class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string MainOutputDir; // папка, в которой будут лежать все результаты. Необязательно этой конкретной игры.

  public readonly string ProblemDir;      // где лежит файл с конфигурацией игры
  public readonly string ProblemFileName; // имя файла с конфигурацией игры
  public readonly string ProblemFilePath; // путь до файла с конфигурацией игры


  public readonly string BrConfigDir; // где лежит файл с конфигурацией мостов


  public string OutputDir; // где будут лежать результаты

  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры

  public string definitionTerminalSetType; // тип терминальных множеств в файле


  public BridgeCreator(string outputDir, string problemDir, string problemFileName, string bridgeConfigDir) {
    // просто устанавливаем все пути
    MainOutputDir   = outputDir;
    ProblemDir      = problemDir;
    ProblemFileName = problemFileName;
    BrConfigDir     = bridgeConfigDir;

    // путь до файла конфига игры
    ProblemFilePath = Path.Combine(ProblemDir, ProblemFileName) + ".gameconfig";

    //todo: ещё на ошибки проверить надо
    gd        = new Geometry<TNum, TConv>.GameData(new Geometry<TNum, TConv>.ParamReader(ProblemFilePath));
    OutputDir = Path.Combine(MainOutputDir, $"{gd.ProblemName}_T={gd.T}");
  }


  public void ReadTerminalSetConfigAndSolve(string terminalBrConfigFileName) { // Тут будет обработка ошибок ещё

    string                            brConfigPath = Path.Combine(BrConfigDir, terminalBrConfigFileName + ".terminalsetconfig");
    Geometry<TNum, TConv>.ParamReader pr           = new Geometry<TNum, TConv>.ParamReader(brConfigPath);
    definitionTerminalSetType = pr.ReadString("DefinitionType");


    int numTSet = pr.ReadNumber<int>("numberOfTerminalSets");

    // в цикле перебираем все терминальные множества из файла
    for (int i = 0; i < numTSet; i++) {
      string num = $"{i + 1:00}";
      TerminalSetBase<TNum, TConv> terminalSetBase =
        definitionTerminalSetType switch
          {
            "Explicit"             => new TerminalSet_Explicit<TNum, TConv>(pr, gd)
          , "Minkowski functional" => new TerminalSet_MinkowskiFunctional<TNum, TConv>(pr, gd)
          , "Epigraph"             => new TerminalSet_Epigraph<TNum, TConv>(pr, gd)
          , "Level set"            => new TerminalSet_LevelSet<TNum, TConv>(pr, gd)
          , _                      => throw new ArgumentOutOfRangeException()
          };

      terminalSetBase.DoSolve(Path.Combine(OutputDir, $"{terminalSetBase.TerminalSetName}_{num}"));
    }
  }


  // public void CleanAll() {
  //   Console.WriteLine($"Warning! This function will erase all files and folders at the path: {ProblemPath}/");
  //   Console.WriteLine($"Do you want to continue? [y]es");
  //   if (Console.ReadKey().KeyChar == 'y') {
  //     Directory.Delete($"{ProblemPath}", true);
  //   }
  // }

}

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string mainDir   = "F:\\Works\\IMM\\Аспирантура\\LDG\\Bridges\\";
    string configDir = Path.Combine(mainDir, "Configs");

    BridgeCreator<double, DConvertor> bridgeCreator =
      new BridgeCreator<double, DConvertor>(mainDir, configDir, "SimpleMotion", configDir);
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion");
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_MinkFunc");
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_LevelSet");
    // bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_Epigraph");
  }

}
