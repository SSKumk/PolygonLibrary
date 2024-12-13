using System.Globalization;


namespace Bridges;
// todo: ПОКА ЗАБИЛ НА ВСЯКИЕ ХЕШИ, ПРОВЕРКИ КОРРЕКТНОСТИ И ПРОЧЕЕ!!!

// todo: Если есть файл описания объектов, то с ним работать надо АККУРАТНО! исправляя только то, что изменилось. Возможно стоит втянуть ВСЁ. !!!

public class LDGPathHolder<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств

  public readonly string PathBr; // Путь к папке, в которой лежат папки с мостами для разных терминальных множеств
  public readonly string PathPs; // Путь к папке, в которой лежат вектограммы первого игрока
  public readonly string PathQs; // Путь к папке, в которой лежат вектограммы второго игрока

  public readonly Dictionary<string, (int dynFileName, string comment)>
    name2dyn; // имя_динамики --> внутреннее имя файла динамики

  public readonly Dictionary<string, (int polFileName, string comment)>
    name2pol; // имя_многогранника --> внутреннее имя файла многогранника

  public readonly Dictionary<string, (int tmsFileName, string comment)>
    name2tms; // имя_терминального_множества --> внутреннее имя файла терминального множества

  public LDGPathHolder(string pathLdg, string outputFolderName) {
    PathLDG          = pathLdg;
    PathDynamics     = Path.Combine(PathLDG, "Dynamics");
    PathPolytopes    = Path.Combine(PathLDG, "Polytopes");
    PathTerminalSets = Path.Combine(PathLDG, "Terminal sets");

    string pathOut = Path.Combine(PathLDG, "_Out", outputFolderName);
    PathBr = Path.Combine(pathOut, "Br");
    PathPs = Path.Combine(pathOut, "Ps");
    PathQs = Path.Combine(pathOut, "Qs");

    // Считываем словари, переводящие имена во внутренние имена файлов
    name2dyn = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathDynamics + "!Dict_dynamics.txt"));
    name2pol = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathPolytopes + "!Dict_polytopes.txt"));
    name2tms = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(PathTerminalSets + "!Dict_terminalsets.txt"));
  }

  private static Dictionary<string, (int fileName, string comment)> ReadDictionary(Geometry<TNum, TConv>.ParamReader pr) {
    Dictionary<string, (int fileName, string comment)> dict = new Dictionary<string, (int fileName, string comment)>();

    int k = pr.ReadNumber<int>("Qnt");
    for (int i = 0; i < k; i++) {
      string key      = pr.ReadString("Key");
      int    fileName = pr.ReadNumber<int>("Value");
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
  public readonly LDGPathHolder<TNum, TConv>     dh; // пути к основным папкам и словари-связки
  public readonly Geometry<TNum, TConv>.GameData gd; // данные по динамике игры
  public readonly TerminalSet<TNum, TConv>       ts; // данные о терминальном множестве
#endregion

  public BridgeCreator(string pathLDG, string problemFileName) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.

    var pr = new Geometry<TNum, TConv>.ParamReader(Path.Combine(pathLDG, "Problems", problemFileName) + ".gameconfig");

    // Имя выходной папки совпадает с полем Problem Name в файле задачи
    dh = new LDGPathHolder<TNum, TConv>(pathLDG, pr.ReadString("Problem Name")); // установили пути и прочитали словари-связки

    // Читаем имена динамики и многогранников ограничений на управления игроков
    string dynName   = pr.ReadString("Dynamics Name");
    string fpPolName = pr.ReadString("FP Name");
    string spPolName = pr.ReadString("SP Name");
    string tmsName   = pr.ReadString("TS Name");

    // заполняем динамику и многогранники ограничений на управления игроков
    gd =
      new Geometry<TNum, TConv>.GameData
        (dh.OpenDynamicsReader(dynName), dh.OpenPolytopeReader(fpPolName), dh.OpenPolytopeReader(spPolName));

    ts = new TerminalSet<TNum, TConv>(tmsName, dh, ref gd);
  }

  public void Solve() {
    while (ts.GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms)) {
      Geometry<TNum, TConv>.SolverLDG slv = new Geometry<TNum, TConv>.SolverLDG(dh.PathBr, dh.PathPs, dh.PathQs, gd, tms!);
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

    // string ldgDir = "F:\\Works\\IMM\\Аспирантура\\LDG\\";
    string ldgDir = "E:\\Work\\LDG\\";

    Geometry<double,DConvertor>.ParamReader pr = new Geometry<double,DConvertor>.ParamReader(ldgDir + "1.txt");
    Console.WriteLine(pr.ReadString("some"));
    for (int i = 0; i < 3; i++) {
      Console.WriteLine(new Geometry<double,DConvertor>.Vector(pr.ReadNumberLine(3)));
    }
    Console.WriteLine(pr.ReadString("some"));
    // BridgeCreator<double,DConvertor>.SetUpDirectories(ldgDir);

    // BridgeCreator<double, DConvertor> bridgeCreator = new BridgeCreator<double, DConvertor>(ldgDir, "SimpleMotion");
  }

}
