using System.Globalization;
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
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств
  
  public readonly string PathGame;         // Путь к папке игры
  public readonly string PathTrajectories; // Путь к папке с файлами траекторий
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
