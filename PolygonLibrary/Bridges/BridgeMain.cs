using System.Globalization;


namespace Bridges;
// todo: ПОКА ЗАБИЛ НА ВСЯКИЕ ХЕШИ, ПРОВЕРКИ КОРРЕКТНОСТИ И ПРОЧЕЕ!!!

// todo: Если есть файл описания объектов, то с ним работать надо АККУРАТНО! исправляя только то, что изменилось. Возможно стоит втянуть ВСЁ. !!!

public class LDGDirHolder<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств

  public readonly Dictionary<string, (string dynFileName, string comment)>
    name2dyn; // имя_динамики      --> внутреннее имя файла динамики

  public readonly Dictionary<string, (string polFileName, string comment)>
    name2pol; // имя_многогранника --> внутреннее имя файла многогранника

  public readonly Dictionary<string, (string tmsFileName, string comment)>
    name2tms; // имя_терминального_множества --> внутреннее имя файла терминального множества

  public LDGDirHolder(string pathLdg) {
    PathLDG          = pathLdg;
    PathDynamics     = Path.Combine(PathLDG, "Dynamics");
    PathPolytopes    = Path.Combine(PathLDG, "Polytopes");
    PathTerminalSets = Path.Combine(PathLDG, "Terminal sets");

    // Считываем словари, переводящие имена во внутренние имена файлов
    name2dyn = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathDynamics + "!Dict_dynamics.txt"));
    name2pol = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathPolytopes + "!Dict_polytopes.txt"));
    name2tms = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathTerminalSets + "!Dict_terminalsets.txt"));
  }

  private static Dictionary<string, (string fileName, string comment)> ReadDictionary(Geometry<TNum, TConv>.ParamReader pr) {
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

  public Geometry<TNum, TConv>.ParamReader OpenDynamicsReader(string name)
    => new(PathDynamics + name2dyn[name].dynFileName + ".gamedata");

  public Geometry<TNum, TConv>.ParamReader OpenPolytopeReader(string name)
    => new(PathPolytopes + name2pol[name].polFileName + ".polytope");

  public Geometry<TNum, TConv>.ParamReader OpenTerminalSetReader(string name)
    => new(PathTerminalSets + name2tms[name].tmsFileName + ".terminalset");

}

class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Data
  public readonly LDGDirHolder<TNum, TConv>      dh; // пути к основным папкам и словари-связки
  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры
  public readonly TerminalSet<TNum, TConv>       ts; // данные о терминальном множестве

  public readonly string PathOutput; // где будут лежать результаты игры
#endregion

  public BridgeCreator(string pathLdg, string ProblemFileName) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.

    dh = new LDGDirHolder<TNum, TConv>(pathLdg); // установили пути и прочитали словари-связки

    // начинаем читать файл задачи

    Geometry<TNum, TConv>.ParamReader pr =
      new Geometry<TNum, TConv>.ParamReader(Path.Combine(dh.PathLDG, "Problems", ProblemFileName) + ".gameconfig");

    // Имя выходной папки совпадает с полем Problem Name в файле задачи
    PathOutput = pr.ReadString("Problem Name");

    // Читаем имена динамики и многогранников ограничений на управления игроков
    string dynName   = pr.ReadString("Dynamics Name");
    string fpPolName = pr.ReadString("FP Name");
    string spPolName = pr.ReadString("SP Name");
    string tmsName   = pr.ReadString("TS Name");

    // заполняем динамику и многогранники ограничений на управления игроков
    gd =
      new Geometry<TNum, TConv>.GameData
        (dh.OpenDynamicsReader(dynName), dh.OpenPolytopeReader(fpPolName), dh.OpenPolytopeReader(spPolName));

    ts = new TerminalSet<TNum,TConv>(tmsName, dh);
  }

  public void Solve() {
    while (ts.GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop tms)) {
      // Geometry<TNum,TConv>.SolverLDG solver = new Geometry<TNum, TConv>.SolverLDG()
    }
  }
  

  // ---------------------------------------------

  public static void SetUpDirectories(string ldgDir) {
    Directory.CreateDirectory(Path.Combine(ldgDir, "_Out"));

    Directory.CreateDirectory(Path.Combine(ldgDir, "Dynamics"));
    string dynPath = Path.Combine(ldgDir, "Dynamics", "!Dict_dynamics.txt");
    if (!File.Exists(dynPath)) { File.Create(dynPath); }

    Directory.CreateDirectory(Path.Combine(ldgDir, "Polytopes"));
    string polPath = Path.Combine(ldgDir, "Polytopes", "!Dict_polytopes.txt");
    if (!File.Exists(polPath)) { File.Create(polPath); }

    Directory.CreateDirectory(Path.Combine(ldgDir, "Problems"));

    Directory.CreateDirectory(Path.Combine(ldgDir, "Terminal sets"));
    string tmsPath = Path.Combine(ldgDir, "Terminal sets", "!Dict_terminalsets.txt");
    if (!File.Exists(tmsPath)) { File.Create(tmsPath); }
  }
  // ---------------------------------------------

}

class Program {

  static void Main(string[] args) {
    CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";

    BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, "SimpleMotion");
  }

}
