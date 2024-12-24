﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


namespace LDG;

/// <summary>
/// Holds paths to the game data and provides methods to open data files.
/// </summary>
public class LDGPathHolder<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

  #region Paths
  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathGame;         // Путь к папке игры
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств

  public readonly string PathBr; // Путь к папке, в которой лежат папки с мостами для разных терминальных множеств
  public readonly string PathPs; // Путь к папке, в которой лежат вектограммы первого игрока
  public readonly string PathQs; // Путь к папке, в которой лежат вектограммы второго игрока
  #endregion

  #region Dicts
  public readonly Dictionary<string, (int dynFileName, string comment)>
    name2dyn; // имя_динамики --> внутреннее имя файла динамики

  public readonly Dictionary<string, (int polFileName, string comment)>
    name2pol; // имя_многогранника --> внутреннее имя файла многогранника

  public readonly Dictionary<string, (int tmsFileName, string comment)>
    name2tms; // имя_терминального_множества --> внутреннее имя файла терминального множества
  #endregion

  /// <summary>
  /// Initializes a new instance of the <see cref="LDGPathHolder{TNum, TConv}"/> class.
  /// </summary>
  /// <param name="pathLdg">The root path for all data.</param>
  /// <param name="outputFolderName">The name of the output folder.</param>
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

  /// <summary>
  /// Reads a dictionary from a given parameter reader.
  /// </summary>
  /// <param name="pr">The parameter reader to read from.</param>
  /// <returns>A dictionary: Key -> (Value, Comment)</returns>
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

  /// <summary>
  /// Opens a reader for a dynamics file.
  /// </summary>
  /// <param name="name">The name of the dynamics.</param>
  /// <returns>A parameter reader for the dynamics file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenDynamicsReader(string name)
    => new(Path.Combine(PathDynamics, name2dyn[name].dynFileName + ".gamedata"));

  /// <summary>
  /// Opens a reader for a polytope file.
  /// </summary>
  /// <param name="name">The name of the polytope.</param>
  /// <returns>A parameter reader for the polytope file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenPolytopeReader(string name)
    => new(Path.Combine(PathPolytopes, name2pol[name].polFileName + ".polytope"));

  /// <summary>
  /// Opens a reader for a terminal set file.
  /// </summary>
  /// <param name="name">The name of the terminal set.</param>
  /// <returns>A parameter reader for the terminal set file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenTerminalSetReader(string name)
    => new(Path.Combine(PathTerminalSets, name2tms[name].tmsFileName + ".terminalset"));

  /// <summary>
  /// Opens a reader for the game hash file.
  /// </summary>
  /// <returns>A parameter reader for the game hash file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenGameHashReader()
    => new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathGame, "game.md5hash"));

}

/// <summary>
/// Creates bridges for the game based on the given problem definition.
/// </summary>
public class BridgeCreator<TNum, TConv>
  where TNum : struct, INumber<TNum>, ITrigonometricFunctions<TNum>, IPowerFunctions<TNum>, IRootFunctions<TNum>,
  IFloatingPoint<TNum>, IFormattable
  where TConv : INumConvertor<TNum> {

#region Data
  public readonly LDGPathHolder<TNum, TConv> ph; // пути к основным папкам и словари-связки
  public readonly GameData<TNum, TConv> gd;      // данные по динамике игры
  public readonly TerminalSet<TNum, TConv> ts;   // данные о терминальном множестве
#endregion

  /// <summary>
  ///  Creates the main object responsible for constructing bridges.
  /// </summary>
  /// <param name="pathLDG">The root path for all data.</param>
  /// <param name="problemFileName">The name of the problem configuration file.</param>
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
    pw.WriteString("Problem", RemoveComment(problemReader.GetSanitizedData()));
    pw.WriteString("Dynamic", RemoveComment(dynamicsReader.GetSanitizedData()));
    pw.WriteString("P", RemoveComment(fpPolytopeReader.GetSanitizedData()));
    pw.WriteString("Q", RemoveComment(spPolytopeReader.GetSanitizedData()));
    pw.WriteString("TerminalSet", RemoveComment(ph.OpenTerminalSetReader(tmsName).GetSanitizedData()));
    pw.Close();
  }

  /// <summary>
  /// Solves the game by creating and solving bridges for each terminal set.
  /// </summary>
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

  /// <summary>
  /// Sets up the required directories for the game if they do not exist.
  /// </summary>
  /// <param name="ldgDir">The root directory where all data is stored.</param>
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
