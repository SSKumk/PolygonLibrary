using System.Diagnostics;
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

  public readonly string NumType;     // числовой тип
  public readonly string NumAccuracy; // текущая точность вычислений

#region Paths
  public readonly string PathLDG;          // корневая папка для всех данных
  public readonly string PathDynamics;     // Путь к папке с файлами динамик системы
  public readonly string PathPolytopes;    // Путь к папке с файлами многогранников
  public readonly string PathTerminalSets; // Путь к папке с файлами терминальных множеств

  public readonly string PathGame;         // Путь к папке игры
  public readonly string PathTrajectories; // Путь к папке с файлами траекторий
  public readonly string PathTrajConfigs;  // Путь к папке с файлами настроек траекторий

  public          string PathBr(int i) => Path.Combine(PathBrs, i.ToString(), NumType, NumAccuracy);
  public readonly string PathBrs; // Путь к папке, в которой лежат папки с мостами для разных терминальных множеств
  public readonly string PathPs;  // Путь к папке, в которой лежат вектограммы первого игрока
  public readonly string PathQs;  // Путь к папке, в которой лежат вектограммы второго игрока

  public readonly string PathMinTimes; // Путь к файлу минимальных просчитанных времён для каждого моста.
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
    NumType     = typeof(TNum).ToString();
    NumAccuracy = $"{TConv.ToDouble(Geometry<TNum, TConv>.Tools.Eps):e0}";

    // глобальные пути
    PathLDG          = pathLdg;
    PathDynamics     = Path.Combine(PathLDG, "Dynamics");
    PathPolytopes    = Path.Combine(PathLDG, "Polytopes");
    PathTerminalSets = Path.Combine(PathLDG, "Terminal sets");

    // пути для данной игры
    PathGame         = Path.Combine(PathLDG, "_Out", outputFolderName);
    PathBrs          = Path.Combine(PathGame, "Br");
    PathPs           = Path.Combine(PathGame, "Ps", NumType, NumAccuracy);
    PathQs           = Path.Combine(PathGame, "Qs", NumType, NumAccuracy);
    PathTrajectories = Path.Combine(PathGame, "Trajectories");
    PathTrajConfigs  = Path.Combine(PathTrajectories, "!Configs");

    // пути к конкретным файлам
    PathMinTimes = Path.Combine(PathBrs, ".mintimes");

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

#region Open reader
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
  public Geometry<TNum, TConv>.ParamReader OpenGameInfoReader()
    => new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathGame, "game.md5hash"));

  /// <summary>
  /// Opens a reader for the trajectory config file.
  /// </summary>
  /// <param name="name">The name of the trajectory configuration file.</param>
  /// <returns>A parameter reader for the trajectory config file.</returns>
  public Geometry<TNum, TConv>.ParamReader OpenTrajConfigReader(string name)
    => new Geometry<TNum, TConv>.ParamReader(Path.Combine(PathTrajConfigs, name + ".trajconfig"));
#endregion

#region Load
  public void LoadBridgeSection(SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> bridge, int i, TNum t)
    => ReadSection(bridge, "W", PathBr(i), t);

  public void LoadMinimalTimes(Dictionary<int, TNum> times) {
    int          i  = 0;
    StreamReader sr = new StreamReader(PathMinTimes);
    while (!sr.EndOfStream) {
      times.Add(i, TNum.Parse(sr.ReadLine(), CultureInfo.InvariantCulture));
      i++;
    }
  }
#endregion


#region Aux
  /// <summary>
  /// Constructs the file path for the specified section at a given time t.
  /// </summary>
  /// <param name="sectionPrefix">The prefix of the section (e.g., "W", "P", or "Q").</param>
  /// <param name="basePath">The base directory path where the section file is located.</param>
  /// <param name="t">The time instant for which the section file is constructed.</param>
  /// <returns>The full path of the section file corresponding to the given time.</returns>
  /// <exception cref="ArgumentException">Thrown if the sectionPrefix is invalid (i.e., not "W", "P", or "Q").</exception>
  public string GetSectionPath(string sectionPrefix, string basePath, TNum t) {
    string prefix =
      sectionPrefix switch
        {
          "W" => "w"
        , "P" => "p"
        , "Q" => "q"
        , _   => throw new ArgumentException($"Unknown section prefix: '{sectionPrefix}'. Expected 'W', 'P', or 'Q'.")
        };

    return Path.Combine(basePath, $"{Tools<TNum, TConv>.ToPrintTNum(t)}.{prefix}section");
  }

  /// <summary>
  /// Reads the specified section of a convex polytope for a given time t from a file.
  /// </summary>
  /// <param name="sectionDict">The dictionary where the read convex polytope will be stored for the specified time.</param>
  /// <param name="sectionPrefix">The prefix of the section, e.g. "W", "P", or "Q".</param>
  /// <param name="basePath">The base directory path from which the section is read.</param>
  /// <param name="t">The time instant for which the section is read.</param>
  public void ReadSection(
      SortedDictionary<TNum, Geometry<TNum, TConv>.ConvexPolytop> sectionDict
    , string                                                      sectionPrefix
    , string                                                      basePath
    , TNum                                                        t
    ) {

    string filePath = GetSectionPath(sectionPrefix, basePath, t);
    Debug.Assert
      (File.Exists(filePath), $"LDG.PathHolder.ReadSection: There is no {sectionPrefix} section at time {t}. File: {filePath}");

    Geometry<TNum, TConv>.ParamReader prR = new Geometry<TNum, TConv>.ParamReader(filePath);
    sectionDict.Add(t, Geometry<TNum, TConv>.ConvexPolytop.CreateFromReader(prR));
  }
#endregion

}
