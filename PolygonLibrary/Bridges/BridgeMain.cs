using System.Globalization;
using System.Numerics;
using CGLibrary;


namespace Bridges;

// todo: Если есть файл описания объектов, то с ним работать надо АККУРАТНО! исправляя только то, что изменилось. Возможно стоит втянуть ВСЁ. !!!

class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  // ---------------------------------------------
  // public static string pathToDynamics(string  ldgDir) => Path.Combine(ldgDir, "Dynamics");
  // public static string pathToPolytopes(string ldgDir) => Path.Combine(ldgDir, "Polytopes");
  //
  // public static string pathToDictDynamic(string   ldgDir) => Path.Combine(pathToDynamics(ldgDir), "!Dict_dynamics.txt");
  // public static string pathToDictPolytopes(string ldgDir) => Path.Combine(pathToPolytopes(ldgDir), "!Dict_polytopes.txt");

  public static void SetUpDirectories(string ldgDir) {
    Directory.CreateDirectory(Path.Combine(ldgDir, "_Out"));

    Directory.CreateDirectory(Path.Combine(ldgDir, "Dynamics"));
    string dynPath = Path.Combine(ldgDir, "Dynamics", "!Dict_dynamics.txt");
    if (!File.Exists(dynPath)) { File.Create(dynPath); }

    Directory.CreateDirectory(Path.Combine(ldgDir, "Polytopes"));
    string polPath = Path.Combine(ldgDir, "Polytopes", "!Dict_polytopes.txt");
    if (!File.Exists(polPath)) { File.Create(polPath); }

    Directory.CreateDirectory(Path.Combine(ldgDir, "Problems"));
  }
  // ---------------------------------------------

  public readonly string PathLDG;       // корневая папка для всех данных
  public readonly string PathOutput;    // где будут лежать результаты игры
  public          string PathDynamics;  // Путь к папке с файлами динамик системы
  public          string PathPolytopes; // Путь к папке с файлами многогранников

  public readonly Dictionary<string, (string dynFileName, string comment)>
    name2dyn; // имя_динамики      --> внутреннее имя файла динамики

  public readonly Dictionary<string, (string polFileName, string comment)>
    name2pol; // имя_многогранника --> внутреннее имя файла многогранника


  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры

  public string definitionTerminalSetType; // тип терминальных множеств в файле


  public BridgeCreator(string pathLdg, string ProblemFileName) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.
    PathLDG       = pathLdg;
    PathDynamics  = Path.Combine(PathLDG, "Dynamics");
    PathPolytopes = Path.Combine(PathLDG, "Polytopes");

    // Считываем словари, переводящие имена во внутренние имена файлов
    name2dyn = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathDynamics + "!Dict_dynamics.txt"));
    name2pol = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathPolytopes + "!Dict_polytopes.txt"));

    // ------------------------

    Geometry<TNum, TConv>.ParamReader pr =
      new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathLDG, "Problems", ProblemFileName) + ".gameconfig");

    PathOutput = pr.ReadString("Problem Name");


    string dynFileName = name2dyn[pr.ReadString("Dynamics Name")].dynFileName;
    gd = new Geometry<TNum, TConv>.GameData(new Geometry<TNum, TConv>.ParamReader(PathDynamics + dynFileName + ".gamedata"));


    // PathOutput = Path.Combine(MainOutputDir, $"{gd.ProblemName}_T={gd.T}");
  }


  public static Dictionary<string, (string fileName, string comment)> ReadDictionary(Geometry<TNum, TConv>.ParamReader pr) {
    Dictionary<string, (string fileName, string comment)> dict = new Dictionary<string, (string fileName, string comment)>();

    int k = pr.ReadNumber<int>("Qnt");
    for (int i = 0; i < k; i++) {
      string key      = pr.ReadString("Key");
      string fileName = pr.ReadString("Value");
      string comment  = pr.ReadString("Comment");
      dict.Add(key, (fileName, comment));
    }

    return dict;
  }


  public void ReadTerminalSetConfigAndSolve(string terminalBrConfigFileName) { // Тут будет обработка ошибок ещё

    string                            brConfigPath = Path.Combine(BrConfigDir, terminalBrConfigFileName + ".terminalsetconfig");
    Geometry<TNum, TConv>.ParamReader pr           = new Geometry<TNum, TConv>.ParamReader(brConfigPath);
    definitionTerminalSetType = pr.ReadString("DefinitionType");


    int numTSet = pr.ReadNumber<int>("Count");

    // в цикле перебираем все терминальные множества из файла
    for (int i = 0; i < numTSet; i++) {
      string num = $"{i + 1:00}";
      TerminalSetBase<TNum, TConv> terminalSetBase =
        definitionTerminalSetType switch
          {
            "Explicit"             => new TerminalSet_Explicit<TNum, TConv>(pr, gd, definitionTerminalSetType)
          , "Minkowski functional" => new TerminalSet_MinkowskiFunctional<TNum, TConv>(pr, gd, definitionTerminalSetType)
          , "Epigraph"             => new TerminalSet_Epigraph<TNum, TConv>(pr, gd, definitionTerminalSetType)
          , "Level set"            => new TerminalSet_LevelSet<TNum, TConv>(pr, gd, definitionTerminalSetType)
          , _                      => throw new ArgumentOutOfRangeException()
          };

      terminalSetBase.DoSolve(Path.Combine(PathOutput, $"{terminalSetBase.terminalSetName}_{num}"));
    }
  }

}

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";

    BridgeCreator<double, DConvertor> bridgeCreator =
      new BridgeCreator<double, DConvertor>(ldgDir, configDir, "SimpleMotion", configDir);
    bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion");
    // bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_MinkFunc");
    // bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_LevelSet");
    // bridgeCreator.ReadTerminalSetConfigAndSolve("SimpleMotion_Epigraph");
  }

}
