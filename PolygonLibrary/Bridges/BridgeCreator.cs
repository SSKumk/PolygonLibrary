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

  public static readonly string NumericalType = typeof(TNum).ToString(); // текущий используемый числовой тип
  public static readonly string Eps = $"{TConv.ToDouble(Geometry<TNum, TConv>.Tools.Eps):e0}"; // текущая точность в библиотеке


  public readonly string MainOutputDir; // папка, в которой будут лежать все результаты. Необязательно этой конкретной игры.

  public readonly string ProblemDir;      // где лежит файл с конфигурацией игры
  public readonly string ProblemFileName; // имя файла с конфигурацией игры
  public readonly string ProblemFilePath; // путь до файла с конфигурацией игры


  public readonly string BrConfigDir; // где лежит файл с конфигурацией мостов


  public string OutputDir; // где будут лежать результаты

  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры

  public string definitionTerminalSetType; // тип терминальных множеств в файле
  public int    projDim;                   // размерность выделенных m координат
  public int[]  projInd;                   // индексы выделенных m координат
  public string projInfo;                  // текстовая характеристика пространства выделенных координат


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
    projDim                   = pr.ReadNumber<int>("ProjDim");
    projInd                   = pr.Read1DArray<int>("ProjInd", projDim);
    projInfo                  = $"{projDim}{string.Join(';', projInd)}";

    int numTSet = pr.ReadNumber<int>("numberOfTerminalSets");

    // в цикле перебираем все терминальные множества из файла
    for (int i = 0; i < numTSet; i++) {
      string num = $"{i + 1:00}";
      TerminalSetBase<TNum, TConv> terminalSetBase =
        definitionTerminalSetType switch
          {
            "Explicit"             => new TerminalSet_Explicit<TNum, TConv>(pr, projDim)
          , "Minkowski functional" => new TerminalSet_MinkowskiFunctional<TNum, TConv>(pr, projDim)
          , "Epigraph"             => new TerminalSet_Epigraph<TNum, TConv>(pr, projDim)
          , "Level set"            => new TerminalSet_LevelSet<TNum, TConv>(pr)
          , _                      => throw new ArgumentOutOfRangeException()
          };

      terminalSetBase.Solve
        (
         Path.Combine(OutputDir, $"{terminalSetBase.TerminalSetName}_{num}")
       , gd
       , projDim
       , projInd
       , $"{gd.GameInfo}{NumericalType}{Eps}"
       , $"{gd.PSetInfo}{NumericalType}{Eps}{projInfo}"
       , $"{gd.QSetInfo}{NumericalType}{Eps}{projInfo}"
        );
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

    string mainDir    = "F:\\Works\\IMM\\Аспирантура\\LDG_bridges\\";
    string problemDir = Path.Combine(mainDir, "Problems");

    BridgeCreator<double, DConvertor> bridgeCreator =
      new BridgeCreator<double, DConvertor>(mainDir, problemDir, "SimpleMotion", problemDir);
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion");
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_MinkFunc");
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_LevelSet");
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_Epigraph");
  }

}
