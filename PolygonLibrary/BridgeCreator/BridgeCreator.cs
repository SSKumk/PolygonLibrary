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

  public readonly string NumericalType; // текущий используемый числовой тип
  public readonly string Eps;           // текущая точность в библиотеке

  public readonly string MainOutputDir; // папка, в которой будут лежать все результаты. Необязательно этой конкретной игры.

  public readonly string ProblemDir;      // где лежит файл с конфигурацией игры
  public readonly string ProblemFileName; // имя файла с конфигурацией игры
  public readonly string ProblemFilePath; // путь до файла с конфигурацией игры


  public readonly string BrConfigDir;  // где лежит файл с конфигурацией мостов
  public readonly string BrConfigPath; // полный путь до файла с конфигурацией мостов


  public string OutputDir; // где будут лежать результаты

  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры

  public BridgeCreator(string outputDir, string problemDir, string problemFileName, string bridgeConfigDir) {
    // просто устанавливаем все пути
    MainOutputDir   = outputDir;
    ProblemDir      = problemDir;
    ProblemFileName = problemFileName;
    BrConfigDir     = bridgeConfigDir;

    // путь до файла конфига игры
    ProblemFilePath = Path.Combine(ProblemDir, ProblemFileName) + ".gameconfig";

    // имя файла конфига мостов должно совпадать с именем файла задачи, но с другим расширением
    BrConfigPath = Path.Combine(BrConfigDir, ProblemFileName) + ".tsconfig";


    // понимаем с какими настройками библиотеки работаем
    NumericalType = typeof(TNum).ToString();
    Eps           = $"{TConv.ToDouble(Geometry<TNum, TConv>.Tools.Eps):e0}";

    //todo: ещё на ошибки проверить надо
    gd  = new Geometry<TNum, TConv>.GameData(new Geometry<TNum, TConv>.ParamReader(ProblemFilePath));
    OutputDir = Path.Combine(MainOutputDir, $"{gd.ProblemName}_T={gd.T}");
  }


  public void ReadBridgeConfigAndSolve() { // Тут будет обработка ошибок ещё
    Geometry<TNum, TConv>.ParamReader pr      = new Geometry<TNum, TConv>.ParamReader(BrConfigPath);
    int                               numTSet = pr.ReadNumber<int>("numberOfTSets");

    // в цикле перебираем все терминальные множества из файла
    for (int i = 0; i < numTSet; i++) {
      string                   pref        = $"T{i + 1:00}";
      TerminalSet<TNum, TConv> terminalSet = new TerminalSet<TNum, TConv>(pr);
      if (!terminalSet.IsLevelSet) {
        string workDir = Path.Combine(OutputDir, $"{pref}_{terminalSet.TSName}", NumericalType, Eps);

        string gameInfo = $"{gd.GameInfo}{NumericalType}{Eps}{terminalSet.TSetInfo}";
        string PsInfo   = $"{gd.PSetInfo}{NumericalType}{Eps}{terminalSet.ProjInfo}";
        string QsInfo   = $"{gd.QSetInfo}{NumericalType}{Eps}{terminalSet.ProjInfo}";

        Geometry<TNum, TConv>.SolverLDG sl = new Geometry<TNum, TConv>.SolverLDG
          (
           workDir
         , gd
         , terminalSet.projDim
         , terminalSet.projInd
         , terminalSet.M
         , gameInfo
         , PsInfo
         , QsInfo
          );

        sl.Solve();
      }
      else { }
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

    BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>
      (mainDir, problemDir, "SimpleMotion", problemDir);
    bridgeCreator.ReadBridgeConfigAndSolve();
  }

}
