using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace LDG;

public class LDGPathHolder<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathGame;         // Путь к папке игры
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

    PathGame = Path.Combine(PathLDG, "_Out", outputFolderName);
    PathBr   = Path.Combine(PathGame, "Br");
    PathPs   = Path.Combine(PathGame, "Ps");
    PathQs   = Path.Combine(PathGame, "Qs");

    // Считываем словари, переводящие имена во внутренние имена файлов
    name2dyn = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathDynamics, "!Dict_dynamics.txt")));
    name2pol = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathPolytopes, "!Dict_polytopes.txt")));
    name2tms = ReadDictionary(new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathTerminalSets, "!Dict_terminalsets.txt")));
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
    => new(Path.Combine(PathDynamics, name2dyn[name].dynFileName + ".gamedata"));

  public Geometry<TNum, TConv>.ParamReader OpenPolytopeReader(string name)
    => new(Path.Combine(PathPolytopes, name2pol[name].polFileName + ".polytope"));

  public Geometry<TNum, TConv>.ParamReader OpenTerminalSetReader(string name)
    => new(Path.Combine(PathTerminalSets, name2tms[name].tmsFileName + ".terminalset"));

  public Geometry<TNum, TConv>.ParamReader OpenGameHashReader()
    => new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathGame, "game.md5hash"));

}

public class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Data
  public readonly LDGPathHolder<TNum, TConv>     ph; // пути к основным папкам и словари-связки
  public readonly GameData<TNum, TConv> gd; // данные по динамике игры
  public readonly TerminalSet<TNum, TConv>       ts; // данные о терминальном множестве
#endregion

  public BridgeCreator(string pathLDG, string problemFileName) {
    // Предполагаем, что структура папок LDG создана и корректна. Если это не так, вызвать SetUpDirectories.

    var problemReader = new Geometry<TNum, TConv>.ParamReader(Path.Combine(pathLDG, "Problems", problemFileName) + ".gameconfig");

    // Имя выходной папки совпадает с полем ProblemName в файле задачи
    ph =
      new LDGPathHolder<TNum, TConv>
        (pathLDG, problemReader.ReadString("ProblemName")); // установили пути и прочитали словари-связки

    // Читаем имена динамики и многогранников ограничений на управления игроков
    string dynName      = problemReader.ReadString("DynamicsName");
    string fpPolName    = problemReader.ReadString("FPName");
    var    fpTransform  = TransformReader<TNum, TConv>.ReadTransform(problemReader);
    string spPolName    = problemReader.ReadString("SPName");
    var    spTransform  = TransformReader<TNum, TConv>.ReadTransform(problemReader);
    string tmsName      = problemReader.ReadString("TSName");
    var    tmsTransform = TransformReader<TNum, TConv>.ReadTransform(problemReader);

    // заполняем динамику и многогранники ограничений на управления игроков
    Geometry<TNum, TConv>.ParamReader dynamicsReader   = ph.OpenDynamicsReader(dynName);
    Geometry<TNum, TConv>.ParamReader fpPolytopeReader = ph.OpenPolytopeReader(fpPolName);
    Geometry<TNum, TConv>.ParamReader spPolytopeReader = ph.OpenPolytopeReader(spPolName);
    gd = new GameData<TNum, TConv>(dynamicsReader, fpPolytopeReader, spPolytopeReader, fpTransform, spTransform);

    ts = new TerminalSet<TNum, TConv>(tmsName, ph, ref gd, tmsTransform);

    // Считаем и пишем хеш игры
    Geometry<TNum, TConv>.ParamWriter pw = new Geometry<TNum, TConv>.ParamWriter(Path.Combine(ph.PathGame, "game.md5hash"));
    pw.WriteString("Problem", RemoveComment(problemReader.GetCleanedData()));
    pw.WriteString("Dynamic", RemoveComment(dynamicsReader.GetCleanedData()));
    pw.WriteString("P", RemoveComment(fpPolytopeReader.GetCleanedData()));
    pw.WriteString("Q", RemoveComment(spPolytopeReader.GetCleanedData()));
    pw.WriteString("TerminalSet", RemoveComment(ph.OpenTerminalSetReader(tmsName).GetCleanedData()));
    pw.Close();
  }

  public void Solve() {
    int i = 1;
    while (ts.GetNextTerminalSet(out Geometry<TNum, TConv>.ConvexPolytop? tms)) {
      SolverLDG<TNum, TConv> slv =
        new SolverLDG<TNum, TConv>(Path.Combine(ph.PathBr, i.ToString()), ph.PathPs, ph.PathQs, gd, tms!);
      slv.Solve();
      i++;
    }
  }


  /// <summary>
  /// Removes the part of the string that matches Comment="...";
  /// </summary>
  /// <param name="str">The input string to process.</param>
  /// <returns>The string with the 'Comment="...";' part removed.</returns>
  public static string RemoveComment(string str) { return Regex.Replace(str, "Comment=\".*?\";", ""); }

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
